using System;
using System.Reflection;

namespace Jsonics.ToJson
{
    public abstract class ToJsonEmitter
    {
        protected readonly JsonILGenerator _generator;

        public ToJsonEmitter(JsonILGenerator generator)
        {
            _generator = generator;
        }

        public abstract void EmitValue(Type type, Action<JsonILGenerator> getValueOnStack);

        public abstract void EmitProperty(PropertyInfo property, Action<JsonILGenerator> getValueOnStack);

        public abstract bool TypeSupported(Type type);
    }
}