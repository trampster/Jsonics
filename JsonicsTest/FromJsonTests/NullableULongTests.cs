using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class NullableULongTests
    {
        IJsonConverter<NullableULongClass> _propertyFactory;
        IJsonConverter<ulong?> _valueFactory;
        
        public class NullableULongClass
        {
            public ulong? Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<NullableULongClass>();
            _valueFactory = JsonFactory.Compile<ulong?>();
        }

        [TestCase((ulong)0)]
        [TestCase((ulong)1)]
        [TestCase((ulong)42)]
        [TestCase(ulong.MaxValue)]
        [TestCase(ulong.MinValue)]
        [TestCase(null)]
        public void NullableULongProperty_CorrectlyDeserialized(ulong? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();

            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{value}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase((ulong)0)]
        [TestCase((ulong)1)]
        [TestCase((ulong)42)]
        [TestCase(ulong.MaxValue)]
        [TestCase(ulong.MinValue)]
        [TestCase(null)]
        public void NullableULongValue_CorrectlyDeserialized(ulong? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();
            
            //act
            ulong? result = _valueFactory.FromJson(value);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}