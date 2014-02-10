using System.Collections.Generic;
using Gma.CodeVisuals.Generator.DependencyForceGraph.DataStructures.DependencyGraph;
using Gma.CodeVisuals.Generator.DependencyForceGraph.DataStructures.TypeTree;

namespace Gma.CodeVisuals.Generator.DependencyForceGraph.Do
{
    internal class Graph
    {
        private readonly INode m_Tree;
        private readonly IEnumerable<Edge> m_Links;

        public Graph(INode tree, IEnumerable<Edge> links)
        {
            m_Tree = tree;
            m_Links = links;
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
            get { return m_Tree.Name; }
        }
    }
}