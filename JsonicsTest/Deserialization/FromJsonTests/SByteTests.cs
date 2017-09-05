using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class SByteTests
    {
        IJsonConverter<SByteClass> _propertyFactory;
        IJsonConverter<sbyte> _valueFactory;

        public class SByteClass
        {
            public sbyte Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<SByteClass>();
            _valueFactory = JsonFactory.Compile<sbyte>();
        }

        [TestCase("1", 1)]
        [TestCase("-1", -1)]
        [TestCase(" 9", 9)]
        [TestCase("\n 127", 127)]
        [TestCase("\n -128", -128)]
        [TestCase(" 0", 0)]
        public void SByteProperty_CorrectlyDeserialized(string jsonValue, sbyte expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{jsonValue}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase("1", 1)]
        [TestCase("-1", -1)]
        [TestCase(" 9", 9)]
        [TestCase("\n 127", 127)]
        [TestCase("\n -128", -128)]
        [TestCase(" 0", 0)]
        public void SByteValue_CorrectlyDeserialized(string jsonValue, sbyte expected)
        {
            //arrange
            //act
            sbyte result = _valueFactory.FromJson(jsonValue);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}