using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jsonics.FromJson
{
    public class DictionaryEmitter : FromJsonEmitter
    {
        readonly Func<Type, FieldBuilder> _addStaticField;

        public DictionaryEmitter(LocalBuilder lazyStringLocal, JsonILGenerator generator, FromJsonEmitters emitters, Func<Type, FieldBuilder> addStaticField)
            : base(lazyStringLocal, generator, emitters)
        {
            _addStaticField = addStaticField;
        }
        
        public override void Emit(LocalBuilder indexLocal, Type type)
        {
            //(inputIndex, currentValue) = json.ReadToAny(inputIndex, '{', 'n') + 1;
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(indexLocal);
            _generator.LoadConstantInt32('{');
            _generator.LoadConstantInt32('n');
            _generator.Call(typeof(LazyString).GetRuntimeMethod("ReadToAny", new []{typeof(int),typeof(char),typeof(char)}));
            _generator.Duplicate();

            //update inputIndex
            Type tupleType = typeof(ValueTuple<int,char>);
            _generator.LoadField(tupleType.GetRuntimeField("Item1"));
            _generator.StoreLocal(indexLocal);
            //check for null
            _generator.LoadField(tupleType.GetRuntimeField("Item2"));
            _generator.LoadConstantInt32('n');
            var notNullLabel = _generator.DefineLabel();
            _generator.BranchIfNotEqualUnsigned(notNullLabel);
            //it's null
            _generator.LoadLocal(indexLocal);
            _generator.LoadConstantInt32(4);
            _generator.Add();
            _generator.StoreLocal(indexLocal);
            _generator.LoadNull();
            var endLabel = _generator.DefineLabel();
            _generator.Branch(endLabel);
            // //it's not null
            _generator.Mark(notNullLabel);
            //var dictionary = new Dictionary<int, string>();
            var dictionaryLocal = _generator.DeclareLocal(type);
            _generator.NewObject(type.GetTypeInfo().GetConstructor(new Type[0]));
            _generator.StoreLocal(dictionaryLocal);
            //inputIndex++;
            _generator.LoadLocal(indexLocal);
            _generator.LoadConstantInt32(1);
            _generator.Add();
            _generator.StoreLocal(indexLocal);
            //currentValue = json.At(inputIndex);
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(indexLocal);
            _generator.Call(typeof(LazyString).GetRuntimeMethod("At", new []{typeof(int)}));
            var currentValueLocal = _generator.DeclareLocal<char>();
            _generator.StoreLocal(currentValueLocal);
            
            var loopCheckLabel = _generator.DefineLabel();
            _generator.Branch(loopCheckLabel);
            //loop start
            var loopStartLabel = _generator.DefineLabel();
            _generator.Mark(loopStartLabel);
                Type keyType = type.GenericTypeArguments[0];
                var keyPrimitiveType = _emitters.GetPrimitiveType(keyType);
                if(keyPrimitiveType != JsonPrimitive.String)
                {
                    //inputIndex = json.ReadTo(inputIndex,'\"') + 1;
                    _generator.LoadLocalAddress(_lazyStringLocal);
                    _generator.LoadLocal(indexLocal);
                    _generator.LoadConstantInt32('\"');
                    _generator.Call(typeof(LazyString).GetRuntimeMethod("ReadTo", new []{typeof(int), typeof(char)}));
                    _generator.LoadConstantInt32(1);
                    _generator.Add();
                    _generator.StoreLocal(indexLocal);
                }

                //int key;
                //(key, inputIndex) = json.ToInt(inputIndex);
                _emitters.Emit(indexLocal, keyType);
                var keyLocal = _generator.DeclareLocal(keyType);
                _generator.StoreLocal(keyLocal);

                //inputIndex = json.ReadTo(inputIndex, ':') + 1;
                _generator.LoadLocalAddress(_lazyStringLocal);
                _generator.LoadLocal(indexLocal);
                _generator.LoadConstantInt32(':');
                _generator.Call(typeof(LazyString).GetRuntimeMethod("ReadTo", new []{typeof(int), typeof(char)}));
                _generator.LoadConstantInt32(1);
                _generator.Add();
                _generator.StoreLocal(indexLocal);

                //string value;
                //(value, inputIndex) = json.ToString(inputIndex);
                Type valueType = type.GenericTypeArguments[1];
                _emitters.Emit(indexLocal, valueType);
                var valueLocal = _generator.DeclareLocal(valueType);
                _generator.StoreLocal(valueLocal);

                //dictionary.Add(key, value);
                _generator.LoadLocal(dictionaryLocal);
                _generator.LoadLocal(keyLocal);
                _generator.LoadLocal(valueLocal);
                _generator.Call(type.GetRuntimeMethod("Add", new []{keyType, valueType}));
                
                //(inputIndex, currentValue) = json.ReadToAny(inputIndex, ',', '}');
                _generator.LoadLocalAddress(_lazyStringLocal);
                _generator.LoadLocal(indexLocal);
                _generator.LoadConstantInt32(',');
                _generator.LoadConstantInt32('}');
                _generator.Call(typeof(LazyString).GetRuntimeMethod("ReadToAny", new []{typeof(int), typeof(char), typeof(char)}));
                _generator.Duplicate();
                _generator.LoadField(tupleType.GetRuntimeField("Item1"));
                _generator.StoreLocal(indexLocal);
                _generator.LoadField(tupleType.GetRuntimeField("Item2"));
                _generator.StoreLocal(currentValueLocal);
                
            //loop check
            _generator.Mark(loopCheckLabel);
            _generator.LoadLocal(currentValueLocal);
            _generator.LoadConstantInt32('}');
            _generator.BranchIfNotEqualUnsigned(loopStartLabel);

            _generator.LoadLocal(indexLocal);
            _generator.LoadConstantInt32(1);
            _generator.Add();
            _generator.StoreLocal(indexLocal);

            _generator.LoadLocal(dictionaryLocal);

            _generator.Mark(endLabel);
        }

        public override bool TypeSupported(Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        public override JsonPrimitive PrimitiveType => JsonPrimitive.Object;
    }
}