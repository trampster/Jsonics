using System;
using System.Reflection;

namespace Jsonics.ToJson
{
    public class ToJsonEmitters
    {
        readonly ToJsonEmitter[] _emitters;

        public ToJsonEmitters(JsonILGenerator generator)
        {
            _emitters = new ToJsonEmitter[]
            {
                new NullableIntEmitter(generator)
            };
        }

        public bool EmitValue(Type type, Action<JsonILGenerator> getValueOnStack)
        {
            foreach(var emitter in _emitters)
            {
                if(emitter.TypeSupported(type))
                {
                    emitter.EmitValue(type, getValueOnStack);
                    return true;
                }
            }
            return false;
        }

        public bool EmitProperty(PropertyInfo property, Action<JsonILGenerator> getValueOnStack)
        {
            foreach(var emitter in _emitters)
            {
                if(emitter.TypeSupported(property.PropertyType))
                {
                    emitter.EmitProperty(property, getValueOnStack);
                    return true;
                }
            }
            return false;
        }
    }
}