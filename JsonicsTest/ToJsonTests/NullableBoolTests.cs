using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.ToJsonTests
{
    [TestFixture]
    public class NullableBoolTests
    {
        [TestCase(true,"true")]
        [TestCase(false,"false")]
        [TestCase(null,"null")]
        public void ToJson_Bool_CorrectJson(bool? input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<bool?>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        public class BoolTestClass
        {
            public bool? BoolProperty {get;set;}
        }

        [TestCase(true,"true")]
        [TestCase(false,"false")]
        [TestCase(null,"null")]
        public void ToJson_BoolProperty_CorrectJson(bool? input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<BoolTestClass>();

            //act
            string json = converter.ToJson(new BoolTestClass{BoolProperty=input});

            //assert
            Assert.That(json, Is.EqualTo($"{{\"BoolProperty\":{expectedJson}}}"));
        }
    }
}