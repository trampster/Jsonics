using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jsonics.ToJson
{
    internal class DictionaryEmitter : ToJsonEmitter
    {
        readonly ListMethods _listMethods;
        readonly FieldBuilder _stringBuilderField;
        readonly TypeBuilder _typeBuilder;
        readonly ToJsonEmitters _toJsonEmitters;

        internal DictionaryEmitter(ListMethods listMethods, FieldBuilder stringBuilderField, TypeBuilder typeBuilder, ToJsonEmitters toJsonEmitters)
        {
            _listMethods = listMethods;
            _stringBuilderField = stringBuilderField;
            _typeBuilder = typeBuilder;
            _toJsonEmitters = toJsonEmitters;
        }

        internal override void EmitProperty(IJsonPropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            var propertyValueLocal = generator.DeclareLocal(property.Type);
            var endLabel = generator.DefineLabel();
            var nonNullLabel = generator.DefineLabel();

            getValueOnStack(generator);
            property.EmitGetValue(generator);
            generator.StoreLocal(propertyValueLocal);
            generator.LoadLocal(propertyValueLocal);

            //check for null
            generator.BrIfTrue(nonNullLabel);
            
            //property is null
            generator.Append($"\"{property.Name}\":null");
            generator.Branch(endLabel);

            //property is not null
            generator.Mark(nonNullLabel);
            generator.Append($"\"{property.Name}\":");
            EmitValue(property.Type, (gen, address) => gen.LoadLocal(propertyValueLocal), generator);

            generator.Mark(endLabel);
        }

        internal override void EmitValue(Type type, Action<JsonILGenerator, bool> getValueOnStack, JsonILGenerator generator)
        {
            var methodInfo = _listMethods.GetMethod(type, () => EmitDictionaryMethod(type));
            generator.Pop();     //remove StringBuilder from the stack
            generator.LoadArg(typeof(object), 0, false);
            generator.LoadStaticField(_stringBuilderField);
            getValueOnStack(generator, false);
            generator.Call(methodInfo);
        }

        internal override bool TypeSupported(Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        MethodBuilder EmitDictionaryMethod(Type dictionaryType)
        {

            var methodBuilder = _typeBuilder.DefineMethod(
                "Get" + Guid.NewGuid().ToString().Replace("-", ""),
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(StringBuilder),
                new Type[] { typeof(StringBuilder), dictionaryType});
            
            var generator = new JsonILGenerator(methodBuilder.GetILGenerator(), new StringBuilder());
            generator.LoadArg(typeof(StringBuilder), 1, false);
            generator.Append("{");
            generator.Pop();
            generator.LoadArg(dictionaryType, 2, false);
            var getEnumeratorMethodInfo = dictionaryType.GetRuntimeMethod("GetEnumerator", new Type[]{});
            var enumeratorType = getEnumeratorMethodInfo.ReturnType;
            generator.CallVirtual(getEnumeratorMethodInfo);
            var enumeratorLocal = generator.DeclareLocal(getEnumeratorMethodInfo.ReturnType);
            generator.StoreLocal(enumeratorLocal);
            generator.LoadLocalAddress(enumeratorLocal);
            var moveNextMethod = enumeratorType.GetRuntimeMethod("MoveNext", new Type[0]);
            generator.Call(moveNextMethod);
            Label returnLabel = generator.DefineLabel();
            generator.BranchIfFalse(returnLabel);
            var currentMethod = enumeratorType.GetRuntimeMethod("get_Current", new Type[0]);
            var currentLocal = generator.DeclareLocal(currentMethod.ReturnType);
            EmitCurrentKeyValue(generator, enumeratorLocal, currentMethod, currentLocal);

            var loopConditionLabel = generator.DefineLabel();

            generator.Branch(loopConditionLabel);

            //loop start
            var loopStartLabel = generator.DefineLabel();
            generator.Mark(loopStartLabel);
            generator.LoadArg(typeof(StringBuilder), 1, false);
            generator.Append(",");
            generator.Pop(); //remove StringBuilder from stack
            EmitCurrentKeyValue(generator, enumeratorLocal, currentMethod, currentLocal);

            //loop condition
            generator.Mark(loopConditionLabel);
            generator.LoadLocalAddress(enumeratorLocal);
            generator.Call(moveNextMethod);
            generator.BrIfTrue(loopStartLabel);

            generator.Mark(returnLabel);
            generator.LoadArg(typeof(StringBuilder), 1, false);
            generator.Append("}");
            generator.Return();

            return methodBuilder;
        }

        void EmitCurrentKeyValue(JsonILGenerator generator, LocalBuilder enumeratorLocal, MethodInfo currentMethod, LocalBuilder currentLocal)
        {
            generator.LoadLocalAddress(enumeratorLocal);
            generator.Call(currentMethod);
            generator.StoreLocal(currentLocal);
            generator.LoadArg(typeof(StringBuilder), 1, false);
            //key
            var getKeyMethod = currentMethod.ReturnType.GetRuntimeMethod("get_Key", new Type[0]);
            var keyType = getKeyMethod.ReturnType;
            if(keyType == typeof(string))
            {
                generator.LoadLocalAddress(currentLocal);
                generator.Call(getKeyMethod);
                generator.EmitAppendEscaped();
            }
            else if(keyType.GetTypeInfo().IsPrimitive || keyType == typeof(Guid) || keyType == typeof(DateTime))
            {
                generator.Append("\"");
                _toJsonEmitters.EmitValue(
                    keyType, 
                    (gen, address) => 
                    {
                        gen.LoadLocalAddress(currentLocal);
                        gen.Call(getKeyMethod);
                        if(address)
                        {
                            var local = gen.DeclareLocal(keyType);
                            gen.StoreLocal(local);
                            gen.LoadLocalAddress(local);
                        }
                    },
                    generator);
                generator.Append("\"");
            }
            else if(keyType.GetTypeInfo().IsValueType)
            {
                generator.LoadLocalAddress(currentLocal);
                generator.Call(getKeyMethod);
                var structLocal = generator.DeclareLocal(keyType);
                generator.StoreLocal(structLocal);
                generator.LoadLocalAddress(structLocal);
                generator.Constrain(keyType);
                var toStringMethod = typeof(object).GetRuntimeMethod("ToString", new Type[0]);
                generator.CallVirtual(toStringMethod);
                generator.EmitAppendEscaped();
            }
            else //objects
            {
                generator.LoadLocalAddress(currentLocal);
                generator.Call(getKeyMethod);
                var toStringMethod = typeof(object).GetRuntimeMethod("ToString", new Type[0]);
                generator.Call(toStringMethod);
                generator.EmitAppendEscaped();
            }
            generator.Append(":");
            //value
            var getValueMethod = currentMethod.ReturnType.GetRuntimeMethod("get_Value", new Type[0]);
            
            //generator.LoadArg(typeof(StringBuilder), 1);
            _toJsonEmitters.EmitValue(
                getValueMethod.ReturnType, 
                (gen, address) => 
                {
                    gen.LoadLocalAddress(currentLocal);
                    gen.Call(getValueMethod);
                    if(address)
                    {
                        var local = gen.DeclareLocal(keyType);
                        gen.StoreLocal(local);
                        gen.LoadLocalAddress(local);
                    }
                },
                generator);
            generator.Pop(); //get the StringBuilder off the stack
        }
    }
}