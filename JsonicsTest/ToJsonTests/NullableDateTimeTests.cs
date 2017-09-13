using System;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTest.JsonTests
{
    [TestFixture]
    public class NullableDateTimeTests
    {
        public class NullableDateTimeObject
        {
            public DateTime? DateTime
            {
                get;
                set;
            }
        }

        [Test]
        public void ToJson_DateOnly_CorrectJson()
        {
            //arrange
            var dateTimeObject = new NullableDateTimeObject();
            dateTimeObject.DateTime = new DateTime(2017,3,7);
            var converter = JsonFactory.Compile<NullableDateTimeObject>();

            //act
            string json = converter.ToJson(dateTimeObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"DateTime\":\"2017-03-07T00:00:00\"}"));
        }

        [Test]
        public void ToJson_NullProperty_CorrectJson()
        {
            //arrange
            var dateTimeObject = new NullableDateTimeObject();
            dateTimeObject.DateTime = null;
            var converter = JsonFactory.Compile<NullableDateTimeObject>();

            //act
            string json = converter.ToJson(dateTimeObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"DateTime\":null}"));
        }

        [Test]
        public void ToJson_DateAndTimeNoOffset_CorrectJson()
        {
            //arrange
            var dateTimeObject = new NullableDateTimeObject();
            dateTimeObject.DateTime = new DateTime(2016,1,2,23,59,58,555);
            var converter = JsonFactory.Compile<NullableDateTimeObject>();

            //act
            string json = converter.ToJson(dateTimeObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"DateTime\":\"2016-01-02T23:59:58.555\"}"));
        }

        [Test]
        public void ToJson_Utc_CorrectJson()
        {
            //arrange
            var dateTimeObject = new NullableDateTimeObject();
            dateTimeObject.DateTime = new DateTime(2016,1,2,23,59,58,555, DateTimeKind.Utc);
            var converter = JsonFactory.Compile<NullableDateTimeObject>();

            //act
            string json = converter.ToJson(dateTimeObject);

            //assert
            Assert.That(json, Is.EqualTo("{\"DateTime\":\"2016-01-02T23:59:58.555Z\"}"));
        }

        [Test]
        public void ToJson_Local_CorrectJson()
        {
            //arrange
            var dateTimeObject = new NullableDateTimeObject();
            dateTimeObject.DateTime = new DateTime(2016,1,2,23,59,58,555, DateTimeKind.Local);
            var converter = JsonFactory.Compile<NullableDateTimeObject>();

            //act
            string json = converter.ToJson(dateTimeObject);

            //assert
            var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            var sign = offset.Duration().TotalMinutes > 0 ? "+" : "-";
            var hours = Math.Abs(offset.Hours).ToString("00");
            var minutes = offset.Minutes.ToString("00");
            Assert.That(json, Is.EqualTo($"{{\"DateTime\":\"2016-01-02T23:59:58.555{sign}{hours}:{minutes}\"}}"));
        }

        [Test]
        public void ToJson_DateTime_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<DateTime?>();

            //act
            string json = converter.ToJson(new DateTime(2017,03,07, 23,59,42, DateTimeKind.Utc));

            //assert
            Assert.That(json, Is.EqualTo("\"2017-03-07T23:59:42Z\""));
        }

        [Test]
        public void ToJson_DateTimeNull_CorrectJson()
        {
            //arrange
            var converter = JsonFactory.Compile<DateTime?>();

            //act
            string json = converter.ToJson(null);

            //assert
            Assert.That(json, Is.EqualTo("null"));
        }
    }
}