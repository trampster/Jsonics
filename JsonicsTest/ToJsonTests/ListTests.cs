using System.Collections.Generic;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.ToJsonTests
{
    [TestFixture]
    public class ListTests
    {
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
    }
}