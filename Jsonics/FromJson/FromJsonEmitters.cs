using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Jsonics.FromJson
{
    public class FromJsonEmitters
    {
        List<FromJsonEmitter> _emitters;

        public FromJsonEmitters(Type jsonObjectType, LocalBuilder lazyStringLocal, JsonILGenerator generator)
        {
            _emitters = new List<FromJsonEmitter>();
            _emitters.Add(new LazyStringEmitter<byte>(lazyStringLocal, generator, this, "ToByte"));
            _emitters.Add(new LazyStringEmitter<byte?>(lazyStringLocal, generator, this, "ToNullableByte"));
            _emitters.Add(new LazyStringEmitter<short>(lazyStringLocal, generator, this, "ToShort"));
            _emitters.Add(new LazyStringEmitter<short?>(lazyStringLocal, generator, this, "ToNullableShort"));
            _emitters.Add(new LazyStringEmitter<ushort>(lazyStringLocal, generator, this, "ToUShort"));
            _emitters.Add(new LazyStringEmitter<ushort?>(lazyStringLocal, generator, this, "ToNullableUShort"));
            _emitters.Add(new LazyStringEmitter<int>(lazyStringLocal, generator, this, "ToInt"));
            _emitters.Add(new LazyStringEmitter<int?>(lazyStringLocal, generator, this, "ToNullableInt"));
            _emitters.Add(new LazyStringEmitter<uint>(lazyStringLocal, generator, this, "ToUInt"));
            _emitters.Add(new LazyStringEmitter<uint?>(lazyStringLocal, generator, this, "ToNullableUInt"));
            _emitters.Add(new LazyStringEmitter<long>(lazyStringLocal, generator, this, "ToLong"));
            _emitters.Add(new LazyStringEmitter<long?>(lazyStringLocal, generator, this, "ToNullableLong"));
            _emitters.Add(new LazyStringEmitter<ulong>(lazyStringLocal, generator, this, "ToULong"));
            _emitters.Add(new LazyStringEmitter<ulong?>(lazyStringLocal, generator, this, "ToNullableULong"));
            _emitters.Add(new LazyStringEmitter<double>(lazyStringLocal, generator, this, "ToDouble"));
            _emitters.Add(new LazyStringEmitter<double?>(lazyStringLocal, generator, this, "ToNullableDouble"));
            _emitters.Add(new LazyStringEmitter<bool>(lazyStringLocal, generator, this, "ToBool"));
            _emitters.Add(new LazyStringEmitter<bool?>(lazyStringLocal, generator, this, "ToNullableBool"));
            _emitters.Add(new LazyStringEmitter<string>(lazyStringLocal, generator, this, "ToString"));
            _emitters.Add(new ObjectFromJsonEmitterFactory(lazyStringLocal, generator, this));
        }

        public void Emit(LocalBuilder indexLocal, Type type)
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
    }
}