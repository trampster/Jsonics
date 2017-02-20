using System;
using Jsonic;

namespace JsonicTests
{
    public class JsonicTest
    {
        public static void Main(string[] args)
        {
            var jsonConverter = JsonicFactory.Compile<SimpleTestObject>();
            var json = jsonConverter.ToJson(new SimpleTestObject(){FirstName="Luke", LastName="Skywalker"});
            Console.WriteLine(json);
        }
    }
}
