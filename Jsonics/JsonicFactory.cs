using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Linq;
using Jsonics.PropertyHashing;
using Jsonics.FromJson;
using System.Collections.Generic;

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

            CreateToJsonMethod<T>(typeBuilder, builderField);

            CreateFromJsonMethod<T>(typeBuilder);

            var defaultConstructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[0]);
            var defautlConstructorGenerator = new JsonILGenerator(defaultConstructor.GetILGenerator(), new StringBuilder());
            defautlConstructorGenerator.Return();

            var typeInfo = typeBuilder.CreateTypeInfo();
            var myType = typeInfo.AsType();

            return (IJsonConverter<T>)Activator.CreateInstance(myType);
        }

        static void CreateFromJsonMethod<T>(TypeBuilder typeBuilder)
        {
            var type = typeof(T);
            var methodBuilder = typeBuilder.DefineMethod(
                "FromJson",
                MethodAttributes.Public | MethodAttributes.Virtual,
                type,
                new Type[] { typeof(string) });

            var jsonILGenerator = new JsonILGenerator(methodBuilder.GetILGenerator(), new StringBuilder());


            //new LazyString(input)
            var lazyStringLocal = jsonILGenerator.DeclareLocal<LazyString>();
            jsonILGenerator.LoadLocalAddress(lazyStringLocal);
            jsonILGenerator.LoadArg(typeof(string), 1);
            var lazyStringConstructor = typeof(LazyString).GetTypeInfo().GetConstructor(new Type[]{typeof(string)});
            jsonILGenerator.Call(lazyStringConstructor);

            //inputIndex = 0
            var indexLocal = jsonILGenerator.DeclareLocal<int>();
            jsonILGenerator.LoadConstantInt32(0);
            jsonILGenerator.StoreLocal(indexLocal);

            //define static constructor
            var staticConstructor = typeBuilder.DefineConstructor(MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.Private | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[]{});
            var staticConstructorGenerator = new JsonILGenerator(staticConstructor.GetILGenerator(), new StringBuilder());
            var staticFields = new Dictionary<Type, FieldBuilder>();
            var emitters = new FromJsonEmitters(
                type, lazyStringLocal, jsonILGenerator, 
                staticFieldType => AddStaticField(staticFieldType, typeBuilder, staticConstructorGenerator, staticFields));
            emitters.Emit(indexLocal, type);

            //finish static constructor
            staticConstructorGenerator.Return();
            
            jsonILGenerator.Return();
        }

        static FieldBuilder AddStaticField(Type type, TypeBuilder typeBuilder, JsonILGenerator generator, Dictionary<Type, FieldBuilder> staticFields)
        {
            if(staticFields.TryGetValue(type, out FieldBuilder fieldBuilder))
            {
                return fieldBuilder;
            }

            //add field
            var fieldName = Guid.NewGuid().ToString().Replace("-","");
            var field = typeBuilder.DefineField(fieldName, type, FieldAttributes.Static | FieldAttributes.Private);
            ConstructorInfo attributeConstructor = typeof(ThreadStaticAttribute).GetTypeInfo().GetConstructor(new Type[]{});
            field.SetCustomAttribute(attributeConstructor, new byte[]{01,00,00,00});

            //add to static constructor
            generator.NewObject(type.GetTypeInfo().GetConstructor(new Type[0]));
            generator.StoreStaticField(field);
            staticFields.Add(type, field);
            return field;
        }

        static void CreateToJsonMethod<T>(TypeBuilder typeBuilder, FieldBuilder builderField)
        {
            var methodBuilder = typeBuilder.DefineMethod(
                "ToJson",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(string),
                new Type[] { typeof(T) });

            var jsonILGenerator = new JsonILGenerator(methodBuilder.GetILGenerator(), new StringBuilder());
            
            var emitters = new Emitters(typeBuilder, jsonILGenerator.AppendQueue, builderField);
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
            emitters.TypeEmitter.EmitType(type, jsonILGenerator, gen => gen.LoadArg(type, 1));

            jsonILGenerator.CallToString();

            jsonILGenerator.Return();
        }

        
    }
}