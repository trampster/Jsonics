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

        public string Name
        {
            get
            {
                var nameAttribute = _fieldInfo.GetCustomAttribute<NameAttribute>(true);
                if(nameAttribute == null)
                {
                    return _fieldInfo.Name;
                }
                return nameAttribute.JsonName;
            }
        }

        public Type Type => _fieldInfo.FieldType;

        public void EmitGetValue(JsonILGenerator generator) => generator.LoadField(_fieldInfo);

        public void EmitSetValue(JsonILGenerator generator)
        {
            generator.StoreField(_fieldInfo);
        }
    }
}