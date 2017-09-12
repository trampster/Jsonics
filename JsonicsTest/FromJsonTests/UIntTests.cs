using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class UIntTests
    {
        IJsonConverter<UIntClass> _propertyFactory;
        IJsonConverter<uint> _valueFactory;
        

        public class UIntClass
        {
            public uint Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<UIntClass>();
            _valueFactory = JsonFactory.Compile<uint>();
        }

        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(uint.MaxValue)]
        [TestCase(uint.MinValue)]
        public void UIntProperty_CorrectlyDeserialized(uint expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{expected}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(uint.MaxValue)]
        [TestCase(uint.MinValue)]
        public void UIntValue_CorrectlyDeserialized(uint expected)
        {
            //arrange
            //act
            uint result = _valueFactory.FromJson($"{expected}");

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}