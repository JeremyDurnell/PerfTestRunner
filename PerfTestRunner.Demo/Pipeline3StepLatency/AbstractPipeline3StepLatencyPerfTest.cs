using PerfTestRunner.Common;
using PerfTestRunner.Common.Runner;
using PerfTestRunner.Demo.Support;

namespace PerfTestRunner.Demo.Pipeline3StepLatency
{
    [ScenarioType(ScenarioTypes.Pipeline3StepLatency)]
    public abstract class AbstractPipeline3StepLatencyPerfTest :LatencyPerfTest
    {
        protected const int Size = 1024 * 32;
        protected const long PauseNanos = 1000;

        protected AbstractPipeline3StepLatencyPerfTest(int iterations) : base(iterations)
        {
        }

        public override int MinimumCoresRequired
        {
            get { return 4; }
        }
    }
}