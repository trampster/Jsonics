using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class StructFieldTests
    {
        [Test]
        public void FromJson_TwoFields_ReturnsClassInstance()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<TwoFields>();

            //act
            var instance = jsonConverter.FromJson("{\"FirstName\":\"Ob\\t Won\",\"LastName\":\"Ken\\noby\",\"Age\":60,\"PowerFactor\":104.6789,\"IsJedi\":true}");

            //assert
            Assert.That(instance, Is.Not.Null);
        }

        public struct TwoFields
        {
            public int First;

            public int Secon;
        }

        [Test]
        public void FromJson_TestClass_PropertiesSetCorrectly()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<TwoFields>();

            //act
            var instance = jsonConverter.FromJson("{\"First\":1,\"Secon\":2}");

            //assert
            Assert.That(instance.First, Is.EqualTo(1));
            Assert.That(instance.Secon, Is.EqualTo(2));
        }

        public struct ThreeFields
        {
            public int First;

            public int Second;

            public int Third;
        }

        [Test]
        public void FromJson_ThreeFields_PropertiesSetCorrectly()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<ThreeFields>();

            //act
            var instance = jsonConverter.FromJson("{\"First\":1,\"Second\":2,\"Third\":3}");

            //assert
            Assert.That(instance.First, Is.EqualTo(1));
            Assert.That(instance.Second, Is.EqualTo(2));
            Assert.That(instance.Third, Is.EqualTo(3));
        }

        public struct CollisionFields
        {
            public int AAA;

            public int AAB;

            public int BAA;
        }

        [Test]
        public void FromJson_HashCollision_FieldsSetCorrectly()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<CollisionFields>();

            //act
            var instance = jsonConverter.FromJson("{\"AAA\":1,\"AAB\":2,\"BAA\":3}");

            //assert
            Assert.That(instance.AAA, Is.EqualTo(1));
            Assert.That(instance.AAB, Is.EqualTo(2));
            Assert.That(instance.BAA, Is.EqualTo(3));
        }
    }
}