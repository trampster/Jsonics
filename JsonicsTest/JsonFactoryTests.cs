using System.Collections.Generic;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTests
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
        public void ToJson_ListTestObject_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<ListTestObject>();
            
            var testObject = new ListTestObject()
            {
                IntListProperty = new List<int>(){1,2,3,4,5}
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"IntListProperty\":[1,2,3,4,5]}"));
        }
    }
}