using System;
using System.Reflection;

namespace Jsonics.ToJson
{
    internal class StringEmitter : ToJsonEmitter
    {
        internal override void EmitProperty(IJsonPropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
           generator.Append($"\"{property.Name}\":");

            EmitValue(
                property.Type, 
                gen => 
                {
                    getValueOnStack(gen);
                    property.EmitGetValue(gen);
                },
                generator);
        }

        internal override void EmitValue(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            getValueOnStack(generator);
            generator.EmitAppendEscaped();
        }

        internal override bool TypeSupported(Type type)
        {
            return type == typeof(String);
        }
    }
}