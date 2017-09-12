using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class NullableUShortTests
    {
        IJsonConverter<NullableUShortClass> _propertyFactory;
        IJsonConverter<ushort?> _valueFactory;
        
        public class NullableUShortClass
        {
            public ushort? Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<NullableUShortClass>();
            _valueFactory = JsonFactory.Compile<ushort?>();
        }

        [TestCase((ushort)0)]
        [TestCase((ushort)1)]
        [TestCase((ushort)42)]
        [TestCase(ushort.MaxValue)]
        [TestCase(ushort.MinValue)]
        [TestCase(null)]
        public void NullableUShortProperty_CorrectlyDeserialized(ushort? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();

            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{value}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase((ushort)0)]
        [TestCase((ushort)1)]
        [TestCase((ushort)42)]
        [TestCase(ushort.MaxValue)]
        [TestCase(ushort.MinValue)]
        [TestCase(null)]
        public void NullableUShortValue_CorrectlyDeserialized(ushort? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();
            
            //act
            ushort? result = _valueFactory.FromJson(value);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}