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
        public LatencyDemo() : base(Million)
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
    }
}