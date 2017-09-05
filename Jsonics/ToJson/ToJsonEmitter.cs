using System;
using System.Reflection;

namespace Jsonics.ToJson
{
    public abstract class ToJsonEmitter
    {
        public abstract void EmitValue(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator);

        public abstract void EmitProperty(PropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator);

        public abstract bool TypeSupported(Type type);
    }
}