using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.ToJsonTests
{
    [TestFixture]
    public class StringTests
    {
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
    }
}