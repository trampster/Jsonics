using System;
using System.Reflection;

namespace Jsonics.ToJson
{
    internal class JsonPropertyInfo : IJsonPropertyInfo
    {
        readonly PropertyInfo _propertyInfo;

        internal JsonPropertyInfo(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public Type Type => _propertyInfo.PropertyType;

        public string Name => _propertyInfo.Name;

        public void EmitGetValue(JsonILGenerator generator) => generator.GetProperty(_propertyInfo);
    }
}