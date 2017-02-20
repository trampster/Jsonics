using Jsonic;
using NUnit.Framework;

namespace JsonicTests
{
    [TestFixture]
    public class JsonicFactoryTest
    {
        [Test]
        public void ToJson_Person_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonicFactory.Compile<SimpleTestObject>();

            //act
            var json = jsonConverter.ToJson(new SimpleTestObject(){FirstName="Rey", LastName="Kenoby"});

            //assert
            Assert.That(json, Is.EqualTo("{\"FirstName\":\"Rey\",\"LastName\":\"Kenoby\"}"));
        }
    }
}