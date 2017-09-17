using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class ClassFieldTests
    {
        [Test]
        public void FromJson_TwoFields_ReturnsClassInstance()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<TwoFields>();

            //act
            var instance = jsonConverter.FromJson("{\"First\":1,\"Secon\":2}");

            //assert
            Assert.That(instance, Is.Not.Null);
        }

        public class TwoFields
        {
            public int First;

            public int Secon;
        }

        [Test]
        public void FromJson_TestClass_FieldsSetCorrectly()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<TwoFields>();

            //act
            var instance = jsonConverter.FromJson("{\"First\":1,\"Secon\":2}");

            //assert
            Assert.That(instance.First, Is.EqualTo(1));
            Assert.That(instance.Secon, Is.EqualTo(2));
        }

        [TestCase("null")]
        [TestCase(" null")]
        [TestCase("\tnull")]
        public void FromJson_Null_ReturnsNull(string json)
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<TwoFields>();

            //act
            var instance = jsonConverter.FromJson(json);

            //assert
            Assert.That(instance, Is.Null);
        }

        public class ThreeFields
        {
            public int First;

            public int Second;

            public int Third;
        }

        [Test]
        public void FromJson_ThreeFields_FieldsSetCorrectly()
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

        public class CollisionFields
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