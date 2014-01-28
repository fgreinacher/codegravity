using System;
using System.Reflection;

namespace nsplit.CodeAnalyzis.DataStructures.DependencyGraph
{
    public class MethodCall : Dependecy
    {
        private readonly MethodBase m_Method;

        public MethodCall(Type source, Type target, MethodBase method)
            : base(source, target)
        {
            m_Method = method;
        }

        public override DependencyKind Kind
        {
            get { return DependencyKind.MethodCall; }
        }

        public MethodBase Method
        {
            get { return m_Method; }
        }

        public override string ToString()
        {
            return string.Format("{0,20}=[{1,20}]=>{2,-20}", Source.Name, Method.Name, Target.Name);
        }
    }
}