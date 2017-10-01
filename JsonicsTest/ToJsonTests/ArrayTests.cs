using System;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.ToJsonTests
{
    [TestFixture]
    public class ArrayTests
    {
        [Test]
        public void ToJson_StringArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<string[]>();

            //act
            string json = converter.ToJson(new string[]{"1","2","3","4","5"});

            //assert
            Assert.That(json, Is.EqualTo("[\"1\",\"2\",\"3\",\"4\",\"5\"]"));
        }

        [Test]
        public void ToJson_IntArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<int[]>();

            //act
            string json = converter.ToJson(new int[]{1,2,3,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,2,3,4,5]"));
        }

        public class IntArrayObject
        {
            public int[] IntArrayProperty { get; set; }
        }

        [Test]
        public void ToJson_IntArrayObjectNullProperty_PropertyNotIncludedInJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<IntArrayObject>();
            
            var testObject = new IntArrayObject();

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"IntArrayProperty\":null}"));
        }

        [Test]
        public void ToJson_IntArrayProperty_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<IntArrayObject>();
            
            var testObject = new IntArrayObject()
            {
                IntArrayProperty = new int[]{1,2,3,4,5}
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"IntArrayProperty\":[1,2,3,4,5]}"));
        }

        public class StringArrayObject
        {
            public string[] StringArrayProperty { get; set; }
        }

        [Test]
        public void ToJson_StringArrayObjectNullProperty_PropertyNotIncludedInJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringArrayObject>();
            
            var testObject = new StringArrayObject();

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringArrayProperty\":null}"));
        }

        [Test]
        public void ToJson_StringArrayProperty_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringArrayObject>();
            
            var testObject = new StringArrayObject()
            {
                StringArrayProperty = new string[]{"1","2","3","4","5"}
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringArrayProperty\":[\"1\",\"2\",\"3\",\"4\",\"5\"]}"));
        }

        [Test]
        public void ToJson_StringArrayPropertyNeedsEscaping_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringArrayObject>();
            
            var testObject = new StringArrayObject()
            {
                StringArrayProperty = new string[]{"1","2","3\"","4","5"}
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringArrayProperty\":[\"1\",\"2\",\"3\\\"\",\"4\",\"5\"]}"));
        }

        [Test]
        public void ToJson_StringArrayPropertyEmpty_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringArrayObject>();
            
            var testObject = new StringArrayObject()
            {
                StringArrayProperty = new string[0]
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringArrayProperty\":[]}"));
        }
         
        [Test]
        public void ToJson_NullableIntArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<int?[]>();

            //act
            string json = converter.ToJson(new int?[]{1,-2,3,null,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,-2,3,null,4,5]"));
        }

        [Test]
        public void ToJson_NullableUIntArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<uint?[]>();

            //act
            string json = converter.ToJson(new uint?[]{1,2,3,null,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,2,3,null,4,5]"));
        }

        [Test]
        public void ToJson_UIntArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<uint[]>();

            //act
            string json = converter.ToJson(new uint[]{1,2,3,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,2,3,4,5]"));
        }

        [Test]
        public void ToJson_NullableShortArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<short?[]>();

            //act
            string json = converter.ToJson(new short?[]{-11,2,3,null,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[-11,2,3,null,4,5]"));
        }

        [Test]
        public void ToJson_ShortArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<short[]>();

            //act
            string json = converter.ToJson(new short[]{-11,2,3,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[-11,2,3,4,5]"));
        }

        [Test]
        public void ToJson_NullableUShortArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<ushort?[]>();

            //act
            string json = converter.ToJson(new ushort?[]{1,2,3,null,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,2,3,null,4,5]"));
        }

        [Test]
        public void ToJson_UShortArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<ushort[]>();

            //act
            string json = converter.ToJson(new ushort[]{1,2,3,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,2,3,4,5]"));
        }

        [Test]
        public void ToJson_NullableFloatArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<float?[]>();

            //act
            string json = converter.ToJson(new float?[]{-11,2.2f,3,null,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[-11,2.2,3,null,4,5]"));
        }

        [Test]
        public void ToJson_FloatArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<float[]>();

            //act
            string json = converter.ToJson(new float[]{-11.12f,2.2f,3,4,5.5f});

            //assert
            Assert.That(json, Is.EqualTo("[-11.12,2.2,3,4,5.5]"));
        }

        [Test]
        public void ToJson_NullableDoubleArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<double?[]>();

            //act
            string json = converter.ToJson(new double?[]{-11,2.3d,3,null,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[-11,2.3,3,null,4,5]"));
        }

        [Test]
        public void ToJson_DoubleArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<double[]>();

            //act
            string json = converter.ToJson(new double[]{-11.12d,2.4d,3,4,5.5f});

            //assert
            Assert.That(json, Is.EqualTo("[-11.12,2.4,3,4,5.5]"));
        }

        [Test]
        public void ToJson_NullableByteArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<byte?[]>();

            //act
            string json = converter.ToJson(new byte?[]{255,0,1,null,3,4});

            //assert
            Assert.That(json, Is.EqualTo("[255,0,1,null,3,4]"));
        }

        [Test]
        public void ToJson_ByteArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<byte[]>();

            //act
            string json = converter.ToJson(new byte[]{255,0,1,3,4});

            //assert
            Assert.That(json, Is.EqualTo("[255,0,1,3,4]"));
        }

        [Test]
        public void ToJson_NullableSByteArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<sbyte?[]>();

            //act
            string json = converter.ToJson(new sbyte?[]{127,-128,1,null,3,4});

            //assert
            Assert.That(json, Is.EqualTo("[127,-128,1,null,3,4]"));
        }

        [Test]
        public void ToJson_SByteArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<sbyte[]>();

            //act
            string json = converter.ToJson(new sbyte[]{127,-128,1,3,4});

            //assert
            Assert.That(json, Is.EqualTo("[127,-128,1,3,4]"));
        }

        [Test]
        public void ToJson_NullableCharArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<char?[]>();

            //act
            string json = converter.ToJson(new char?[]{'a','b','c',null,'d','e'});

            //assert
            Assert.That(json, Is.EqualTo("[\"a\",\"b\",\"c\",null,\"d\",\"e\"]"));
        }

        [Test]
        public void ToJson_CharArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<char[]>();

            //act
            string json = converter.ToJson(new char[]{'a','b','c','d','e'});

            //assert
            Assert.That(json, Is.EqualTo("[\"a\",\"b\",\"c\",\"d\",\"e\"]"));
        }

        [Test]
        public void ToJson_NullableBoolArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<bool?[]>();

            //act
            string json = converter.ToJson(new bool?[]{true, null, false});

            //assert
            Assert.That(json, Is.EqualTo("[true,null,false]"));
        }

        [Test]
        public void ToJson_BoolArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<bool[]>();

            //act
            string json = converter.ToJson(new bool[]{true, false});

            //assert
            Assert.That(json, Is.EqualTo("[true,false]"));
        }

        [Test]
        public void ToJson_NullableGuidArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<Guid?[]>();

            //act
            string json = converter.ToJson(new Guid?[]{new Guid(1,2,3,4,5,6,7,8,9,10,11), null, new Guid(2,3,4,5,6,7,8,9,10,11,12)});

            //assert
            Assert.That(json, Is.EqualTo("[\"00000001-0002-0003-0405-060708090a0b\",null,\"00000002-0003-0004-0506-0708090a0b0c\"]"));
        }

        [Test]
        public void ToJson_GuidArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<Guid[]>();

            //act
            string json = converter.ToJson(new Guid[]{new Guid(1,2,3,4,5,6,7,8,9,10,11), new Guid(2,3,4,5,6,7,8,9,10,11,12)});

            //assert
            Assert.That(json, Is.EqualTo("[\"00000001-0002-0003-0405-060708090a0b\",\"00000002-0003-0004-0506-0708090a0b0c\"]"));
        }

        [Test]
        public void ToJson_NullableDateTimeArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<DateTime?[]>();

            //act
            string json = converter.ToJson(new DateTime?[]{new DateTime(2018,04,08, 1,2,3, DateTimeKind.Utc), null, new DateTime(2017,03,07, 23,59,42, DateTimeKind.Utc)});

            //assert
            Assert.That(json, Is.EqualTo("[\"2018-04-08T01:02:03Z\",null,\"2017-03-07T23:59:42Z\"]"));
        }

        [Test]
        public void ToJson_DateTimeArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<DateTime[]>();

            //act
            string json = converter.ToJson(new DateTime[]{new DateTime(2018,04,08, 1,2,3, DateTimeKind.Utc), new DateTime(2017,03,07, 23,59,42, DateTimeKind.Utc)});

            //assert
            Assert.That(json, Is.EqualTo("[\"2018-04-08T01:02:03Z\",\"2017-03-07T23:59:42Z\"]"));
        }

        [Test]
        public void ToJson_DecimalArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<decimal[]>();

            //act
            string json = converter.ToJson(new decimal[]{-11.12M,2.4M,3M,4M,5.5M});

            //assert
            Assert.That(json, Is.EqualTo("[-11.12,2.4,3,4,5.5]"));
        }

        [Test]
        public void ToJson_NullableDecimalArray_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<decimal?[]>();

            //act
            string json = converter.ToJson(new decimal?[]{-11.12M,2.4M,null,3M,4M,5.5M});

            //assert
            Assert.That(json, Is.EqualTo("[-11.12,2.4,null,3,4,5.5]"));
        }
    }
}