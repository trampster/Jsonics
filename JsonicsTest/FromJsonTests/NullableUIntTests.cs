using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class NullableUIntTests
    {
        IJsonConverter<NullableUIntClass> _propertyFactory;
        IJsonConverter<uint?> _valueFactory;
        
        public class NullableUIntClass
        {
            public uint? Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<NullableUIntClass>();
            _valueFactory = JsonFactory.Compile<uint?>();
        }

        [TestCase((uint)0)]
        [TestCase((uint)1)]
        [TestCase((uint)42)]
        [TestCase(uint.MaxValue)]
        [TestCase(uint.MinValue)]
        [TestCase(null)]
        public void NullableUIntProperty_CorrectlyDeserialized(uint? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();

            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{value}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase((uint)0)]
        [TestCase((uint)1)]
        [TestCase((uint)42)]
        [TestCase(uint.MaxValue)]
        [TestCase(uint.MinValue)]
        [TestCase(null)]
        public void NullableUIntValue_CorrectlyDeserialized(uint? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();
            
            //act
            uint? result = _valueFactory.FromJson(value);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}