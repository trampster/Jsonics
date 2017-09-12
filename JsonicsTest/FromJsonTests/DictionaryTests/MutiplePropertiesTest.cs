using System.Collections;
using System.Collections.Generic;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests.DictionaryTests
{
    [TestFixture]
    public class MutiplePropertiesTest
    {
        string _json;
        IJsonConverter<TestClass> _compiled;

        [SetUp]
        public void SetUp()
        {
            _json = "{\"First\":{\"1\":\"one\",\"2\":\"two\"},\"Secon\":{\"3\":\"three\",\"4\":\"four\"},\"Third\":{\"5\":\"five\",\"6\":\"six\"}}";
            _compiled = JsonFactory.Compile<TestClass>();
        }

        [Test]
        public void MultipleProperties_FirstPropertyCorrect()
        {
            //arrange
            var expected = new Dictionary<int, string>()
            {
                [1] = "one",
                [2] = "two",
            };

            //act
            var result = _compiled.FromJson(_json);

            //assert
            Assert.That(result.First, Is.EqualTo(expected));
        }

        [Test]
        public void MultipleProperties_SeconPropertyCorrect()
        {
            //arrange
            var expected = new Dictionary<int, string>()
            {
                [3] = "three",
                [4] = "four",
            };

            //act
            var result = _compiled.FromJson(_json);

            //assert
            Assert.That(result.Secon, Is.EqualTo(expected));
        }

        [Test]
        public void MultipleProperties_ThirdPropertyCorrect()
        {
            //arrange
            var expected = new Dictionary<int, string>()
            {
                [5] = "five",
                [6] = "six",
            };

            //act
            var result = _compiled.FromJson(_json);

            //assert
            Assert.That(result.Third, Is.EqualTo(expected));
        }

        public class TestClass
        {
            public Dictionary<int, string> First
            {
                get;
                set;
            }

            public Dictionary<int, string> Secon
            {
                get;
                set;
            }

            public Dictionary<int, string> Third
            {
                get;
                set;
            }
        }
    }
}