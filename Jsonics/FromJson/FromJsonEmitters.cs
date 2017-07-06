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
            _emitters.Add(new IntEmitter(lazyStringLocal, generator, this));
            _emitters.Add(new BoolEmitter(lazyStringLocal, generator, this));
            _emitters.Add(new StringEmitter(lazyStringLocal, generator, this));
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