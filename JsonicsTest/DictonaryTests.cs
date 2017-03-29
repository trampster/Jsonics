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
    }
}