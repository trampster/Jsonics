using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jsonics.ToJson
{
    internal class ListEmitter : ToJsonEmitter
    {
        readonly ListMethods _listMethods;
        readonly FieldBuilder _stringBuilderField;
        readonly TypeBuilder _typeBuilder;
        readonly ToJsonEmitters _toJsonEmitters;


        internal ListEmitter(ListMethods listMethods, FieldBuilder stringBuilderField, TypeBuilder typeBuilder, ToJsonEmitters toJsonEmitters)
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
            EmitValue(property.Type, (gen, address) => 
            {
                if(address)
                {
                    gen.LoadLocalAddress(propertyValueLocal);    
                }
                else
                {
                    gen.LoadLocal(propertyValueLocal);
                }
            }, generator);

            generator.Mark(endLabel);
        }

        internal override void EmitValue(Type type, Action<JsonILGenerator, bool> getValueOnStack, JsonILGenerator generator)
        {
            Action<JsonILGenerator, Action<JsonILGenerator, bool>> emitElement = (gen, getElementOnStack) => _toJsonEmitters.EmitValue(type.GenericTypeArguments[0], getElementOnStack, gen);
            var methodInfo = _listMethods.GetMethod(
                type, 
                () => EmitListMethod(type, type.GenericTypeArguments[0], emitElement));
            generator.Pop();     //remove StringBuilder from the stack
            generator.LoadArg(typeof(object), 0, false);
            generator.LoadStaticField(_stringBuilderField);
            getValueOnStack(generator, false);
            generator.Call(methodInfo);
        }

        internal override bool TypeSupported(Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        MethodBuilder EmitListMethod(Type listType, Type elementType, Action<JsonILGenerator, Action<JsonILGenerator, bool>> emitElement)
        {
            var methodBuilder = _typeBuilder.DefineMethod(
                "Get" + Guid.NewGuid().ToString().Replace("-", ""),
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(StringBuilder),
                new Type[] { typeof(StringBuilder), listType});
            
            var generator = new JsonILGenerator(methodBuilder.GetILGenerator(), new StringBuilder());

            var emptyArray = generator.DefineLabel();
            var beforeLoop = generator.DefineLabel();

            generator.LoadArg(listType, 2, false);
            generator.CallVirtual(listType.GetRuntimeMethod("get_Count", new Type[0]));
            generator.LoadConstantInt32(1);
            generator.BranchIfLargerThan(emptyArray);

            // //length > 1
            generator.LoadArg(typeof(StringBuilder), 1, false);
            generator.LoadConstantInt32('[');
            generator.EmitAppend(typeof(char));
            
            emitElement(generator, (gen, address) => 
            {
                gen.LoadArg(listType, 2, false);
                gen.LoadConstantInt32(0);
                gen.LoadListElement(listType);
                if(address)
                {
                    var local = gen.DeclareLocal(elementType);
                    gen.StoreLocal(local);
                    gen.LoadLocalAddress(local);
                }
            });
            generator.Pop();
            generator.Branch(beforeLoop);

            //empty array
            generator.Mark(emptyArray);
            generator.LoadArg(typeof(StringBuilder), 1, false);
            generator.Append("[]");
            generator.Return();

            //before loop            
            generator.Mark(beforeLoop);
            generator.LoadConstantInt32(1);
            var indexLocal = generator.DeclareLocal(typeof(int));
            generator.StoreLocal(indexLocal);

            var lengthCheckLabel = generator.DefineLabel();
            generator.Branch(lengthCheckLabel);

            //loop start
            var loopStart = generator.DefineLabel();
            generator.Mark(loopStart);
            generator.LoadArg(typeof(StringBuilder), 1, false);
            generator.LoadConstantInt32(',');
            generator.EmitAppend(typeof(char));
            emitElement(generator, (gen, address) => 
            {
                gen.LoadArg(listType, 2, false);
                gen.LoadLocal(indexLocal);
                gen.LoadListElement(listType);
                if(address)
                {
                    var local = gen.DeclareLocal(elementType);
                    gen.StoreLocal(local);
                    gen.LoadLocalAddress(local);
                }
            });
            generator.Pop();
            generator.LoadLocal(indexLocal);
            generator.LoadConstantInt32(1);
            generator.Add();
            generator.StoreLocal(indexLocal);

            generator.Mark(lengthCheckLabel);
            generator.LoadLocal(indexLocal);
            generator.LoadArg(listType, 2, false);
            generator.CallVirtual(listType.GetRuntimeMethod("get_Count", new Type[0]));
            generator.ConvertToInt32();
            generator.BranchIfLargerThan(loopStart);
            //end loop

            generator.LoadArg(typeof(StringBuilder), 1, false);
            generator.LoadConstantInt32(']');
            generator.EmitAppend(typeof(char));
            generator.Return();
            
            return methodBuilder;
        }
    }
}