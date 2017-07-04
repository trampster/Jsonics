using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class StringTests
    {
        IJsonConverter<StringClass> _propertyFactory;
        IJsonConverter<string> _valueFactory;
        

        public class StringClass
        {
            public string Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<StringClass>();
            _valueFactory = JsonFactory.Compile<string>();
        }

        [TestCase("test", "test")]
        [TestCase("te\\\"st", "te\"st")]
        public void StringProperty_CorrectlyDeserialized(string json, string expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":\"{json}\"}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase("test", "test")]
        [TestCase("te\\\"st", "te\"st")]
        [TestCase("", "")]
        public void IntValue_CorrectlyDeserialized(string json, string expected)
        {
            //arrange
            //act
            string result = _valueFactory.FromJson($"\"{json}\"");

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}