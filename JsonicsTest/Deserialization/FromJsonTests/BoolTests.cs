using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class BoolTests
    {
        IJsonConverter<BoolClass> _propertyFactory;
        IJsonConverter<bool> _valueFactory;

        public class BoolClass
        {
            public bool Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<BoolClass>();
            _valueFactory = JsonFactory.Compile<bool>();
        }

        [TestCase("true", true)]
        [TestCase(" true", true)]
        [TestCase("\n true", true)]
        [TestCase("\n false", false)]
        public void BoolProperty_CorrectlyDeserialized(string jsonValue, bool expected)
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
        public void BoolValue_CorrectlyDeserialized(string jsonValue, bool expected)
        {
            //arrange
            //act
            bool result = _valueFactory.FromJson($"{jsonValue}");

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}