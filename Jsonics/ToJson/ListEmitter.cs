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


        public ListEmitter(ListMethods listMethods, FieldBuilder stringBuilderField, TypeBuilder typeBuilder)
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
            var methodInfo = _listMethods.GetMethod(type, (gen, getElementOnStack) => _listMethods.TypeEmitter.EmitType(type.GenericTypeArguments[0], gen, getElementOnStack), null);
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
    }
}