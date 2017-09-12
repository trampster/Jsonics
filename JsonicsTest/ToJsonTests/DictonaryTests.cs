using System;
using System.Collections.Generic;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTest.ToJsonTests
{
    [TestFixture]
    public class DictonaryTests
    {
        [Test]
        public void ToJson_DictionaryStringInt_CorrectJson()
        {
            //arrange
            var dictionary = new Dictionary<string, int>()
            {
                { "James", 9001 },
                { "Jo", 3474 },
                { "Jess", 11926 }
            };
            var converter = JsonFactory.Compile<Dictionary<string, int>>();

            //act
            var json = converter.ToJson(dictionary);

            //assert
            Assert.That(json, Is.EqualTo("{\"James\":9001,\"Jo\":3474,\"Jess\":11926}"));
        }

        [Test]
        public void ToJson_DictionaryIntInt_CorrectJson()
        {
            //arrange
            var dictionary = new Dictionary<int, int>()
            {
                { 1, 9001 },
                { 2, 3474 },
                { 3, 11926 }
            };
            var converter = JsonFactory.Compile<Dictionary<int, int>>();

            //act
            var json = converter.ToJson(dictionary);

            //assert
            Assert.That(json, Is.EqualTo("{\"1\":9001,\"2\":3474,\"3\":11926}"));
        }

        public struct TestStruct
        {
            public string FirstName{get;set;}
            public string LastName{get;set;}

            public override string ToString()
            {
                return $"{FirstName} {LastName}";
            }
        }

        [Test]
        public void ToJson_DictionaryStructInt_CorrectJson()
        {
            //arrange
            var dictionary = new Dictionary<TestStruct, int>()
            {
                { new TestStruct(){FirstName="Bob", LastName="Marley"}, 9001 },
                { new TestStruct(){FirstName="Luke", LastName="Skywalker"}, 3474 },
                { new TestStruct(){FirstName="Gandalf", LastName="Gray"}, 11926 }
            };
            var converter = JsonFactory.Compile<Dictionary<TestStruct, int>>();

            //act
            var json = converter.ToJson(dictionary);

            //assert
            Assert.That(json, Is.EqualTo("{\"Bob Marley\":9001,\"Luke Skywalker\":3474,\"Gandalf Gray\":11926}"));
        }

        public class TestClass
        {
            public string FirstName{get;set;}
            public string LastName{get;set;}

            public override string ToString()
            {
                return $"{FirstName} {LastName}";
            }
        }

        [Test]
        public void ToJson_DictionaryStringTestClass_CorrectJson()
        {
            //arrange
            var dictionary = new Dictionary<string, TestClass>()
            {
                { "one", new TestClass(){FirstName="Bob", LastName="Marley"}},
                { "two", new TestClass(){FirstName="Luke", LastName="Skywalker"}},
                { "three", new TestClass(){FirstName="Gandalf", LastName="Gray"}}
            };
            var converter = JsonFactory.Compile<Dictionary<string, TestClass>>();

            //act
            var json = converter.ToJson(dictionary);

            //assert
            Assert.That(json, Is.EqualTo("{" +
                "\"one\":{\"FirstName\":\"Bob\",\"LastName\":\"Marley\"}," + 
                "\"two\":{\"FirstName\":\"Luke\",\"LastName\":\"Skywalker\"}," +
                "\"three\":{\"FirstName\":\"Gandalf\",\"LastName\":\"Gray\"}" +
                "}"
                ));
        }

        public class ClassWithDictionaryProperty
        {
            public Dictionary<int, string> DictionaryProperty{get;set;}
        }

        [Test]
        public void ToJson_ClassWithDictionaryProperty_CorrectJson()
        {
            //arrange
            var classWithDictionaryProperty = new ClassWithDictionaryProperty
            {
                DictionaryProperty = new Dictionary<int, string>
                {
                    [1] = "one"
                }
            };
            var converter = JsonFactory.Compile<ClassWithDictionaryProperty>();

            //act
            var json = converter.ToJson(classWithDictionaryProperty);

            //assert
            Assert.That(json, Is.EqualTo("{\"DictionaryProperty\":{\"1\":\"one\"}}"));
        }
    }
}