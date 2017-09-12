using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Jsonics.ToJson;

namespace Jsonics
{
    public class TypeEmitter : Emitter 
    {
        readonly ToJsonEmitters _toJsonEmitters;

        public TypeEmitter(TypeBuilder typeBuilder, StringBuilder appendQueue, ListMethods listMethods, FieldBuilder builderField, ToJsonEmitters toJsonEmitters)
            : base(typeBuilder, appendQueue, listMethods, builderField)
        {
            _toJsonEmitters = toJsonEmitters;
        }

        public void EmitType(Type type, JsonILGenerator generator, Action<JsonILGenerator> getTypeOnStack)
        {
            if(_toJsonEmitters.EmitValue(type, getTypeOnStack, generator))
            {
                return;
            }


            _listMethods.ObjectEmitter.GenerateObject(type, generator, getTypeOnStack);
        }
    }
}