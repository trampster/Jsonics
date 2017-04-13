using Jsonics.PropertyHashing;
using NUnit.Framework;

namespace JsonicsTest
{
    [TestFixture]
    public class ColumnCollisionsTests
    {
        [Test]
        public void NumberOfCollisions_NoCollisionsYe_StartsAtZero()
        {
            //arrange
            //act
            var columnCollisions = new ColumnCollisions(1);
            
            //assert
            Assert.That(columnCollisions.NumberOfCollisions, Is.EqualTo(0));
        }

        [Test]
        public void AddCollision_Called_IncrementsNumberOfCollisions()
        {
            //arrange
            var columnCollisions = new ColumnCollisions(1);

            //act
            columnCollisions.AddCollision();
            
            //assert
            Assert.That(columnCollisions.NumberOfCollisions, Is.EqualTo(1));
        }

        [Test]
        public void ColumnIndex_PassedToConstructor_ReturnsCorrectValue()
        {
            //arrange
            //act
            var columnCollisions = new ColumnCollisions(43);
            
            //assert
            Assert.That(columnCollisions.ColumnIndex, Is.EqualTo(43));
        }
    }
}