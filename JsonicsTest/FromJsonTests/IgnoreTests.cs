using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
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
        public void ClassWithIgnoredProperty_IncludedPropertySet()
        {
            //arrange
            var converter = JsonFactory.Compile<ClassWithIgnoredProperty>();

            //act
            var json = converter.FromJson("{\"IncludedProperty\":2\"IgnoredProperty\":3}");

            //assert
            Assert.That(json.IncludedProperty, Is.EqualTo(2));
        }

        [Test]
        public void ClassWithIgnoredProperty_IgnorePropertyNotSet()
        {
            //arrange
            var converter = JsonFactory.Compile<ClassWithIgnoredProperty>();

            //act
            var json = converter.FromJson("{\"IncludedProperty\":2\"IgnoredProperty\":3}");

            //assert
            Assert.That(json.IgnoredProperty, Is.EqualTo(0));
        }

        public class ClassWithIgnoredField
        {
            [Jsonics.Ignore]
            public int IgnoredField;

            public int IncludedField;
        }

        [Test]
        public void ClassWithIgnoredField_IncludedFieldSet()
        {
            //arrange
            var converter = JsonFactory.Compile<ClassWithIgnoredField>();

            //act
            var json = converter.FromJson("{\"IncludedField\":2,\"IgnoredField\":3}");

            //assert
            Assert.That(json.IncludedField, Is.EqualTo(2));
        }

        [Test]
        public void ClassWithIgnoredField_IgnoredFieldNotSet()
        {
            //arrange
            var converter = JsonFactory.Compile<ClassWithIgnoredField>();

            //act
            var json = converter.FromJson("{\"IncludedField\":2,\"IgnoredField\":3}");

            //assert
            Assert.That(json.IgnoredField, Is.EqualTo(0));
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
        public void StructWithIgnoredProperty_IncludedPropertySet()
        {
            //arrange
            var converter = JsonFactory.Compile<StructWithIgnoredProperty>();

            //act
            var json = converter.FromJson("{\"IncludedProperty\":2\"IgnoredProperty\":3}");

            //assert
            Assert.That(json.IncludedProperty, Is.EqualTo(2));
        }

        [Test]
        public void StructWithIgnoredProperty_IgnorePropertyNotSet()
        {
            //arrange
            var converter = JsonFactory.Compile<StructWithIgnoredProperty>();

            //act
            var json = converter.FromJson("{\"IncludedProperty\":2\"IgnoredProperty\":3}");

            //assert
            Assert.That(json.IgnoredProperty, Is.EqualTo(0));
        }

        public struct StructWithIgnoredField
        {
            [Jsonics.Ignore]
            public int IgnoredField;

            public int IncludedField;
        }

        [Test]
        public void StructWithIgnoredField_IncludedPropertySet()
        {
            //arrange
            var converter = JsonFactory.Compile<StructWithIgnoredField>();

            //act
            var json = converter.FromJson("{\"IncludedField\":2,\"IgnoredField\":3}");

            //assert
            Assert.That(json.IncludedField, Is.EqualTo(2));
        }

        [Test]
        public void StructWithIgnoredField_IgnorePropertyNotSet()
        {
            //arrange
            var converter = JsonFactory.Compile<StructWithIgnoredField>();

            //act
            var json = converter.FromJson("{\"IncludedField\":2,\"IgnoredField\":3}");

            //assert
            Assert.That(json.IgnoredField, Is.EqualTo(0));
        }
    }
}