using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.ToJsonTests
{
    [TestFixture]
    public class ArrayTests
    {
        [Test]
        public void ToJson_StringArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<string[]>();

            //act
            string json = converter.ToJson(new string[]{"1","2","3","4","5"});

            //assert
            Assert.That(json, Is.EqualTo("[\"1\",\"2\",\"3\",\"4\",\"5\"]"));
        }

        [Test]
        public void ToJson_IntArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<int[]>();

            //act
            string json = converter.ToJson(new int[]{1,2,3,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,2,3,4,5]"));
        }
    }
}