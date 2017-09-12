using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class NullableShortTests
    {
        IJsonConverter<NullableshortClass> _propertyFactory;
        IJsonConverter<short?> _valueFactory;
        

        public class NullableshortClass
        {
            public short? Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<NullableshortClass>();
            _valueFactory = JsonFactory.Compile<short?>();
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(short.MaxValue)]
        [TestCase(short.MinValue)]
        [TestCase(null)]
        public void NullableShortProperty_CorrectlyDeserialized(short? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();

            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{value}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(short.MaxValue)]
        [TestCase(short.MinValue)]
        [TestCase(null)]
        public void NullableShortValue_CorrectlyDeserialized(short? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();
            
            //act
            short? result = _valueFactory.FromJson($"{value}");

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}