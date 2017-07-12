using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class ULongTests
    {
        IJsonConverter<ULongClass> _propertyFactory;
        IJsonConverter<ulong> _valueFactory;
        
        public class ULongClass
        {
            public ulong Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<ULongClass>();
            _valueFactory = JsonFactory.Compile<ulong>();
        }

        [TestCase((ulong)0)]
        [TestCase((ulong)1)]
        [TestCase((ulong)42)]
        [TestCase(ulong.MaxValue)]
        [TestCase(ulong.MinValue)]
        public void ULongProperty_CorrectlyDeserialized(ulong expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{expected}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCase((ulong)0)]
        [TestCase((ulong)1)]
        [TestCase((ulong)42)]
        [TestCase(ulong.MaxValue)]
        [TestCase(ulong.MinValue)]
        public void ULongValue_CorrectlyDeserialized(ulong expected)
        {
            //arrange
            //act
            ulong result = _valueFactory.FromJson($"{expected}");

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}