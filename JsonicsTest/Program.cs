using System;
using Jsonics;

namespace JsonicsTests
{
    public class JsonicsTest
    {
        public static void Main(string[] args)
        {
            var jsonConverter = JsonFactory.Compile<SimpleTestObject>();
            var json = jsonConverter.ToJson(new SimpleTestObject(){FirstName="Luke", LastName="Skywalker"});
            Console.WriteLine(json);
        }
    }
}
