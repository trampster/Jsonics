using Jsonics;
using JsonicsTests.TestCaseSources;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class NullableDecimalTests
    {
        IJsonConverter<NullableDecimalClass> _propertyFactory;
        IJsonConverter<decimal?> _valueFactory;
        
        public class NullableDecimalClass
        {
            public decimal? Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<NullableDecimalClass>();
            _valueFactory = JsonFactory.Compile<decimal?>();
        }

        [Test, TestCaseSource(typeof(DecimalTestCaseSource), "FromJsonNullableTestCases")]
        public void NullableDoubleProperty_CorrectlyDeserialized(decimal? expected, string value)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{value}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(DecimalTestCaseSource), "FromJsonNullableTestCases")]
        public void NullableDoubleValue_CorrectlyDeserialized(decimal? expected, string value)
        {
            //arrange
            //act
            decimal? result = _valueFactory.FromJson(value);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}