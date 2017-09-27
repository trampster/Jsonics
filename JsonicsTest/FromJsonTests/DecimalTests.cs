using Jsonics;
using JsonicsTests.TestCaseSources;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class DecimalTests
    {
        IJsonConverter<DecimalClass> _propertyFactory;
        IJsonConverter<decimal> _valueFactory;
        
        public class DecimalClass
        {
            public decimal Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<DecimalClass>();
            _valueFactory = JsonFactory.Compile<decimal>();
        }

        [Test, TestCaseSource(typeof(DecimalTestCaseSource), "TestCases")]
        public void DoubleProperty_CorrectlyDeserialized(decimal expected, string value)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{value}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(DecimalTestCaseSource), "TestCases")]
        public void DoubleValue_CorrectlyDeserialized(decimal expected, string value)
        {
            //arrange
            //act
            decimal result = _valueFactory.FromJson(value);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}