using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Jsonics;
using NUnitLite;
//using Newtonsoft.Json;

namespace JsonicsTests
{
    public class JsonicsTest
    {
        public static void Main(string[] args)
        {
            new AutoRun(typeof(JsonicsTest).GetTypeInfo().Assembly).Execute(args);          
        }
    }

    public class Example : IJsonConverter<SimpleTestObject>
    {
        [ThreadStatic]
        static StringBuilder _builder;

        public string ToJson(SimpleTestObject jsonObject)
        {
            if(_builder == null)
            {
                 _builder = new StringBuilder();
            }
            return _builder
                .Clear()
                .Append("{\"FirstName\":\"")
                .AppendEscaped(jsonObject.FirstName)
                .Append("\",\"LastName\":\"")
                .AppendEscaped(jsonObject.LastName)
                .Append("\",\"Age\":")
                .Append(jsonObject.Age)
                .Append(jsonObject.IsJedi ? ",\"IsJedi\":true" : ",\"IsJedi\":false")
                .Append(",\"PowerFactor\":")
                .Append(jsonObject.PowerFactor)
                .Append("}")
                .ToString(); 
        }
    }
}
