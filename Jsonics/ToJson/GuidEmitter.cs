using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.ToJson
{
    public class GuidEmitter : ToJsonEmitter
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
            var propertyValueLocal = generator.DeclareLocal<Guid>();
            
            generator.Append($"\"");

            getValueOnStack(generator);

            generator.StoreLocal(propertyValueLocal);
            generator.LoadLocalAddress(propertyValueLocal);

            generator.Constrain<Guid>();
            generator.CallToString();
            generator.EmitAppend(typeof(string));

            generator.Append($"\"");
            generator.EmitQueuedAppends();
        }

        public override bool TypeSupported(Type type)
        {
            return type == typeof(Guid);
        }
    }
}