using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
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
                IsJedi=true
            };

            watch.Start();
            for(int index = 0; index < 1000000; index++)
            {
                converter.ToJson(testObject);
            }
            watch.Stop();
            Console.WriteLine($"Time: {watch.ElapsedMilliseconds}");
        }

        [ThreadStatic]
        static StringBuilder _builder;

        const string _firstName = "{\"FirstName\":\"";
        const string _lastName = "\",\"LastName\":\"";
        const string _age = "\",\"Age\":";
        const string _isJediTrue = ",\"IsJedi\":true}";
        const string _isJediFalse = ",\"IsJedi\":false}";
        
        public static string StringBuilderFormat(SimpleTestObject testObj)
        {
            if(_builder == null)
            {
                 _builder = new StringBuilder();
            }
            return _builder
                .Clear()
                .Append(_firstName)
                .Append(testObj.FirstName)
                .Append(_lastName)
                .Append(testObj.LastName)
                .Append(_age)
                .Append(testObj.Age)
                .Append(testObj.IsJedi ? _isJediTrue : _isJediFalse)
                .ToString(); 
        }

    }

    public class Example : IJsonConverter<SimpleTestObject>
    {
        //[ThreadStatic]
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
                .Append(jsonObject.FirstName)
                .Append("\",\"LastName\":\"")
                .Append(jsonObject.LastName)
                .Append("\",\"Age\":")
                .Append(jsonObject.Age)
                .Append(jsonObject.IsJedi ? ",\"IsJedi\":true}" : ",\"IsJedi\":false}")
                .ToString(); 
        }
    }
}
