using System;
using Gma.CodeGravity.Tests.Annotations;

namespace Gma.CodeGravity.Tests.TestData
{
    class TypeWithFieldOfTypeTupleIntFloat
    {
        [UsedImplicitly]
        Tuple<int, float> _;
    }
}