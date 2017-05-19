using Jsonics;
using NUnit.Framework;

namespace JsonicsTest
{
    [TestFixture]
    public class JsonFactoryTest
    {
        [Test]
        public void ToJson_Person_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<SimpleTestObject>();
            var testObject = new SimpleTestObject()
            {
                FirstName="Ob Won", 
                LastName="Kenoby", 
                Age=60,
                PowerFactor=104.6789,
                IsJedi=true
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"FirstName\":\"Ob Won\",\"LastName\":\"Kenoby\",\"Age\":60,\"PowerFactor\":104.6789,\"IsJedi\":true}"));
        }

        [Test]
        public void ToJson_PersonEscapingNeeded_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<SimpleTestObject>();
            var testObject = new SimpleTestObject()
            {
                FirstName="Ob\t Won", 
                LastName="Ken\noby", 
                Age=60,
                PowerFactor=104.6789,
                IsJedi=true
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"FirstName\":\"Ob\\t Won\",\"LastName\":\"Ken\\noby\",\"Age\":60,\"PowerFactor\":104.6789,\"IsJedi\":true}"));
        }

        [Test]
        public void FromJson_SimpleTestObject_ReturnsClassInstance()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<SimpleTestObject>();

            //act
            var instance = jsonConverter.FromJson("{\"FirstName\":\"Ob\\t Won\",\"LastName\":\"Ken\\noby\",\"Age\":60,\"PowerFactor\":104.6789,\"IsJedi\":true}");

            //assert
            Assert.That(instance, Is.Not.Null);
        }

        public class TestClass
        {
            public int First
            {
                get;
                set;
            }

            public int Secon
            {
                get;
                set;
            }
        }

        [Test]
        public void FromJson_TestClass_PropertiesSetCorrectly()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<TestClass>();

            //act
            var instance = jsonConverter.FromJson("{\"First\":1,\"Secon\":2}");

            //assert
            Assert.That(instance.First, Is.EqualTo(1));
            Assert.That(instance.Secon, Is.EqualTo(2));
        }
    }
}