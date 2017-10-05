
using System;

namespace Jsonics
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, Inherited=false, AllowMultiple=false)]
    public class IgnoreAttribute : Attribute
    {
    }
}