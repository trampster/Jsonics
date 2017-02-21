using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

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

            var methodBuilder = typeBuilder.DefineMethod(
                "ToJson",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(string),
                new Type[] { typeof(T) });

            var generator = methodBuilder.GetILGenerator();
        
            var builder = new StringBuilder();
            builder.Append("{{");
            var type = typeof(T);
            var emitProperties = new List<Action>();
            int propertyIndex = 0;

            bool isFirstProperty = true;
            foreach(var property in type.GetRuntimeProperties())
            {
                if((!property.CanRead && property.CanWrite))
                {
                    continue;
                }
                
                builder.Append($"\"{property.Name}\":\"{{{propertyIndex}}}\"");
                if(isFirstProperty)
                {
                    builder.Append(",");
                    isFirstProperty = false;
                }
                emitProperties.Add(() => 
                { 
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Call, typeof(T).GetRuntimeMethod($"get_{property.Name}", new Type[0]));
                });
                propertyIndex++;
            }
            builder.Append("}}");

            //string.Format
            generator.Emit(OpCodes.Ldstr, builder.ToString());

            foreach(var emitProperty in emitProperties)
            {
                emitProperty();
            }

            generator.Emit(OpCodes.Call, typeof(string).GetRuntimeMethod("Format", new Type[] {typeof(string), typeof(object), typeof(object)} ));

            generator.Emit(OpCodes.Ret);


            var typeInfo = typeBuilder.CreateTypeInfo();
            var myType = typeInfo.AsType();
            return (IJsonConverter<T>)Activator.CreateInstance(myType);
        }
    }
}