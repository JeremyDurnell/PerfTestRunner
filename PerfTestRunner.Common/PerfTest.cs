using System;
using PerfTestRunner.Common.Runner;

namespace PerfTestRunner.Common
{
    public abstract class PerfTest : MarshalByRefObject
    {
        protected PerfTest(int iterations)
        {
            Iterations = iterations;
        }

        public int PassNumber { get; set; }
        public int Iterations { get; private set; }
        protected abstract void RunAsUnitTest();
        public abstract void RunPerformanceTest();
        public abstract TestRun CreateTestRun(int pass, int availableCores);
        protected const int Million = 1000*1000;
        public abstract int MinimumCoresRequired { get; }
    }
}