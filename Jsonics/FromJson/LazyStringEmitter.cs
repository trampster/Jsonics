using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.FromJson
{
    internal class LazyStringEmitter<T> : FromJsonEmitter
    {
        readonly string _methodName;
        readonly JsonPrimitive _jsonPrimitive;

        internal LazyStringEmitter(LocalBuilder lazyStringLocal, JsonILGenerator generator, FromJsonEmitters emitters, string methodName, JsonPrimitive jsonPrimitive)
            : base(lazyStringLocal, generator, emitters)
        {
            _methodName = methodName;
            _jsonPrimitive  = jsonPrimitive;
        }
        
        internal override void Emit(LocalBuilder indexLocal, Type type)
        {
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(indexLocal);
            Type tupleType = LazyStringCallToX<T>(_methodName, _generator);
            _generator.Duplicate();

            _generator.LoadField(tupleType.GetRuntimeField("Item2"));
            _generator.StoreLocal(indexLocal);

            _generator.LoadField(tupleType.GetRuntimeField("Item1"));
        }

        internal override bool TypeSupported(Type type)
        {
            return type == typeof(T);
        }

        internal override JsonPrimitive PrimitiveType => _jsonPrimitive;
    }
}