using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jsonics.ToJson
{
    public class ArrayEmitter : ToJsonEmitter
    {
        readonly ListMethods _listMethods;
        readonly FieldBuilder _stringBuilderField;
        readonly TypeBuilder _typeBuilder;


        public ArrayEmitter(ListMethods listMethods, FieldBuilder stringBuilderField, TypeBuilder typeBuilder)
        {
            _listMethods = listMethods;
            _stringBuilderField = stringBuilderField;
            _typeBuilder = typeBuilder;
        }

        public override void EmitProperty(PropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            var propertyValueLocal = generator.DeclareLocal(property.PropertyType);
            var endLabel = generator.DefineLabel();
            var nonNullLabel = generator.DefineLabel();

            getValueOnStack(generator);
            generator.GetProperty(property);
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
            EmitValue(property.PropertyType, gen => gen.LoadLocal(propertyValueLocal), generator);

            generator.Mark(endLabel);
        }

        public override void EmitValue(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            var methodInfo = _listMethods.GetMethod(type,
                (gen, getElementOnStack) => _listMethods.TypeEmitter.EmitType(type.GetElementType(), gen, getElementOnStack),
                () => EmitArrayMethod(type.GetElementType(), (gen, getElementOnStack) => _listMethods.TypeEmitter.EmitType(type.GetElementType(), gen, getElementOnStack)));
            generator.Pop(); //remove StringBuilder from the stack
            generator.LoadArg(typeof(object), 0);  //load this
            generator.LoadStaticField(_stringBuilderField);
            getValueOnStack(generator);
            generator.Call(methodInfo);
        }

        public override bool TypeSupported(Type type)
        {
            return type.IsArray;
        }

        public MethodBuilder EmitArrayMethod(Type elementType, Action<JsonILGenerator, Action<JsonILGenerator>> emitElement)
        {
            Type arrayType = elementType.MakeArrayType();

            var methodBuilder = _typeBuilder.DefineMethod(
                "Get" + Guid.NewGuid().ToString().Replace("-", ""),
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(StringBuilder),
                new Type[] { typeof(StringBuilder), arrayType});
            
            var generator = new JsonILGenerator(methodBuilder.GetILGenerator(), new StringBuilder());

            var emptyArray = generator.DefineLabel();
            var beforeLoop = generator.DefineLabel();

            generator.LoadArg(arrayType, 2);
            generator.LoadLength();
            generator.ConvertToInt32();
            generator.LoadConstantInt32(1);
            generator.BranchIfLargerThan(emptyArray);

            //length > 1
            generator.LoadArg(typeof(StringBuilder), 1);
            generator.LoadConstantInt32('[');
            generator.EmitAppend(typeof(char));
            generator.LoadArg(arrayType, 2);
            generator.LoadConstantInt32(0);
            emitElement(generator, gen => gen.LoadArrayElement(elementType));
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
            generator.LoadArg(arrayType, 2);
            generator.LoadLocal(indexLocal);
            emitElement(generator, gen => gen.LoadArrayElement(elementType));
            generator.Pop();
            generator.LoadLocal(indexLocal);
            generator.LoadConstantInt32(1);
            generator.Add();
            generator.StoreLocal(indexLocal);

            generator.Mark(lengthCheckLabel);
            generator.LoadLocal(indexLocal);
            generator.LoadArg(arrayType, 2);
            generator.LoadLength();
            generator.ConvertToInt32();
            generator.BranchIfLargerThan(loopStart);
            //end loop

            generator.LoadArg(typeof(StringBuilder), 1);
            generator.LoadConstantInt32(']');
            generator.EmitAppend(typeof(char));
            generator.Return();
            
            return methodBuilder;
        }
    }
}