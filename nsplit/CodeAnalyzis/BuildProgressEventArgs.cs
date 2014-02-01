using System;

namespace nsplit.CodeAnalyzis
{
    internal class BuildProgressEventArgs : EventArgs
    {
        private readonly string m_TaskName;
        private readonly int m_Actual;
        private readonly int m_Max;

        public BuildProgressEventArgs(string taskName, int actual, int max)
        {
            m_TaskName = taskName;
            m_Actual = actual;
            m_Max = max;
        }

        public string TaskName
        {
            get { return m_TaskName; }
        }

        public int Actual
        {
            get { return m_Actual; }
        }

        public int Max
        {
            get { return m_Max; }
        }
    }
}