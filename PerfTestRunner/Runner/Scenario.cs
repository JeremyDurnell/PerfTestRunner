using System.Collections.Generic;
using System.Text;
using PerfTestRunner.Common.Runner;

namespace PerfTestRunner.Runner
{
    internal class Scenario
    {
        private readonly IList<Implementation> _implementations = new List<Implementation>();

        public Scenario(ScenarioTypeAttribute scenario, int numberOfCores, Config config)
        {
            if (config.Implementation == null)
            {
                foreach (var implementation in config.KnownImplementations)
                {
                    _implementations.Add(new Implementation(scenario, implementation, (int) config.Runs, numberOfCores, config));
                }
            }
            else
            {
                _implementations.Add(new Implementation(scenario, config.Implementation, (int)config.Runs, numberOfCores, config));
            }
        }

        public void Run()
        {
            foreach (var implementation in _implementations)
            {
                implementation.Run();
            }
        }

        public void AppendDetailedHtmlReport(StringBuilder sb)
        {
            foreach (var implementation in _implementations)
            {
                implementation.AppendDetailedHtmlReport(sb);
            }
        }
    }
}