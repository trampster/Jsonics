using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Jsonics.FromJson.PropertyHashing;

namespace Jsonics.FromJson
{
    internal class ObjectFromJsonEmitter : FromJsonEmitter
    {
        LocalBuilder _propertyNameLocal;
        LocalBuilder _jsonObjectLocal;
        LocalBuilder _indexLocal;
        LocalBuilder _propertyStartLocal;
        LocalBuilder _propertyEndLocal;
        Type _jsonObjectType;

        internal ObjectFromJsonEmitter(LocalBuilder lazyStringLocal, JsonILGenerator generator, FromJsonEmitters emitters)
            : base(lazyStringLocal, generator, emitters)
        {
            _lazyStringLocal = lazyStringLocal;
            _generator = generator;
        }

        internal override void Emit(LocalBuilder indexLocal, Type jsonObjectType)
        {
            _jsonObjectType = jsonObjectType;
            _indexLocal = indexLocal;
            //construct object
            var constructor = _jsonObjectType.GetTypeInfo().GetConstructor(new Type[0]);
            _generator.NewObject(constructor);
            _jsonObjectLocal = _generator.DeclareLocal(_jsonObjectType);
            _generator.StoreLocal(_jsonObjectLocal);

            
            //null check
            //(inputIndex, character) = json.ReadToAny(inputIndex, '{', 'n');
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(_indexLocal);
            _generator.LoadConstantInt32('{');
            _generator.LoadConstantInt32('n');
            var readToAnyMethod = typeof(LazyString).GetRuntimeMethod("ReadToAny", new Type[]{typeof(int), typeof(char), typeof(char)});
            _generator.Call(readToAnyMethod);
            _generator.Duplicate();
            Type tupleType = typeof(ValueTuple<int,char>);
            _generator.LoadField(tupleType.GetRuntimeField("Item1"));
            _generator.StoreLocal(_indexLocal);
            //check for null
            _generator.LoadField(tupleType.GetRuntimeField("Item2"));
            _generator.LoadConstantInt32('n');
            Label loopCheck =  _generator.DefineLabel();
            _generator.BranchIfNotEqualUnsigned(loopCheck);
            //it's null
            _generator.LoadLocal(_indexLocal);
            _generator.LoadConstantInt32(4);
            _generator.Add();
            _generator.StoreLocal(_indexLocal);
            _generator.LoadNull();
            var endLabel = _generator.DefineLabel();
            _generator.Branch(endLabel);
            
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
            _generator.StoreLocal(_indexLocal);

            //properties
            var unknownPropertyLabel = _generator.DefineLabel();
            var propertiesQuery = 
                from property in _jsonObjectType.GetRuntimeProperties()
                where property.CanWrite
                select (IJsonPropertyInfo)new JsonPropertyInfo(property);

            var fieldsQuery = 
                from field in _jsonObjectType.GetRuntimeFields()
                where field.IsPublic
                select (IJsonPropertyInfo)new JsonFieldInfo(field);

            EmitProperties(propertiesQuery.Concat(fieldsQuery).ToArray(), loopCheck, unknownPropertyLabel);

            //unknown property
            _generator.Mark(unknownPropertyLabel);
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(_indexLocal);
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
            _generator.Mark(endLabel);
        }

        public void EmitProperties(IJsonPropertyInfo[] properties, Label loopCheckLabel, Label unknownPropertyLabel)
        {           
            var propertyHandlers = new List<Action>();
            if(properties.Length == 0)
            {
                return;
            }

            EmitGroup(properties, propertyHandlers, loopCheckLabel, unknownPropertyLabel);

            _generator.Branch(unknownPropertyLabel);
            foreach(var propertyHandler in propertyHandlers)
            {
                propertyHandler();
            }
        }

        void EmitGroup(IJsonPropertyInfo[] properties, List<Action> propertyHandlers, Label loopCheckLabel, Label unknownPropertyLabel)
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
            Label propertyLabel, IGrouping<int, IJsonPropertyInfo> propertyHash, 
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

            _emitters.Emit(_indexLocal, property.Type);
            
            property.EmitSetValue(_generator);

            _generator.Branch(loopCheckLabel);
        }

        void EmitIfGroup(
            SwitchGroup ifGroup, List<Action> propertyHandlers, 
            Label unknownPropertyLabel, Label loopCheckLabel, LocalBuilder hashLocal)
        {
            foreach(var hashGroup in ifGroup)
            {
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

        IEnumerable<SwitchGroup> FindSwitchGroups(IGrouping<int, IJsonPropertyInfo>[] hashes)
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

        class SwitchGroup : List<IGrouping<int, IJsonPropertyInfo>>{}

        internal override bool TypeSupported(Type type)
        {
            if(type.GetTypeInfo().IsValueType)
            {
                return false;
            }
            if(type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return false;
            }
            if(type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                return false;
            }
            if(type.IsArray)
            {
                return false;
            }
            if(type == typeof(string))
            {
                return false;
            }
            return true;
        }

        internal override JsonPrimitive PrimitiveType => JsonPrimitive.Object;
    }
}