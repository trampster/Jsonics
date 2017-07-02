using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class IntTests
    {
        IJsonConverter<IntClass> _propertyFactory;
        IJsonConverter<int> _valueFactory;
        

        public class IntClass
        {
            public int Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<IntClass>();
            //_valueFactory = JsonFactory.Compile<int>();
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public void IntProperty_CorrectlyDeserialized(int expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{expected}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }
    }
}