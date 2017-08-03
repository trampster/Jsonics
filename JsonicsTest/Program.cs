using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Jsonics;
using Jsonics.FromJson;
using Jsonics.PropertyHashing;
using JsonicsTest.Deserialization;
using JsonicsTests.FromJsonTests;
using NUnitLite;

namespace JsonicsTest
{
    public class JsonicsTest
    {
        public static void Main(string[] args)
        {
            Benchmark();
        }

        public static void Benchmark()
        {
            string json = "{\"First\":[1,2,3, 4],\"Secon\":[1,2,3, 4],\"Third\":[1,2,3, 4]}";

            var example = new Example();

            Time("Example", () => 
            {
                for(int index = 0; index < 1000000; index ++)
                {
                    example.FromJson(json);
                }
            });

            var compiled = JsonFactory.Compile<TestClass>();
            Time("Compiled", () => 
            {
                for(int index = 0; index < 1000000; index ++)
                {
                    compiled.FromJson(json);
                }
            });

            Time("Newtonsoft", () => 
            {
                for(int index = 0; index < 1000000; index ++)
                {
                    Newtonsoft.Json.JsonConvert.DeserializeObject<TestClass>(json);
                }
            });

            Time("NetJson", () => 
            {
                for(int index = 0; index < 1000000; index ++)
                {
                    NetJSON.NetJSON.Deserialize(typeof(TestClass), json);
                }
            });
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
        public  List<int> First
        {
            get;
            set;
        }

        public  List<int> Secon
        {
            get;
            set;
        }

        public List<int> Third
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
                .Append(jsonObject.First)
                .ToString(); 
        }

        [ThreadStatic]
        static List<int> _arrayBuilder = new List<int>();

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
                int intStart = json.ReadTo(propertyEnd + 1, ':') + 1;
                int hash = propertyName.At(0 % propertyName.Length) % 3;
                switch (hash)
                {
                    case 0:
                        if(propertyName.EqualsString("Third"))
                        {
                            char currentValue;
                            (inputIndex, currentValue) = json.ReadToAny(intStart, '[', 'n');
                            if(currentValue == 'n')
                            {
                                inputIndex += 4;
                                testClass.Third = null;
                            }
                            else
                            {
                                inputIndex++;
                                var list = new List<int>();
                                currentValue = json.At(inputIndex);
                                while(currentValue != ']')
                                {
                                    int arrayValue;
                                    (arrayValue, inputIndex) = json.ToInt(inputIndex);
                                   list.Add(arrayValue);
                                    (inputIndex, currentValue) = json.ReadToAny(inputIndex, ',', ']');
                                }
                                inputIndex++;
                                testClass.Third = list;
                            }
                        }
                        else
                        {
                            goto UnknownProperty;
                        }
                        break;
                    case 2:
                        if(propertyName.EqualsString("Secon"))
                        {
                            char currentValue;
                            (inputIndex, currentValue) = json.ReadToAny(intStart, '[', 'n');
                            if(currentValue == 'n')
                            {
                                inputIndex += 4;
                                testClass.Secon = null;
                            }
                            else
                            {
                                inputIndex++;
                                var list = new List<int>();
                                currentValue = json.At(inputIndex);
                                while(currentValue != ']')
                                {
                                    int arrayValue;
                                    (arrayValue, inputIndex) = json.ToInt(inputIndex);
                                   list.Add(arrayValue);
                                    (inputIndex, currentValue) = json.ReadToAny(inputIndex, ',', ']');
                                }
                                inputIndex++;
                                testClass.Secon = list;
                            }
                        }
                        else
                        {
                            goto UnknownProperty;
                        }
                        break;
                    case 1:
                        if(propertyName.EqualsString("First"))
                        {
                            char currentValue;
                            (inputIndex, currentValue) = json.ReadToAny(intStart, '[', 'n');
                            if(currentValue == 'n')
                            {
                                inputIndex += 4;
                                testClass.First = null;
                            }
                            else
                            {
                                inputIndex++;
                                var list = new List<int>();
                                currentValue = json.At(inputIndex);
                                while(currentValue != ']')
                                {
                                    int arrayValue;
                                    (arrayValue, inputIndex) = json.ToInt(inputIndex);
                                   list.Add(arrayValue);
                                    (inputIndex, currentValue) = json.ReadToAny(inputIndex, ',', ']');
                                }
                                inputIndex++;
                                testClass.First = list;
                            }
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
