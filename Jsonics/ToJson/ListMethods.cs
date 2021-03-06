using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Jsonics.ToJson;

namespace Jsonics.ToJson
{
    internal class ListMethods
    {
        readonly TypeBuilder _typeBuilder;
        readonly StringBuilder _appendQueue;
        readonly FieldBuilder _stringBuilderField;

        internal ListMethods(TypeBuilder typeBuilder, StringBuilder appendQueue, FieldBuilder stringBuilderField)
        {
            _typeBuilder = typeBuilder;
            _appendQueue = appendQueue;
            _stringBuilderField = stringBuilderField;
        }

        Dictionary<Type, MethodInfo> _methodLookup = new Dictionary<Type, MethodInfo>();

        internal MethodInfo GetMethod(Type type, Func<MethodBuilder> getMethodBuilder)
        {
            if(!_methodLookup.ContainsKey(type))
            {
                _methodLookup[type] = getMethodBuilder();
            }
            return _methodLookup[type];
        }
    }
}