using System;
using System.Collections.Generic;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTest
{
    [TestFixture]
    public class JsonPrimitivesTests
    {
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