using System;
using System.Collections.Generic;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.ToJsonTests
{
    [TestFixture]
    public class ListTests
    {
        [Test]
        public void ToJson_IntList_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<int>>();

            //act
            string json = converter.ToJson(new List<int>{1,2,3,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,2,3,4,5]"));
        }

        [Test]
        public void ToJson_StringList_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<string>>();

            //act
            string json = converter.ToJson(new List<string>{"1","2","3","4","5"});

            //assert
            Assert.That(json, Is.EqualTo("[\"1\",\"2\",\"3\",\"4\",\"5\"]"));
        }

        public class IntListObject
        {
            public List<int> IntListProperty { get; set; }
        }

        [Test]
        public void ToJson_ListTestObjectNullProperty_PropertyNotIncludedInJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<IntListObject>();
            
            var testObject = new IntListObject();

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"IntListProperty\":null}"));
        }


        [Test]
        public void ToJson_ListTestObject_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<IntListObject>();
            
            var testObject = new IntListObject()
            {
                IntListProperty = new List<int>(){1,2,3,4,5}
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"IntListProperty\":[1,2,3,4,5]}"));
        }

        public class StringListObject
        {
            public List<string> StringListProperty { get; set; }
        }

        [Test]
        public void ToJson_StringListObjectNullProperty_PropertyNotIncludedInJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringListObject>();
            
            var testObject = new StringListObject();

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringListProperty\":null}"));
        }

        [Test]
        public void ToJson_StringListProperty_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringListObject>();
            
            var testObject = new StringListObject()
            {
                StringListProperty = new List<string>{"1","2","3","4","5"}
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringListProperty\":[\"1\",\"2\",\"3\",\"4\",\"5\"]}"));
        }

        [Test]
        public void ToJson_StringListPropertyNeedsEscaping_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringListObject>();
            
            var testObject = new StringListObject()
            {
                StringListProperty = new List<string>{"1","2","3\"","4","5"}
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringListProperty\":[\"1\",\"2\",\"3\\\"\",\"4\",\"5\"]}"));
        }

        [Test]
        public void ToJson_StringListPropertyEmtpy_CorrectJson()
        {
            //arrange
            var jsonConverter = JsonFactory.Compile<StringListObject>();
            
            var testObject = new StringListObject()
            {
                StringListProperty = new List<string>{}
            };

            //act
            var json = jsonConverter.ToJson(testObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"StringListProperty\":[]}"));
        }

        [Test]
        public void ToJson_ListNullableInt_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<int?>>();

            //act
            string json = converter.ToJson(new List<int?>{1,-2,3,null,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,-2,3,null,4,5]"));
        }

        [Test]
        public void ToJson_ListNullableUInt_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<uint?>>();

            //act
            string json = converter.ToJson(new List<uint?>(){1,2,3,null,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,2,3,null,4,5]"));
        }

        [Test]
        public void ToJson_ListUInt_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<uint>>();

            //act
            string json = converter.ToJson(new List<uint>{1,2,3,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,2,3,4,5]"));
        }

        [Test]
        public void ToJson_ListNullableShort_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<short?>>();

            //act
            string json = converter.ToJson(new List<short?>{-11,2,3,null,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[-11,2,3,null,4,5]"));
        }

        [Test]
        public void ToJson_ListShort_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<short>>();

            //act
            string json = converter.ToJson(new List<short>{-11,2,3,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[-11,2,3,4,5]"));
        }

        [Test]
        public void ToJson_ListNullableUShort_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<ushort?>>();

            //act
            string json = converter.ToJson(new List<ushort?>(){1,2,3,null,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,2,3,null,4,5]"));
        }

        [Test]
        public void ToJson_ListUShort_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<ushort>>();

            //act
            string json = converter.ToJson(new List<ushort>{1,2,3,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[1,2,3,4,5]"));
        }

        [Test]
        public void ToJson_ListNullableFloat_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<float?>>();

            //act
            string json = converter.ToJson(new List<float?>{-11,2.2f,3,null,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[-11,2.2,3,null,4,5]"));
        }

        [Test]
        public void ToJson_ListFloat_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<float>>();

            //act
            string json = converter.ToJson(new List<float>{-11.12f,2.2f,3,4,5.5f});

            //assert
            Assert.That(json, Is.EqualTo("[-11.12,2.2,3,4,5.5]"));
        }

        [Test]
        public void ToJson_ListNullableDouble_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<double?>>();

            //act
            string json = converter.ToJson(new List<double?>{-11,2.3d,3,null,4,5});

            //assert
            Assert.That(json, Is.EqualTo("[-11,2.3,3,null,4,5]"));
        }

        [Test]
        public void ToJson_ListDouble_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<double>>();

            //act
            string json = converter.ToJson(new List<double>{-11.12d,2.4d,3,4,5.5f});

            //assert
            Assert.That(json, Is.EqualTo("[-11.12,2.4,3,4,5.5]"));
        }

        [Test]
        public void ToJson_ListNullableByte_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<byte?>>();

            //act
            string json = converter.ToJson(new List<byte?>{255,0,1,null,3,4});

            //assert
            Assert.That(json, Is.EqualTo("[255,0,1,null,3,4]"));
        }

        [Test]
        public void ToJson_ListByte_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<byte>>();

            //act
            string json = converter.ToJson(new List<byte>{255,0,1,3,4});

            //assert
            Assert.That(json, Is.EqualTo("[255,0,1,3,4]"));
        }

        [Test]
        public void ToJson_ListNullableSByte_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<sbyte?>>();

            //act
            string json = converter.ToJson(new List<sbyte?>{127,-128,1,null,3,4});

            //assert
            Assert.That(json, Is.EqualTo("[127,-128,1,null,3,4]"));
        }

        [Test]
        public void ToJson_ListSByte_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<sbyte>>();

            //act
            string json = converter.ToJson(new List<sbyte>{127,-128,1,3,4});

            //assert
            Assert.That(json, Is.EqualTo("[127,-128,1,3,4]"));
        }

        [Test]
        public void ToJson_ListNullableChar_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<char?>>();

            //act
            string json = converter.ToJson(new List<char?>{'a','b','c',null,'d','e'});

            //assert
            Assert.That(json, Is.EqualTo("[\"a\",\"b\",\"c\",null,\"d\",\"e\"]"));
        }

        [Test]
        public void ToJson_ListChar_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<char>>();

            //act
            string json = converter.ToJson(new List<char>{'a','b','c','d','e'});

            //assert
            Assert.That(json, Is.EqualTo("[\"a\",\"b\",\"c\",\"d\",\"e\"]"));
        }

        [Test]
        public void ToJson_ListNullableBool_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<bool?>>();

            //act
            string json = converter.ToJson(new List<bool?>{true, null, false});

            //assert
            Assert.That(json, Is.EqualTo("[true,null,false]"));
        }

        [Test]
        public void ToJson_ListBool_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<bool>>();

            //act
            string json = converter.ToJson(new List<bool>{true, false});

            //assert
            Assert.That(json, Is.EqualTo("[true,false]"));
        }

        [Test]
        public void ToJson_ListNullableGuid_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<Guid?>>();

            //act
            string json = converter.ToJson(new List<Guid?>{new Guid(1,2,3,4,5,6,7,8,9,10,11), null, new Guid(2,3,4,5,6,7,8,9,10,11,12)});

            //assert
            Assert.That(json, Is.EqualTo("[\"00000001-0002-0003-0405-060708090a0b\",null,\"00000002-0003-0004-0506-0708090a0b0c\"]"));
        }

        [Test]
        public void ToJson_ListGuid_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<Guid>>();

            //act
            string json = converter.ToJson(new List<Guid>{new Guid(1,2,3,4,5,6,7,8,9,10,11), new Guid(2,3,4,5,6,7,8,9,10,11,12)});

            //assert
            Assert.That(json, Is.EqualTo("[\"00000001-0002-0003-0405-060708090a0b\",\"00000002-0003-0004-0506-0708090a0b0c\"]"));
        }

        [Test]
        public void ToJson_ListNullableDateTime_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<DateTime?>>();

            //act
            string json = converter.ToJson(new List<DateTime?>{new DateTime(2018,04,08, 1,2,3, DateTimeKind.Utc), null, new DateTime(2017,03,07, 23,59,42, DateTimeKind.Utc)});

            //assert
            Assert.That(json, Is.EqualTo("[\"2018-04-08T01:02:03Z\",null,\"2017-03-07T23:59:42Z\"]"));
        }

        [Test]
        public void ToJson_ListDateTime_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<List<DateTime>>();

            //act
            string json = converter.ToJson(new List<DateTime>{new DateTime(2018,04,08, 1,2,3, DateTimeKind.Utc), new DateTime(2017,03,07, 23,59,42, DateTimeKind.Utc)});

            //assert
            Assert.That(json, Is.EqualTo("[\"2018-04-08T01:02:03Z\",\"2017-03-07T23:59:42Z\"]"));
        }
    }
}