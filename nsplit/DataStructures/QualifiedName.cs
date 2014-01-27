using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace nsplit.DataStructures
{

    public class QualifiedName
    {
        private readonly string[] m_Path;
        private readonly string m_Leaf;

        protected QualifiedName(string[] path, string leaf)
        {
            m_Path = path;
            m_Leaf = leaf;
        }

        public IEnumerable<string> Nodes
        {
            get { return m_Path; }
        }

        public string Leaf
        {
            get
            {
                return m_Leaf;
            }
        }

        public static QualifiedName Parse(string fullName)
        {
            string[] parts = fullName.Split(Qualifier);
            var length = parts.Length - 1;
            var path = new string[length];
            Array.Copy(parts, path, length);
            var leaf = parts[length];
            return new QualifiedName(path, leaf);
        }

        public const char Qualifier = '.';
    }
}
