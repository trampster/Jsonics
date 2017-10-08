using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
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
        public void ClassWithNameProperty_PropertySet()
        {
            //arrange
            var converter = JsonFactory.Compile<ClassWithNameProperty>();

            //act
            var json = converter.FromJson("{\"json_property\":42}");

            //assert
            Assert.That(json.DotnetProperty, Is.EqualTo(42));
        }

        public class ClassWithNameField
        {
            [Name("json_field")]
            public int DotnetField;
        }

        [Test]
        public void ClassWithNameField_FieldSet()
        {
            //arrange
            var converter = JsonFactory.Compile<ClassWithNameField>();

            //act
            var json = converter.FromJson("{\"json_field\":42}");

            //assert
            Assert.That(json.DotnetField, Is.EqualTo(42));
        }

        public class StructWithNameProperty
        {
            [Name("json_property")]
            public int DotnetProperty
            {
                get;
                set;
            }
        }

        [Test]
        public void StructWithNameProperty_PropertySet()
        {
            //arrange
            var converter = JsonFactory.Compile<StructWithNameProperty>();

            //act
            var json = converter.FromJson("{\"json_property\":42}");

            //assert
            Assert.That(json.DotnetProperty, Is.EqualTo(42));
        }

        public class StructWithNameField
        {
            [Name("json_field")]
            public int DotnetField;
        }

        [Test]
        public void StructWithNameField_IncludedFieldSet()
        {
            //arrange
            var converter = JsonFactory.Compile<StructWithNameField>();

            //act
            var json = converter.FromJson("{\"json_field\":42}");

            //assert
            Assert.That(json.DotnetField, Is.EqualTo(42));
        }
    }
}