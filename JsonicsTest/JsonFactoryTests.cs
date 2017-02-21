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

            //act
            var json = jsonConverter.ToJson(new SimpleTestObject(){FirstName="Rey", LastName="Kenoby", Age=60});

            //assert
            Assert.That(json, Is.EqualTo("{\"FirstName\":\"Rey\",\"LastName\":\"Kenoby\",\"Age\":60}"));
        }
    }
}