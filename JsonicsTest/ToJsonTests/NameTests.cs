using NUnit.Framework;
using Jsonics;

namespace JsonicsTests.ToJsonTests
{
    [TestFixture]
    public class NameTests
    {
        public class ClassWithNameProperty
        {
            [Name("json_property")]
            public int DotnetProperty
            {
                get;
                set;
            }
        }

        [Test]
        public void ClassWithNameProperty_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<ClassWithNameProperty>();
            var instance = new ClassWithNameProperty()
            {
                DotnetProperty = 42,
            };

            //act
            string json = converter.ToJson(instance);

            //assert
            Assert.That(json, Is.EqualTo("{\"json_property\":42}"));
        }

        public class ClassWithNameField
        {
            [Name("json_field")]
            public int DotnetField
            {
                get;
                set;
            }
        }

        [Test]
        public void ClassWithNameField_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<ClassWithNameField>();
            var instance = new ClassWithNameField()
            {
                DotnetField = 42,
            };

            //act
            string json = converter.ToJson(instance);

            //assert
            Assert.That(json, Is.EqualTo("{\"json_field\":42}"));
        }

        public struct StructWithNameProperty
        {
            [Name("json_property")]
            public int DotnetProperty
            {
                get;
                set;
            }
        }

        [Test]
        public void StructWithNameProperty_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<StructWithNameProperty>();
            var instance = new StructWithNameProperty()
            {
                DotnetProperty = 42,
            };

            //act
            string json = converter.ToJson(instance);

            //assert
            Assert.That(json, Is.EqualTo("{\"json_property\":42}"));
        }

        public struct StructWithNameField
        {
            [Name("json_field")]
            public int DotnetField
            {
                get;
                set;
            }
        }

        [Test]
        public void StructWithNameField_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<StructWithNameField>();
            var instance = new StructWithNameField()
            {
                DotnetField = 42,
            };

            //act
            string json = converter.ToJson(instance);

            //assert
            Assert.That(json, Is.EqualTo("{\"json_field\":42}"));
        }
    }
}