using System.Collections.Generic;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTest
{
    [TestFixture]
    public class ListSerializationTests
    {
         public class IntListObject
        {
            public List<int> IntListProperty { get; set; }
        }

        [Test]
        public void ToJson_ListTestObjectNullProperty_PropertyNotIncludedInJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<IntListObject>();
            
            var testObject = new IntListObject();

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"IntListProperty\":null}"));
        }


        [Test]
        public void ToJson_ListTestObject_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<IntListObject>();
            
            var testObject = new IntListObject()
            {
                IntListProperty = new List<int>(){1,2,3,4,5}
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"IntListProperty\":[1,2,3,4,5]}"));
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
        public void ToJson_IntArray_CorrectJson()
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
        public void ToJson_StringArray_CorrectJson()
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
        public void ToJson_StringArrayNeedsEscaping_CorrectJson()
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
        public void ToJson_StringArrayEmtpy_CorrectJson()
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

        public class StringListObject
        {
            public List<string> StringListProperty { get; set; }
        }

        [Test]
        public void ToJson_StringListObjectNullProperty_PropertyNotIncludedInJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringListObject>();
            
            var testObject = new StringListObject();

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringListProperty\":null}"));
        }

        [Test]
        public void ToJson_StringList_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringListObject>();
            
            var testObject = new StringListObject()
            {
                StringListProperty = new List<string>{"1","2","3","4","5"}
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringListProperty\":[\"1\",\"2\",\"3\",\"4\",\"5\"]}"));
        }

        [Test]
        public void ToJson_StringListNeedsEscaping_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringListObject>();
            
            var testObject = new StringListObject()
            {
                StringListProperty = new List<string>{"1","2","3\"","4","5"}
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringListProperty\":[\"1\",\"2\",\"3\\\"\",\"4\",\"5\"]}"));
        }

        [Test]
        public void ToJson_StringListEmtpy_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringListObject>();
            
            var testObject = new StringListObject()
            {
                StringListProperty = new List<string>{}
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringListProperty\":[]}"));
        }
    }
}