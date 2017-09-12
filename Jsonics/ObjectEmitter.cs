using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Jsonics.ToJson;

namespace Jsonics
{
    public class ObjectEmitter : Emitter
    {
        readonly ToJsonEmitters _toJsonEmitters;

        public ObjectEmitter(TypeBuilder typeBuilder, StringBuilder appendQueue, ListMethods listMethods, FieldBuilder stringBuilderField, ToJsonEmitters toJsonEmitters)
            : base(typeBuilder, appendQueue, listMethods, stringBuilderField)
        {
            _toJsonEmitters = toJsonEmitters;
        }

        public void GenerateObject(Type type, JsonILGenerator jsonILGenerator, Action<JsonILGenerator> getTypeOnStack)
        {
            jsonILGenerator.Append("{");

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
                    jsonILGenerator.Append(",");
                }
                isFirstProperty = false;
                if(_toJsonEmitters.EmitProperty(property, getTypeOnStack, jsonILGenerator))
                {
                    continue;
                }
                if(property.PropertyType == typeof(DateTime))
                {
                    CreateDateTimeProperty(property, jsonILGenerator, getTypeOnStack);
                }
                else if(property.PropertyType == typeof(Guid))
                {
                    CreateGuidProperty(property, jsonILGenerator, getTypeOnStack);
                }
                else
                {
                    CreateObjectProperty(property, jsonILGenerator, getTypeOnStack);
                }
            }
            jsonILGenerator.Append("}");
            jsonILGenerator.EmitQueuedAppends();
        }

        void CreateObjectProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            generator.Append($"\"{property.Name}\":");

            GenerateObject(property.PropertyType, generator, gen => 
            {
                loadType(gen);
                gen.GetProperty(property);
            });
        }

        void CreateDateTimeProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            generator.Append($"\"{property.Name}\":");

            _listMethods.ValueEmitter.CreateDateTime(generator, gen =>
            {
                loadType(gen);
                gen.GetProperty(property);
            });
        }

        void CreateGuidProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            generator.Append($"\"{property.Name}\":");

            _listMethods.ValueEmitter.CreateGuid(generator, gen =>
            {
                loadType(gen);
                gen.GetProperty(property);
            });
        }
    }
}