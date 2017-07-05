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

        [TestCase("\"test\"", "test")]
        [TestCase("    \"test\"", "test")]
        [TestCase("\"te\\\"st\"", "te\"st")]
        [TestCase("null", null)]
        [TestCase(" null", null)]
        public void StringProperty_CorrectlyDeserialized(string json, string expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{json}}}");

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

        public class TwoStringProperties
        {
            public string First
            {
                get;
                set;
            }
            public string Secon
            {
                get;
                set;
            }
        }

        [Test]
        public void TwoStringProperty_CorrectlyDeserialized()
        {
            //arrange
            var converter = JsonFactory.Compile<TwoStringProperties>();
            string json = "{\"First\":\"one\",\"Secon\":\"two\"}";

            //act
            var result = converter.FromJson(json);

            //assert
            Assert.That(result.First, Is.EqualTo("one"));
            Assert.That(result.Secon, Is.EqualTo("two"));
        }

        public class TestClass1
        {
            public string First
            {
                get;
                set;
            }

            public string Second
            {
                get;
                set;
            }

            public string Third
            {
                get;
                set;
            }
        }

        [Test]
        public void ThreeStringProperties_CorrectlyDeserialized()
        {
            //arrange
            var converter = JsonFactory.Compile<TestClass1>();
            string json = "{\"First\":\"one\",\"Second\":\"two\",\"Third\":\"three\"}";

            //act
            var result = converter.FromJson(json);

            //assert
            Assert.That(result.First, Is.EqualTo("one"));
            Assert.That(result.Second, Is.EqualTo("two"));
            Assert.That(result.Third, Is.EqualTo("three"));
        }
    }
}