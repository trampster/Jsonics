using System.Collections;
using System.Collections.Generic;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests.DictionaryTests
{
    [TestFixture]
    public class IntStringTests : DictionaryTestsBase<int, string>
    {
       
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData("{}", new Dictionary<int, string>{});
                yield return new TestCaseData("{\"1\":\"One\"}", new Dictionary<int, string>{{1, "One"}});
                yield return new TestCaseData("{\"1\":\"One\",\"42\":\"FourtyTwo\"}", new Dictionary<int, string>{{1, "One"},{42, "FourtyTwo"}});
                yield return new TestCaseData("null", null);
                yield return new TestCaseData(" null", null);
            }
        }
    }
}