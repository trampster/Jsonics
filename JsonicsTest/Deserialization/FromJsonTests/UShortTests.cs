using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class UShortTests
    {
        IJsonConverter<UShortClass> _propertyFactory;
        IJsonConverter<ushort> _valueFactory;

        public class UShortClass
        {
            public ushort Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<UShortClass>();
            _valueFactory = JsonFactory.Compile<ushort>();
        }

        [TestCase((ushort)0)]
        [TestCase((ushort)1)]
        [TestCase(ushort.MaxValue)]
        [TestCase(ushort.MinValue)]
        public void UShortProperty_CorrectlyDeserialized(ushort expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{expected}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase((ushort)0)]
        [TestCase((ushort)1)]
        [TestCase(ushort.MaxValue)]
        [TestCase(ushort.MinValue)]
        public void UShortValue_CorrectlyDeserialized(ushort expected)
        {
            //arrange
            //act
            ushort result = _valueFactory.FromJson($"{expected}");

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}