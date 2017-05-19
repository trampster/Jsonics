using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Jsonics;
using Jsonics.PropertyHashing;
using NUnitLite;

namespace JsonicsTest
{
    public class JsonicsTest
    {
        public static void Main(string[] args)
        {
            // var test = new JsonFactoryTest();
            // test.FromJson_SimpleTestObject_ReturnsClassInstance();

            new AutoRun(typeof(JsonicsTest).GetTypeInfo().Assembly).Execute(args);

//             string json = "{\"First\":1,\"Extra\":1,\"Secon\":2}";
  //           var example = new Example();
    //         var result = example.FromJson(json);
      //       Console.WriteLine($"result First: {result.First} Secon: {result.Secon}");
            // Time("Mine", () => 
            // {
            //     for(int index = 0; index < 1000000; index ++)
            //     {
            //         example.FromJson(json);
            //     }
            // });

            // Time("Newtonsoft", () => 
            // {
            //     for(int index = 0; index < 1000000; index ++)
            //     {
            //         Newtonsoft.Json.JsonConvert.DeserializeObject<TestClass>(json);
            //     }
            // });

            // Time("NetJson", () => 
            // {
            //     for(int index = 0; index < 1000000; index ++)
            //     {
            //         NetJSON.NetJSON.Deserialize(typeof(TestClass), json);
            //     }
            // });

        }

        static void Time(string name, Action action)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            Console.WriteLine($"{name}: {stopwatch.ElapsedMilliseconds} ms");
        }
    }

    public class TestClass
    {
        public int First
        {
            get;
            set;
        }

        public int Secon
        {
            get;
            set;
        }
    }

    public class Example : IJsonConverter<TestClass>
    {

        [ThreadStatic]
        static StringBuilder _builder;

        public string ToJson(TestClass jsonObject)
        {
            if(_builder == null)
            {
                 _builder = new StringBuilder();
            }

            return _builder
                .Clear()
                .AppendInt((int)jsonObject.First)
                .ToString(); 
        }

        static PropertyHash _propertyHash = new PropertyHash()
        {
            Column = 0,
            ModValue = 2
        };

        public TestClass FromJson(string input)
        {
            var json = new LazyString(input);

            var testClass = new TestClass();
            int inputIndex = 0;
            while(inputIndex < input.Length)
            {
                //read to start of first property
                int indexOfQuote = json.ReadTo(inputIndex, '\"');
                if(indexOfQuote == -1)
                {
                    //no more properties
                    break;
                }
                int propertyStart = indexOfQuote + 1;
                int propertyEnd = json.ReadTo(propertyStart, '\"');
                var propertyName = json.SubString(propertyStart, propertyEnd - propertyStart);
                int hash = propertyName.At(0 % propertyName.Length) % 2;
                int intStart = json.ReadTo(propertyEnd + 1, ':') + 1;
                switch (hash)
                {
                    case 0:
                        if(propertyName.EqualsString("First"))
                        {
                            (testClass.First, inputIndex) = json.ToInt(intStart);
                        }
                        else
                        {
                            goto UnknownProperty;
                        }
                        break;
                    case 1:
                        if(propertyName.EqualsString("Secon"))
                        {
                            (testClass.Secon, inputIndex) = json.ToInt(intStart);
                        }
                        else
                        {
                            goto UnknownProperty;
                        }
                        break;
                    default:
                        UnknownProperty:
                        //is an unknown property so just need to read past it.
                        inputIndex = json.ReadToPropertyValueEnd(intStart);
                        break;
                }
            }

            return testClass;
        }        
    }
}
