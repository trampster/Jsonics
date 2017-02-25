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
                else if(property.PropertyType == typeof(int) || property.PropertyType == typeof(uint) ||
                   property.PropertyType == typeof(long) || property.PropertyType == typeof(ulong) ||
                   property.PropertyType == typeof(byte) || property.PropertyType == typeof(sbyte) ||
                   property.PropertyType == typeof(short) || property.PropertyType == typeof(ushort) ||
                   property.PropertyType == typeof(float) || property.PropertyType == typeof(double))
                {
                    CreateNumber32BitProperty<T>(queuedAppends, property, generator);
                }
                else if(property.PropertyType == typeof(bool))
                {
                    CreateBoolProperty<T>(queuedAppends, property, generator);
                }
                else
                {
                    throw new NotSupportedException($"PropertyType {property.PropertyType} is not supported.");
                }
            }
            QueueAppend(queuedAppends, "}");
            EmitQueuedAppends(queuedAppends, generator);

            generator.Emit(OpCodes.Callvirt, typeof(Object).GetRuntimeMethod("ToString", new Type[0]));

            generator.Emit(OpCodes.Ret);


            var typeInfo = typeBuilder.CreateTypeInfo();
            var myType = typeInfo.AsType();

            return (IJsonConverter<T>)Activator.CreateInstance(myType);
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

        static void CreateStringProperty<T>(StringBuilder queuedAppends, PropertyInfo property, ILGenerator generator)
        {
            QueueAppend(queuedAppends, $"\"{property.Name}\":\"");
            EmitQueuedAppends(queuedAppends, generator);

            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, typeof(T).GetRuntimeMethod($"get_{property.Name}", new Type[0]));
            EmitAppend(generator, typeof(string));

            QueueAppend(queuedAppends, "\"");
        }

        static void CreateNumber32BitProperty<T>(StringBuilder queuedAppends, PropertyInfo property, ILGenerator generator)
        {
            QueueAppend(queuedAppends, $"\"{property.Name}\":");
            EmitQueuedAppends(queuedAppends, generator);

            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, typeof(T).GetRuntimeMethod($"get_{property.Name}", new Type[0]));
            EmitAppend(generator, property.PropertyType);
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
    }
}