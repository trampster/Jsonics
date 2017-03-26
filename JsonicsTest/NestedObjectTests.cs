using Jsonics;
using NUnit.Framework;

namespace JsonicsTest
{
    [TestFixture]
    public class NestedObjectTests
    {
        public class ObjectB
        {
            public int Size{get;set;}
        }

        public class ObjectA
        {
            public ObjectB ObjectB{get;set;}
        }

        [Test]
        public void ToJson_NestedObject_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<ObjectA>();
            var testObject = new ObjectA()
            {
                ObjectB = new ObjectB()
                {
                    Size = 12
                }
            };

            //act
            var json = converter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"ObjectB\":{\"Size\":12}}"));
        }
    }
}