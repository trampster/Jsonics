using System;
using System.Reflection;

namespace Jsonics
{
    internal class JsonFieldInfo : IJsonPropertyInfo
    {
        readonly FieldInfo _fieldInfo;

        internal JsonFieldInfo(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public string Name => _fieldInfo.Name;

        public Type Type => _fieldInfo.FieldType;

        public void EmitGetValue(JsonILGenerator generator) => generator.LoadField(_fieldInfo);

        public void EmitSetValue(JsonILGenerator generator)
        {
            generator.StoreField(_fieldInfo);
        }
    }
}