using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class EnumTests
    {
        IJsonConverter<EnumClass> _propertyFactory;
        IJsonConverter<TestEnum> _valueFactory;

        public enum TestEnum
        {
            Zero,
            One,
            Two,
        }

        public class EnumClass
        {
            public TestEnum Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<EnumClass>();
            _valueFactory = JsonFactory.Compile<TestEnum>();
        }

        [TestCase("0", TestEnum.Zero)]
        [TestCase("1", TestEnum.One)]
        [TestCase("2", TestEnum.Two)]
        [TestCase(" 1", TestEnum.One)]
        [TestCase("\n 1", TestEnum.One)]
        public void EnumProperty_CorrectlyDeserialized(string jsonValue, TestEnum expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{jsonValue}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase("0", TestEnum.Zero)]
        [TestCase("1", TestEnum.One)]
        [TestCase("2", TestEnum.Two)]
        [TestCase(" 1", TestEnum.One)]
        [TestCase("\n 1", TestEnum.One)]
        public void EnumValue_CorrectlyDeserialized(string jsonValue, TestEnum expected)
        {
            //arrange
            //act
            TestEnum result = _valueFactory.FromJson(jsonValue);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}