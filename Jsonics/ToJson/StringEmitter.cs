using System;
using System.Reflection;

namespace Jsonics.ToJson
{
    public class StringEmitter : ToJsonEmitter
    {
        public override void EmitProperty(PropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
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

        public override void EmitValue(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            getValueOnStack(generator);
            generator.EmitAppendEscaped();
        }

        public override bool TypeSupported(Type type)
        {
            return type == typeof(String);
        }
    }
}