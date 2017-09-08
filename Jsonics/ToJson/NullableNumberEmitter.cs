using System;
using System.Reflection;

namespace Jsonics.ToJson
{
    public class NullableNumberEmitter : ToJsonEmitter
    {
        public override void EmitProperty(PropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            Type type = property.PropertyType;
            Type underlyingType = Nullable.GetUnderlyingType(type);
            var propertyValueLocal = generator.DeclareLocal(type);
            var endLabel = generator.DefineLabel();
            var nonNullLabel = generator.DefineLabel();

            getValueOnStack(generator);
            generator.GetProperty(property);
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
            generator.LoadLocalAddress(propertyValueLocal);
            generator.Call(type.GetTypeInfo().GetMethod("get_Value", new Type[0]));
            generator.EmitAppend(underlyingType);
            generator.Mark(endLabel);
        }

        public override void EmitValue(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type);
            getValueOnStack(generator);
            var hasValueLabel = generator.DefineLabel();
            var endLabel = generator.DefineLabel();

            generator.Call(type.GetTypeInfo().GetMethod("get_HasValue", new Type[0]));
            generator.BrIfTrue(hasValueLabel);

            generator.Append("null");
            generator.Branch(endLabel);

            generator.Mark(hasValueLabel);
            getValueOnStack(generator);
            generator.Call(type.GetTypeInfo().GetMethod("get_Value", new Type[0]));
            generator.EmitAppend(underlyingType);
            generator.Mark(endLabel);
        }

        public override bool TypeSupported(Type type)
        {
            return 
                type == typeof(uint?) ||
                type == typeof(long?) || type == typeof(ulong?) ||
                type == typeof(byte?) || type == typeof(sbyte?) ||
                type == typeof(short?) || type == typeof(ushort?) ||
                type == typeof(float?) || type == typeof(double?);
        }
    }
}