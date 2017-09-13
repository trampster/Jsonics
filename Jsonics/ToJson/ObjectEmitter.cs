using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.ToJson
{
    internal class ObjectEmitter : ToJsonEmitter
    {
        readonly ToJsonEmitters _toJsonEmitters;

        internal ObjectEmitter(ToJsonEmitters toJsonEmitters)
        {
            _toJsonEmitters = toJsonEmitters;
        }

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
            generator.Append("{");

            var propertiesQuery = 
                from property in type.GetRuntimeProperties()
                where property.CanRead && property.CanWrite
                select property;
            var properties = propertiesQuery.ToArray();

            //do the properties
            bool isFirstProperty = true;
            foreach(var property in type.GetRuntimeProperties())
            {
                if(!isFirstProperty)
                {
                    generator.Append(",");
                }
                isFirstProperty = false;
                _toJsonEmitters.EmitProperty(property, getValueOnStack, generator);
            }
            generator.Append("}");
            generator.EmitQueuedAppends();
        }

        internal override bool TypeSupported(Type type)
        {
            return true;
        }
    }
}