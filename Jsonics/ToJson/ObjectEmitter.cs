using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.ToJson
{
    public class ObjectEmitter : ToJsonEmitter
    {
        readonly ToJsonEmitters _toJsonEmitters;

        public ObjectEmitter(ToJsonEmitters toJsonEmitters)
        {
            _toJsonEmitters = toJsonEmitters;
        }

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

        public override bool TypeSupported(Type type)
        {
            return true;
        }
    }
}