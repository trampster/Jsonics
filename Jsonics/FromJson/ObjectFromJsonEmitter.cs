using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Jsonics.PropertyHashing;

namespace Jsonics.FromJson
{
    internal class ObjectFromJsonEmitter
    {
        LocalBuilder _propertyNameLocal;
        LocalBuilder _jsonObjectLocal;
        readonly LocalBuilder _lazyStringLocal;
        LocalBuilder _propertyValueStartLocal;
        LocalBuilder _indexLocal;
        LocalBuilder _propertyStartLocal;
        LocalBuilder _propertyEndLocal;
        readonly Type _jsonObjectType;
        readonly JsonILGenerator _generator;

        public ObjectFromJsonEmitter(Type jsonObjectType, LocalBuilder lazyStringLocal, JsonILGenerator generator, LocalBuilder indexLocal)
        {
            _jsonObjectType = jsonObjectType;
            _lazyStringLocal = lazyStringLocal;
            _generator = generator;
            _indexLocal = indexLocal;
        }

        public void EmitObject()
        {
            
            //construct object
            var constructor = _jsonObjectType.GetTypeInfo().GetConstructor(new Type[0]);
            _generator.NewObject(constructor);
            _jsonObjectLocal = _generator.DeclareLocal(_jsonObjectType);
            _generator.StoreLocal(_jsonObjectLocal);

            Label loopCheck =  _generator.DefineLabel();
            
            _generator.Branch(loopCheck);
            
            //loop start
            Label loopStart =  _generator.DefineLabel();
            _generator.Mark(loopStart);

            //read to start of property
            // int indexOfQuote = json.ReadTo(inputIndex, '\"');
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(_indexLocal);
            _generator.LoadConstantInt32('\"');
            var readToMethod = typeof(LazyString).GetRuntimeMethod("ReadTo", new Type[]{typeof(int), typeof(char)});
            _generator.Call(readToMethod);

            //int propertyStart = indexOfQuote + 1;
            _generator.LoadConstantInt32(1);
            _generator.Add();

            //int propertyEnd = json.ReadTo(propertyStart, '\"');
            _propertyStartLocal = _generator.DeclareLocal<int>();
            _generator.StoreLocal(_propertyStartLocal);
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(_propertyStartLocal);
            _generator.LoadConstantInt32('\"');
            _generator.Call(readToMethod);
            _propertyEndLocal = _generator.DeclareLocal<int>();
            _generator.StoreLocal(_propertyEndLocal);

            //var propertyName = json.SubString(propertyStart, propertyEnd - propertyStart);
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(_propertyStartLocal);
            _generator.LoadLocal(_propertyEndLocal);
            _generator.LoadLocal(_propertyStartLocal);
            _generator.Subtract();
            _generator.Call(typeof(LazyString).GetRuntimeMethod("SubString", new Type[]{typeof(int), typeof(int)}));
            _propertyNameLocal = _generator.DeclareLocal<LazyString>();
            _generator.StoreLocal(_propertyNameLocal);
            
            //int intStart = json.ReadTo(propertyEnd + 1, ':') + 1;
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(_propertyEndLocal);
            _generator.LoadConstantInt32(1);
            _generator.Add();
            _generator.LoadConstantInt32(':');
            _generator.Call(readToMethod);
            _generator.LoadConstantInt32(1);
            _generator.Add();
            _propertyValueStartLocal = _generator.DeclareLocal<int>();
            _generator.StoreLocal(_propertyValueStartLocal);

            //properties
            var unknownPropertyLabel = _generator.DefineLabel();
            var propertiesQuery = 
                from property in _jsonObjectType.GetRuntimeProperties()
                where property.CanRead && property.CanWrite
                select property;
            var properties = propertiesQuery.ToArray();
            EmitProperties(properties, loopCheck, unknownPropertyLabel);

            //unknown property
            _generator.Mark(unknownPropertyLabel);
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(_propertyValueStartLocal);
            _generator.Call(typeof(LazyString).GetRuntimeMethod("ReadToPropertyValueEnd", new Type[]{typeof(int)}));
            _generator.StoreLocal(_indexLocal);

            //loopCheck
            _generator.Mark(loopCheck); 
            _generator.LoadArg(typeof(string), 1);
            _generator.LoadLocal(_indexLocal);
            _generator.CallVirtual(typeof(string).GetRuntimeMethod("get_Chars", new Type[]{typeof(int)}));
            _generator.LoadConstantInt32('}');
            _generator.BranchIfNotEqualUnsigned(loopStart);

            //return
            _generator.LoadLocal(_jsonObjectLocal);
            _generator.Return();
        }

        public void EmitProperties(PropertyInfo[] properties, Label loopCheckLabel, Label unknownPropertyLabel)
        {           
            var propertyHandlers = new List<Action>();

            EmitGroup(properties, propertyHandlers, loopCheckLabel, unknownPropertyLabel);

            _generator.Branch(unknownPropertyLabel);
            foreach(var propertyHandler in propertyHandlers)
            {
                propertyHandler();
            }
        }

        void EmitGroup(PropertyInfo[] properties, List<Action> propertyHandlers, Label loopCheckLabel, Label unknownPropertyLabel)
        {
            var propertyHashFactory = new PropertyHashFactory();
            var propertyNames = properties.Select(property => property.Name).ToArray();
            var hashFunction = propertyHashFactory.FindBestHash(propertyNames);

            var hashLocal = hashFunction.EmitHash(_generator, _propertyNameLocal);
            var hashesQuery =
                from property in properties
                let hash = hashFunction.Hash(property.Name)
                group property by hash into hashGroup
                orderby hashGroup.Key
                select hashGroup;
            
            var hashes = hashesQuery.ToArray();
            var switchGroups = FindSwitchGroups(hashes);
            
            foreach(var switchGroup in switchGroups)
            {
                if(switchGroup.Count <= 2)
                {
                    EmitIfGroup(switchGroup, propertyHandlers, unknownPropertyLabel, loopCheckLabel, hashLocal);
                    continue;
                }
                EmitSwitchGroup(switchGroup, propertyHandlers, unknownPropertyLabel, loopCheckLabel, hashLocal);
            }
        }

        void EmitSwitchGroup(SwitchGroup switchGroup, List<Action> propertyHandlers, Label unknownPropertyLabel, Label loopCheckLabel, LocalBuilder hashLocal)
        {
            var jumpTable = new List<Label>();
            int tableIndex = 0;
            int offset = switchGroup[0].Key;

            foreach(var hashGroup in switchGroup)
            {
                if(hashGroup.ToArray()[0].PropertyType != typeof(int))
                {
                    //only support int at this time
                    continue;
                }
                int hashValueIndex = hashGroup.Key - offset;
                while(tableIndex != hashValueIndex)
                {
                    //fill in gaps 
                    tableIndex++;
                    jumpTable.Add(loopCheckLabel);
                }
                
                var propertyLabel = _generator.DefineLabel();
                jumpTable.Add(propertyLabel);

                propertyHandlers.Add(() =>
                {
                    EmitPropertyHandler(propertyLabel, hashGroup, unknownPropertyLabel, loopCheckLabel);
                });
                tableIndex++;
            }
            //substract
            _generator.LoadLocal(hashLocal);
            if(offset != 0)
            {
                _generator.LoadConstantInt32(offset);
                _generator.Subtract();
            }

            //switch
            _generator.Switch(jumpTable.ToArray());
            
        }

        void EmitPropertyHandler(
            Label propertyLabel, IGrouping<int, PropertyInfo> propertyHash, 
            Label unknownPropertyLabel, Label loopCheckLabel)
        {
            _generator.Mark(propertyLabel);
            var subProperties = propertyHash.ToArray();
            if(subProperties.Length != 1)
            {
                //there was a hash collision so need to nest
                EmitProperties(subProperties, loopCheckLabel, unknownPropertyLabel);
                return;
            }
            var property = subProperties[0];
            _generator.LoadLocalAddress(_propertyNameLocal);
            _generator.LoadString(property.Name);
            _generator.Call(typeof(LazyString).GetRuntimeMethod("EqualsString", new Type[]{typeof(string)}));
            _generator.BranchIfFalse(unknownPropertyLabel);
            //Parse property value
            _generator.LoadLocal(_jsonObjectLocal);
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(_propertyValueStartLocal);
            Type tupleType = LazyStringCallToX<int>("ToInt", _generator);
            _generator.Duplicate();

            _generator.LoadField(tupleType.GetRuntimeField("Item2"));
            _generator.StoreLocal(_indexLocal);

            _generator.LoadField(tupleType.GetRuntimeField("Item1"));
            
            _generator.CallVirtual(_jsonObjectType.GetRuntimeMethod($"set_{property.Name}", new Type[]{property.PropertyType}));
            _generator.Branch(loopCheckLabel);
        }

        void EmitIfGroup(
            SwitchGroup ifGroup, List<Action> propertyHandlers, 
            Label unknownPropertyLabel, Label loopCheckLabel, LocalBuilder hashLocal)
        {
            foreach(var hashGroup in ifGroup)
            {
                if(hashGroup.ToArray()[0].PropertyType != typeof(int))
                {
                    //only support int at this time
                    continue;
                }
                int hash = hashGroup.Key;
                var propertyLabel = _generator.DefineLabel();
                _generator.LoadLocal(hashLocal);
                if(hash == 0)
                {
                    _generator.BranchIfFalse(propertyLabel);
                }
                else
                {
                    _generator.LoadConstantInt32(hash);
                    _generator.BranchIfEqual(propertyLabel);
                }
                propertyHandlers.Add(() =>
                {
                    EmitPropertyHandler(propertyLabel, hashGroup, unknownPropertyLabel, loopCheckLabel);
                });
            }
        }

        IEnumerable<SwitchGroup> FindSwitchGroups(IGrouping<int, PropertyInfo>[] hashes)
        {
            int last = 0;
            int gaps = 0;
            var switchGroup = new SwitchGroup();
            foreach(var grouping in hashes)
            {
                int hash = grouping.Key;
                gaps += hash - last -1;
                if(gaps > 8)
                {
                    //to many gaps this switch group is finished
                    yield return switchGroup;
                    switchGroup = new SwitchGroup();
                    gaps = 0;
                }
                switchGroup.Add(grouping);
            }
            yield return switchGroup;
        }

        class SwitchGroup : List<IGrouping<int, PropertyInfo>>{}

        Type LazyStringCallToX<T>(string methodName, JsonILGenerator generator)
        {
            generator.Call(typeof(LazyString).GetRuntimeMethod(methodName, new Type[]{typeof(T)}));
            return typeof(ValueTuple<T,int>);
        }
    }
}