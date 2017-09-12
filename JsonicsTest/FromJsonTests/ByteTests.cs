using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class ByteTests
    {
        IJsonConverter<ByteClass> _propertyFactory;
        IJsonConverter<byte> _valueFactory;

        public class ByteClass
        {
            public byte Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<ByteClass>();
            _valueFactory = JsonFactory.Compile<byte>();
        }

        [TestCase("1", 1)]
        [TestCase(" 9", 9)]
        [TestCase("\n 255", 255)]
        [TestCase(" 0", 0)]
        public void ByteProperty_CorrectlyDeserialized(string jsonValue, byte expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{jsonValue}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase("1", 1)]
        [TestCase(" 9", 9)]
        [TestCase("\n 255", 255)]
        [TestCase(" 0", 0)]
        public void ByteValue_CorrectlyDeserialized(string jsonValue, byte expected)
        {
            //arrange
            //act
            byte result = _valueFactory.FromJson(jsonValue);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}