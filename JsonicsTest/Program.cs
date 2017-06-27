 using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Jsonics;
using Jsonics.PropertyHashing;
using JsonicsTests.FromJsonTests;
using NUnitLite;

namespace JsonicsTest
{
    public class JsonicsTest
    {
        public static void Main(string[] args)
        {
            // var hash = new Jsonics.PropertyHashing.PropertyHashFactory().FindBestHash(new string[]{"First", "Secon", "Third"});
            // Console.WriteLine($"Collisions: {hash.CollisionCount}");
            // Console.WriteLine($"First: {hash.Hash("First")}");
            // Console.WriteLine($"Secon: {hash.Hash("Secon")}");
            // Console.WriteLine($"Third: {hash.Hash("Third")}");
            
            var test = new IntPropertyTests();
            test.FromJson_ThreeProperties_PropertiesSetCorrectly();

            new AutoRun(typeof(JsonicsTest).GetTypeInfo().Assembly).Execute(args);

            //string json = "{\"First\":1,\"Extra\":1,\"Secon\":2}";


            //var example = new Example();
            // var result = example.FromJson(json);
            // Console.WriteLine($"result First: {result.First} Secon: {result.Secon}");


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

        public int Third
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

        public int TestSwitch(int input)
        {
            switch(input)
            {
                case 0:
                    return 0;
                case 1:
                    return 1;
                case 3:
                    return 3;
                case 5:
                    return 5;
                case 11:
                    return 11;
                case 12:
                    return 12;
                case 13:
                    return 13;
                case 14:
                    return 14;
            }
            return -1;
        }

        public TestClass FromJson(string input)
        {
            var json = new LazyString(input);

            var testClass = new TestClass();
            int inputIndex = 0;
            while(input[inputIndex] != '}')
            {
                //read to start of first property
                int indexOfQuote = json.ReadTo(inputIndex, '\"');
                int propertyStart = indexOfQuote + 1;
                int propertyEnd = json.ReadTo(propertyStart, '\"');
                var propertyName = json.SubString(propertyStart, propertyEnd - propertyStart);
                int hash = propertyName.At(0 % propertyName.Length) % 2;
                int intStart = json.ReadTo(propertyEnd + 1, ':') + 1;
                switch (hash)
                {
                    case 0:
                        if(propertyName.EqualsString("Third"))
                        {
                            (testClass.First, inputIndex) = json.ToInt(intStart);
                        }
                        else
                        {
                            goto UnknownProperty;
                        }
                        break;
                    case 2:
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
