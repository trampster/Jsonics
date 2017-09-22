using System;
using System.Reflection;

namespace Jsonics.ToJson
{
    internal class NullableCharEmitter : ToJsonEmitter
    {
        internal override void EmitProperty(IJsonPropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            generator.Append($"\"{property.Name}\":");

            EmitValue(
                property.Type, 
                gen => 
                {
                    getValueOnStack(generator);
                    property.EmitGetValue(generator);
                },
                generator);
        }

        internal override void EmitValue(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            getValueOnStack(generator);
            generator.EmitAppendNullableChar();
        }

        internal override bool TypeSupported(Type type)
        {
            return type == typeof(char?);
        }
    }
}