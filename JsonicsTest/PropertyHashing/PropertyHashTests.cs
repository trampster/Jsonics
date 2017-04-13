using Jsonics.PropertyHashing;
using NUnit.Framework;

namespace JsonicsTest.PropertyHashing
{
    [TestFixture]
    public class PropertyHashTests
    {
        [Test]
        public void Hash_FirstColumn_CorrectHash()
        {
            //arrange
            var propertyHash = new PropertyHash()
            {
                Column = 0,
                ModValue = 13,
            };

            //act
            int hash = propertyHash.Hash("abcd");

            //assert
            Assert.That(hash, Is.EqualTo(6));
        }

        [Test]
        public void Hash_ColumnPastEnd_CorrectHash()
        {
            //arrange
            var propertyHash = new PropertyHash()
            {
                Column = 5,
                ModValue = 13,
            };

            //act
            int hash = propertyHash.Hash("cbcd");

            //assert
            Assert.That(hash, Is.EqualTo(7));
        }

        [Test]
        public void Hash_Length_CorrectHash()
        {
            //arrange
            var propertyHash = new PropertyHash()
            {
                UseLength = true,
                ModValue = 7,
            };

            //act
            int hash = propertyHash.Hash("abcde");

            //assert
            Assert.That(hash, Is.EqualTo(5));
        }

        [Test]
        public void IsBetterHash_MoreCollissions_NotBetter()
        {
            //arrange
            var propertyHash1 = new PropertyHash()
            {
                CollisionCount = 1
            };
            var propertyHash2 = new PropertyHash()
            {
                CollisionCount = 2
            };

            //act
            var isBetter = propertyHash1.IsBetterHash(propertyHash2);

            //assert
            Assert.That(isBetter, Is.False);
        }

        [Test]
        public void IsBetterHash_LessCollissions_Better()
        {
            //arrange
            var propertyHash1 = new PropertyHash()
            {
                CollisionCount = 2
            };
            var propertyHash2 = new PropertyHash()
            {
                CollisionCount = 1
            };

            //act
            var isBetter = propertyHash1.IsBetterHash(propertyHash2);

            //assert
            Assert.That(isBetter, Is.True);
        }

        [Test]
        public void IsBetterHash_LessCollissionslargerMod_Better()
        {
            //arrange
            var propertyHash1 = new PropertyHash()
            {
                CollisionCount = 2,
                ModValue = 7
            };
            var propertyHash2 = new PropertyHash()
            {
                CollisionCount = 1,
                ModValue = 17
            };

            //act
            var isBetter = propertyHash1.IsBetterHash(propertyHash2);

            //assert
            Assert.That(isBetter, Is.True);
        }

        [Test]
        public void IsBetterHash_SameCollissionsLargerMod_NotBetter()
        {
            //arrange
            var propertyHash1 = new PropertyHash()
            {
                CollisionCount = 2,
                ModValue = 7
            };
            var propertyHash2 = new PropertyHash()
            {
                CollisionCount = 2,
                ModValue = 17
            };

            //act
            var isBetter = propertyHash1.IsBetterHash(propertyHash2);

            //assert
            Assert.That(isBetter, Is.False);
        }

        [Test]
        public void IsBetterHash_SameCollissionsSmallerMod_Better()
        {
            //arrange
            var propertyHash1 = new PropertyHash()
            {
                CollisionCount = 2,
                ModValue = 7
            };
            var propertyHash2 = new PropertyHash()
            {
                CollisionCount = 2,
                ModValue = 6
            };

            //act
            var isBetter = propertyHash1.IsBetterHash(propertyHash2);

            //assert
            Assert.That(isBetter, Is.True);
        }
    }
}