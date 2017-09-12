using System;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTest.ToJsonTests
{
    [TestFixture]
    public class GuidTests
    {
        public class GuidObject
        {
            public Guid GuidProperty
            {
                get;
                set;
            }
        }

        [Test]
        public void ToJson_GuidProperty_CorrectJson()
        {
            //arrange
            var guidbject = new GuidObject()
            {
                GuidProperty = new Guid(1,2,3,4,5,6,7,8,9,10,11)
            };
            var converter = JsonFactory.Compile<GuidObject>();

            //act
            string json = converter.ToJson(guidbject);

            //assert
            Assert.That(json, Is.EqualTo("{\"GuidProperty\":\"00000001-0002-0003-0405-060708090a0b\"}"));
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