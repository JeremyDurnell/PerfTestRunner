using System;
using System.Collections.Generic;
using System.Diagnostics;
using PerfTestRunner.Common;
using PerfTestRunner.Common.Runner;

namespace PerfTestRunner.Demo
{
    [ImplementationType("Demo Implementation")]
    [ScenarioType("Throughput")]
    public class SampleTests : ThroughputPerfTest
    {
        public SampleTests() : base(2*Million)
        {
        }

        //[Test]
        public override int MinimumCoresRequired
        {
            get { return 1; }
        }

        public override void RunPerformanceTest()
        {
            RunAsUnitTest();
        }

        public override long RunPass()
        {
            // using a list of object (instead of Int32) to demonstrate GC compactions
            var testObjs = new List<object>();
            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < Million; i++)
            {
                testObjs.Add(i);

                // interestingly, higher throughput is obtained by clearing the list
                if (i%10000 == 0)
                {
                    // comment following line and notice throughput plummits and less GC activity
                    //testObjs.Clear();
                }
            }

            long opsPerSecond = Million*1000L/sw.ElapsedMilliseconds;

            // assert

            return opsPerSecond;
        }
    }

    [ImplementationType("Demo Implementation")]
    [ScenarioType("Latency")]
    public class LatencyDemo : LatencyPerfTest
    {
        protected static readonly double TicksToNanos = 1000*1000*1000/(double) Stopwatch.Frequency;
        private static long _stopwatchTimestampCostInNano;
        private readonly Histogram _histogram;

        public LatencyDemo() : base(Million)
        {
            _histogram = InitHistogram();
            _stopwatchTimestampCostInNano = InitStopwatchTimestampCostInNano();
        }

        //[Test]
        public override int MinimumCoresRequired
        {
            get { return 1; }
        }

        public override Histogram Histogram
        {
            get { return _histogram; }
        }

        protected static long StopwatchTimestampCostInNano
        {
            get { return _stopwatchTimestampCostInNano; }
        }

        public override void RunPerformanceTest()
        {
            RunAsUnitTest();
        }

        public override void RunPass()
        {
            var testObjs = new List<int>();

            for (int i = 0; i < Million; i++)
            {
                long start = Stopwatch.GetTimestamp();

                testObjs.Add(i);

                if (i%10000 == 0)
                {
                    // comment following line and notice mean time plummits
                    //Thread.Sleep(16);
                }

                Histogram.AddObservation(
                    (long) ((Stopwatch.GetTimestamp() - start)*TicksToNanos - StopwatchTimestampCostInNano));
            }
        }

        private Histogram InitHistogram()
        {
            var intervals = new long[31];
            long intervalUpperBound = 1L;
            for (int i = 0; i < intervals.Length - 1; i++)
            {
                intervalUpperBound *= 2;
                intervals[i] = intervalUpperBound;
            }

            intervals[intervals.Length - 1] = long.MaxValue;
            return new Histogram(intervals);
        }

        private static long InitStopwatchTimestampCostInNano()
        {
            const long iterations = 10*1000*1000;
            long start = Stopwatch.GetTimestamp();
            long finish = start;

            for (int i = 0; i < iterations; i++)
            {
                finish = Stopwatch.GetTimestamp();
            }

            if (finish <= start)
            {
                throw new Exception();
            }

            finish = Stopwatch.GetTimestamp();
            return (long) (((finish - start)/(double) iterations)*TicksToNanos);
        }
    }
}