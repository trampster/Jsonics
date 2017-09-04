using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.ToJsonTests
{
    [TestFixture]
    public class NullableIntTests
    {
        [TestCase(0,"0")]
        [TestCase(1,"1")]
        [TestCase(-1,"-1")]
        [TestCase(42,"42")]
        [TestCase(int.MaxValue,"2147483647")]
        [TestCase(int.MinValue,"-2147483648")]
        [TestCase(null,"null")]
        public void ToJson_NullableInt_CorrectJson(int? input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<int?>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        public class NullableIntTestClass
        {
            public int? NullableIntProperty {get;set;}
        }

        [TestCase(0,"0")]
        [TestCase(1,"1")]
        [TestCase(-1,"-1")]
        [TestCase(42,"42")]
        [TestCase(int.MaxValue,"2147483647")]
        [TestCase(int.MinValue,"-2147483648")]
        [TestCase(null,"null")]
        public void ToJson_NullableIntProperty_CorrectJson(int? input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<NullableIntTestClass>();

            //act
            string json = converter.ToJson(new NullableIntTestClass{NullableIntProperty=input});

            //assert
            Assert.That(json, Is.EqualTo($"{{\"NullableIntProperty\":{(input == null ? "null" : input.Value.ToString())}}}"));
        }
    }
}