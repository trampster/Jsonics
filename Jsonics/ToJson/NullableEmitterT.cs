using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.ToJson
{
    internal class NullableEmitter<T> : ToJsonEmitter where T : struct
    {
        readonly ToJsonEmitters _toJsonEmitters;

        internal NullableEmitter(ToJsonEmitters toJsonEmitters)
        {
            _toJsonEmitters = toJsonEmitters;
        }

        internal override void EmitProperty(IJsonPropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            Type type = property.Type;
            Type underlyingType = Nullable.GetUnderlyingType(type);
            var propertyValueLocal = generator.DeclareLocal(type);
            var endLabel = generator.DefineLabel();
            var nonNullLabel = generator.DefineLabel();

            getValueOnStack(generator);
            property.EmitGetValue(generator);
            generator.StoreLocal(propertyValueLocal);
            generator.LoadLocalAddress(propertyValueLocal);

            //check for null
            generator.Call(type.GetTypeInfo().GetMethod("get_HasValue", new Type[0]));
            generator.BrIfTrue(nonNullLabel);
            
            //property is null
            generator.Append($"\"{property.Name}\":null");
            generator.Branch(endLabel);

            //property is not null
            generator.Mark(nonNullLabel);

            generator.Append($"\"{property.Name}\":");

            _toJsonEmitters.EmitValue(
                underlyingType, 
                gen =>
                {
                    gen.LoadLocalAddress(propertyValueLocal);
                    gen.Call(type.GetTypeInfo().GetMethod("get_Value", new Type[0]));
                },
                generator);

            generator.Mark(endLabel);
        }

        internal override void EmitValue(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            getValueOnStack(generator);
            var hasValueLabel = generator.DefineLabel();
            var endLabel = generator.DefineLabel();

            generator.Call(type.GetTypeInfo().GetMethod("get_HasValue", new Type[0]));
            generator.BrIfTrue(hasValueLabel);

            generator.Append("null");
            generator.Branch(endLabel);

            //has value
            generator.Mark(hasValueLabel);

            _toJsonEmitters.EmitValue(
                Nullable.GetUnderlyingType(type),
                gen =>
                {
                    getValueOnStack(generator);
                    gen.Call(type.GetTypeInfo().GetMethod("get_Value", new Type[0]));
                },
                generator);

            //end
            generator.Mark(endLabel);
        }

        internal override bool TypeSupported(Type type)
        {
            return type == typeof(T?);
        }
    }
}