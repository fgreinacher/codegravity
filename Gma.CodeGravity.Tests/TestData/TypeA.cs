using Gma.CodeGravity.Tests.Annotations;

namespace Gma.CodeGravity.Tests.TestData
{
    internal class TypeA
    {
        [UsedImplicitly]
        public static void Method2Call()
        {
            TypeC.Method2();
        }
    }
}