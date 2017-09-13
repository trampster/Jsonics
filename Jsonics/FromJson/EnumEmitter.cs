using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.FromJson
{
    internal class EnumEmitter : FromJsonEmitter
    {
        internal EnumEmitter(LocalBuilder lazyStringLocal, JsonILGenerator generator, FromJsonEmitters emitters)
            : base(lazyStringLocal, generator, emitters)
        {
        }
        
        internal override void Emit(LocalBuilder indexLocal, Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if(underlyingType != null)
            {
                EmitNullable(indexLocal, type);
                return;
            }

            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(indexLocal);
            Type tupleType = LazyStringCallToX<int>("ToInt", _generator);
            _generator.Duplicate();

            _generator.LoadField(tupleType.GetRuntimeField("Item2"));
            _generator.StoreLocal(indexLocal);

            _generator.LoadField(tupleType.GetRuntimeField("Item1"));
        }

        void EmitNullable(LocalBuilder indexLocal, Type type)
        {
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(indexLocal);
            Type tupleType = LazyStringCallToX<int?>("ToNullableInt", _generator);
            _generator.Duplicate();

            _generator.LoadField(tupleType.GetRuntimeField("Item2"));
            _generator.StoreLocal(indexLocal);

            _generator.LoadField(tupleType.GetRuntimeField("Item1"));
            var nullableIntLocal = _generator.DeclareLocal<int?>();
            _generator.StoreLocal(nullableIntLocal);
            _generator.LoadLocalAddress(nullableIntLocal);

            _generator.Call(typeof(int?).GetRuntimeMethod("get_HasValue", new Type[0]));
            var hasValueLabel = _generator.DefineLabel();
            _generator.BrIfTrue(hasValueLabel);

            //it's null
            var resultLocal = _generator.DeclareLocal(type);
            _generator.LoadLocalAddress(resultLocal);
            _generator.InitObject(type);
            _generator.LoadLocal(resultLocal);
            var endLabel = _generator.DefineLabel();
            _generator.Branch(endLabel);

            //it's not null
            _generator.Mark(hasValueLabel);
            _generator.LoadLocalAddress(nullableIntLocal);
            _generator.Call(typeof(int?).GetRuntimeMethod("GetValueOrDefault", new Type[0]));
            _generator.NewObject(type.GetTypeInfo().GetConstructor(new []{Nullable.GetUnderlyingType(type)}));
            
            _generator.Mark(endLabel);
        }

        internal override bool TypeSupported(Type type)
        {
            if(type.GetTypeInfo().IsEnum)
            {
                return true;
            }
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if(underlyingType != null)
            {
                return underlyingType.GetTypeInfo().IsEnum;
            }
            return false;
        }

        internal override JsonPrimitive PrimitiveType => JsonPrimitive.Number;
    }
}