using System.Collections.Generic;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.ToJsonTests
{
    [TestFixture]
    public class ListTests
    {
        [Test]
        public void ToJson_IntList_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<int>>();

            //act
            string json = converter.ToJson(new List<int>{1,2,3,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,2,3,4,5]"));
        }

        [Test]
        public void ToJson_StringList_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<string>>();

            //act
            string json = converter.ToJson(new List<string>{"1","2","3","4","5"});

            //assert
            Assert.That(json, Is.EqualTo("[\"1\",\"2\",\"3\",\"4\",\"5\"]"));
        }

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
        public void ToJson_StringListProperty_CorrectJson()
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
        public void ToJson_StringListPropertyNeedsEscaping_CorrectJson()
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
        public void ToJson_StringListPropertyEmtpy_CorrectJson()
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