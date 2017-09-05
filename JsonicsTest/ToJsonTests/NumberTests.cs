using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.ToJsonTests
{
    [TestFixture]
    public class NumberTests
    {
        [TestCase(1,"1")]
        [TestCase(-1,"-1")]
        [TestCase(42.42,"42.42")]
        [TestCase(double.MaxValue,"1.79769313486232E+308")]
        [TestCase(double.MinValue,"-1.79769313486232E+308")]
        public void ToJson_Double_CorrectJson(double input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<double>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [TestCase(1,"1")]
        [TestCase(-1,"-1")]
        [TestCase(42.42f,"42.42")]
        [TestCase(float.MaxValue,"3.402823E+38")]
        [TestCase(float.MinValue,"-3.402823E+38")]
        public void ToJson_Float_CorrectJson(float input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<float>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [TestCase((short)1,"1")]
        [TestCase((short)-1,"-1")]
        [TestCase((short)42,"42")]
        [TestCase(short.MaxValue,"32767")]
        [TestCase(short.MinValue,"-32768")]
        public void ToJson_Short_CorrectJson(short input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<short>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [TestCase((ushort)1,"1")]
        [TestCase((ushort)42,"42")]
        [TestCase(ushort.MaxValue,"65535")]
        [TestCase(ushort.MinValue,"0")]
        public void ToJson_UShort_CorrectJson(ushort input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<ushort>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [TestCase(1,"1")]
        [TestCase(-1,"-1")]
        [TestCase(42,"42")]
        [TestCase(sbyte.MaxValue,"127")]
        [TestCase(sbyte.MinValue,"-128")]
        public void ToJson_SByte_CorrectJson(sbyte input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<sbyte>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [TestCase(1,"1")]
        [TestCase(42,"42")]
        [TestCase(byte.MaxValue,"255")]
        [TestCase(byte.MinValue,"0")]
        public void ToJson_SByte_CorrectJson(byte input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<byte>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [TestCase(0,"0")]
        [TestCase(1,"1")]
        [TestCase(-1,"-1")]
        [TestCase(42,"42")]
        [TestCase(long.MaxValue,"9223372036854775807")]
        [TestCase(long.MinValue,"-9223372036854775808")]
        public void ToJson_Long_CorrectJson(long input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<long>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [TestCase((ulong)0,"0")]
        [TestCase((ulong)1,"1")]
        [TestCase((ulong)42,"42")]
        [TestCase(ulong.MaxValue,"18446744073709551615")]
        [TestCase(ulong.MinValue,"0")]
        public void ToJson_ULong_CorrectJson(ulong input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<ulong>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [TestCase(0u,"0")]
        [TestCase(1u,"1")]
        [TestCase(42u,"42")]
        [TestCase(uint.MaxValue,"4294967295")]
        [TestCase(uint.MinValue,"0")]
        public void ToJson_UInt_CorrectJson(uint input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<uint>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }
    }
}