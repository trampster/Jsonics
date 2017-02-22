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
                IsJedi=true
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"FirstName\":\"Ob Won\",\"LastName\":\"Kenoby\",\"Age\":60,\"IsJedi\":true}"));
        }
    }
}