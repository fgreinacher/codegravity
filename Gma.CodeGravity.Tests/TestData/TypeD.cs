using System;
using System.Collections.Generic;
using Gma.CodeGravity.Tests.Annotations;

namespace Gma.CodeGravity.Tests.TestData
{
    class TypeD
    {
        [UsedImplicitly]
        IEnumerable<string> m_IEnumerableString;

        [UsedImplicitly]
        Tuple<int, float> m_TupleIntFloat;
    }
}