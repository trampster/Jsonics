using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jsonics
{
    public class ListEmitter : Emitter
    {
        public ListEmitter(TypeBuilder typeBuilder, StringBuilder appendQueue, Emitters emitters, FieldBuilder stringBuilderField)
            : base(typeBuilder, appendQueue, emitters, stringBuilderField)
        {
        }

        public MethodBuilder EmitArrayMethod(Type elementType, Action<JsonILGenerator, Action<JsonILGenerator>> emitElement)
        {
            var methodBuilder = _typeBuilder.DefineMethod(
                "Get" + Guid.NewGuid().ToString().Replace("-", ""),
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(StringBuilder),
                new Type[] { typeof(StringBuilder), elementType.MakeArrayType()});
            
            var generator = new JsonILGenerator(methodBuilder.GetILGenerator(), new StringBuilder());

            var emptyArray = generator.DefineLabel();
            var beforeLoop = generator.DefineLabel();

            generator.LoadArg(2);
            generator.LoadLength();
            generator.ConvertToInt32();
            generator.LoadConstantInt32(1);
            generator.BranchIfLargerThan(emptyArray);

            //length > 1
            generator.LoadArg(1);
            generator.LoadConstantInt32('[');
            generator.EmitAppend(typeof(char));
            generator.LoadArg(2);
            generator.LoadConstantInt32(0);
            emitElement(generator, gen => gen.LoadArrayElement(elementType));
            generator.Pop();
            generator.Branch(beforeLoop);

            //empty array
            generator.Mark(emptyArray);
            generator.LoadArg(1);
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
            generator.LoadArg(1);
            generator.LoadConstantInt32(',');
            generator.EmitAppend(typeof(char));
            generator.LoadArg(2);
            generator.LoadLocal(indexLocal);
            emitElement(generator, gen => gen.LoadArrayElement(elementType));
            generator.Pop();
            generator.LoadLocal(indexLocal);
            generator.LoadConstantInt32(1);
            generator.Add();
            generator.StoreLocal(indexLocal);

            generator.Mark(lengthCheckLabel);
            generator.LoadLocal(indexLocal);
            generator.LoadArg(2);
            generator.LoadLength();
            generator.ConvertToInt32();
            generator.BranchIfLargerThan(loopStart);
            //end loop

            generator.LoadArg(1);
            generator.LoadConstantInt32(']');
            generator.EmitAppend(typeof(char));
            generator.Return();
            
            return methodBuilder;
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

            generator.LoadArg(2);
            generator.CallVirtual(listType.GetRuntimeMethod("get_Count", new Type[0]));
            generator.LoadConstantInt32(1);
            generator.BranchIfLargerThan(emptyArray);

            //length > 1
            generator.LoadArg(1);
            generator.LoadConstantInt32('[');
            generator.EmitAppend(typeof(char));
            generator.LoadArg(2);
            generator.LoadConstantInt32(0);
            emitElement(generator, gen => gen.LoadListElement(listType));
            generator.Pop();
            generator.Branch(beforeLoop);

            //empty array
            generator.Mark(emptyArray);
            generator.LoadArg(1);
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
            generator.LoadArg(1);
            generator.LoadConstantInt32(',');
            generator.EmitAppend(typeof(char));
            generator.LoadArg(2);
            generator.LoadLocal(indexLocal);
            emitElement(generator, gen => gen.LoadListElement(listType));
            generator.Pop();
            generator.LoadLocal(indexLocal);
            generator.LoadConstantInt32(1);
            generator.Add();
            generator.StoreLocal(indexLocal);

            generator.Mark(lengthCheckLabel);
            generator.LoadLocal(indexLocal);
            generator.LoadArg(2);
            generator.CallVirtual(listType.GetRuntimeMethod("get_Count", new Type[0]));
            generator.ConvertToInt32();
            generator.BranchIfLargerThan(loopStart);
            //end loop

            generator.LoadArg(1);
            generator.LoadConstantInt32(']');
            generator.EmitAppend(typeof(char));
            generator.Return();
            
            return methodBuilder;
        }
    }
}