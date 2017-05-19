using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Jsonics.PropertyHashing;

namespace Jsonics.FromJson
{
    public class ObjectFromJsonEmitter
    {
        public void EmitObject(Type type, LocalBuilder lazyStringLocal, JsonILGenerator generator, LocalBuilder indexLocal)
        {
            //construct object
            var constructor = type.GetTypeInfo().GetConstructor(new Type[0]);
            generator.NewObject(constructor);
            var jsonObjectLocal = generator.DeclareLocal(type);
            generator.StoreLocal(jsonObjectLocal);

            Label loopCheck =  generator.DefineLabel();
            
            generator.Branch(loopCheck);
            
            //loop start
            Label loopStart =  generator.DefineLabel();
            generator.Mark(loopStart);

            //read to start of property
            generator.LoadLocalAddress(lazyStringLocal);
            generator.LoadLocal(indexLocal);
            generator.LoadConstantInt32('\"');
            var readToMethod = typeof(LazyString).GetRuntimeMethod("ReadTo", new Type[]{typeof(int), typeof(char)});
            generator.Call(readToMethod);
            var indexOfQuoteLocal = generator.DeclareLocal<int>();
            generator.StoreLocal(indexOfQuoteLocal);
            generator.LoadLocal(indexOfQuoteLocal);
            generator.LoadConstantInt32(-1);
            var returnLabel = generator.DefineLabel();
            generator.BranchIfEqual(returnLabel);

            //read property name
            generator.LoadLocal(indexOfQuoteLocal);
            generator.LoadConstantInt32(1);
            generator.Add();
            var propertyStartLocal = generator.DeclareLocal<int>();
            generator.StoreLocal(propertyStartLocal);
            generator.LoadLocalAddress(lazyStringLocal);
            generator.LoadLocal(propertyStartLocal);
            generator.LoadConstantInt32('\"');
            generator.Call(readToMethod);
            var propertyEndLocal = generator.DeclareLocal<int>();
            generator.StoreLocal(propertyEndLocal);
            generator.LoadLocalAddress(lazyStringLocal);
            generator.LoadLocal(propertyStartLocal);
            generator.LoadLocal(propertyEndLocal);
            generator.LoadLocal(propertyStartLocal);
            generator.Subtract();
            generator.Call(typeof(LazyString).GetRuntimeMethod("SubString", new Type[]{typeof(int), typeof(int)}));
            var propertyNameLocal = generator.DeclareLocal<LazyString>();
            generator.StoreLocal(propertyNameLocal);
            
            //int intStart = json.ReadTo(propertyEnd + 1, ':') + 1;
            generator.LoadLocalAddress(lazyStringLocal);
            generator.LoadLocal(propertyEndLocal);
            generator.LoadConstantInt32(1);
            generator.Add();
            generator.LoadConstantInt32(':');
            generator.Call(typeof(LazyString).GetRuntimeMethod("ReadTo", new Type[]{typeof(int), typeof(char)}));
            generator.LoadConstantInt32(1);
            generator.Add();
            var propertyValueStartLocal = generator.DeclareLocal<int>();
            generator.StoreLocal(propertyValueStartLocal);

            var unknownPropertyLabel = generator.DefineLabel();
            var propertiesQuery = 
                from property in type.GetRuntimeProperties()
                where property.CanRead && property.CanWrite
                select property;
            var properties = propertiesQuery.ToArray();
            EmitProperties(type, properties, generator, propertyNameLocal, loopCheck, propertyValueStartLocal, lazyStringLocal, jsonObjectLocal, indexLocal, unknownPropertyLabel);

            //unknown property
            generator.Mark(unknownPropertyLabel);
            generator.LoadLocalAddress(lazyStringLocal);
            generator.LoadLocal(propertyStartLocal);
            generator.Call(typeof(LazyString).GetRuntimeMethod("ReadToPropertyValueEnd", new Type[]{typeof(int)}));
            generator.StoreLocal(indexLocal);

            //loopCheck
            generator.Mark(loopCheck);
            generator.LoadLocal(indexLocal);
            generator.LoadArg(typeof(string),1);
            generator.CallVirtual(typeof(string).GetRuntimeMethod("get_Length", new Type[0]));
            generator.BranchIfLargerThan(loopStart);

            //return
            generator.Mark(returnLabel);
            generator.LoadLocal(jsonObjectLocal);
            generator.Return();
        }

        public void EmitProperties(Type jsonObjectType, PropertyInfo[] properties, JsonILGenerator generator, LocalBuilder propertyNameLocal, Label loopCheckLabel, LocalBuilder propertyValueStartLocal, LocalBuilder lazyStringLocal, LocalBuilder jsonObjectLocal, LocalBuilder indexLocal, Label unknownPropertyLabel)
        {           
            var propertyHashFactory = new PropertyHashFactory();
            var propertyNames = properties.Select(property => property.Name).ToArray();
            var hashFunction = propertyHashFactory.FindBestHash(propertyNames);

            LocalBuilder hashLocal = hashFunction.EmitHash(generator, propertyNameLocal);
            var hashes =
                from property in properties
                let hash = hashFunction.Hash(property.Name)
                group property by hash;
            var propertyHandlers = new List<Action>();
            foreach(var propertyHash in hashes)
            {
                if(propertyHash.ToArray()[0].PropertyType != typeof(int))
                {
                    //only support int at this time
                    continue;
                }
                int hash = propertyHash.Key;
                var propertyLabel = generator.DefineLabel();
                generator.LoadLocal(hashLocal);
                if(hash == 0)
                {
                    generator.BranchIfFalse(propertyLabel);
                }
                else
                {
                    generator.LoadConstantInt32(hash);
                    generator.BranchIfEqual(propertyLabel);
                }
               
                propertyHandlers.Add(() =>
                {
                    generator.Mark(propertyLabel);
                    var subProperties = propertyHash.ToArray();
                    if(subProperties.Length != 1)
                    {
                        throw new NotImplementedException("need to nest because there is a hash collision");
                    }
                    var property = subProperties[0];
                    generator.LoadLocalAddress(propertyNameLocal);
                    generator.LoadString(property.Name);
                    generator.Call(typeof(LazyString).GetRuntimeMethod("EqualsString", new Type[]{typeof(string)}));
                    generator.BranchIfFalse(unknownPropertyLabel);
                    generator.LoadLocal(jsonObjectLocal);
                    generator.LoadLocalAddress(lazyStringLocal);
                    generator.LoadLocal(propertyValueStartLocal);
                    Type tupleType = LazyStringCallToX<int>("ToInt", generator);
                    generator.Duplicate();
                    generator.LoadField(tupleType.GetRuntimeField("Item1"));
                    var propertyValueLocal = generator.DeclareLocal(property.PropertyType);
                    generator.StoreLocal(propertyValueLocal);
                    generator.LoadField(tupleType.GetRuntimeField("Item2"));
                    generator.StoreLocal(indexLocal);
                    generator.LoadLocal(propertyValueLocal);
                    generator.CallVirtual(jsonObjectType.GetRuntimeMethod($"set_{property.Name}", new Type[]{property.PropertyType}));
                    generator.Branch(loopCheckLabel);
                });
            }
            
            generator.Branch(unknownPropertyLabel);
            foreach(var propertyHandler in propertyHandlers)
            {
                propertyHandler();
            }
        }

        Type LazyStringCallToX<T>(string methodName, JsonILGenerator generator)
        {
            generator.Call(typeof(LazyString).GetRuntimeMethod(methodName, new Type[]{typeof(T)}));
            return typeof(ValueTuple<T,int>);
        }
    }
}