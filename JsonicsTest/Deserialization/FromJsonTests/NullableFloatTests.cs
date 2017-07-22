using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class NullableFloatTests
    {
        IJsonConverter<NullableFloatClass> _propertyFactory;
        IJsonConverter<float?> _valueFactory;
        

        public class NullableFloatClass
        {
            public float? Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<NullableFloatClass>();
            _valueFactory = JsonFactory.Compile<float?>();
        }

        [TestCase("0", 0f)]
        [TestCase("1", 1f)]
        [TestCase("-1", -1f)]
        [TestCase("1.23", 1.23f)]
        [TestCase("-1.23E4", -1.23E4f)]
        [TestCase("3.40282347E+38", float.MaxValue)]
        [TestCase("-3.40282347E+38F", float.MinValue)]
        [TestCase("null", null)]
        [TestCase(" null", null)]
        public void NullableDoubleProperty_CorrectlyDeserialized(string value, float? expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{value}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase("0", 0f)]
        [TestCase("1", 1f)]
        [TestCase("-1", -1f)]
        [TestCase("1.23", 1.23f)]
        [TestCase("-1.23E4", -1.23E4f)]
        [TestCase("3.40282347E+38", float.MaxValue)]
        [TestCase("-3.40282347E+38", float.MinValue)]
        [TestCase("null", null)]
        [TestCase(" null", null)]
        public void NullableDoubleValue_CorrectlyDeserialized(string value, float? expected)
        {
            //arrange
            //act
            float? result = _valueFactory.FromJson(value);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}