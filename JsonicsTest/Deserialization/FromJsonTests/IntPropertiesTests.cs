using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class IntPropertyTests
    {
        [Test]
        public void FromJson_TwoProperties_ReturnsClassInstance()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<TwoProperties>();

            //act
            var instance = jsonConverter.FromJson("{\"FirstName\":\"Ob\\t Won\",\"LastName\":\"Ken\\noby\",\"Age\":60,\"PowerFactor\":104.6789,\"IsJedi\":true}");

            //assert
            Assert.That(instance, Is.Not.Null);
        }

        public class TwoProperties
        {
            public int First
            {
                get;
                set;
            }

            public int Secon
            {
                get;
                set;
            }
        }

        [Test]
        public void FromJson_TestClass_PropertiesSetCorrectly()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<TwoProperties>();

            //act
            var instance = jsonConverter.FromJson("{\"First\":1,\"Secon\":2}");

            //assert
            Assert.That(instance.First, Is.EqualTo(1));
            Assert.That(instance.Secon, Is.EqualTo(2));
        }

        public class ThreeProperties
        {
            public int First
            {
                get;
                set;
            }

            public int Second
            {
                get;
                set;
            }

            public int Third
            {
                get;
                set;
            }
        }

        [Test]
        public void FromJson_ThreeProperties_PropertiesSetCorrectly()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<ThreeProperties>();

            //act
            var instance = jsonConverter.FromJson("{\"First\":1,\"Second\":2,\"Third\":3}");

            //assert
            Assert.That(instance.First, Is.EqualTo(1));
            Assert.That(instance.Second, Is.EqualTo(2));
            Assert.That(instance.Third, Is.EqualTo(3));
        }
    }
}