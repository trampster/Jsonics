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

        static MethodInfo GetMethod(Type type, TypeBuilder typeBuilder, StringBuilder appendQueue)
        {
            if(!_methodLookup.ContainsKey(type))
            {
                _methodLookup[type] = new ListEmitter().EmitArrayMethod(typeBuilder, type.GetElementType(), appendQueue);
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
            if(type == typeof(string))
            {
                CreateString(jsonILGenerator, gen => gen.LoadArg1());
            }
            else if(type == typeof(int))
            {
                CreateInt(jsonILGenerator, gen => gen.LoadArg1());
            }
            else if(type == typeof(uint) ||
                type == typeof(long) || type == typeof(ulong) ||
                type == typeof(byte) || type == typeof(sbyte) ||
                type == typeof(short) || type == typeof(ushort) ||
                type == typeof(float) || type == typeof(double))
            {
                CreateNumber(jsonILGenerator, gen => gen.LoadArg1(), type);
            }
            else if(type == typeof(bool))
            {
                CreateBool(jsonILGenerator, gen => gen.LoadArg1());
            }
            else if(type == typeof(string[]))
            {
                var methodInfo = GetMethod(type, typeBuilder, jsonILGenerator.AppendQueue);
                jsonILGenerator.Pop();
                jsonILGenerator.LoadArg(0);
                jsonILGenerator.LoadStaticField(builderField);
                jsonILGenerator.LoadArg(1);
                jsonILGenerator.Call(methodInfo);
            }
            else if(type == typeof(List<int>) || type == typeof(int[]) || 
                    type == typeof(List<string>))
            {
                CreateList(type, jsonILGenerator, gen => gen.LoadArg1());
            }
            else if(type == typeof(DateTime))
            {
                CreateDateTime(jsonILGenerator, gen => gen.LoadArg1());
            }
            else if(type == typeof(Guid))
            {
                CreateGuid(jsonILGenerator, gen => gen.LoadArg1());
            }
            else
            {
                GenerateObject<T>(jsonILGenerator);
            }

            jsonILGenerator.CallToString();

            jsonILGenerator.Return();


            var typeInfo = typeBuilder.CreateTypeInfo();
            var myType = typeInfo.AsType();

            return (IJsonConverter<T>)Activator.CreateInstance(myType);
        }

        public static void GenerateObject<T>(JsonILGenerator jsonILGenerator)
        {
            jsonILGenerator.Append("{");

            var type = typeof(T);

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
                    CreateStringProperty(property, jsonILGenerator, gen => gen.LoadArg1());
                }
                else if (property.PropertyType == typeof(int))
                {
                    CreateIntProperty(property, jsonILGenerator, gen => gen.LoadArg1());
                }
                else if(property.PropertyType == typeof(uint) ||
                   property.PropertyType == typeof(long) || property.PropertyType == typeof(ulong) ||
                   property.PropertyType == typeof(byte) || property.PropertyType == typeof(sbyte) ||
                   property.PropertyType == typeof(short) || property.PropertyType == typeof(ushort) ||
                   property.PropertyType == typeof(float) || property.PropertyType == typeof(double))
                {
                    CreateNumberProperty(property, jsonILGenerator, gen => gen.LoadArg1());
                }
                else if(property.PropertyType == typeof(bool))
                {
                    CreateBoolProperty(property, jsonILGenerator, gen => gen.LoadArg1());
                }
                else if(property.PropertyType == typeof(List<int>) || property.PropertyType == typeof(int[]) || 
                    property.PropertyType == typeof(List<string>) || property.PropertyType == typeof(string[]))
                {
                    CreateListProperty(property, jsonILGenerator, gen => gen.LoadArg1());
                }
                else if(property.PropertyType == typeof(DateTime))
                {
                    CreateDateTimeProperty(property, jsonILGenerator, gen => gen.LoadArg1());
                }
                else if(property.PropertyType == typeof(Guid))
                {
                    CreateGuidProperty(property, jsonILGenerator, gen => gen.LoadArg1());
                }
                else
                {
                    throw new NotSupportedException($"PropertyType {property.PropertyType} is not supported.");
                }
            }
            jsonILGenerator.Append("}");
            jsonILGenerator.EmitQueuedAppends();
        }

        static void QueueAppend(StringBuilder queuedAppends, string constant)
        {
            queuedAppends.Append(constant);
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

        static void CreateList(Type type, JsonILGenerator generator, Action<JsonILGenerator> getValueOnStack)
        {
            var propertyValueLocal = generator.DeclareLocal(type);
            var endLabel = generator.DefineLabel();
            var nonNullLabel = generator.DefineLabel();
            
            getValueOnStack(generator);
            generator.StoreLocal(propertyValueLocal);
            generator.LoadLocal(propertyValueLocal);

            //check for null
            generator.BrIfTrue(nonNullLabel);
            
            //property is null
            generator.Append("null");
            generator.Branch(endLabel);

            //property is not null
            generator.Mark(nonNullLabel);
            generator.LoadLocal(propertyValueLocal);

            CreateListValue(generator, type);
            generator.Mark(endLabel);
        }

        static void CreateListProperty(PropertyInfo property, JsonILGenerator generator, Action<JsonILGenerator> loadType)
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
            generator.LoadLocal(propertyValueLocal);

            CreateListValue(generator, property.PropertyType);

            generator.Mark(endLabel);
            
        }

        /// <summary>
        /// Creates a list assuming the following:
        /// 1. The list is already on the top of the stack
        /// 2. The list is not null Value
        /// </summary>
        static void CreateListValue(JsonILGenerator generator, Type listType)
        {
            var specificMethod = typeof(StringBuilderExtension).GetRuntimeMethod("AppendList", new Type[]{typeof(StringBuilder), listType});

            if(specificMethod != null)
            {
                //if we have specific method for this type then we use it otherwise we fall back to the generic one
                generator.Call(specificMethod);
                return;
            }

            Func<ParameterInfo, bool> parametersSelecter = parameter =>  
                listType.IsArray ?
                parameter.ParameterType.IsArray :
                parameter.ParameterType == listType.GetGenericTypeDefinition();

            var methodQuery = 
                from method in typeof(StringBuilderExtension).GetRuntimeMethods()
                where 
                    method.IsGenericMethod && 
                    method.Name == "AppendList" &&
                    method.GetParameters().Where(parametersSelecter).Any()
                select method;
            var genericMethod = methodQuery.First();
            generator.Call(genericMethod.MakeGenericMethod(listType));
        }
    }
}