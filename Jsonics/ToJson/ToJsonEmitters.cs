using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.ToJson
{
    internal class ToJsonEmitters
    {
        readonly ToJsonEmitter[] _emitters;

        internal ToJsonEmitters(ListMethods listMethods, FieldBuilder stringBuilderField, TypeBuilder typeBuilder)
        {
            _emitters = new ToJsonEmitter[]
            {
                new BoolEmitter(),
                new NullableBoolEmitter(),
                new IntEmitter(),
                new NullableIntEmitter(),
                new NumberEmitter(),
                new NullableNumberEmitter(),
                new StringEmitter(),
                new ArrayEmitter(listMethods, stringBuilderField, typeBuilder, this),
                new ListEmitter(listMethods, stringBuilderField, typeBuilder, this),
                new DictionaryEmitter(listMethods, stringBuilderField, typeBuilder, this),
                new DateTimeEmitter(),
                new NullableEmitter<DateTime>(this),
                new GuidEmitter(),
                new NullableEmitter<Guid>(this),
                new ObjectEmitter(this),
            };
        }

        internal bool EmitValue(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
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

        internal bool EmitProperty(PropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
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