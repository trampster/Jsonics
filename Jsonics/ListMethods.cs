using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Jsonics.ToJson;

namespace Jsonics
{
    public class ListMethods
    {
        readonly TypeBuilder _typeBuilder;
        readonly StringBuilder _appendQueue;
        readonly FieldBuilder _stringBuilderField;
        readonly ToJsonEmitters _toJsonEmitters;

        public ListMethods(TypeBuilder typeBuilder, StringBuilder appendQueue, FieldBuilder stringBuilderField)
        {
            _typeBuilder = typeBuilder;
            _appendQueue = appendQueue;
            _stringBuilderField = stringBuilderField;
            _toJsonEmitters = new ToJsonEmitters(this, stringBuilderField, typeBuilder);
        }

        public ListEmitter ListEmitter
        {
            get
            {
                return new ListEmitter(_typeBuilder, _appendQueue, this, _stringBuilderField);
            }
        }

        public TypeEmitter TypeEmitter
        {
            get
            {
                return new TypeEmitter(_typeBuilder, _appendQueue, this, _stringBuilderField, _toJsonEmitters);
            }
        }

        public ObjectEmitter ObjectEmitter
        {
            get
            {
                return new ObjectEmitter(_typeBuilder, _appendQueue, this, _stringBuilderField, _toJsonEmitters);
            }
        }

        Dictionary<Type, MethodInfo> _methodLookup = new Dictionary<Type, MethodInfo>();

        public MethodInfo GetMethod(Type type, Action<JsonILGenerator, Action<JsonILGenerator>> emitElement, Func<MethodBuilder> getMethodBuilder)
        {
            if(!_methodLookup.ContainsKey(type))
            {
                if(type.IsArray)
                {
                    _methodLookup[type] = getMethodBuilder();
                }
                else if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    _methodLookup[type] = ListEmitter.EmitListMethod(type, type.GenericTypeArguments[0], emitElement);
                }
                else if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    _methodLookup[type] = ListEmitter.EmitDictionaryMethod(type);
                }
            }
            return _methodLookup[type];
        }
    }
}