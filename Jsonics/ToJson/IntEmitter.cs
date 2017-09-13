using System;
using System.Reflection;

namespace Jsonics.ToJson
{
    internal class IntEmitter : ToJsonEmitter
    {
        internal override void EmitProperty(PropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            generator.Append($"\"{property.Name}\":");

            EmitValue(
                property.PropertyType, 
                gen =>
                {
                    getValueOnStack(gen);
                    gen.GetProperty(property);
                },
                generator);
        }

        internal override void EmitValue(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            getValueOnStack(generator);
            generator.AppendInt();
        }

        internal override bool TypeSupported(Type type)
        {
            return type == typeof(int) || type.GetTypeInfo().IsEnum;
        }
    }
}