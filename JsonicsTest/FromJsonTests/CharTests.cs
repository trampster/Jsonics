using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class CharTests
    {
        IJsonConverter<CharClass> _propertyFactory;
        IJsonConverter<char> _valueFactory;

        public class CharClass
        {
            public char Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<CharClass>();
            _valueFactory = JsonFactory.Compile<char>();
        }

        [TestCase("\"c\"", 'c')]
        [TestCase(" \"a\"", 'a')]
        [TestCase("\n \"1\"", '1')]
        [TestCase(" \"0\"", '0')]
        [TestCase("\"\\\\\"",'\\')] // escaped backslash
        [TestCase("\"\\\"\"", '\"')] // escaped quote
        [TestCase("\"\\/\"", '/')] // escaped forward slash
        [TestCase("\"\\b\"", '\b')] // escaped backspace
        [TestCase("\"\\f\"", '\f')] // escaped formfeed
        [TestCase("\"\\n\"", '\n')] // escaped newline
        [TestCase("\"\\r\"", '\r')] // escaped carrage return
        [TestCase("\"\\t\"", '\t')] // escaped tab
        [TestCase("\"\\u1234\"", '\u1234')] // unicode
        public void CharProperty_CorrectlyDeserialized(string jsonValue, char expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{jsonValue}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase("\"c\"", 'c')]
        [TestCase(" \"a\"", 'a')]
        [TestCase("\n \"1\"", '1')]
        [TestCase(" \"0\"", '0')]
        [TestCase("\"\\\\\"",'\\')] // escaped backslash
        [TestCase("\"\\\"\"", '\"')] // escaped quote
        [TestCase("\"\\/\"", '/')] // escaped forward slash
        [TestCase("\"\\b\"", '\b')] // escaped backspace
        [TestCase("\"\\f\"", '\f')] // escaped formfeed
        [TestCase("\"\\n\"", '\n')] // escaped newline
        [TestCase("\"\\r\"", '\r')] // escaped carrage return
        [TestCase("\"\\t\"", '\t')] // escaped tab
        [TestCase("\"\\u1234\"", '\u1234')] // unicode
        public void CharValue_CorrectlyDeserialized(string jsonValue, char expected)
        {
            //arrange
            //act
            char result = _valueFactory.FromJson(jsonValue);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}