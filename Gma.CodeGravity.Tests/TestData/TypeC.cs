using System;
using Gma.CodeGravity.Tests.Annotations;

namespace Gma.CodeGravity.Tests.TestData
{
    internal static class TypeC
    {
        [UsedImplicitly]
        public static void Method1()
        {
            Method2();
        }

        public static void Method2()
        {
        }

        public static TypeA Do()
        {
            throw new Exception();
        }
    }
}