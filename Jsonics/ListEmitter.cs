using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jsonics
{
    public class ListEmitter : Emitter
    {
        public ListEmitter(TypeBuilder typeBuilder, StringBuilder appendQueue, ListMethods emitters, FieldBuilder stringBuilderField)
            : base(typeBuilder, appendQueue, emitters, stringBuilderField)
        {
        }

        public MethodBuilder EmitListMethod(Type listType, Type elementType, Action<JsonILGenerator, Action<JsonILGenerator>> emitElement)
        {
            var methodBuilder = _typeBuilder.DefineMethod(
                "Get" + Guid.NewGuid().ToString().Replace("-", ""),
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(StringBuilder),
                new Type[] { typeof(StringBuilder), listType});
            
            var generator = new JsonILGenerator(methodBuilder.GetILGenerator(), new StringBuilder());

            var emptyArray = generator.DefineLabel();
            var beforeLoop = generator.DefineLabel();

            generator.LoadArg(listType, 2);
            generator.CallVirtual(listType.GetRuntimeMethod("get_Count", new Type[0]));
            generator.LoadConstantInt32(1);
            generator.BranchIfLargerThan(emptyArray);

            //length > 1
            generator.LoadArg(typeof(StringBuilder), 1);
            generator.LoadConstantInt32('[');
            generator.EmitAppend(typeof(char));
            generator.LoadArg(listType, 2);
            generator.LoadConstantInt32(0);
            emitElement(generator, gen => gen.LoadListElement(listType));
            generator.Pop();
            generator.Branch(beforeLoop);

            //empty array
            generator.Mark(emptyArray);
            generator.LoadArg(typeof(StringBuilder), 1);
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
            generator.LoadArg(typeof(StringBuilder), 1);
            generator.LoadConstantInt32(',');
            generator.EmitAppend(typeof(char));
            generator.LoadArg(listType, 2);
            generator.LoadLocal(indexLocal);
            emitElement(generator, gen => gen.LoadListElement(listType));
            generator.Pop();
            generator.LoadLocal(indexLocal);
            generator.LoadConstantInt32(1);
            generator.Add();
            generator.StoreLocal(indexLocal);

            generator.Mark(lengthCheckLabel);
            generator.LoadLocal(indexLocal);
            generator.LoadArg(listType, 2);
            generator.CallVirtual(listType.GetRuntimeMethod("get_Count", new Type[0]));
            generator.ConvertToInt32();
            generator.BranchIfLargerThan(loopStart);
            //end loop

            generator.LoadArg(typeof(StringBuilder), 1);
            generator.LoadConstantInt32(']');
            generator.EmitAppend(typeof(char));
            generator.Return();
            
            return methodBuilder;
        }

        public MethodBuilder EmitDictionaryMethod(Type dictionaryType)
        {

            var methodBuilder = _typeBuilder.DefineMethod(
                "Get" + Guid.NewGuid().ToString().Replace("-", ""),
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(StringBuilder),
                new Type[] { typeof(StringBuilder), dictionaryType});
            
            var generator = new JsonILGenerator(methodBuilder.GetILGenerator(), new StringBuilder());
            generator.LoadArg(typeof(StringBuilder), 1);
            generator.Append("{");
            generator.Pop();
            generator.LoadArg(dictionaryType, 2);
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
            generator.LoadArg(typeof(StringBuilder), 1);
            generator.Append(",");
            generator.Pop(); //remove StringBuilder from stack
            EmitCurrentKeyValue(generator, enumeratorLocal, currentMethod, currentLocal);

            //loop condition
            generator.Mark(loopConditionLabel);
            generator.LoadLocalAddress(enumeratorLocal);
            generator.Call(moveNextMethod);
            generator.BrIfTrue(loopStartLabel);

            generator.Mark(returnLabel);
            generator.LoadArg(typeof(StringBuilder), 1);
            generator.Append("}");
            generator.Return();

            return methodBuilder;
        }

        void EmitCurrentKeyValue(JsonILGenerator generator, LocalBuilder enumeratorLocal, MethodInfo currentMethod, LocalBuilder currentLocal)
        {
            generator.LoadLocalAddress(enumeratorLocal);
            generator.Call(currentMethod);
            generator.StoreLocal(currentLocal);
            generator.LoadArg(typeof(StringBuilder), 1);
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
                _listMethods.TypeEmitter.EmitType(keyType, generator, gen => 
                {
                    gen.LoadLocalAddress(currentLocal);
                    gen.Call(getKeyMethod);
                });
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
            _listMethods.TypeEmitter.EmitType(getValueMethod.ReturnType, generator, gen => 
            {
                gen.LoadLocalAddress(currentLocal);
                gen.Call(getValueMethod);
            });
            generator.Pop(); //get the StringBuilder off the stack
        }
    }
}