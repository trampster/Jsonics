using System;
using System.Reflection;

namespace Jsonics.ToJson
{
    public class ToJsonEmitters
    {
        readonly ToJsonEmitter[] _emitters;

        public ToJsonEmitters()
        {
            _emitters = new ToJsonEmitter[]
            {
                new BoolEmitter(),
                new NullableBoolEmitter(),
                new IntEmitter(),
                new NullableIntEmitter(),
                new NumberEmitter(),
                new NullableNumberEmitter(),
                new StringEmitter()
            };
        }

        public bool EmitValue(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            foreach(var emitter in _emitters)
            {
                if(emitter.TypeSupported(type))
                {
                    emitter.EmitValue(type, getValueOnStack, generator);
                    return true;
                }
            }
            return false;
        }

        public bool EmitProperty(PropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            foreach(var emitter in _emitters)
            {
                if(emitter.TypeSupported(property.PropertyType))
                {
                    emitter.EmitProperty(property, getValueOnStack, generator);
                    return true;
                }
            }
            return false;
        }
    }
}