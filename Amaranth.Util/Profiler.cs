using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Amaranth.Util
{
    public static class Profiler
    {
        public static IDisposable Block()
        {
            StackTrace stack = new StackTrace(1);
            return new DisposableLabel(stack.GetFrame(0).GetMethod().Name);
        }

        public static IDisposable Block(string label)
        {
            return new DisposableLabel(label);
        }

        public static void Init()
        {
            sProcess = Process.GetCurrentProcess();
            sCurrentCounters = new Stack<CurrentCounter>();
            sCounters = new Dictionary<string, Counter>();
        }

        public static void Shutdown()
        {
            Console.WriteLine("Path                                                                    Total ms   Count      Ave ms");
            Console.WriteLine("----------------------------------------------------------------------- --------   -----   ---------");
            foreach (Counter counter in sCounters.Values)
            {
                Console.WriteLine("{0,-70} {1,9:F4} / {2,5} = {3,9:F4}", counter.Label, counter.TotalTime, counter.Count, counter.TotalTime / counter.Count);
            }

            sProcess = null;
            sCurrentCounters = null;
            sCounters = null;
        }

        public static void Push(string label)
        {
            if (sProcess == null) return;

            CurrentCounter counter = new CurrentCounter(label, sProcess.UserProcessorTime.TotalMilliseconds);
            sCurrentCounters.Push(counter);
        }

        public static void Pop()
        {
            if (sProcess == null) return;

            CurrentCounter thisCounter = sCurrentCounters.Pop();
            double elapsed = sProcess.UserProcessorTime.TotalMilliseconds - thisCounter.StartTime;

            // build the label
            string longLabel = String.Empty;
            foreach (CurrentCounter current in sCurrentCounters)
            {
                longLabel = current.Label + "/" + longLabel;
            }
            longLabel += thisCounter.Label;

            Counter counter = null;
            if (!sCounters.ContainsKey(longLabel))
            {
                counter = new Counter();
                counter.Label = longLabel;

                sCounters[longLabel] = counter;
            }
            else
            {
                counter = sCounters[longLabel];
            }

            counter.TotalTime += elapsed;
            counter.Count++;
        }

        private class CurrentCounter
        {
            public string Label;
            public double StartTime;

            public CurrentCounter(string label, double startTime)
            {
                Label = label;
                StartTime = startTime;
            }
        }

        private class Counter
        {
            public string Label;
            public double TotalTime;
            public int Count;
        }

        private class DisposableLabel : IDisposable
        {
            #region IDisposable Members

            public void Dispose()
            {
                Pop();
            }

            #endregion

            public DisposableLabel(string label)
            {
                mLabel = label;

                Push(mLabel);
            }

            private string mLabel;
        }

        private static Process sProcess;
        private static Stack<CurrentCounter> sCurrentCounters;
        private static Dictionary<string, Counter> sCounters;
    }
}
