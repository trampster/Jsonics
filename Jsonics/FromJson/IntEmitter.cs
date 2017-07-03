using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.FromJson
{
    public class IntEmitter : FromJsonEmitter
    {
        public IntEmitter(LocalBuilder lazyStringLocal, JsonILGenerator generator, FromJsonEmitters emitters)
            : base(lazyStringLocal, generator, emitters)
        {
        }
        
        public override void Emit(LocalBuilder indexLocal, Type type)
        {
            _generator.LoadLocalAddress(_lazyStringLocal);
            _generator.LoadLocal(indexLocal);
            Type tupleType = LazyStringCallToX<int>("ToInt", _generator);
            _generator.Duplicate();

            _generator.LoadField(tupleType.GetRuntimeField("Item2"));
            _generator.StoreLocal(indexLocal);

            _generator.LoadField(tupleType.GetRuntimeField("Item1"));
        }

        Type LazyStringCallToX<T>(string methodName, JsonILGenerator generator)
        {
            generator.Call(typeof(LazyString).GetRuntimeMethod(methodName, new Type[]{typeof(T)}));
            return typeof(ValueTuple<T,int>);
        }

        public override bool TypeSupported(Type type)
        {
            return type == typeof(int);
        }
    }
}