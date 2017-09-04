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

        public TypeEmitter(TypeBuilder typeBuilder, StringBuilder appendQueue, Emitters emitters, FieldBuilder builderField, ToJsonEmitters toJsonEmitters)
            : base(typeBuilder, appendQueue, emitters, builderField)
        {
            _toJsonEmitters = toJsonEmitters;
        }

        public void EmitType(Type type, JsonILGenerator generator, Action<JsonILGenerator> getTypeOnStack)
        {
            if(_toJsonEmitters.EmitValue(type, getTypeOnStack))
            {
                return;
            }

            var valueEmitter = _emitters.ValueEmitter;
            if(type == typeof(string))
            {
                valueEmitter.CreateString(generator, getTypeOnStack);
            }
            else if(type == typeof(int) || type.GetTypeInfo().IsEnum)
            {
                valueEmitter.CreateInt(generator, getTypeOnStack);
            }
            else if(type == typeof(uint) ||
                type == typeof(long) || type == typeof(ulong) ||
                type == typeof(byte) || type == typeof(sbyte) ||
                type == typeof(short) || type == typeof(ushort) ||
                type == typeof(float) || type == typeof(double))
            {
                valueEmitter.CreateNumber(generator, getTypeOnStack, type);
            }
            else if(type == typeof(bool))
            {
                valueEmitter.CreateBool(generator, getTypeOnStack);
            }
            else if(type.IsArray)
            {
                valueEmitter.CreateArrayValue(type, generator, getTypeOnStack);
            }
            else if(type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                valueEmitter.CreateListValue(type, generator, getTypeOnStack);
            }
            else if(type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                valueEmitter.CreateDictionaryValue(type, generator, getTypeOnStack);
            }
            else if(type == typeof(DateTime))
            {
                valueEmitter.CreateDateTime(generator, getTypeOnStack);
            }
            else if(type == typeof(Guid))
            {
                valueEmitter.CreateGuid(generator, getTypeOnStack);
            }
            else
            {
                _emitters.ObjectEmitter.GenerateObject(type, generator, getTypeOnStack);
            }
        }
    }
}