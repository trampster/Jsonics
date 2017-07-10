using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class ShortTests
    {
        IJsonConverter<ShortClass> _propertyFactory;
        IJsonConverter<short> _valueFactory;

        public class ShortClass
        {
            public short Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<ShortClass>();
            _valueFactory = JsonFactory.Compile<short>();
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(short.MaxValue)]
        [TestCase(short.MinValue)]
        public void ShortProperty_CorrectlyDeserialized(short expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{expected}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(short.MaxValue)]
        [TestCase(short.MinValue)]
        public void ShortValue_CorrectlyDeserialized(short expected)
        {
            //arrange
            //act
            short result = _valueFactory.FromJson($"{expected}");

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}