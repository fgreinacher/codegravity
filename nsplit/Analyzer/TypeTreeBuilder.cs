using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using nsplit.DataStructures;
using nsplit.DataStructures.Tree;

namespace nsplit.Analyzer
{
    internal class TypeTreeBuilder
    {
        private readonly Tree m_Tree;

        public TypeTreeBuilder()
        {
            var nodeFactory = new NodeFactory();
            m_Tree = new Tree(nodeFactory);
        }

        public Tree Tree
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
