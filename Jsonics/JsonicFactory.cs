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

            var generator = methodBuilder.GetILGenerator();

            //lazy construct a StringBuilder
            generator.Emit(OpCodes.Ldsfld, builderField);
            
            Label beforeClear = generator.DefineLabel();
            generator.Emit(OpCodes.Brtrue_S, beforeClear);
            generator.Emit(OpCodes.Newobj, typeof(StringBuilder).GetTypeInfo().GetConstructor(new Type[]{}));
            generator.Emit(OpCodes.Stsfld, builderField);
            generator.MarkLabel(beforeClear);
            
            //Clear the StringBuilder
            generator.Emit(OpCodes.Ldsfld, builderField);
            generator.EmitCall(OpCodes.Callvirt, typeof(StringBuilder).GetRuntimeMethod("Clear", new Type[0]), null);            
            
            
            var queuedAppends = new StringBuilder();

            Type type = typeof(T);
            if(type == typeof(string))
            {
                generator.Emit(OpCodes.Ldarg_1);
                EmitAppendEscaped(generator);
            }
            else if(type == typeof(int))
            {
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Call, typeof(StringBuilderExtension).GetRuntimeMethod("AppendInt", new Type[]{typeof(StringBuilder), typeof(int)}));
            }
            else if(type == typeof(uint) ||
                type == typeof(long) || type == typeof(ulong) ||
                type == typeof(byte) || type == typeof(sbyte) ||
                type == typeof(short) || type == typeof(ushort) ||
                type == typeof(float) || type == typeof(double))
            {
                generator.Emit(OpCodes.Ldarg_1);
                EmitAppend(generator, type);
            }
            else if(type == typeof(bool))
            {
                Label trueLable = generator.DefineLabel();
                Label callAppend = generator.DefineLabel();

                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Brtrue, trueLable);
                //false case
                generator.Emit(OpCodes.Ldstr, "false");
                generator.Emit(OpCodes.Br_S, callAppend);
                //true calse
                generator.MarkLabel(trueLable);
                generator.Emit(OpCodes.Ldstr, "true");

                generator.MarkLabel(callAppend);
                EmitAppend(generator, typeof(string));
            }
            else if(type == typeof(List<int>) || type == typeof(int[]) || 
                    type == typeof(List<string>) || type == typeof(string[]))
            {
                CreateList<T>(queuedAppends, type, generator);
            }
            else if(type == typeof(DateTime))
            {
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Call, typeof(StringBuilderExtension).GetRuntimeMethod("AppendDate", new Type[]{typeof(StringBuilder), type}));
            }
            else if(type == typeof(Guid))
            {
                CreateGuid<T>(queuedAppends, generator);
            }
            else
            {
                GenerateObject<T>(generator, queuedAppends);
            }


            generator.Emit(OpCodes.Callvirt, typeof(Object).GetRuntimeMethod("ToString", new Type[0]));

            generator.Emit(OpCodes.Ret);


            var typeInfo = typeBuilder.CreateTypeInfo();
            var myType = typeInfo.AsType();

            return (IJsonConverter<T>)Activator.CreateInstance(myType);
        }

        public static void GenerateObject<T>(ILGenerator generator, StringBuilder queuedAppends)
        {
            queuedAppends.Append("{");

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
                    QueueAppend(queuedAppends, ",");
                }
                isFirstProperty = false;

                if(property.PropertyType == typeof(string))
                {
                    CreateStringProperty<T>(queuedAppends, property, generator);
                }
                else if (property.PropertyType == typeof(int))
                {
                    CreateIntProperty<T>(queuedAppends, property, generator);
                }
                else if(property.PropertyType == typeof(uint) ||
                   property.PropertyType == typeof(long) || property.PropertyType == typeof(ulong) ||
                   property.PropertyType == typeof(byte) || property.PropertyType == typeof(sbyte) ||
                   property.PropertyType == typeof(short) || property.PropertyType == typeof(ushort) ||
                   property.PropertyType == typeof(float) || property.PropertyType == typeof(double))
                {
                    CreateNumberProperty<T>(queuedAppends, property, generator);
                }
                else if(property.PropertyType == typeof(bool))
                {
                    CreateBoolProperty<T>(queuedAppends, property, generator);
                }
                else if(property.PropertyType == typeof(List<int>) || property.PropertyType == typeof(int[]) || 
                    property.PropertyType == typeof(List<string>) || property.PropertyType == typeof(string[]))
                {
                    CreateListProperty<T>(queuedAppends, property, generator);
                }
                else if(property.PropertyType == typeof(DateTime))
                {
                    CreateDateTimeProperty<T>(queuedAppends, property, generator);
                }
                else if(property.PropertyType == typeof(Guid))
                {
                    CreateGuidProperty<T>(queuedAppends, property, generator);
                }
                else
                {
                    throw new NotSupportedException($"PropertyType {property.PropertyType} is not supported.");
                }
            }
            QueueAppend(queuedAppends, "}");
            EmitQueuedAppends(queuedAppends, generator);
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

        static void EmitAppendEscaped(ILGenerator generator)
        {
            generator.Emit(OpCodes.Call, typeof(StringBuilderExtension).GetRuntimeMethod("AppendEscaped", new Type[]{typeof(StringBuilder), typeof(string)}));
        }

        static void CreateStringProperty<T>(StringBuilder queuedAppends, PropertyInfo property, ILGenerator generator)
        {
            QueueAppend(queuedAppends, $"\"{property.Name}\":");
            EmitQueuedAppends(queuedAppends, generator);

            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, typeof(T).GetRuntimeMethod($"get_{property.Name}", new Type[0]));
            EmitAppendEscaped(generator);
        }

        static void CreateIntProperty<T>(StringBuilder queuedAppends, PropertyInfo property, ILGenerator generator)
        {
            QueueAppend(queuedAppends, $"\"{property.Name}\":");
            EmitQueuedAppends(queuedAppends, generator);

            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, typeof(T).GetRuntimeMethod($"get_{property.Name}", new Type[0]));
            generator.Emit(OpCodes.Call, typeof(StringBuilderExtension).GetRuntimeMethod("AppendInt", new Type[]{typeof(StringBuilder), typeof(int)}));
        }

        static void CreateNumberProperty<T>(StringBuilder queuedAppends, PropertyInfo property, ILGenerator generator)
        {
            QueueAppend(queuedAppends, $"\"{property.Name}\":");
            EmitQueuedAppends(queuedAppends, generator);

            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, typeof(T).GetRuntimeMethod($"get_{property.Name}", new Type[0]));
            EmitAppend(generator, property.PropertyType);
        }

        static void CreateDateTimeProperty<T>(StringBuilder queuedAppends, PropertyInfo property, ILGenerator generator)
        {
            QueueAppend(queuedAppends, $"\"{property.Name}\":");
            EmitQueuedAppends(queuedAppends, generator);

            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, typeof(T).GetRuntimeMethod($"get_{property.Name}", new Type[0]));
            generator.Emit(OpCodes.Call, typeof(StringBuilderExtension).GetRuntimeMethod("AppendDate", new Type[]{typeof(StringBuilder), property.PropertyType}));
        }

        static void CreateGuid<T>(StringBuilder queuedAppends, ILGenerator generator)
        {
            var propertyValueLocal = generator.DeclareLocal(typeof(Guid));
            
            QueueAppend(queuedAppends, $"\"");
            EmitQueuedAppends(queuedAppends, generator);

            generator.Emit(OpCodes.Ldarg_1);

            generator.Emit(OpCodes.Stloc, propertyValueLocal);
            generator.Emit(OpCodes.Ldloca_S, propertyValueLocal);

            generator.Emit(OpCodes.Constrained, typeof(Guid));
            generator.Emit(OpCodes.Callvirt, typeof(Object).GetRuntimeMethod("ToString", new Type[0]));
            EmitAppend(generator, typeof(string));

            QueueAppend(queuedAppends, $"\"");
            EmitQueuedAppends(queuedAppends, generator);
        }

        static void CreateGuidProperty<T>(StringBuilder queuedAppends, PropertyInfo property, ILGenerator generator)
        {
            var propertyValueLocal = generator.DeclareLocal(typeof(Guid));
            
            QueueAppend(queuedAppends, $"\"{property.Name}\":\"");
            EmitQueuedAppends(queuedAppends, generator);

            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, typeof(T).GetRuntimeMethod($"get_{property.Name}", new Type[0]));

            generator.Emit(OpCodes.Stloc, propertyValueLocal);
            generator.Emit(OpCodes.Ldloca_S, propertyValueLocal);

            generator.Emit(OpCodes.Constrained, typeof(Guid));
            generator.Emit(OpCodes.Callvirt, typeof(Object).GetRuntimeMethod("ToString", new Type[0]));
            EmitAppend(generator, typeof(string));

            QueueAppend(queuedAppends, $"\"");
        }

        static void CreateBoolProperty<T>(StringBuilder queuedAppends, PropertyInfo property, ILGenerator generator)
        {
            EmitQueuedAppends(queuedAppends, generator);
            Label trueLable = generator.DefineLabel();
            Label callAppend = generator.DefineLabel();

            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, typeof(T).GetRuntimeMethod($"get_{property.Name}", new Type[0]));
            generator.Emit(OpCodes.Brtrue, trueLable);
            //false case
            generator.Emit(OpCodes.Ldstr, $"\"{property.Name}\":false");
            generator.Emit(OpCodes.Br_S, callAppend);
            //true calse
            generator.MarkLabel(trueLable);
            generator.Emit(OpCodes.Ldstr, $"\"{property.Name}\":true");

            generator.MarkLabel(callAppend);
            EmitAppend(generator, typeof(string));
        }

        static void CreateList<T>(StringBuilder queuedAppends, Type type, ILGenerator generator)
        {
            var propertyValueLocal = generator.DeclareLocal(type);
            var endLabel = generator.DefineLabel();
            var nonNullLabel = generator.DefineLabel();

            EmitQueuedAppends(queuedAppends, generator);

            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stloc_0);            
            generator.Emit(OpCodes.Ldloc_0);

            //check for null
            generator.Emit(OpCodes.Brtrue_S, nonNullLabel);
            
            //property is null
            QueueAppend(queuedAppends, "null");
            EmitQueuedAppends(queuedAppends, generator);
            generator.Emit(OpCodes.Br_S, endLabel);

            //property is not null
            generator.MarkLabel(nonNullLabel);
            generator.Emit(OpCodes.Ldloc_0);

            var specificMethod = typeof(StringBuilderExtension).GetRuntimeMethod("AppendList", new Type[]{typeof(StringBuilder), type});

            if(specificMethod != null)
            {
                //if we have specific method for this type then we use it otherwise we fall back to the generic one
                generator.Emit(OpCodes.Call, specificMethod);
                generator.MarkLabel(endLabel);
                return;
            }

            Func<ParameterInfo, bool> parametersSelecter = parameter =>  
                type.IsArray ?
                parameter.ParameterType.IsArray :
                parameter.ParameterType ==type.GetGenericTypeDefinition();

            var methodQuery = 
                from method in typeof(StringBuilderExtension).GetRuntimeMethods()
                where 
                    method.IsGenericMethod && 
                    method.Name == "AppendList" &&
                    method.GetParameters().Where(parametersSelecter).Any()
                select method;
            var genericMethod = methodQuery.First();
            generator.Emit(OpCodes.Call, genericMethod.MakeGenericMethod(type));
            generator.MarkLabel(endLabel);
        }

        static void CreateListProperty<T>(StringBuilder queuedAppends, PropertyInfo property, ILGenerator generator)
        {
            var propertyValueLocal = generator.DeclareLocal(property.PropertyType);
            var endLabel = generator.DefineLabel();
            var nonNullLabel = generator.DefineLabel();

            EmitQueuedAppends(queuedAppends, generator);

            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, typeof(T).GetRuntimeMethod($"get_{property.Name}", new Type[0]));
            generator.Emit(OpCodes.Stloc_0);            
            generator.Emit(OpCodes.Ldloc_0);

            //check for null
            generator.Emit(OpCodes.Brtrue_S, nonNullLabel);
            
            //property is null
            QueueAppend(queuedAppends, $"\"{property.Name}\":null");
            EmitQueuedAppends(queuedAppends, generator);
            generator.Emit(OpCodes.Br_S, endLabel);

            //property is not null
            generator.MarkLabel(nonNullLabel);
            QueueAppend(queuedAppends, $"\"{property.Name}\":");
            EmitQueuedAppends(queuedAppends, generator);
            generator.Emit(OpCodes.Ldloc_0);

            var specificMethod = typeof(StringBuilderExtension).GetRuntimeMethod("AppendList", new Type[]{typeof(StringBuilder), property.PropertyType});

            if(specificMethod != null)
            {
                //if we have specific method for this type then we use it otherwise we fall back to the generic one
                generator.Emit(OpCodes.Call, specificMethod);
                generator.MarkLabel(endLabel);
                return;
            }

            Func<ParameterInfo, bool> parametersSelecter = parameter =>  
                property.PropertyType.IsArray ?
                parameter.ParameterType.IsArray :
                parameter.ParameterType == property.PropertyType.GetGenericTypeDefinition();

            var methodQuery = 
                from method in typeof(StringBuilderExtension).GetRuntimeMethods()
                where 
                    method.IsGenericMethod && 
                    method.Name == "AppendList" &&
                    method.GetParameters().Where(parametersSelecter).Any()
                select method;
            var genericMethod = methodQuery.First();
            generator.Emit(OpCodes.Call, genericMethod.MakeGenericMethod(property.PropertyType));
            generator.MarkLabel(endLabel);
            
        }
    }
}