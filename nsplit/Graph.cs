using System.Collections.Generic;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;
using nsplit.CodeAnalyzis.DataStructures.TypeTree;

namespace nsplit
{
    internal class Graph
    {
        private readonly INode m_Tree;
        private readonly IEnumerable<Edge> m_Links;
        private readonly string m_Name;

        public Graph(INode tree, IEnumerable<Edge> links, string name)
        {
            m_Tree = tree;
            m_Links = links;
            m_Name = name;
        }

        public INode Tree
        {
            get { return m_Tree; }
        }

        public IEnumerable<Edge> Links
        {
            get { return m_Links; }
        }

        public string Name
        {
            get { return m_Name; }
        }
    }
}