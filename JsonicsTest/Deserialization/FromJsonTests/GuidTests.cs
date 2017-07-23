using System;
using System.Collections;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    public class GuidTestCaseData
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(new Guid("01234567-8901-2345-6789-ABCDEF012345"));
                yield return new TestCaseData(new Guid("00000000-0000-0000-0000-000000000000"));
                yield return new TestCaseData(new Guid("01234567-8901-2345-6789-ABCDEF012345"));
            }
        }  
    }

    [TestFixture]
    public class GuidTests
    {
        IJsonConverter<GuidClass> _propertyFactory;
        IJsonConverter<Guid> _valueFactory;
        

        public class GuidClass
        {
            public Guid Property
            {
                get;
                set;
            }
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _propertyFactory = JsonFactory.Compile<GuidClass>();
            _valueFactory = JsonFactory.Compile<Guid>();
        }

        [TestCaseSource(typeof(GuidTestCaseData))]
        public void GuidProperty_CorrectlyDeserialized(Guid expected)
        {
            //arrange
            //act
            var result = _propertyFactory.FromJson($"{{\"Property\":{expected}}}");

            //assert
            Assert.That(result.Property, Is.EqualTo(expected));
        }

        [TestCaseSource(typeof(GuidTestCaseData))]
        public void GuidValue_CorrectlyDeserialized(int expected)
        {
            //arrange
            //act
            Guid result = _valueFactory.FromJson($"{expected}");

            //assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}