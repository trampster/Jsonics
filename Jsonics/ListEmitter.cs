using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jsonics
{
    public class ListEmitter
    {
        public MethodBuilder EmitArrayMethod(TypeBuilder typeBuilder, Type elementType, StringBuilder appendQueue)
        {
            var methodBuilder = typeBuilder.DefineMethod(
                $"Get{elementType}Array",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(StringBuilder),
                new Type[] { typeof(StringBuilder), elementType.MakeArrayType()});
            
            var generator = new JsonILGenerator(methodBuilder.GetILGenerator(), appendQueue);

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
            generator.LoadArrayElementRef();
            generator.Call(typeof(StringBuilderExtension).GetRuntimeMethod("AppendEscaped", new Type[]{typeof(StringBuilder), typeof(string)}));
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
            generator.LoadArrayElementRef();
            generator.Call(typeof(StringBuilderExtension).GetRuntimeMethod("AppendEscaped", new Type[]{typeof(StringBuilder), typeof(string)}));
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
    }
}