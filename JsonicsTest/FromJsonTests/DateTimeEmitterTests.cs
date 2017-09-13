using System;
using System.Collections;
using Jsonics;
using Jsonics.FromJson;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests
{
    [TestFixture]
    public class DateTimeEmitterTests
    {
        public class DateTimeCaseData
        {
            public static IEnumerable NullableDateTimeTestCases
            {
                get
                {
                    foreach(var testCase in DateTimeTestCases)
                    {
                        yield return testCase;
                    }
                    yield return new TestCaseData("null", 0, 4, 0, null, 4, DateTimeKind.Unspecified);
                    yield return new TestCaseData(" null", 0, 5, 0, null, 5, DateTimeKind.Unspecified);
                }
            }

            public static IEnumerable DateTimeTestCases
            {
                get
                {
                    yield return new TestCaseData("\"2017-07-25\"", 0, 12, 0, new DateTime(2017,7,25), 12, DateTimeKind.Unspecified);
                    yield return new TestCaseData("\"2017-07-25T23:59:58\"", 0, 21, 0, new DateTime(2017,7,25,23,59,58), 21, DateTimeKind.Unspecified);
                    var dateTime = new DateTime(2017,7,25,23,59,58).AddMilliseconds(196.329);
                    yield return new TestCaseData("\"2017-07-25T23:59:58.196329\"", 0, 28, 0, dateTime, 28, DateTimeKind.Unspecified);
                    dateTime = new DateTime(2017,7,25,23,59,58).AddMilliseconds(123.45678);
                    yield return new TestCaseData("\"2017-07-25T23:59:58.12345678\"", 0, 30, 0, dateTime, 30, DateTimeKind.Unspecified);
                    dateTime = new DateTime(2017,7,25,23,59,58, DateTimeKind.Utc).AddMilliseconds(123.45678);

                    //utc
                    yield return new TestCaseData("\"2017-07-25T23:59:58.12345678Z\"", 0, 31, 0, dateTime, 31, DateTimeKind.Utc);
                    
                    //with offset
                    var utc = new DateTime(2017,7,25,23,59,58, DateTimeKind.Utc).AddMilliseconds(123.45678).Subtract(new TimeSpan(3,15,0));
                    var local = utc.ToLocalTime();
                    yield return new TestCaseData("\"2017-07-25T23:59:58.12345678+03:15\"", 0, 36, 0, local, 36, DateTimeKind.Local);

                    //whitespace at start
                    yield return new TestCaseData(" \"2017-07-25\"", 0, 13, 0, new DateTime(2017,7,25), 13, DateTimeKind.Unspecified);

                    //lazy doesn't start at beginning
                    yield return new TestCaseData(" \"2017-07-25\"", 1, 12, 0, new DateTime(2017,7,25), 12, DateTimeKind.Unspecified);
                }
            }
        }

        [Test, TestCaseSource(typeof(DateTimeCaseData), "DateTimeTestCases")]
        public void ToDateTime_CorrectResult(string lazy, int start, int length, int index, DateTime expected, int expectedEndIndex, DateTimeKind expectedKind)
        {
            //arrange
            var lazyString = new LazyString(lazy, start, length);

            //act
            (DateTime result, int endIndex) = ToDateTimeStaticMethods.ToDateTime(lazyString, index);

            //assert
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(endIndex, Is.EqualTo(expectedEndIndex));
            Assert.That(result.Kind, Is.EqualTo(expectedKind));
        }

        [Test, TestCaseSource(typeof(DateTimeCaseData), "NullableDateTimeTestCases")]
        public void ToNullableDateTime_CorrectResult(string lazy, int start, int length, int index, DateTime? expected, int expectedEndIndex, DateTimeKind expectedKind)
        {
            //arrange
            var lazyString = new LazyString(lazy, start, length);

            //act
            (DateTime? result, int endIndex) = ToDateTimeStaticMethods.ToNullableDateTime(lazyString, index);

            //assert
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(endIndex, Is.EqualTo(expectedEndIndex));
        }
    }
}