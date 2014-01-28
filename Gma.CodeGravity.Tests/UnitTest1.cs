using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nsplit;
using nsplit.CodeAnalyzis;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;

namespace Gma.CodeGravity.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var typeA = typeof (TypeA);
            var calls = typeA.Calls().ToArray();
            Assert.AreEqual(1, calls.Length);
            var call = calls[0];
            Assert.AreEqual(call.Kind, DependencyKind.MethodCall);
        }
    }
}
