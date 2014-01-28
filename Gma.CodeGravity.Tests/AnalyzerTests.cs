using System;
using System.Collections.Generic;
using System.Linq;
using Gma.CodeGravity.Tests.TestData;
using nsplit;
using nsplit.CodeAnalyzis;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;
using NUnit.Framework;

namespace Gma.CodeGravity.Tests
{
    [TestFixture]
    public class AnalyzerTests
    {
        [Test]
        public void Calls_CallToStaticMethodOfType_IsDetected()
        {
            var typeA = typeof(TypeA);

            var calls = typeA.Calls().ToArray();

            Assert.AreEqual(1, calls.Length);
            var call = calls[0];
            Assert.AreEqual(call.Kind, DependencyKind.MethodCall);
        }

        [Test]
        public void Uses_SingleGenericTypeArgumentInField_IsDetected()
        {
            var typeD = typeof(TypeD);

            var uses = typeD.Uses();

            Assert.That(uses, Has.Member(new Uses(typeD, typeof(string))));
        }

        [Test]
        public void Uses_MultipleGenericTypeArgumentsInField_AreDetected()
        {
            var typeD = typeof(TypeD);

            var uses = typeD.Uses();

            Assert.That(uses, Has.Member(new Uses(typeD, typeof(string))));
        }


        [Test]
        public void Uses_GenericTypeInField_IsDetected()
        {
            var typeD = typeof(TypeD);

            var uses = typeD.Uses();

            Assert.That(uses, Has.Member(new Uses(typeD, typeof(IEnumerable<>))));
        }
    }
}
