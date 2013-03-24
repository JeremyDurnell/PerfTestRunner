using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Policy;
using System.Text;
using PerfTestRunner.Common;
using PerfTestRunner.Common.Runner;

namespace PerfTestRunner.Runner
{
    internal class Implementation : IDisposable
    {
        private readonly string _scenarioName;
        private readonly string _implementationName;
        private readonly IList<TestRun> _testRuns = new List<TestRun>();
        private readonly AppDomain _domain;

        public Implementation(ScenarioTypeAttribute scenario, ImplementationTypeAttribute implementation, int runs, int numberOfCores, Config config)
        {
            _scenarioName = scenario.Description;
            _implementationName = implementation.Description;
            var type = config.Groups[scenario][implementation];

            // HACK: shouldn't add combinations that don't exist
            if (type == null)
            {
                return;
            }

            _domain = CreateSandboxDomain("Sandbox Domain", config.PluginPath, SecurityZone.Internet);

            for (int i = 0; i < config.Runs; i++)
            {
                var perfTest = (PerfTest)_domain.CreateInstanceAndUnwrap(type.AssemblyName, type.TypeName);
                _testRuns.Add(perfTest.CreateTestRun(i,numberOfCores));
            }
        }

        public void Run()
        {
            foreach (var testRun in _testRuns)
            {
                testRun.Run();
            }
        }

        public void AppendDetailedHtmlReport(StringBuilder sb)
        {
            foreach (var testRun in _testRuns)
            {
                sb.AppendLine("            <tr>");
                sb.AppendLine("                <td>" + _scenarioName + "</td>");
                sb.AppendLine("                <td>" + _implementationName + "</td>");
                testRun.AppendResultHtml(sb);
                sb.AppendLine("            </tr>");
            }
        }

        /// <summary>
        /// Returns a new <see cref="AppDomain"/> according to the specified criteria.
        /// Based on Tim Coulter's work: http://stackoverflow.com/questions/4145713/looking-for-a-practical-approach-to-sandboxing-net-plugins
        /// </summary>
        /// <param name="name">The name to be assigned to the new instance.</param>
        /// <param name="path">The root folder path in which assemblies will be resolved.</param>
        /// <param name="zone">A <see cref="SecurityZone"/> that determines the permission set to be assigned to this instance.</param>
        /// <returns></returns>
        private AppDomain CreateSandboxDomain(
            string name,
            string path,
            SecurityZone zone)
        {
            var setup = new AppDomainSetup { ApplicationBase = Path.GetFullPath(path) };

            var evidence = new Evidence();
            evidence.AddHostEvidence(new Zone(zone));
            var permissions = SecurityManager.GetStandardSandbox(evidence);

            var strongName = typeof(Program).Assembly.Evidence.GetHostEvidence<StrongName>();

            return AppDomain.CreateDomain(name, null, setup, permissions, strongName);
        }

        public void Dispose()
        {
            if (_domain != null)
            {
                AppDomain.Unload(_domain);
            }
        }
    }
}