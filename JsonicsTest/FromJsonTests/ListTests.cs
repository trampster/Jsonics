using System.Collections;
using System.Collections.Generic;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class ListTests
    {
        IJsonConverter<ListClass> _listClassFactory;
        IJsonConverter<List<int>> _intListValueFactory;
        IJsonConverter<List<string>> _stringListValueFactory;
        
        public static IEnumerable IntListTestCases
        {
            get
            {
                yield return new TestCaseData("[]", new List<int>{});
                yield return new TestCaseData("[1,2,3,4]", new List<int>{1,2,3,4});
                yield return new TestCaseData("[ 1,2,3,4]", new List<int>{1,2,3,4});
                yield return new TestCaseData(" [ 1,2,3,4]", new List<int>{1,2,3,4});
                yield return new TestCaseData(" [-1,0,999]", new List<int>{-1,0,999});
                yield return new TestCaseData("null", null);
                yield return new TestCaseData(" null", null);
            }
        }

        public static IEnumerable StringListTestCases
        {
            get
            {
                yield return new TestCaseData("[\"one\",\"two\",\"three\",\"four\"]", new List<string>{"one","two","three","four"});
                yield return new TestCaseData("[\"\",null, null,\"four\"]", new List<string>{"",null,null,"four"});
                yield return new TestCaseData("null", null);
                yield return new TestCaseData("[]", new List<string>{});
            }
        }  

        public class ListClass
        {
            public List<int> IntProperty
            {
                get;
                set;
            }

            public List<string> StringProperty
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _listClassFactory = JsonFactory.Compile<ListClass>();
            _intListValueFactory = JsonFactory.Compile<List<int>>();
            _stringListValueFactory = JsonFactory.Compile<List<string>>();
        }

        [Test, TestCaseSource(typeof(ListTests), "IntListTestCases")]
        public void IntListProperty_CorrectlyDeserialized(string json, List<int> expected)
        {
            //arrange
            //act
            var result = _listClassFactory.FromJson($"{{\"IntProperty\":{json}}}");

            //assert
            Assert.That(result.IntProperty, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(ListTests), "StringListTestCases")]
        public void StringListProperty_CorrectlyDeserialized(string json, List<string> expected)
        {
            //arrange
            //act
            var result = _listClassFactory.FromJson($"{{\"StringProperty\":{json}}}");

            //assert
            Assert.That(result.StringProperty, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(ListTests), "IntListTestCases")]
        public void IntListValue_CorrectlyDeserialized(string json, List<int> expected)
        {
            //arrange
            //act
            List<int> result = _intListValueFactory.FromJson(json);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(ListTests), "StringListTestCases")]
        public void StringListValue_CorrectlyDeserialized(string json, List<string> expected)
        {
            //arrange
            //act
            List<string> result = _stringListValueFactory.FromJson(json);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable NullableDecimalListTestCases
        {
            get
            {
                yield return new TestCaseData("[1,1.1,2.2,-3.4]", new List<decimal?>(){1,1.1M,2.2M,-3.4M});
                yield return new TestCaseData("[1,null, null,-3.4]", new List<decimal?>(){1,null,null,-3.4M});
                yield return new TestCaseData("null", null);
                yield return new TestCaseData("[]", new List<decimal?>(){});
            }
        }  

        [Test, TestCaseSource(typeof(ListTests), "NullableDecimalListTestCases")]
        public void NullableDecimalArrayValue_CorrectlyDeserialized(string json, List<decimal?> expected)
        {
            //arrange
            IJsonConverter<List<decimal?>> converter = JsonFactory.Compile<List<decimal?>>();

            //act
            List<decimal?> result = converter.FromJson(json);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}