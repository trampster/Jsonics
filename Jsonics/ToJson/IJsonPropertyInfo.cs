using System;

namespace Jsonics.ToJson
{
    internal interface IJsonPropertyInfo
    {
        string Name
        {
            get;
        }

        Type Type
        {
            get;
        }

        void EmitGetValue(JsonILGenerator generator);
    }    
}