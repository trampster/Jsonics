using Jsonics;
using NUnit.Framework;

namespace JsonicsTest.ToJsonTests
{
    [TestFixture]
    public class EnumTests
    {
        public enum TestEnum
        {
            First,
            Second,
            Third
        }

        [TestCase(TestEnum.First, "0")]
        [TestCase(TestEnum.Second, "1")]
        [TestCase(TestEnum.Third, "2")]
        public void ToJson_Enum_CorrectJson(TestEnum input, string expectedJson)
        {
            //arrange
            var converter = JsonFactory.Compile<TestEnum>();

            //act
            var json = converter.ToJson(input);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        public class EnumObject
        {
            public TestEnum EnumProperty
            {
                get;
                set;
            }
        }

        [TestCase(TestEnum.First, "{\"EnumProperty\":0}")]
        [TestCase(TestEnum.Second, "{\"EnumProperty\":1}")]
        [TestCase(TestEnum.Third, "{\"EnumProperty\":2}")]
        public void ToJson_EnumInObject_CorrectJson(TestEnum input, string expectedJson)
        {
            //arrange
            var enumObject = new EnumObject()
            {
                EnumProperty = input
            };
            var converter = JsonFactory.Compile<EnumObject>();

            //act
            var json = converter.ToJson(enumObject);

            //assert
            Assert.That(json, Is.EqualTo(expectedJson));
        }
        
    }
}