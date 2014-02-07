using System;
using System.Reflection;
using System.Threading;
using nsplit.CodeAnalyzis;

namespace nsplit
{
    internal class AnalyzerTask
    {
        private readonly Analyzer m_Analyzer;
        private readonly CancellationTokenSource m_TokenSource;
        private AnalyzesProgressEventArgs m_CurrentProgress;
        private Action<AnalyzerTask> m_OnFinishedAction;

        private AnalyzerTask(Analyzer analyzer)
        {
            m_Analyzer = analyzer;
            m_CurrentProgress = AnalyzesProgressEventArgs.Idle();
            m_TokenSource = new CancellationTokenSource();
        }


        public static AnalyzerTask Idle()
        {
            var analyzer = Analyzer.Empty();
            return new AnalyzerTask(analyzer);
        }

        public static AnalyzerTask Create(Assembly assembly)
        {
            var tokenSource = new CancellationTokenSource();
            var analyzer = Analyzer.StartAnalyzesAsync(assembly, tokenSource.Token);
            Console.WriteLine("Analyzes started.\n\rAssembly: [{0}]", assembly.GetName());
            var task = new AnalyzerTask(analyzer)
            {
                m_CurrentProgress = AnalyzesProgressEventArgs.Started()
            };
            analyzer.OnProgress += (sender, e) =>
            {
                if (e.IsFinished)
                {
                    Console.WriteLine("Analyzes finished.");
                    if (task.m_OnFinishedAction != null)
                    {
                        task.m_OnFinishedAction.Invoke(task);
                    }
                }
                task.m_CurrentProgress = e;
            };
            return task;
        }

        public void Cancel()
        {
            Console.WriteLine("Analyzes canceled.");
            m_TokenSource.Cancel();
        }

        public AnalyzesProgressEventArgs GetProgress()
        {
            return m_CurrentProgress;
        }

        public Graph GetGraph()
        {
            return m_Analyzer.GetGraph();
        }

        public void OnFinished(Action<AnalyzerTask> action)
        {
            m_OnFinishedAction = action;
        }
    }
}