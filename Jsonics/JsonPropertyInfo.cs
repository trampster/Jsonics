using System;
using System.Reflection;

namespace Jsonics
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

        public void EmitSetValue(JsonILGenerator generator)
        {
            var type = _propertyInfo.DeclaringType;
            var underlyingType = Nullable.GetUnderlyingType(type);
            if(underlyingType != null)
            {
                type = underlyingType;
            }
            generator.CallVirtual(type.GetRuntimeMethod($"set_{_propertyInfo.Name}", new Type[]{_propertyInfo.PropertyType}));
        }
    }
}