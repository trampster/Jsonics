using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Jsonics;
using Newtonsoft.Json;

namespace JsonicsTests
{
    public class JsonicsTest
    {
        public static void Main(string[] args)
        {
            var watch = new Stopwatch();
            var converter = JsonFactory.Compile<SimpleTestObject>();
            var testObject = new SimpleTestObject()
            {
                FirstName="Ob Won", 
                LastName="Kenoby",
                Age=60,
                IsJedi=true,
                PowerFactor=104.6789
            };

            var example = new Example();

            watch.Start();
            for(int index = 0; index < 1000000; index++)
            {
                example.ToJson(testObject);
                //converter.ToJson(testObject);
            }
            watch.Stop();
            Console.WriteLine($"Time: {watch.ElapsedMilliseconds}");

            Console.WriteLine($"Output: {example.ToJson(testObject)}");           
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
