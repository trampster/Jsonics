using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.FromJson
{
    internal abstract class FromJsonEmitter
    {
        protected LocalBuilder _lazyStringLocal;
        protected JsonILGenerator _generator;
        protected FromJsonEmitters _emitters;

        internal FromJsonEmitter(LocalBuilder lazyStringLocal, JsonILGenerator generator, FromJsonEmitters emitters)
        {
            _lazyStringLocal = lazyStringLocal;
            _generator = generator;
            _emitters = emitters;
        }

        internal abstract bool TypeSupported(Type type);

        /// <summary>
        /// Create the IL required to parse this property
        /// Should set the indexLocal to the character after the property
        /// Should finish with the property value on the stack.
        /// </summary>
        /// <param name="indexLocal"></param>
        /// <param name="type"></param>
        internal abstract void Emit(LocalBuilder indexLocal, Type type);

        internal Type LazyStringCallToX<T>(string methodName, JsonILGenerator generator)
        {
            generator.Call(typeof(LazyString).GetRuntimeMethod(methodName, new Type[]{typeof(int)}));
            return typeof(ValueTuple<T,int>);
        }

        internal abstract JsonPrimitive PrimitiveType {get;}
    }
}