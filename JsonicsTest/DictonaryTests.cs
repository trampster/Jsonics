using System;
using System.Collections.Generic;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTest
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

        [Test]
        public void ToJson_DictionaryObjectInt_CorrectJson()
        {
            //arrange
            var dictionary = new Dictionary<object, int>()
            {
                { new object(), 9001 },
                { new object(), 3474 },
                { new object(), 11926 }
            };
            var converter = JsonFactory.Compile<Dictionary<object, int>>();

            //act
            var json = converter.ToJson(dictionary);

            //assert
            Assert.That(json, Is.EqualTo("{\"System.Object\":9001,\"System.Object\":3474,\"System.Object\":11926}"));
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
    }
}