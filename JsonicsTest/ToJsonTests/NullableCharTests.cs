using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.ToJsonTests
{
    [TestFixture]
    public class NullableCharTests
    {
        [TestCase("null", null)]
        [TestCase("\"c\"", 'c')]
        [TestCase("\"a\"", 'a')]
        [TestCase("\"1\"", '1')]
        [TestCase("\"0\"", '0')]
        [TestCase("\"\\\\\"",'\\')]  // escaped backslash
        [TestCase("\"\\\"\"", '\"')] // escaped quote
        [TestCase("\"\\/\"", '/')]   // escaped forward slash
        [TestCase("\"\\b\"", '\b')]  // escaped backspace
        [TestCase("\"\\f\"", '\f')]  // escaped formfeed
        [TestCase("\"\\n\"", '\n')]  // escaped newline
        [TestCase("\"\\r\"", '\r')]  // escaped carrage return
        [TestCase("\"\\t\"", '\t')]  // escaped tab
        [TestCase("\"\u1234\"", '\u1234')] // unicode
        public void ToJson_NullableChar_CorrectJson(string expectedJson, char? input)
        {
            //arrange
            var converter = JsonFactory.Compile<char?>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }
    }
}