using System;
using System.Collections;
using System.Collections.Generic;
using Jsonics;
using NUnit.Framework;

namespace JsonicsTests.FromJsonTests.DictionaryTests
{
    [TestFixture]
    public class GuidStringTests : DictionaryTestsBase<Guid, string>
    {
       
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData("{}", new Dictionary<Guid, string>{});
                yield return new TestCaseData("{\"e586bb3e-4d84-4c56-aef1-c3beed3748f0\":\"One\"}", new Dictionary<Guid, string>{{Guid.Parse("e586bb3e-4d84-4c56-aef1-c3beed3748f0"), "One"}});
                yield return new TestCaseData("{\"b9f2f816-3a67-474c-b587-a2b77349b3e2\":\"One\",\"8a1b28dd-743b-4f58-ab2a-426abcb0b67e\":\"Two\"}", new Dictionary<Guid, string>{{Guid.Parse("b9f2f816-3a67-474c-b587-a2b77349b3e2"), "One"},{Guid.Parse("8a1b28dd-743b-4f58-ab2a-426abcb0b67e"), "Two"}});
                yield return new TestCaseData("null", null);
                yield return new TestCaseData(" null", null);
            }
        }
    }
}