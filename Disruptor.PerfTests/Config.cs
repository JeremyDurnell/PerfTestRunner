using System.Collections.Generic;
using System.Linq;
using Disruptor.PerfTests.Runner;
using PerfTestRunner.Common.Runner;

namespace Disruptor.PerfTests
{
    internal class Config
    {
        public Config(ScenarioTypeAttribute scenario
                      , ImplementationTypeAttribute implementation
                      , uint runs, ScenarioTypeAttribute[] knownScenarios
                      , ImplementationTypeAttribute[] knownImplementations
                      , IList<TypeLocator> perfTests
                      , string pluginPath)
        {
            Scenario = scenario;
            Implementation = implementation;
            Runs = runs;
            KnownScenarios = knownScenarios;
            KnownImplementations = knownImplementations;
            PerfTests = perfTests;
            PluginPath = pluginPath;
            Groups = new Dictionary<ScenarioTypeAttribute, IDictionary<ImplementationTypeAttribute, TypeLocator>>();

            foreach (var knownScenario in KnownScenarios)
            {
                var inner = new Dictionary<ImplementationTypeAttribute, TypeLocator>();

                foreach (var knownImplementation in KnownImplementations)
                {
                    inner[knownImplementation] = perfTests.FirstOrDefault(pt => pt.Attributes.Contains(knownImplementation) && pt.Attributes.Contains(knownScenario));
                }

                Groups[knownScenario] = inner;
            }
        }

        public ScenarioTypeAttribute Scenario { get; private set; }
        public ImplementationTypeAttribute Implementation { get; private set; }
        public ScenarioTypeAttribute[] KnownScenarios { get; private set; }
        public ImplementationTypeAttribute[] KnownImplementations { get; private set; }
        public uint Runs { get; private set; }
        public IList<TypeLocator> PerfTests { get; private set; }
        public IDictionary<ScenarioTypeAttribute,IDictionary<ImplementationTypeAttribute, TypeLocator>> Groups { get; private set; }
        public string PluginPath { get; private set; }
    }
}