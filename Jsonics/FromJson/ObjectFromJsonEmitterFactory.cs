using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.FromJson
{
    public class ObjectFromJsonEmitterFactory : FromJsonEmitter
    {
        public ObjectFromJsonEmitterFactory(LocalBuilder lazyStringLocal, JsonILGenerator generator, FromJsonEmitters emitters)
            : base(lazyStringLocal, generator, emitters)
        {
        }
        
        public override void Emit(LocalBuilder indexLocal, Type type)
        {
            //we need a new one each time otherwise the nesting breaks because the the shared state
            var emitter = new ObjectFromJsonEmitter(_lazyStringLocal, _generator, _emitters);
            emitter.Emit(indexLocal, type);
        }

        public override bool TypeSupported(Type type)
        {
            return new ObjectFromJsonEmitter(_lazyStringLocal, _generator, _emitters).TypeSupported(type);
        }
    }
}