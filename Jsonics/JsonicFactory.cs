using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Linq;

namespace Jsonics
{
    public class JsonFactory
    {
        static Dictionary<Type, MethodInfo> _methodLookup = new Dictionary<Type, MethodInfo>();

        static MethodInfo GetMethod(Type type, TypeBuilder typeBuilder, StringBuilder appendQueue, Action<JsonILGenerator, Action<JsonILGenerator>> emitElement)
        {
            if(!_methodLookup.ContainsKey(type))
            {
                if(type.IsArray)
                {
                    _methodLookup[type] = new ListEmitter().EmitArrayMethod(typeBuilder, type.GetElementType(), appendQueue, emitElement);
                }
                else if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    _methodLookup[type] = new ListEmitter().EmitListMethod(typeBuilder, type, type.GenericTypeArguments[0],  appendQueue, emitElement);
                }
            }
            return _methodLookup[type];
        }

        public static IJsonConverter<T> Compile<T>()
        {
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName(Guid.NewGuid().ToString()),
                AssemblyBuilderAccess.Run);
            var module = assemblyBuilder.DefineDynamicModule("module1");

            var typeBuilder = module.DefineType("Person.Endurer", TypeAttributes.Public|TypeAttributes.Class);
            typeBuilder.AddInterfaceImplementation(typeof(IJsonConverter<T>));

            //create a StringBuilder cache per instance
            var builderField = typeBuilder.DefineField("_builder", typeof(StringBuilder), FieldAttributes.Static | FieldAttributes.Private);
            ConstructorInfo attributeConstructor = typeof(ThreadStaticAttribute).GetTypeInfo().GetConstructor(new Type[]{});
            builderField.SetCustomAttribute(attributeConstructor, new byte[]{01,00,00,00});

            var methodBuilder = typeBuilder.DefineMethod(
                "ToJson",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(string),
                new Type[] { typeof(T) });

            var jsonILGenerator = new JsonILGenerator(methodBuilder.GetILGenerator(), new StringBuilder());
            
            //lazy construct a StringBuilder
            jsonILGenerator.LoadStaticField(builderField);
            
            Label beforeClear = jsonILGenerator.DefineLabel();
            jsonILGenerator.BrIfTrue(beforeClear);
            jsonILGenerator.NewObject(typeof(StringBuilder).GetTypeInfo().GetConstructor(new Type[]{}));
            jsonILGenerator.StoreStaticField(builderField);
            jsonILGenerator.Mark(beforeClear);
            
            //Clear the StringBuilder
            jsonILGenerator.LoadStaticField(builderField);
            jsonILGenerator.CallVirtual(typeof(StringBuilder).GetRuntimeMethod("Clear", new Type[0]));

            Type type = typeof(T);
            EmitType(type, jsonILGenerator, typeBuilder, builderField, gen => gen.LoadArg(1));

            jsonILGenerator.CallToString();

            jsonILGenerator.Return();


            var typeInfo = typeBuilder.CreateTypeInfo();
            var myType = typeInfo.AsType();

            return (IJsonConverter<T>)Activator.CreateInstance(myType);
        }

        public static void EmitType(Type type, JsonILGenerator generator, TypeBuilder typeBuilder, FieldBuilder stringBuilderField, Action<JsonILGenerator> getTypeOnStack)
        {
            if(type == typeof(string))
            {
                CreateString(generator, getTypeOnStack);
            }
            else if(type == typeof(int))
            {
                CreateInt(generator, getTypeOnStack);
            }
            else if(type == typeof(uint) ||
                type == typeof(long) || type == typeof(ulong) ||
                type == typeof(byte) || type == typeof(sbyte) ||
                type == typeof(short) || type == typeof(ushort) ||
                type == typeof(float) || type == typeof(double))
            {
                CreateNumber(generator, getTypeOnStack, type);
            }
            else if(type == typeof(bool))
            {
                CreateBool(generator, getTypeOnStack);
            }
            else if(type.IsArray)
            {
                CreateArrayValue(type, typeBuilder, generator, stringBuilderField, getTypeOnStack);
            }
            else if(type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                CreateListValue(type, typeBuilder, generator, stringBuilderField, getTypeOnStack);
            }
            else if(type == typeof(DateTime))
            {
                CreateDateTime(generator, getTypeOnStack);
            }
            else if(type == typeof(Guid))
            {
                CreateGuid(generator, getTypeOnStack);
            }
            else
            {
                GenerateObject(type, generator, getTypeOnStack, typeBuilder, stringBuilderField);
            }
        }

        public static void CreateArrayValue(Type type, TypeBuilder typeBuilder, JsonILGenerator generator, FieldBuilder stringBuilderField, Action<JsonILGenerator> getTypeOnStack)
        {
            var methodInfo = GetMethod(type, typeBuilder, generator.AppendQueue, (gen, getElementOnStack) => EmitType(type.GetElementType(), gen, typeBuilder, stringBuilderField, getElementOnStack));
            generator.Pop();
            generator.LoadArg(0);
            generator.LoadStaticField(stringBuilderField);
            getTypeOnStack(generator);
            generator.Call(methodInfo);
        }

        public static void CreateListValue(Type type, TypeBuilder typeBuilder, JsonILGenerator generator, FieldBuilder stringBuilderField, Action<JsonILGenerator> getTypeOnStack)
        {
            var methodInfo = GetMethod(type, typeBuilder, generator.AppendQueue, (gen, getElementOnStack) => EmitType(type.GenericTypeArguments[0], gen, typeBuilder, stringBuilderField, getElementOnStack));
            generator.Pop();     //remove StringBuilder from the stack
            generator.LoadArg(0);
            generator.LoadStaticField(stringBuilderField);
            getTypeOnStack(generator);
            generator.Call(methodInfo);
        }


        public static void GenerateObject(Type type, JsonILGenerator jsonILGenerator, Action<JsonILGenerator> getTypeOnStack, TypeBuilder typeBuilder, FieldBuilder stringBuilderField)
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
                else if (property.PropertyType == typeof(int))
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
                    CreateArrayProperty(property, jsonILGenerator, getTypeOnStack, typeBuilder, stringBuilderField);
                }
                else if(property.PropertyType.GetTypeInfo().IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    CreateListProperty(property, jsonILGenerator, getTypeOnStack, typeBuilder, stringBuilderField);
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
                    throw new NotSupportedException($"PropertyType {property.PropertyType} is not supported.");
                }
            }
            jsonILGenerator.Append("}");
            jsonILGenerator.EmitQueuedAppends();
        }

        static void EmitQueuedAppends(StringBuilder queuedAppends, ILGenerator generator)
        {
            if(queuedAppends.Length == 0)
            {
                return;
            }
            generator.Emit(OpCodes.Ldstr, queuedAppends.ToString());
            EmitAppend(generator, typeof(string));
            queuedAppends.Clear();
        }

        static void EmitAppend(ILGenerator generator, Type type)
        {
            generator.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetRuntimeMethod("Append", new Type[]{type}));
        }

        static void CreateStringProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            generator.Append($"\"{property.Name}\":");

            CreateString(generator, gen => 
            {
                loadType(gen);
                gen.GetProperty(property);
            });
        }

        static void CreateString(JsonILGenerator generator, Action<JsonILGenerator> getValueOnStack)
        {
            getValueOnStack(generator);
            generator.EmitAppendEscaped();
        }

        static void CreateIntProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            generator.Append($"\"{property.Name}\":");

            CreateInt(generator, gen =>
            {
                loadType(generator);
                generator.GetProperty(property);
            });
        }

        static void CreateInt(JsonILGenerator generator, Action<JsonILGenerator> getValueOnStack)
        {
            getValueOnStack(generator);
            generator.AppendInt();
        }

        static void CreateNumberProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            generator.Append($"\"{property.Name}\":");

            CreateNumber(generator, gen => 
            {
                loadType(generator);
                generator.GetProperty(property);
            },
            property.PropertyType);
        }

        static void CreateNumber(JsonILGenerator generator, Action<JsonILGenerator> getValueOnStack, Type numberType)
        {
            getValueOnStack(generator);
            generator.EmitAppend(numberType);
        }

        static void CreateDateTimeProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            generator.Append($"\"{property.Name}\":");

            CreateDateTime(generator, gen =>
            {
                loadType(gen);
                gen.GetProperty(property);
            });
        }

        static void CreateDateTime(JsonILGenerator generator, Action<JsonILGenerator> getValueOnStack)
        {
            getValueOnStack(generator);
            generator.AppendDate();
        }

        static void CreateGuid(JsonILGenerator generator, Action<JsonILGenerator> getValueOnStack)
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

        static void CreateGuidProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            generator.Append($"\"{property.Name}\":");

            CreateGuid(generator, gen =>
            {
                loadType(gen);
                gen.GetProperty(property);
            });
        }

        static void CreateBoolProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
        {
            Label trueLabel = generator.DefineLabel();
            Label callAppend = generator.DefineLabel();

            loadType(generator);
            generator.GetProperty(property);
            generator.BrIfTrue(trueLabel);

            //false case
            generator.LoadString($"\"{property.Name}\":false");
            generator.Branch(callAppend);

            //true calse
            generator.Mark(trueLabel);
            generator.LoadString($"\"{property.Name}\":true");

            generator.Mark(callAppend);
            generator.EmitAppend(typeof(string));
        }

        static void CreateBool(JsonILGenerator generator, Action<JsonILGenerator> getValueOnStack)
        {
            Label trueLabel = generator.DefineLabel();
            Label callAppend = generator.DefineLabel();

            getValueOnStack(generator);
            generator.BrIfTrue(trueLabel);

            //false case
            generator.LoadString("false");
            generator.Branch(callAppend);

            //true calse
            generator.Mark(trueLabel);
            generator.LoadString($"true");

            generator.Mark(callAppend);
            generator.EmitAppend(typeof(string));
        }

        static void CreateArrayProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType, TypeBuilder typeBuilder, FieldBuilder stringBuilderField)
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
            CreateArrayValue(property.PropertyType, typeBuilder, generator, stringBuilderField, gen => gen.LoadLocal(propertyValueLocal));

            generator.Mark(endLabel);
            
        }

        static void CreateListProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType, TypeBuilder typeBuilder, FieldBuilder stringBuilderField)
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
            CreateListValue(property.PropertyType, typeBuilder, generator, stringBuilderField, gen => gen.LoadLocal(propertyValueLocal));

            generator.Mark(endLabel);
            
        }
    }
}