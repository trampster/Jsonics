using System;
using System.Reflection;

namespace Jsonics.ToJson
{
    internal abstract class ToJsonEmitter
    {
        internal abstract void EmitValue(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator);

        internal abstract void EmitProperty(IJsonPropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator);

        internal abstract bool TypeSupported(Type type);
    }
}