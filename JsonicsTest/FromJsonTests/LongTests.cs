using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class LongTests
    {
        IJsonConverter<LongClass> _propertyFactory;
        IJsonConverter<long> _valueFactory;
        
        public class LongClass
        {
            public long Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<LongClass>();
            _valueFactory = JsonFactory.Compile<long>();
        }

        [TestCase((long)0)]
        [TestCase((long)1)]
        [TestCase((long)-1)]
        [TestCase(long.MaxValue)]
        [TestCase(long.MinValue)]
        public void IntProperty_CorrectlyDeserialized(long expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{expected}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase((long)0)]
        [TestCase((long)1)]
        [TestCase((long)-1)]
        [TestCase(long.MaxValue)]
        [TestCase(long.MinValue)]
        public void LongValue_CorrectlyDeserialized(long expected)
        {
            //arrange
            //act
            long result = _valueFactory.FromJson($"{expected}");

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}