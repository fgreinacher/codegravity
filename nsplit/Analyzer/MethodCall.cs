using System;
using System.Reflection;

namespace nsplit.Analyzer
{
    public class MethodCall : Dependecy
    {
        private readonly MethodBase m_Method;

        public MethodCall(Type @from, Type to, MethodBase method)
            : base(@from, to)
        {
            m_Method = method;
        }

        public override DependencyKind DependencyKind
        {
            get { return DependencyKind.MethodCall; }
        }

        public MethodBase Method
        {
            get { return m_Method; }
        }

        public override string ToString()
        {
            return string.Format("{0,20}=[{1,20}]=>{2,-20}", From.Name, Method.Name, To.Name);
        }
    }
}