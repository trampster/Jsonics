using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.ToJson
{
    public class DateTimeEmitter : ToJsonEmitter
    {
        public override void EmitProperty(PropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
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

        public override void EmitValue(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            getValueOnStack(generator);
            generator.AppendDate();
        }

        public override bool TypeSupported(Type type)
        {
            return type == typeof(DateTime);
        }
    }
}