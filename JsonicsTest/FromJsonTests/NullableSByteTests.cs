using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class NullableSByteTests
    {
        IJsonConverter<NullableSByteClass> _propertyFactory;
        IJsonConverter<sbyte?> _valueFactory;
        
        public class NullableSByteClass
        {
            public sbyte? Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<NullableSByteClass>();
            _valueFactory = JsonFactory.Compile<sbyte?>();
        }

        [TestCase((sbyte)0)]
        [TestCase((sbyte)1)]
        [TestCase((sbyte)-1)]
        [TestCase((sbyte)42)]
        [TestCase(sbyte.MaxValue)]
        [TestCase(sbyte.MinValue)]
        [TestCase(null)]
        public void NullableSByteProperty_CorrectlyDeserialized(sbyte? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();

            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{value}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase((sbyte)0)]
        [TestCase((sbyte)1)]
        [TestCase((sbyte)-1)]
        [TestCase((sbyte)42)]
        [TestCase(sbyte.MaxValue)]
        [TestCase(sbyte.MinValue)]
        [TestCase(null)]
        public void NullableSByteValue_CorrectlyDeserialized(sbyte? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();
            
            //act
            sbyte? result = _valueFactory.FromJson(value);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}