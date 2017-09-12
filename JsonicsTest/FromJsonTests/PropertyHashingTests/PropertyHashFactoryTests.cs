using Jsonics.FromJson.PropertyHashing;
using NUnit.Framework;

namespace JsonicsTest.FromJsonTests.PropertyHashingTests
{
    [TestFixture]
    public class Propertyhashfactory
    {
        PropertyHashFactory _propertyHashFactory;
        string[] _propertiesColumnOneUnique = new string[]
        {
            "aaa",
            "aba",
            "aca",
            "ada"
        };

        [SetUp]
        public void Setup()
        {
            _propertyHashFactory = new PropertyHashFactory();
        }

        [Test]
        public void FindBestHash_OnlyColumnOneUnique_ChoosesColumnOne()
        {
            //arrange
            //act
            var hash = _propertyHashFactory.FindBestHash(_propertiesColumnOneUnique);

            //assert
            Assert.That(hash.Column, Is.EqualTo(1));
        }

        [Test]
        public void FindBestHash_OnlyColumnOneUnique_DoesntUseLength()
        {
            //arrange
            //act
            var hash = _propertyHashFactory.FindBestHash(_propertiesColumnOneUnique);

            //assert
            Assert.That(hash.UseLength, Is.False);
        }

        [Test]
        public void FindBestHash_NoCollisions_CollisionCountZero()
        {
            //arrange
            //act
            var hash = _propertyHashFactory.FindBestHash(_propertiesColumnOneUnique);

            //assert
            Assert.That(hash.CollisionCount, Is.EqualTo(0));
        }

        [Test]
        public void FindBestHash_NoCollisions_ModCorrect()
        {
            //arrange
            //act
            var hash = _propertyHashFactory.FindBestHash(_propertiesColumnOneUnique);

            //assert
            Assert.That(hash.ModValue, Is.EqualTo(4));
        }

        string[] _propertiesLengthOnlyDifference = new string[]
        {
            "x",
            "xx",
            "xxx",
            "xxxx"
        };

        [Test]
        public void FindBestHash_OnlyDifferenceLength_ChoosesLength()
        {
            //arrange
            //act
            var hash = _propertyHashFactory.FindBestHash(_propertiesLengthOnlyDifference);

            //assert
            Assert.That(hash.UseLength, Is.True);
        }

        [Test]
        public void FindBestHash_OnlyDifferenceLength_CorrectMod()
        {
            //arrange
            //act
            var hash = _propertyHashFactory.FindBestHash(_propertiesLengthOnlyDifference);

            //assert
            Assert.That(hash.ModValue, Is.EqualTo(4));
        }

        string[] _propertiesHasCollisions = new string[]
        {
            "caa",
            "cba",
            "cbd",
            "cbe"
        };

        [Test]
        public void FindBestHash_HasCollisions_CorrectCollisionCount()
        {
            //arrange
            //act
            var hash = _propertyHashFactory.FindBestHash(_propertiesHasCollisions);

            //assert
            Assert.That(hash.CollisionCount, Is.EqualTo(1));
        }

        [Test]
        public void FindBestHash_HasCollisions_ChoosesColumnWithFewestCollisions()
        {
            //arrange
            //act
            var hash = _propertyHashFactory.FindBestHash(_propertiesHasCollisions);

            //assert
            Assert.That(hash.Column, Is.EqualTo(2));
        }
    }
}