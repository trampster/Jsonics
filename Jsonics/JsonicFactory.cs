using System;
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
            
            var emitters = new Emitters(typeBuilder, jsonILGenerator.AppendQueue);
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
            emitters.TypeEmitter.EmitType(type, jsonILGenerator, typeBuilder, builderField, gen => gen.LoadArg(1));

            jsonILGenerator.CallToString();

            jsonILGenerator.Return();


            var typeInfo = typeBuilder.CreateTypeInfo();
            var myType = typeInfo.AsType();

            return (IJsonConverter<T>)Activator.CreateInstance(myType);
        }

        
    }
}