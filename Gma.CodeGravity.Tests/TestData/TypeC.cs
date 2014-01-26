using System;

namespace nsplit
{
    internal static class TypeC
    {
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