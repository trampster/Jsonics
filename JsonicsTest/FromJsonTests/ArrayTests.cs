using System.Collections;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class ArrayTests
    {
        IJsonConverter<ArrayClass> _arrayClassFactory;
        IJsonConverter<int[]> _intArrayValueFactory;
        IJsonConverter<string[]> _stringArrayValueFactory;
        
        public static IEnumerable IntArrayTestCases
        {
            get
            {
                yield return new TestCaseData("[]", new int[]{});
                yield return new TestCaseData("[1,2,3,4]", new int[]{1,2,3,4});
                yield return new TestCaseData("[ 1,2,3,4]", new int[]{1,2,3,4});
                yield return new TestCaseData(" [ 1,2,3,4]", new int[]{1,2,3,4});
                yield return new TestCaseData(" [-1,0,999]", new int[]{-1,0,999});
                yield return new TestCaseData("null", null);
                yield return new TestCaseData(" null", null);
            }
        }

        public static IEnumerable StringArrayTestCases
        {
            get
            {
                yield return new TestCaseData("[\"one\",\"two\",\"three\",\"four\"]", new string[]{"one","two","three","four"});
                yield return new TestCaseData("[\"\",null, null,\"four\"]", new string[]{"",null,null,"four"});
                yield return new TestCaseData("null", null);
                yield return new TestCaseData("[]", new string[]{});
            }
        }  

        public class ArrayClass
        {
            public int[] IntProperty
            {
                get;
                set;
            }

            public string[] StringProperty
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _arrayClassFactory = JsonFactory.Compile<ArrayClass>();
            _intArrayValueFactory = JsonFactory.Compile<int[]>();
            _stringArrayValueFactory = JsonFactory.Compile<string[]>();
        }

        [Test, TestCaseSource(typeof(ArrayTests), "IntArrayTestCases")]
        public void IntArrayProperty_CorrectlyDeserialized(string json, int[] expected)
        {
            //arrange
            //act
            var result = _arrayClassFactory.FromJson($"{{\"IntProperty\":{json}}}");

            //assert
            Assert.That(result.IntProperty, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(ArrayTests), "StringArrayTestCases")]
        public void StringArrayProperty_CorrectlyDeserialized(string json, string[] expected)
        {
            //arrange
            //act
            var result = _arrayClassFactory.FromJson($"{{\"StringProperty\":{json}}}");

            //assert
            Assert.That(result.StringProperty, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(ArrayTests), "IntArrayTestCases")]
        public void IntArrayValue_CorrectlyDeserialized(string json, int[] expected)
        {
            //arrange
            //act
            int[] result = _intArrayValueFactory.FromJson(json);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(ArrayTests), "StringArrayTestCases")]
        public void StringArrayValue_CorrectlyDeserialized(string json, string[] expected)
        {
            //arrange
            //act
            string[] result = _stringArrayValueFactory.FromJson(json);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable NullableDecimalArrayTestCases
        {
            get
            {
                yield return new TestCaseData("[1,1.1,2.2,-3.4]", new decimal?[]{1,1.1M,2.2M,-3.4M});
                yield return new TestCaseData("[1,null, null,-3.4]", new decimal?[]{1,null,null,-3.4M});
                yield return new TestCaseData("null", null);
                yield return new TestCaseData("[]", new decimal?[]{});
            }
        }  

        [Test, TestCaseSource(typeof(ArrayTests), "NullableDecimalArrayTestCases")]
        public void NullableDecimalArrayValue_CorrectlyDeserialized(string json, decimal?[] expected)
        {
            //arrange
            IJsonConverter<decimal?[]> converter = JsonFactory.Compile<decimal?[]>();

            //act
            decimal?[] result = converter.FromJson(json);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}