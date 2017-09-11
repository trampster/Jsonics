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

        public class IntArrayObject
        {
            public int[] IntArrayProperty { get; set; }
        }

        [Test]
        public void ToJson_IntArrayObjectNullProperty_PropertyNotIncludedInJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<IntArrayObject>();
            
            var testObject = new IntArrayObject();

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"IntArrayProperty\":null}"));
        }

        [Test]
        public void ToJson_IntArrayProperty_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<IntArrayObject>();
            
            var testObject = new IntArrayObject()
            {
                IntArrayProperty = new int[]{1,2,3,4,5}
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"IntArrayProperty\":[1,2,3,4,5]}"));
        }

        public class StringArrayObject
        {
            public string[] StringArrayProperty { get; set; }
        }

        [Test]
        public void ToJson_StringArrayObjectNullProperty_PropertyNotIncludedInJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringArrayObject>();
            
            var testObject = new StringArrayObject();

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringArrayProperty\":null}"));
        }

        [Test]
        public void ToJson_StringArrayProperty_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringArrayObject>();
            
            var testObject = new StringArrayObject()
            {
                StringArrayProperty = new string[]{"1","2","3","4","5"}
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringArrayProperty\":[\"1\",\"2\",\"3\",\"4\",\"5\"]}"));
        }

        [Test]
        public void ToJson_StringArrayPropertyNeedsEscaping_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringArrayObject>();
            
            var testObject = new StringArrayObject()
            {
                StringArrayProperty = new string[]{"1","2","3\"","4","5"}
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringArrayProperty\":[\"1\",\"2\",\"3\\\"\",\"4\",\"5\"]}"));
        }

        [Test]
        public void ToJson_StringArrayPropertyEmpty_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringArrayObject>();
            
            var testObject = new StringArrayObject()
            {
                StringArrayProperty = new string[0]
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringArrayProperty\":[]}"));
        }
    }
}