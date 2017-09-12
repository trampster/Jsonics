using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class NullableDoubleTests
    {
        IJsonConverter<NullableDoubleClass> _propertyFactory;
        IJsonConverter<double?> _valueFactory;
        

        public class NullableDoubleClass
        {
            public double? Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<NullableDoubleClass>();
            _valueFactory = JsonFactory.Compile<double?>();
        }

        [TestCase("0", 0)]
        [TestCase("1", 1)]
        [TestCase("-1", -1)]
        [TestCase("1.23", 1.23)]
        [TestCase("-1.23E45", -1.23E45d)]
        [TestCase("1.7976931348623157E+308", double.MaxValue)]
        [TestCase("-1.7976931348623157E+308", double.MinValue)]
        [TestCase("null", null)]
        [TestCase(" null", null)]
        public void NullableDoubleProperty_CorrectlyDeserialized(string value, double? expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{value}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase("0", 0)]
        [TestCase("1", 1)]
        [TestCase("-1", -1)]
        [TestCase("1.23", 1.23)]
        [TestCase("-1.23E45", -1.23E45d)]
        [TestCase("1.7976931348623157E+308", double.MaxValue)]
        [TestCase("-1.7976931348623157E+308", double.MinValue)]
        [TestCase("null", null)]
        [TestCase(" null", null)]
        public void NullableDoubleValue_CorrectlyDeserialized(string value, double? expected)
        {
            //arrange
            //act
            double? result = _valueFactory.FromJson(value);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}