using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.FromJson
{
    internal class ObjectFromJsonEmitterFactory : FromJsonEmitter
    {
        internal ObjectFromJsonEmitterFactory(LocalBuilder lazyStringLocal, JsonILGenerator generator, FromJsonEmitters emitters)
            : base(lazyStringLocal, generator, emitters)
        {
        }
        
        internal override void Emit(LocalBuilder indexLocal, Type type)
        {
            //we need a new one each time otherwise the nesting breaks because the the shared state
            var emitter = new ObjectFromJsonEmitter(_lazyStringLocal, _generator, _emitters);
            emitter.Emit(indexLocal, type);
        }

        internal override bool TypeSupported(Type type)
        {
            return new ObjectFromJsonEmitter(_lazyStringLocal, _generator, _emitters).TypeSupported(type);
        }

        internal override JsonPrimitive PrimitiveType => JsonPrimitive.Object;
    }
}