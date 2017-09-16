using System;
using System.Reflection;

namespace Jsonics.ToJson
{
    internal class NumberEmitter : ToJsonEmitter
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
            generator.EmitAppend(type);
        }

        internal override bool TypeSupported(Type type)
        {
            return 
                type == typeof(uint) ||
                type == typeof(long) || type == typeof(ulong) ||
                type == typeof(byte) || type == typeof(sbyte) ||
                type == typeof(short) || type == typeof(ushort) ||
                type == typeof(float) || type == typeof(double);
        }
    }
}