// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System;

#endregion

namespace nsplit.CodeAnalyzis
{
    internal class BuildProgressEventArgs : EventArgs
    {
        private readonly int m_Actual;
        private readonly int m_Max;
        private readonly string m_TaskName;

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