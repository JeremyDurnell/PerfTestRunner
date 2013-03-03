using System;
using System.Diagnostics;
using System.Text;

namespace Disruptor.PerfTests.Runner
{
    using System.Runtime.Serialization;

    public sealed class LatencyTestRun : TestRun
    {
        private readonly LatencyPerfTest _latencyPerfTest;

        public LatencyTestRun(LatencyPerfTest latencyPerfTest, int run, int availableCores) : base(run, latencyPerfTest, availableCores)
        {
            _latencyPerfTest = latencyPerfTest;
        }

        public override void Run()
        {
            _latencyPerfTest.PassNumber = RunIndex;

            GC.Collect();
            var sw = Stopwatch.StartNew();
            int gen0Count = GC.CollectionCount(0);
            int gen1Count = GC.CollectionCount(1);
            int gen2Count = GC.CollectionCount(2);

            _latencyPerfTest.Histogram.Clear();

            _latencyPerfTest.RunPass();

            if (_latencyPerfTest.Histogram.Count!= 0)
            {
                if (_latencyPerfTest.Iterations != _latencyPerfTest.Histogram.Count)
                {
                    throw new AssertionException("Expected: " + _latencyPerfTest.Iterations + ", Actual: " + _latencyPerfTest.Histogram.Count);
                }
            }

            DurationInMs = sw.ElapsedMilliseconds;

            Gen0Count = GC.CollectionCount(0) - gen0Count;
            Gen1Count = GC.CollectionCount(1) - gen1Count;
            Gen2Count = GC.CollectionCount(2) - gen2Count;
            Console.WriteLine("{0} : {1}", _latencyPerfTest.GetType().Name, _latencyPerfTest.Histogram);
            DumpHistogram();
        }

        protected override void AppendPerfResultHtml(StringBuilder sb)
        {
            var histo = _latencyPerfTest.Histogram;
            sb.AppendLine(string.Format("                <td>mean={0}ns, 99%={1}ns, 99.99%={2}</td>", histo.Mean, histo.TwoNinesUpperBound, histo.FourNinesUpperBound));
        }

        private void DumpHistogram()
        {
            var histo = _latencyPerfTest.Histogram;
            for (var i = 0; i < histo.Size; i++)
            {
                Console.Write(histo.GetUpperBoundAt(i));
                Console.Write("\t");
                Console.Write(histo.GetCountAt(i));
                Console.WriteLine();
            }
        }

        [Serializable]
        private class AssertionException : Exception
        {
            public AssertionException(string message)
                : base(message)
            {
            }

            public AssertionException(string message, Exception inner)
                : base(message, inner)
            {
            }

            protected AssertionException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }
    }
}