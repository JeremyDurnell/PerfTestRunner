using System;
using System.Diagnostics;
using PerfTestRunner.Common.Runner;

namespace PerfTestRunner.Common
{
    public abstract class LatencyPerfTest : PerfTest
    {
        protected static readonly double TicksToNanos = 1000 * 1000 * 1000 / (double)Stopwatch.Frequency;

        protected LatencyPerfTest(int iterations) : base(iterations)
        {
            Histogram = InitHistogram();
            StopwatchTimestampCostInNano = InitStopwatchTimestampCostInNano();
        }

        protected LatencyPerfTest(int iterations, Histogram histogram)
            : base(iterations)
        {
            Histogram = histogram;
            StopwatchTimestampCostInNano = InitStopwatchTimestampCostInNano();
        }

        public abstract void RunPass();
        public virtual Histogram Histogram { get; private set; }

        protected override void RunAsUnitTest()
        {
            RunPass();

            Console.WriteLine("{0} : {1}", GetType().Name, Histogram);
        }

        public override TestRun CreateTestRun(int pass, int availableCores)
        {
            return new LatencyTestRun(this, pass, availableCores);
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
            const long iterations = 10 * 1000 * 1000;
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
            return (long)(((finish - start) / (double)iterations) * TicksToNanos);
        }

        protected static long StopwatchTimestampCostInNano { get; private set; }
    }
}