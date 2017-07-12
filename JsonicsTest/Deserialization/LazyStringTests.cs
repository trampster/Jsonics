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
            Assert.That(index, Is.EqualTo(1));
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
        [TestCase(" 123", 123)]
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

        [Test]
        public void ToInt_MaxValue_CorrectAfterIndex()
        {
            //arrange
            var lazyString = new LazyString($":{int.MaxValue}}}");

            //act
            (int number, int index) = lazyString.ToInt(1);

            //assert
            Assert.That(index, Is.EqualTo(11));
        }

        [TestCase("\"test\"", 0, 6, 0, "test")] // most basic example
        [TestCase("\"te\\\"st\"", 0, 7, 0, "te\"st")] //with escaping
        [TestCase("\"propety\":\"name\",", 0, 16, 9, "name")] //index not at start
        [TestCase("\"extrapropety\":\"name\",", 6, 16, 9, "name")] //lazy string not at start
        [TestCase("   \"test\"", 0, 6, 0, "test")] // whitespace at start
        [TestCase("null", 0, 4, 0, null)] // null at start
        [TestCase(" null", 0, 4, 0, null)] // null with whitespace at start
        public void ToString_CorrectString(string lazy, int start, int length, int index, string expected)
        {
            //arrange
            var lazyString = new LazyString(lazy, start, length);

            //act
            (string result, int endIndex) = lazyString.ToString(index);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }


        [TestCase("true", 0, 4, 0, true)]
        [TestCase("false", 0, 5, 0, false)]
        [TestCase(" true", 0, 5, 0, true)]
        [TestCase(" false", 0, 6, 0, false)]
        [TestCase("\"property\":true", 11, 4, 0, true)]
        [TestCase("\"property\":false", 11, 5, 0, false)]
        public void ToBool_CorrectBool(string lazy, int start, int length, int index, bool expected)
        {
            var lazyString = new LazyString(lazy, start, length);

            //act
            (bool result, int endIndex) = lazyString.ToBool(index);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("123", 0, 3, 0, 123)]
        [TestCase(" 123", 0, 4, 1, 123)]
        [TestCase(" 123", 0, 4, 0, 123)]
        [TestCase("1", 0, 1, 0, 1)]
        [TestCase("\"property\":255,", 11, 4, 0, 255)]
        [TestCase("0", 0, 1, 0, 0)]
        [TestCase("42", 0, 2, 0, 42)]
        public void ToByte_CorrectResult(string lazy, int start, int length, int index, byte expected)
        {
            var lazyString = new LazyString(lazy, start, length);

            //act
            (byte result, int endIndex) = lazyString.ToByte(index);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("1",1,1)]
        [TestCase("12",12,2)]
        [TestCase("123",123,3)]
        [TestCase("-123",-123,4)]
        [TestCase("1234",1234,4)]
        [TestCase("12345",12345,5)]
        [TestCase("32767", short.MaxValue,5)]
        [TestCase("-32768", short.MinValue,6)]
        [TestCase(" 123", 123, 4)]
        public void ToShort_JustValue_ReturnsValue(string numberString, int expectedValue, int expectedIndex)
        {
            //arrange
            var lazyString = new LazyString(numberString);

            //act
            (int number, int index) = lazyString.ToShort(0);

            //assert
            Assert.That(number, Is.EqualTo(expectedValue));
            Assert.That(index, Is.EqualTo(expectedIndex));
        }

        [TestCase("1", 0, 1, 0, 1u, 1)]
        [TestCase("12", 0, 2, 0, 12u, 2)]
        [TestCase("123", 0, 3, 0, 123u, 3)]
        [TestCase("1234", 0, 4, 0, 1234u, 4)]
        [TestCase("12345", 0, 5, 0, 12345u, 5)]
        [TestCase("123456", 0, 6, 0, 123456u, 6)]
        [TestCase("1234567", 0, 7, 0, 1234567u, 7)]
        [TestCase("12345678", 0, 8, 0, 12345678u, 8)]
        [TestCase("123456789", 0, 9, 0, 123456789u, 9)]
        [TestCase("4294967295", 0, 10, 0, uint.MaxValue, 10)]
        [TestCase("0", 0, 1, 0, uint.MinValue, 1)]
        [TestCase("\"property\":42,", 11, 2, 0, 42u, 2)]
        [TestCase(" 42", 0, 3, 0, 42u, 3)]
        [TestCase(" 42", 0, 3, 1, 42u, 3)]
        public void ToUint_CorrectResult(string lazy, int start, int length, int index, uint expected, int expectedEndIndex)
        {
            var lazyString = new LazyString(lazy, start, length);

            //act
            (uint result, int endIndex) = lazyString.ToUInt(index);

            //assert
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(endIndex, Is.EqualTo(expectedEndIndex));
        }

        [TestCase("1", 0, 1, 0, (ushort)1, 1)]
        [TestCase("12", 0, 2, 0, (ushort)12, 2)]
        [TestCase("123", 0, 3, 0, (ushort)123, 3)]
        [TestCase("1234", 0, 4, 0, (ushort)1234, 4)]
        [TestCase("12345", 0, 5, 0, (ushort)12345, 5)]
        [TestCase("65535", 0, 5, 0, ushort.MaxValue, 5)]
        [TestCase("0", 0, 1, 0, ushort.MinValue, 1)]
        [TestCase("\"property\":42,", 11, 2, 0, (ushort)42, 2)]
        [TestCase(" 42", 0, 3, 0, (ushort)42, 3)]
        [TestCase(" 42", 0, 3, 1, (ushort)42, 3)]
        public void ToUShort_CorrectResult(string lazy, int start, int length, int index, ushort expected, int expectedEndIndex)
        {
            var lazyString = new LazyString(lazy, start, length);

            //act
            (ushort result, int endIndex) = lazyString.ToUShort(index);

            //assert
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(endIndex, Is.EqualTo(expectedEndIndex));
        }

        [TestCase("1", 0, 1, 0, (long)1, 1)]
        [TestCase("-1", 0, 2, 0, (long)-1, 2)]
        [TestCase("12", 0, 2, 0, (long)12, 2)]
        [TestCase("123", 0, 3, 0, (long)123, 3)]
        [TestCase("1234", 0, 4, 0, (long)1234, 4)]
        [TestCase("12345", 0, 5, 0, (long)12345, 5)]
        [TestCase("123456", 0, 6, 0, (long)123456, 6)]
        [TestCase("1234567", 0, 7, 0, (long)1234567, 7)]
        [TestCase("12345678", 0, 8, 0, (long)12345678, 8)]
        [TestCase("123456789", 0, 9, 0, (long)123456789, 9)]
        [TestCase("1234567890", 0, 10, 0, (long)1234567890, 10)]
        [TestCase("12345678901", 0, 11, 0, (long)12345678901, 11)]
        [TestCase("123456789012", 0, 12, 0, (long)123456789012, 12)]
        [TestCase("1234567890123", 0, 13, 0, (long)1234567890123, 13)]
        [TestCase("12345678901234", 0, 14, 0, (long)12345678901234, 14)]
        [TestCase("123456789012345", 0, 15, 0, (long)123456789012345, 15)]
        [TestCase("1234567890123456", 0, 16, 0, (long)1234567890123456, 16)]
        [TestCase("12345678901234567", 0, 17, 0, (long)12345678901234567, 17)]
        [TestCase("123456789012345678", 0, 18, 0, (long)123456789012345678, 18)]
        [TestCase("9223372036854775807", 0, 19, 0, long.MaxValue, 19)]
        [TestCase("-9223372036854775808", 0, 20, 0, long.MinValue, 20)]
        [TestCase("\"property\":42,", 11, 2, 0, (long)42, 2)]
        [TestCase(" 42", 0, 3, 0, (long)42, 3)]
        [TestCase(" 42", 0, 3, 1, (long)42, 3)]
        public void ToLong_CorrectResult(string lazy, int start, int length, int index, long expected, int expectedEndIndex)
        {
            var lazyString = new LazyString(lazy, start, length);

            //act
            (long result, int endIndex) = lazyString.ToLong(index);

            //assert
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(endIndex, Is.EqualTo(expectedEndIndex));
        }

        [TestCase("1", 0, 1, 0, (ulong)1, 1)]
        [TestCase("12", 0, 2, 0, (ulong)12, 2)]
        [TestCase("123", 0, 3, 0, (ulong)123, 3)]
        [TestCase("1234", 0, 4, 0, (ulong)1234, 4)]
        [TestCase("12345", 0, 5, 0, (ulong)12345, 5)]
        [TestCase("123456", 0, 6, 0, (ulong)123456, 6)]
        [TestCase("1234567", 0, 7, 0, (ulong)1234567, 7)]
        [TestCase("12345678", 0, 8, 0, (ulong)12345678, 8)]
        [TestCase("123456789", 0, 9, 0, (ulong)123456789, 9)]
        [TestCase("1234567890", 0, 10, 0, (ulong)1234567890, 10)]
        [TestCase("12345678901", 0, 11, 0, (ulong)12345678901, 11)]
        [TestCase("123456789012", 0, 12, 0, (ulong)123456789012, 12)]
        [TestCase("1234567890123", 0, 13, 0, (ulong)1234567890123, 13)]
        [TestCase("12345678901234", 0, 14, 0, (ulong)12345678901234, 14)]
        [TestCase("123456789012345", 0, 15, 0, (ulong)123456789012345, 15)]
        [TestCase("1234567890123456", 0, 16, 0, (ulong)1234567890123456, 16)]
        [TestCase("12345678901234567", 0, 17, 0, (ulong)12345678901234567, 17)]
        [TestCase("123456789012345678", 0, 18, 0, (ulong)123456789012345678, 18)]
        [TestCase("1234567890123456789", 0, 19, 0, (ulong)1234567890123456789, 19)]
        [TestCase("18446744073709551615", 0, 20, 0, ulong.MaxValue, 20)]
        [TestCase("0", 0, 1, 0, ulong.MinValue, 1)]
        [TestCase("\"property\":42,", 11, 2, 0, (ulong)42, 2)]
        [TestCase(" 42", 0, 3, 0, (ulong)42, 3)]
        [TestCase(" 42", 0, 3, 1, (ulong)42, 3)]
        public void ToULong_CorrectResult(string lazy, int start, int length, int index, ulong expected, int expectedEndIndex)
        {
            var lazyString = new LazyString(lazy, start, length);

            //act
            (ulong result, int endIndex) = lazyString.ToULong(index);

            //assert
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(endIndex, Is.EqualTo(expectedEndIndex));
        }

        [TestCase("1", 0, 1, 0, (ulong)1, 1)]
        [TestCase("12", 0, 2, 0, (ulong)12, 2)]
        [TestCase("123", 0, 3, 0, (ulong)123, 3)]
        [TestCase("1234", 0, 4, 0, (ulong)1234, 4)]
        [TestCase("12345", 0, 5, 0, (ulong)12345, 5)]
        [TestCase("123456", 0, 6, 0, (ulong)123456, 6)]
        [TestCase("1234567", 0, 7, 0, (ulong)1234567, 7)]
        [TestCase("12345678", 0, 8, 0, (ulong)12345678, 8)]
        [TestCase("123456789", 0, 9, 0, (ulong)123456789, 9)]
        [TestCase("1234567890", 0, 10, 0, (ulong)1234567890, 10)]
        [TestCase("12345678901", 0, 11, 0, (ulong)12345678901, 11)]
        [TestCase("123456789012", 0, 12, 0, (ulong)123456789012, 12)]
        [TestCase("1234567890123", 0, 13, 0, (ulong)1234567890123, 13)]
        [TestCase("12345678901234", 0, 14, 0, (ulong)12345678901234, 14)]
        [TestCase("123456789012345", 0, 15, 0, (ulong)123456789012345, 15)]
        [TestCase("1234567890123456", 0, 16, 0, (ulong)1234567890123456, 16)]
        [TestCase("12345678901234567", 0, 17, 0, (ulong)12345678901234567, 17)]
        [TestCase("123456789012345678", 0, 18, 0, (ulong)123456789012345678, 18)]
        [TestCase("1234567890123456789", 0, 19, 0, (ulong)1234567890123456789, 19)]
        [TestCase("18446744073709551615", 0, 20, 0, ulong.MaxValue, 20)]
        [TestCase("0", 0, 1, 0, ulong.MinValue, 1)]
        [TestCase("\"property\":42,", 11, 2, 0, (ulong)42, 2)]
        [TestCase(" 42", 0, 3, 0, (ulong)42, 3)]
        [TestCase(" 42", 0, 3, 1, (ulong)42, 3)]
        [TestCase("null", 0, 4, 0, null, 4)]
        [TestCase(" null", 0, 5, 1, null, 5)]
        [TestCase(" null", 1, 4, 0, null, 4)]
        public void ToNullableULong_CorrectResult(string lazy, int start, int length, int index, ulong? expected, int expectedEndIndex)
        {
            var lazyString = new LazyString(lazy, start, length);

            //act
            (ulong? result, int endIndex) = lazyString.ToNullableULong(index);

            //assert
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(endIndex, Is.EqualTo(expectedEndIndex));
        }
    }
}