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
            generator.Append("{");

            bool isFirst = true;
            EmitProperties(type, getValueOnStack, generator, ref isFirst);
            EmitFields(type, getValueOnStack, generator, ref isFirst);

            generator.Append("}");
            generator.EmitQueuedAppends();
        }

        void EmitFields(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator, ref bool isFirst)
        {
            var fieldsQuery = 
                from field in type.GetRuntimeFields()
                where field.IsPublic
                select field;
            var fields = fieldsQuery.ToArray();

            //do the properties
            foreach(var field in fieldsQuery)
            {
                if(!isFirst)
                {
                    generator.Append(",");
                }
                isFirst = false;
                _toJsonEmitters.EmitProperty(new JsonFieldInfo(field), getValueOnStack, generator);
            }
        }

        void EmitProperties(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator, ref bool isFirst)
        {
            var propertiesQuery = 
                from property in type.GetRuntimeProperties()
                where property.CanRead && property.GetGetMethod().IsPublic
                select property;
            var properties = propertiesQuery.ToArray();

            //do the properties
            foreach(var property in properties)
            {
                if(!isFirst)
                {
                    generator.Append(",");
                }
                isFirst = false;
                _toJsonEmitters.EmitProperty(new JsonPropertyInfo(property), getValueOnStack, generator);
            }
        }

        internal override bool TypeSupported(Type type)
        {
            return true;
        }
    }
}