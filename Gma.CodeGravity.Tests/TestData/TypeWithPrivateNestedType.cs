// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

namespace Gma.CodeGravity.Tests.TestData
{
    public class TypeWithPrivateNestedType
    {
        public void Foo()
        {
            var instance = new NestedPrivate();
            instance.Bar();
        }

        private class NestedPrivate
        {
            public void Bar()
            {
            }
        }
    }
}