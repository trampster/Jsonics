using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class NullableByteTests
    {
        IJsonConverter<NullableByteClass> _propertyFactory;
        IJsonConverter<byte?> _valueFactory;
        
        public class NullableByteClass
        {
            public byte? Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<NullableByteClass>();
            _valueFactory = JsonFactory.Compile<byte?>();
        }

        [TestCase((byte)0)]
        [TestCase((byte)1)]
        [TestCase((byte)42)]
        [TestCase(byte.MaxValue)]
        [TestCase(byte.MinValue)]
        [TestCase(null)]
        public void NullableByteProperty_CorrectlyDeserialized(byte? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();

            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{value}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase((byte)0)]
        [TestCase((byte)1)]
        [TestCase((byte)42)]
        [TestCase(byte.MaxValue)]
        [TestCase(byte.MinValue)]
        [TestCase(null)]
        public void NullableByteValue_CorrectlyDeserialized(byte? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();
            
            //act
            byte? result = _valueFactory.FromJson(value);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}