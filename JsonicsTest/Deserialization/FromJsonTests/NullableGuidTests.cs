using System;
using System.Collections;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{

    [TestFixture]
    public class NullableGuidTests
    {
        IJsonConverter<NullableGuidClass> _propertyFactory;
        IJsonConverter<Guid?> _valueFactory;

        public class NullableGuidClass
        {
            public Guid? Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<NullableGuidClass>();
            _valueFactory = JsonFactory.Compile<Guid?>();
        }

        [Test, TestCaseSource(typeof(NullableGuidTestCaseData), "TestCases")]
        public void NullableGuidProperty_CorrectlyDeserialized(string json, Guid? expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{json}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(NullableGuidTestCaseData), "TestCases")]
        public void NullableGuidValue_CorrectlyDeserialized(string json, Guid? expected)
        {
            //arrange
            //act
            Guid? result = _valueFactory.FromJson(json);

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }

        public class NullableGuidTestCaseData
        {
            public static IEnumerable TestCases
            {
                get
                {
                    yield return new TestCaseData("\"01234567-8901-2345-6789-ABCDEF012345\"", new Guid("01234567-8901-2345-6789-ABCDEF012345"));
                    yield return new TestCaseData("\"00000000-0000-0000-0000-000000000000\"", new Guid("00000000-0000-0000-0000-000000000000"));
                    yield return new TestCaseData("\"FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF\"", new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"));
                    yield return new TestCaseData("null", null);
                }
            }  
        }
    }
}