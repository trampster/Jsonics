using System.Collections;
using System.Collections.Generic;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests.DictionaryTests
{
    [TestFixture]
    public class BoolFloatTests : DictionaryTestsBase<bool, float>
    {
       
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData("{}", new Dictionary<bool, float>{});
                yield return new TestCaseData("{\"true\":1.1}", new Dictionary<bool, float>{{true, 1.1f}});
                yield return new TestCaseData("{\"false\":0.05,\"true\":\"42.42\"}", new Dictionary<bool, float>{{false, 0.05f},{true, 42.42f}});
                yield return new TestCaseData("null", null);
                yield return new TestCaseData(" null", null);
            }
        }
    }
}