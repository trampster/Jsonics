using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.ToJson
{
    internal class GuidEmitter : ToJsonEmitter
    {
        internal override void EmitProperty(IJsonPropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            generator.Append($"\"{property.Name}\":");

            EmitValue(
            property.Type, 
            (gen, address) =>
            {
                getValueOnStack(gen);
                property.EmitGetValue(gen);
                if(address)
                {
                    var local = gen.DeclareLocal(property.Type);
                    gen.StoreLocal(local);
                    gen.LoadLocalAddress(local);
                }
            },
            generator);
        }

        internal override void EmitValue(Type type, Action<JsonILGenerator, bool> getValueOnStack, JsonILGenerator generator)
        {
            generator.Append($"\"");

            getValueOnStack(generator, true);

            generator.Constrain<Guid>();
            generator.CallToString();
            generator.EmitAppend(typeof(string));

            generator.Append($"\"");
            generator.EmitQueuedAppends();
        }

        internal override bool TypeSupported(Type type)
        {
            return type == typeof(Guid);
        }
    }
}