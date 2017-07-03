using System;
using System.Collections.Generic;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTest
{
    [TestFixture]
    public class JsonPrimitivesTests
    {
        [TestCase(0,"0")]
        [TestCase(1,"1")]
        [TestCase(-1,"-1")]
        [TestCase(42,"42")]
        [TestCase(int.MaxValue,"2147483647")]
        [TestCase(int.MinValue,"-2147483648")]
        public void ToJson_Int_CorrectJson(int input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<int>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [TestCase("JsonString","\"JsonString\"")]
        [TestCase("Json\"String","\"Json\\\"String\"")]
        [TestCase("","\"\"")]
        public void ToJson_String_CorrectJson(string input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<string>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [TestCase(1,"1")]
        [TestCase(-1,"-1")]
        [TestCase(42.42,"42.42")]
        [TestCase(double.MaxValue,"1.79769313486232E+308")]
        [TestCase(double.MinValue,"-1.79769313486232E+308")]
        public void ToJson_Int_CorrectJson(double input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<double>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [TestCase(true,"true")]
        [TestCase(false,"false")]
        public void ToJson_Int_CorrectJson(bool input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<bool>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [Test]
        public void ToJson_StringArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<string[]>();

            //act
            string json = converter.ToJson(new string[]{"1","2","3","4","5"});

            //assert
            Assert.That(json, Is.EqualTo("[\"1\",\"2\",\"3\",\"4\",\"5\"]"));
        }

        [Test]
        public void ToJson_IntArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<int[]>();

            //act
            string json = converter.ToJson(new int[]{1,2,3,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,2,3,4,5]"));
        }

        [Test]
        public void ToJson_IntList_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<int>>();

            //act
            string json = converter.ToJson(new List<int>{1,2,3,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,2,3,4,5]"));
        }

        [Test]
        public void ToJson_StringList_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<string>>();

            //act
            string json = converter.ToJson(new List<string>{"1","2","3","4","5"});

            //assert
            Assert.That(json, Is.EqualTo("[\"1\",\"2\",\"3\",\"4\",\"5\"]"));
        }

        [Test]
        public void ToJson_DateTime_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<DateTime>();

            //act
            string json = converter.ToJson(new DateTime(2017,03,07, 23,59,42, DateTimeKind.Utc));

            //assert
            Assert.That(json, Is.EqualTo("\"2017-03-07T23:59:42Z\""));
        }

        [Test]
        public void ToJson_Guid_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<Guid>();

            //act
            string json = converter.ToJson(new Guid(1,2,3,4,5,6,7,8,9,10,11));

            //assert
            Assert.That(json, Is.EqualTo("\"00000001-0002-0003-0405-060708090a0b\""));
        }
    }
}