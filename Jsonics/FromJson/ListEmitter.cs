using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jsonics.FromJson
{
    public class ListEmitter : FromJsonEmitter
    {
        readonly Func<Type, FieldBuilder> _addStaticField;

        public ListEmitter(LocalBuilder lazyStringLocal, JsonILGenerator generator, FromJsonEmitters emitters, Func<Type, FieldBuilder> addStaticField)
            : base(lazyStringLocal, generator, emitters)
        {
            _addStaticField = addStaticField;
        }
        
        public override void Emit(LocalBuilder indexLocal, Type listType)
        {
            Type listValueType = listType.GenericTypeArguments[0];

            //inputIndex = json.ReadToAny(inputIndex, '[', 'n') + 1;
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(indexLocal);
            _generator.LoadConstantInt32('[');
            _generator.LoadConstantInt32('n');
            _generator.Call(typeof(LazyString).GetRuntimeMethod("ReadToAny", new []{typeof(int),typeof(char),typeof(char)}));
            _generator.Duplicate();
            //update index local
            Type tupleType = typeof(ValueTuple<int,char>);
            _generator.LoadField(tupleType.GetRuntimeField("Item1"));
            _generator.StoreLocal(indexLocal);
            //check for null
            _generator.LoadField(tupleType.GetRuntimeField("Item2"));
            _generator.LoadConstantInt32('n');
            var notNullLabel = _generator.DefineLabel();
            _generator.BranchIfNotEqualUnsigned(notNullLabel);
            //inputIndex += 4
            _generator.LoadLocal(indexLocal);
            _generator.LoadConstantInt32(4);
            _generator.Add();
            _generator.StoreLocal(indexLocal);
            _generator.LoadNull();
            var endLabel = _generator.DefineLabel();
            _generator.Branch(endLabel);
            
            _generator.Mark(notNullLabel);
            //inputIndex++
            _generator.LoadLocal(indexLocal);
            _generator.LoadConstantInt32(1);
            _generator.Add();
            _generator.StoreLocal(indexLocal);

            //var list = new List<T>();
            var listConstructor = listType.GetTypeInfo().GetConstructor(new Type[]{});
            _generator.NewObject(listConstructor);
            var listLocal = _generator.DeclareLocal(listType);
            _generator.StoreLocal(listLocal);

            //check for end
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(indexLocal);
            _generator.Call(typeof(LazyString).GetRuntimeMethod("At", new []{typeof(int)}));
            var loopCheckLabel = _generator.DefineLabel();
            _generator.Branch(loopCheckLabel);

            //while(true)
            var loopStartLabel = _generator.DefineLabel();
            _generator.Mark(loopStartLabel);

            //deserialize the array value
            _emitters.Emit(indexLocal, listValueType);
            var listValueLocal = _generator.DeclareLocal(listValueType);
            _generator.StoreLocal(listValueLocal);

            //list.Add(arrayValue);
            _generator.LoadLocal(listLocal);
            _generator.LoadLocal(listValueLocal);
            _generator.CallVirtual(listType.GetRuntimeMethod("Add", new Type[]{listValueType}));
            
            //(inputIndex, currentValue) = json.ReadToAny(inputIndex, ',', ']');
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(indexLocal);
            _generator.LoadConstantInt32(',');
            _generator.LoadConstantInt32(']');
            _generator.Call(typeof(LazyString).GetRuntimeMethod("ReadToAny", new Type[]{typeof(int), typeof(char), typeof(char)}));
            _generator.Duplicate();
            _generator.LoadField(tupleType.GetRuntimeField("Item1"));
            _generator.StoreLocal(indexLocal);

            //currentValue != ']' goto start of loop
            _generator.LoadField(tupleType.GetRuntimeField("Item2"));
            _generator.Mark(loopCheckLabel);
            _generator.LoadConstantInt32(']');
            _generator.BranchIfNotEqualUnsigned(loopStartLabel);

            //inputIndex++;
            _generator.LoadLocal(indexLocal);
            _generator.LoadConstantInt32(1);
            _generator.Add();
            _generator.StoreLocal(indexLocal);

            //testClass.First = _arrayBuilder.ToArray();
            _generator.LoadLocal(listLocal);

            _generator.Mark(endLabel);

        }

        public override bool TypeSupported(Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}