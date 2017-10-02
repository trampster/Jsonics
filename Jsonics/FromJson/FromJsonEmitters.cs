using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Jsonics.FromJson
{
    internal class FromJsonEmitters
    {
        List<FromJsonEmitter> _emitters;

        internal FromJsonEmitters(Type jsonObjectType, LocalBuilder lazyStringLocal, JsonILGenerator generator, Func<Type, FieldBuilder> addStaticField)
        {
            _emitters = new List<FromJsonEmitter>();
            _emitters.Add(new LazyStringEmitter<byte>(lazyStringLocal, generator, this, "ToByte", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<byte?>(lazyStringLocal, generator, this, "ToNullableByte", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<sbyte>(lazyStringLocal, generator, this, "ToSByte", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<sbyte?>(lazyStringLocal, generator, this, "ToNullableSByte", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<short>(lazyStringLocal, generator, this, "ToShort", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<short?>(lazyStringLocal, generator, this, "ToNullableShort", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<ushort>(lazyStringLocal, generator, this, "ToUShort", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<ushort?>(lazyStringLocal, generator, this, "ToNullableUShort", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<int>(lazyStringLocal, generator, this, "ToInt", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<int?>(lazyStringLocal, generator, this, "ToNullableInt", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<uint>(lazyStringLocal, generator, this, "ToUInt", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<uint?>(lazyStringLocal, generator, this, "ToNullableUInt", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<long>(lazyStringLocal, generator, this, "ToLong", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<long?>(lazyStringLocal, generator, this, "ToNullableLong", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<ulong>(lazyStringLocal, generator, this, "ToULong", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<ulong?>(lazyStringLocal, generator, this, "ToNullableULong", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<float>(lazyStringLocal, generator, this, "ToFloat", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<float?>(lazyStringLocal, generator, this, "ToNullableFloat", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<double>(lazyStringLocal, generator, this, "ToDouble", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<double?>(lazyStringLocal, generator, this, "ToNullableDouble", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<bool>(lazyStringLocal, generator, this, "ToBool", JsonPrimitive.Bool));
            _emitters.Add(new LazyStringEmitter<bool?>(lazyStringLocal, generator, this, "ToNullableBool", JsonPrimitive.Bool));
            _emitters.Add(new LazyStringEmitter<char>(lazyStringLocal, generator, this, "ToChar", JsonPrimitive.String));
            _emitters.Add(new LazyStringEmitter<char?>(lazyStringLocal, generator, this, "ToNullableChar", JsonPrimitive.String));
            _emitters.Add(new LazyStringEmitter<decimal>(lazyStringLocal, generator, this, "ToDecimal", JsonPrimitive.Number));
            _emitters.Add(new LazyStringEmitter<decimal?>(lazyStringLocal, generator, this, "ToNullableDecimal", JsonPrimitive.Number));
            _emitters.Add(new EnumEmitter(lazyStringLocal, generator, this));
            _emitters.Add(new LazyStringEmitter<Guid>(lazyStringLocal, generator, this, "ToGuid", JsonPrimitive.String));
            _emitters.Add(new LazyStringEmitter<Guid?>(lazyStringLocal, generator, this, "ToNullableGuid", JsonPrimitive.String));
            _emitters.Add(new DateTimeEmitter(lazyStringLocal, generator, this));
            _emitters.Add(new ArrayEmitter(lazyStringLocal, generator, this, addStaticField));
            _emitters.Add(new ListEmitter(lazyStringLocal, generator, this, addStaticField));
            _emitters.Add(new DictionaryEmitter(lazyStringLocal, generator, this, addStaticField));
            _emitters.Add(new LazyStringEmitter<string>(lazyStringLocal, generator, this, "ToString", JsonPrimitive.String));
            _emitters.Add(new StructFromJsonEmitterFactory(lazyStringLocal, generator, this));
            _emitters.Add(new ObjectFromJsonEmitterFactory(lazyStringLocal, generator, this));
        }

        internal void Emit(LocalBuilder indexLocal, Type type)
        {
            foreach(var emitter in _emitters)
            {
                if(emitter.TypeSupported(type))
                {
                    emitter.Emit(indexLocal, type);
                    return;
                }
            }
        }

        internal JsonPrimitive GetPrimitiveType(Type type)
        {
            foreach(var emitter in _emitters)
            {
                if(emitter.TypeSupported(type))
                {
                    return emitter.PrimitiveType;
                }
            }
            return JsonPrimitive.String; //TODO: remove once we have struct support
        }
    }
}