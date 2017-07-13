using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class NullableLongTests
    {
        IJsonConverter<NullableLongClass> _propertyFactory;
        IJsonConverter<long?> _valueFactory;
        
        public class NullableLongClass
        {
            public long? Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<NullableLongClass>();
            _valueFactory = JsonFactory.Compile<long?>();
        }

        [TestCase((long)0)]
        [TestCase((long)1)]
        [TestCase((long)-1)]
        [TestCase(long.MaxValue)]
        [TestCase(long.MinValue)]
        [TestCase(null)]
        public void IntProperty_CorrectlyDeserialized(long? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();

            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{value}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase((long)0)]
        [TestCase((long)1)]
        [TestCase((long)-1)]
        [TestCase(long.MaxValue)]
        [TestCase(long.MinValue)]
        [TestCase(null)]
        public void LongValue_CorrectlyDeserialized(long? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();
            
            //act
            long? result = _valueFactory.FromJson(value);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}