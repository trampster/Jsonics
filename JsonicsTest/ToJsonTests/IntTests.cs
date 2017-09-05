using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.ToJsonTests
{
    [TestFixture]
    public class IntTests
    {
        [TestCase(0,"0")]
        [TestCase(1,"1")]
        [TestCase(-1,"-1")]
        [TestCase(42,"42")]
        [TestCase(int.MaxValue,"2147483647")]
        [TestCase(int.MinValue,"-2147483648")]
        public void ToJson_Int_CorrectJson(int input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<int>();

            //act
            string json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }
    }
}