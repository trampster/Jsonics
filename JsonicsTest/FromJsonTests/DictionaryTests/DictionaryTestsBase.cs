using System.Collections;
using System.Collections.Generic;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests.DictionaryTests
{
    public abstract class DictionaryTestsBase<TKey, TValue>
    {
        IJsonConverter<Dictionary<TKey, TValue>> _dictionaryValueFactory;
        IJsonConverter<DictionaryClass> _dictionaryClassFactory;  

        public class DictionaryClass
        {
            public Dictionary<TKey, TValue> DictionaryProperty
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _dictionaryValueFactory = JsonFactory.Compile<Dictionary<TKey, TValue>>();
            _dictionaryClassFactory = JsonFactory.Compile<DictionaryClass>();
        }

        [Test, TestCaseSource("TestCases")]
        public void DictionaryProperty_CorrectlyDeserialized(string json, Dictionary<TKey, TValue> expected)
        {
            //arrange
            //act
            var result = _dictionaryClassFactory.FromJson($"{{\"DictionaryProperty\":{json}}}");

            //assert
            Assert.That(result.DictionaryProperty, Is.EqualTo(expected));
        }


        [Test, TestCaseSource("TestCases")]
        public void DictionaryValue_CorrectlyDeserialized(string json, Dictionary<TKey,TValue> expected)
        {
            //arrange
            //act
            Dictionary<TKey,TValue> result = _dictionaryValueFactory.FromJson(json);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}