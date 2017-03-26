using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jsonics
{
    public class Emitters
    {
        readonly TypeBuilder _typeBuilder;
        readonly StringBuilder _appendQueue;
        readonly FieldBuilder _stringBuilderField;

        public Emitters(TypeBuilder typeBuilder, StringBuilder appendQueue, FieldBuilder stringBuilderField)
        {
            _typeBuilder = typeBuilder;
            _appendQueue = appendQueue;
            _stringBuilderField = stringBuilderField;
        }

        public ListEmitter ListEmitter
        {
            get
            {
                return new ListEmitter(_typeBuilder, _appendQueue, this, _stringBuilderField);
            }
        }

        public ValueEmitter ValueEmitter
        {
            get
            {
                return new ValueEmitter(_typeBuilder, _appendQueue, this, _stringBuilderField);
            }
        }

        public TypeEmitter TypeEmitter
        {
            get
            {
                return new TypeEmitter(_typeBuilder, _appendQueue, this, _stringBuilderField);
            }
        }

        public ObjectEmitter ObjectEmitter
        {
            get
            {
                return new ObjectEmitter(_typeBuilder, _appendQueue, this, _stringBuilderField);
            }
        }

        Dictionary<Type, MethodInfo> _methodLookup = new Dictionary<Type, MethodInfo>();

        public MethodInfo GetMethod(Type type, StringBuilder appendQueue, Action<JsonILGenerator, Action<JsonILGenerator>> emitElement)
        {
            if(!_methodLookup.ContainsKey(type))
            {
                if(type.IsArray)
                {
                    _methodLookup[type] = ListEmitter.EmitArrayMethod(type.GetElementType(), emitElement);
                }
                else if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    _methodLookup[type] = ListEmitter.EmitListMethod(type, type.GenericTypeArguments[0], emitElement);
                }
            }
            return _methodLookup[type];
        }
    }
}