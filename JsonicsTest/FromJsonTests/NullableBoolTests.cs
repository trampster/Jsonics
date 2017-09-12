using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class NullableBoolTests
    {
        IJsonConverter<NullableBoolClass> _propertyFactory;
        IJsonConverter<bool?> _valueFactory;

        public class NullableBoolClass
        {
            public bool? Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<NullableBoolClass>();
            _valueFactory = JsonFactory.Compile<bool?>();
        }

        [TestCase("true", true)]
        [TestCase(" true", true)]
        [TestCase("\n true", true)]
        [TestCase("\n false", false)]
        [TestCase("null", null)]
        public void NullableBoolProperty_CorrectlyDeserialized(string jsonValue, bool? expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{jsonValue}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase("true", true)]
        [TestCase(" true", true)]
        [TestCase("\n true", true)]
        [TestCase("\n false", false)]
        [TestCase("null", null)]
        public void NullableBoolValue_CorrectlyDeserialized(string jsonValue, bool? expected)
        {
            //arrange
            //act
            bool? result = _valueFactory.FromJson($"{jsonValue}");

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}