using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jsonics
{
    public class ObjectEmitter : Emitter
    {
        public ObjectEmitter(TypeBuilder typeBuilder, StringBuilder appendQueue, Emitters emitters, FieldBuilder stringBuilderField)
            : base(typeBuilder, appendQueue, emitters, stringBuilderField)
        {
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

                if(property.PropertyType == typeof(string))
                {
                    CreateStringProperty(property, jsonILGenerator, getTypeOnStack);
                }
                else if (property.PropertyType == typeof(int) || property.PropertyType.GetTypeInfo().IsEnum)
                {
                    CreateIntProperty(property, jsonILGenerator, getTypeOnStack);
                }
                else if(property.PropertyType == typeof(uint) ||
                   property.PropertyType == typeof(long) || property.PropertyType == typeof(ulong) ||
                   property.PropertyType == typeof(byte) || property.PropertyType == typeof(sbyte) ||
                   property.PropertyType == typeof(short) || property.PropertyType == typeof(ushort) ||
                   property.PropertyType == typeof(float) || property.PropertyType == typeof(double))
                {
                    CreateNumberProperty(property, jsonILGenerator, getTypeOnStack);
                }
                else if(property.PropertyType == typeof(bool))
                {
                    CreateBoolProperty(property, jsonILGenerator, getTypeOnStack);
                }
                else if(property.PropertyType.IsArray)
                {
                    CreateArrayProperty(property, jsonILGenerator, getTypeOnStack);
                }
                else if(property.PropertyType.GetTypeInfo().IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    CreateListProperty(property, jsonILGenerator, getTypeOnStack);
                }
                else if(property.PropertyType.GetTypeInfo().IsGenericType && property.PropertyType .GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    CreateDictionaryProperty(property, jsonILGenerator, getTypeOnStack);
                }
                else if(property.PropertyType == typeof(DateTime))
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

        void CreateStringProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            generator.Append($"\"{property.Name}\":");

            _emitters.ValueEmitter.CreateString(generator, gen => 
            {
                loadType(gen);
                gen.GetProperty(property);
            });
        }

        void CreateIntProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            generator.Append($"\"{property.Name}\":");

            _emitters.ValueEmitter.CreateInt(generator, gen =>
            {
                loadType(generator);
                generator.GetProperty(property);
            });
        }

        void CreateNumberProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            generator.Append($"\"{property.Name}\":");

            _emitters.ValueEmitter.CreateNumber(generator, gen => 
            {
                loadType(generator);
                generator.GetProperty(property);
            },
            property.PropertyType);
        }

        void CreateDateTimeProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            generator.Append($"\"{property.Name}\":");

            _emitters.ValueEmitter.CreateDateTime(generator, gen =>
            {
                loadType(gen);
                gen.GetProperty(property);
            });
        }

        void CreateGuidProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            generator.Append($"\"{property.Name}\":");

            _emitters.ValueEmitter.CreateGuid(generator, gen =>
            {
                loadType(gen);
                gen.GetProperty(property);
            });
        }

        void CreateBoolProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            Label trueLabel = generator.DefineLabel();
            Label callAppend = generator.DefineLabel();

            loadType(generator);
            generator.GetProperty(property);
            generator.BrIfTrue(trueLabel);

            //false case
            generator.LoadString($"\"{property.Name}\":false");
            generator.Branch(callAppend);

            //true case
            generator.Mark(trueLabel);
            generator.LoadString($"\"{property.Name}\":true");

            generator.Mark(callAppend);
            generator.EmitAppend(typeof(string));
        }

        void CreateArrayProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            var propertyValueLocal = generator.DeclareLocal(property.PropertyType);
            var endLabel = generator.DefineLabel();
            var nonNullLabel = generator.DefineLabel();

            loadType(generator);
            generator.GetProperty(property);
            generator.StoreLocal(propertyValueLocal);
            generator.LoadLocal(propertyValueLocal);

            //check for null
            generator.BrIfTrue(nonNullLabel);
            
            //property is null
            generator.Append($"\"{property.Name}\":null");
            generator.Branch(endLabel);

            //property is not null
            generator.Mark(nonNullLabel);
            generator.Append($"\"{property.Name}\":");
            _emitters.ValueEmitter.CreateArrayValue(property.PropertyType, generator, gen => gen.LoadLocal(propertyValueLocal));

            generator.Mark(endLabel);
            
        }

        void CreateListProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            var propertyValueLocal = generator.DeclareLocal(property.PropertyType);
            var endLabel = generator.DefineLabel();
            var nonNullLabel = generator.DefineLabel();

            loadType(generator);
            generator.GetProperty(property);
            generator.StoreLocal(propertyValueLocal);
            generator.LoadLocal(propertyValueLocal);

            //check for null
            generator.BrIfTrue(nonNullLabel);
            
            //property is null
            generator.Append($"\"{property.Name}\":null");
            generator.Branch(endLabel);

            //property is not null
            generator.Mark(nonNullLabel);
            generator.Append($"\"{property.Name}\":");
            _emitters.ValueEmitter.CreateListValue(property.PropertyType, generator, gen => gen.LoadLocal(propertyValueLocal));

            generator.Mark(endLabel);
        }

        void CreateDictionaryProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            var propertyValueLocal = generator.DeclareLocal(property.PropertyType);
            var endLabel = generator.DefineLabel();
            var nonNullLabel = generator.DefineLabel();

            loadType(generator);
            generator.GetProperty(property);
            generator.StoreLocal(propertyValueLocal);
            generator.LoadLocal(propertyValueLocal);

            //check for null
            generator.BrIfTrue(nonNullLabel);
            
            //property is null
            generator.Append($"\"{property.Name}\":null");
            generator.Branch(endLabel);

            //property is not null
            generator.Mark(nonNullLabel);
            generator.Append($"\"{property.Name}\":");
            _emitters.ValueEmitter.CreateDictionaryValue(property.PropertyType, generator, gen => gen.LoadLocal(propertyValueLocal));

            generator.Mark(endLabel);
        }
    }
}