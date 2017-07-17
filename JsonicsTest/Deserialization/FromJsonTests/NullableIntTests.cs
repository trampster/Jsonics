using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class NullableIntTests
    {
        IJsonConverter<NullableIntClass> _propertyFactory;
        IJsonConverter<int?> _valueFactory;
        

        public class NullableIntClass
        {
            public int? Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<NullableIntClass>();
            _valueFactory = JsonFactory.Compile<int?>();
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        [TestCase(null)]
        public void IntProperty_CorrectlyDeserialized(int? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();

            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{value}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        [TestCase(null)]
        public void IntValue_CorrectlyDeserialized(int? expected)
        {
            //arrange
            var value = expected == null ? "null" : expected.ToString();
            
            //act
            int? result = _valueFactory.FromJson($"{value}");

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}