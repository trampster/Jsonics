using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.ToJson
{
    internal class DateTimeEmitter : ToJsonEmitter
    {
        internal override void EmitProperty(PropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            generator.Append($"\"{property.Name}\":");

            EmitValue(property.PropertyType,
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
            generator.AppendDate();
        }

        internal override bool TypeSupported(Type type)
        {
            return type == typeof(DateTime);
        }
    }
}