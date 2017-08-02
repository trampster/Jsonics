using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jsonics.FromJson
{
    public class ArrayEmitter : FromJsonEmitter
    {
        readonly Func<Type, FieldBuilder> _addStaticField;

        public ArrayEmitter(LocalBuilder lazyStringLocal, JsonILGenerator generator, FromJsonEmitters emitters, Func<Type, FieldBuilder> addStaticField)
            : base(lazyStringLocal, generator, emitters)
        {
            _addStaticField = addStaticField;
        }
        
        public override void Emit(LocalBuilder indexLocal, Type type)
        {
            var arrayElementType = type.GetElementType();

            //declare builder
            var fieldName = Guid.NewGuid().ToString().Replace("-","");
            var listType = typeof(List<>).MakeGenericType(arrayElementType);
            var listField = _addStaticField(listType);

            //actual deserailziation
            
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

            //clear
            _generator.LoadStaticField(listField);
            _generator.CallVirtual(listType.GetRuntimeMethod("Clear", new Type[0]));

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
            _emitters.Emit(indexLocal, type.GetElementType());
            var arrayValueLocal = _generator.DeclareLocal(arrayElementType);
            _generator.StoreLocal(arrayValueLocal);

            //_arrayBuilder.Add(arrayValue);
            _generator.LoadStaticField(listField);
            _generator.LoadLocal(arrayValueLocal);
            _generator.CallVirtual(listType.GetRuntimeMethod("Add", new Type[]{arrayElementType}));
            
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
            _generator.LoadStaticField(listField);
            _generator.CallVirtual(listType.GetRuntimeMethod("ToArray", new Type[0]));

            _generator.Mark(endLabel);

        }

        public override bool TypeSupported(Type type)
        {
            return type.IsArray;
        }
    }
}