using System;
using System.Reflection.Emit;

namespace Jsonics.FromJson
{
    public abstract class FromJsonEmitter
    {
        protected LocalBuilder _lazyStringLocal;
        protected JsonILGenerator _generator;
        protected FromJsonEmitters _emitters;

        public FromJsonEmitter(LocalBuilder lazyStringLocal, JsonILGenerator generator, FromJsonEmitters emitters)
        {
            _lazyStringLocal = lazyStringLocal;
            _generator = generator;
            _emitters = emitters;
        }

        public abstract bool TypeSupported(Type type);

        public abstract void Emit(LocalBuilder indexLocal, Type type);
    }
}