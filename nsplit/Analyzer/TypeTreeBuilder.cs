using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using nsplit.DataStructures;

namespace nsplit.Analyzer
{
    internal class TypeTreeBuilder
    {
        private readonly QualifiedTree m_Tree;

        public TypeTreeBuilder()
        {
            var nodeFactory = new QualifiedTreeNodeFactory();
            m_Tree = new QualifiedTree(nodeFactory);
        }

        public QualifiedTree Tree
        {
            get { return m_Tree; }
        }

        public void Add(Assembly assembly)
        {
            var types = assembly.Types();
            foreach (var type in types)
            {
                Add(type);
            }
        }

        public void Add(Type type)
        {
            var fullName = type.FullName;
            m_Tree.Add(fullName);
        }
    }
}
