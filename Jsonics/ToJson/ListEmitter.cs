using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jsonics.ToJson
{
    public class ListEmitter : ToJsonEmitter
    {
        readonly ListMethods _listMethods;
        readonly FieldBuilder _stringBuilderField;
        readonly TypeBuilder _typeBuilder;
        readonly ToJsonEmitters _toJsonEmitters;


        public ListEmitter(ListMethods listMethods, FieldBuilder stringBuilderField, TypeBuilder typeBuilder, ToJsonEmitters toJsonEmitters)
        {
            _listMethods = listMethods;
            _stringBuilderField = stringBuilderField;
            _typeBuilder = typeBuilder;
            _toJsonEmitters = toJsonEmitters;
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
            Action<JsonILGenerator, Action<JsonILGenerator>> emitElement = (gen, getElementOnStack) => _toJsonEmitters.EmitValue(type.GenericTypeArguments[0], getElementOnStack, gen);
            var methodInfo = _listMethods.GetMethod(
                type, 
                () => EmitListMethod(type, type.GenericTypeArguments[0], emitElement));
            generator.Pop();     //remove StringBuilder from the stack
            generator.LoadArg(typeof(object), 0);
            generator.LoadStaticField(_stringBuilderField);
            getValueOnStack(generator);
            generator.Call(methodInfo);
        }

        public override bool TypeSupported(Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        MethodBuilder EmitListMethod(Type listType, Type elementType, Action<JsonILGenerator, Action<JsonILGenerator>> emitElement)
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
    }
}