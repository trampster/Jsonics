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
    }
}