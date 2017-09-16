using System;
using System.Reflection;

namespace Jsonics.ToJson
{
    internal class NullableIntEmitter : ToJsonEmitter
    {
        internal override void EmitProperty(IJsonPropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            var propertyType = property.Type;
            var propertyValueLocal = generator.DeclareLocal(propertyType);
            var endLabel = generator.DefineLabel();
            var nonNullLabel = generator.DefineLabel();

            getValueOnStack(generator);
            property.EmitGetValue(generator);
            generator.StoreLocal(propertyValueLocal);
            generator.LoadLocalAddress(propertyValueLocal);

            //check for null
            generator.Call(propertyType.GetTypeInfo().GetMethod("get_HasValue", new Type[0]));
            generator.BrIfTrue(nonNullLabel);
            
            //property is null
            generator.Append($"\"{property.Name}\":null");
            generator.Branch(endLabel);

            //property is not null
            generator.Mark(nonNullLabel);
            generator.Append($"\"{property.Name}\":");
            generator.LoadLocalAddress(propertyValueLocal);
            generator.Call(propertyType.GetTypeInfo().GetMethod("get_Value", new Type[0]));
            generator.AppendInt();
            generator.Mark(endLabel);
        }

        internal override void EmitValue(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            getValueOnStack(generator);
            var hasValueLabel = generator.DefineLabel();
            var endLabel = generator.DefineLabel();

            generator.Call(typeof(int?).GetTypeInfo().GetMethod("get_HasValue", new Type[0]));
            generator.BrIfTrue(hasValueLabel);

            generator.Append("null");
            generator.Branch(endLabel);

            generator.Mark(hasValueLabel);
            getValueOnStack(generator);
            generator.Call(type.GetTypeInfo().GetMethod("get_Value", new Type[0]));
            generator.AppendInt();
            generator.Mark(endLabel);
        }

        internal override bool TypeSupported(Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if(underlyingType == null)
            {
                return false;
            }
            return underlyingType == typeof(int) || underlyingType.GetTypeInfo().IsEnum;
        }
    }
}