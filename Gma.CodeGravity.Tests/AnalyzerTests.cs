// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System.Collections.Generic;
using System.Linq;
using Gma.CodeGravity.Tests.TestData;
using Gma.CodeVisuals.Generator.DependencyForceGraph;
using Gma.CodeVisuals.Generator.DependencyForceGraph.Do;
using NUnit.Framework;
using Contains = Gma.CodeVisuals.Generator.DependencyForceGraph.Do.Contains;

#endregion

namespace Gma.CodeGravity.Tests
{
    [TestFixture]
    public class AnalyzerTests
    {
        [Test]
        public void Calls_CallToStaticMethodOfType_IsDetected()
        {
            var typeA = typeof (TypeA);

            var calls = typeA.Calls().ToArray();

            Assert.AreEqual(1, calls.Length);
            var call = calls[0];
            Assert.AreEqual(call.Kind, DependencyKind.MethodCall);
        }

        [Test]
        public void Contains_NestedPrivate_IsNotDetected()
        {
            var type = typeof (TypeWithPrivateNestedType);

            var contains = type.Contains();

            Assert.That(contains, Is.Empty);
        }

        [Test]
        public void Contains_NestedPublic_IsDetected()
        {
            var type = typeof (TypeWithPublicNestedType);

            var contains = type.Contains();

            Assert.That(contains, Has.Member(new Contains(type, typeof (TypeWithPublicNestedType.NestedPublic))));
        }

        [Test]
        public void Uses_GenericTypeArgumentInMethodParameter_IsDetected()
        {
            var type = typeof (TypeWithMethodAcceptingIEnumerableOfChar);

            var uses = type.Uses();

            Assert.That(uses, Has.Member(new Uses(type, typeof (char))));
        }

        [Test]
        public void Uses_GenericTypeArgumentInMethodReturn_IsDetected()
        {
            var type = typeof (TypeWithMethodReturnIEnumerableOfInt);

            var uses = type.Uses();

            Assert.That(uses, Has.Member(new Uses(type, typeof (int))));
        }

        [Test]
        public void Uses_GenericTypeInField_IsDetected()
        {
            var type = typeof (TypeWithFieldOfIEnumerableString);

            var uses = type.Uses();

            Assert.That(uses, Has.Member(new Uses(type, typeof (IEnumerable<>))));
        }

        [Test]
        public void Uses_MultipleGenericTypeArgumentsInField_AreDetected()
        {
            var type = typeof (TypeWithFieldOfTypeTupleIntFloat);

            var uses = type.Uses();

            Assert.That(uses, Has.Member(new Uses(type, typeof (int))).And.Member(new Uses(type, typeof (float))));
        }

        [Test]
        public void Uses_PartiallyConstructedGenericTypeArgumentInField_IsDetected()
        {
            var type = typeof (TypeOfTWithFieldOfIDictionaryTChar<>);

            var uses = type.Uses();

            Assert.That(uses, Has.Member(new Uses(type, typeof (char))));
        }

        [Test]
        public void Uses_SingleGenericTypeArgumentInField_IsDetected()
        {
            var type = typeof (TypeWithFieldOfIEnumerableString);

            var uses = type.Uses();

            Assert.That(uses, Has.Member(new Uses(type, typeof (string))));
        }
    }
}