using Jsonics;
using NUnit.Framework;

namespace JsonicsTest.Deserialization
{
    [TestFixture]
    public class LazyStringTests
    {
        [Test]
        public void ToString_FullLength_ReturnsCompleteString()
        {
            //arrange
            const string testString = "test string";
            var lazyString = new LazyString(testString);

            //act
            var result = lazyString.ToString();

            //assert
            Assert.That(result, Is.EqualTo(testString));
        }

        [TestCase(0, 4, "test")]
        [TestCase(5, 6, "string")]
        [TestCase(1, 7, "est str")]
        public void ToString_Partial_ReturnsCorrectPortionOfString(int start, int length, string expected)
        {
            //arrange
            const string testString = "test string";
            var lazyString = new LazyString(testString, start, length);

            //act
            var result = lazyString.ToString();

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(0, 4, "test")]
        [TestCase(5, 6, "string")]
        [TestCase(1, 7, "est str")]
        public void SubString_Partial_CorrectPortion(int start, int length, string expected)
        {
            //arrange
            const string testString = "test string";
            var lazyString = new LazyString(testString);

            //act
            var result = lazyString.SubString(start, length);

            //assert
            Assert.That(result.ToString(), Is.EqualTo(expected));
        }

        [TestCaseAttribute('t', 0)]
        [TestCaseAttribute('e', 1)]
        [TestCaseAttribute(' ', 4)]
        [TestCaseAttribute('n', 9)]
        [TestCaseAttribute('g', 10)]
        public void ReadTo_ValueExists_ReturnsIndex(char value, int expectedIndex)
        {
            //arrange
            var lazyString = new LazyString("test string");

            //act
            var index = lazyString.ReadTo(0, ' ');

            //assert
            Assert.That(index, Is.EqualTo(4));
        }

        [Test]
        public void ReadTo_StartSet_DoesntConsiderBeforeStart()
        {
            var lazyString = new LazyString("test string");

            //act
            var index = lazyString.ReadTo(1, 't');

            //assert
            Assert.That(index, Is.EqualTo(3));
        }

        [Test]
        public void ReadTo_Partial_DoesntConsiderBeforeRange()
        {
            var lazyString = new LazyString("test string", 2, 5);

            //act
            var index = lazyString.ReadTo(0, 't');

            //assert
            Assert.That(index, Is.EqualTo(3));
        }

        [Test]
        public void ReadTo_Partial_DoesntConsiderAfterRange()
        {
            //arrange
            var lazyString = new LazyString("test string", 4, 1);

            //act
            var index = lazyString.ReadTo(0, 't');

            //assert
            Assert.That(index, Is.EqualTo(-1));
        }

        [Test]
        public void ReadTo_NotFound_ReturnsNegitiveOne()
        {
            //arrange
            var lazyString = new LazyString("test string", 4, 1);

            //act
            var index = lazyString.ReadTo(0, ':');

            //assert
            Assert.That(index, Is.EqualTo(-1));
        }

        [TestCase("1",1)]
        [TestCase("12",12)]
        [TestCase("123",123)]
        [TestCase("-123",-123)]
        [TestCase("1234",1234)]
        [TestCase("12345",12345)]
        [TestCase("123456",123456)]
        [TestCase("1234567",1234567)]
        [TestCase("12345678",12345678)]
        [TestCase("123456789",123456789)]
        [TestCase("2147483647", int.MaxValue)]
        [TestCase("-2147483648", int.MinValue)]
        public void ToInt_JustValue_ReturnsValue(string numberString, int expected)
        {
            //arrange
            var lazyString = new LazyString(numberString);

            //act
            (int number, int index) = lazyString.ToInt(0);

            //assert
            Assert.That(number, Is.EqualTo(expected));
        }

        [Test]
        public void ToInt_ThingsAfterInt_CorrectAfterIndex()
        {
            //arrange
            var lazyString = new LazyString(":2}");

            //act
            (int number, int index) = lazyString.ToInt(1);

            //assert
            Assert.That(index, Is.EqualTo(2));
        }
    }
}