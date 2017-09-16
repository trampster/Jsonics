using Jsonics;
using NUnit.Framework;

namespace JsonicsTest.ToJsonTests
{
    [TestFixture]
    public class ObjectFieldTests
    {
        public class Person
        {
            public string FirstName;

            public string LastName;

            public int Age;

            public double PowerFactor;

            public bool IsJedi;
        }

        [Test]
        public void ToJson_Person_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<Person>();
            var testObject = new Person()
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
            var jsonConverter = JsonFactory.Compile<Person>();
            var testObject = new Person()
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