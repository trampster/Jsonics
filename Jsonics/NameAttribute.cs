
using System;

namespace Jsonics
{
    /// <summary>
    /// Allow a .net Property or Field to map to a Json property with a different name
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, Inherited=false, AllowMultiple=false)]
    public class NameAttribute : Attribute
    {
        public NameAttribute(string jsonName)
        {
            JsonName = jsonName;
        }

        /// <summary>
        /// The name of the property in the Json string.
        /// </summary>
        public string JsonName { get; set; }
    }
}