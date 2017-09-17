using System;

namespace Jsonics
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

        void EmitSetValue(JsonILGenerator generator);
    }    
}