using System.Collections.Generic;
using Gma.CodeGravity.Tests.Annotations;

namespace Gma.CodeGravity.Tests.TestData
{
    class TypeWithFieldOfIEnumerableString
    {
        [UsedImplicitly]
        IEnumerable<string> _;
    }
}