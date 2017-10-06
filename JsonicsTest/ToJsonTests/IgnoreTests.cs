using NUnit.Framework;
using Jsonics;

namespace JsonicsTests.ToJsonTests
{
    [TestFixture]
    public class IgnoreTests
    {
        public class ClassWithIgnoredProperty
        {
            [Jsonics.Ignore]
            public int IgnoredProperty
            {
                get;
                set;
            }

            public int IncludedProperty
            {
                get;
                set;
            }
        }

        [Test]
        public void ClassWithIgnoredProperty_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<ClassWithIgnoredProperty>();
            var instance = new ClassWithIgnoredProperty()
            {
                IgnoredProperty = 1,
                IncludedProperty = 2,
            };

            //act
            string json = converter.ToJson(instance);

            //assert
            Assert.That(json, Is.EqualTo("{\"IncludedProperty\":2}"));
        }

        public class ClassWithIgnoredField
        {
            [Jsonics.Ignore]
            public int IgnoredField;

            public int IncludedField;
        }

        [Test]
        public void ClassWithIgnoredField_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<ClassWithIgnoredField>();
            var instance = new ClassWithIgnoredField()
            {
                IgnoredField = 1,
                IncludedField = 2,
            };

            //act
            string json = converter.ToJson(instance);

            //assert
            Assert.That(json, Is.EqualTo("{\"IncludedField\":2}"));
        }

        public struct StructWithIgnoredProperty
        {
            [Jsonics.Ignore]
            public int IgnoredProperty
            {
                get;
                set;
            }

            public int IncludedProperty
            {
                get;
                set;
            }
        }

        [Test]
        public void StructWithIgnoredProperty_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<StructWithIgnoredProperty>();
            var instance = new StructWithIgnoredProperty()
            {
                IgnoredProperty = 1,
                IncludedProperty = 2,
            };

            //act
            string json = converter.ToJson(instance);

            //assert
            Assert.That(json, Is.EqualTo("{\"IncludedProperty\":2}"));
        }

        public struct StructWithIgnoredField
        {
            [Jsonics.Ignore]
            public int IgnoredField;

            public int IncludedField;
        }

        [Test]
        public void StructWithIgnoredField_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<StructWithIgnoredField>();
            var instance = new StructWithIgnoredField()
            {
                IgnoredField = 1,
                IncludedField = 2,
            };

            //act
            string json = converter.ToJson(instance);

            //assert
            Assert.That(json, Is.EqualTo("{\"IncludedField\":2}"));
        }
    }
}