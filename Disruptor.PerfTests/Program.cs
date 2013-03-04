using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Policy;
using Disruptor.PerfTests.Runner;
using PerfTestRunner.Common;

namespace Disruptor.PerfTests
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var domains = new List<AppDomain>();
            var plugins = new List<PerfTest>();
            string pluginPath = @"C:\temp2\Disruptor.PerfTests\PerfTestRunner.Demo\bin\Debug";
            var types = PluginFinder.FindPlugins(pluginPath);

            foreach (var type in types)
            {
                var domain = CreateSandboxDomain("Sandbox Domain", pluginPath, SecurityZone.Internet);
                plugins.Add((PerfTest)domain.CreateInstanceAndUnwrap(type.AssemblyName, type.TypeName));
                domains.Add(domain);
            }

            foreach (PerfTest plugin in plugins)
            {
                //plugin.Initialize(host);
                //plugin.SaySomething();
                //plugin.CallBackToHost();

                var testRun = plugin.CreateTestRun(0, 4);
                testRun.Run();

                // To prove that the sandbox security is working we can call a plugin method that does something
                // dangerous, which throws an exception because the plugin assembly has insufficient permissions.
                //plugin.DoSomethingDangerous();
            }



            //ScenarioType scenarioType;
            //ImplementationType implementationType;
            //int runs;

            //if (args == null
            //    || args.Length <= 3
            //    || !Enum.TryParse(args[0], out scenarioType)
            //    || !Enum.TryParse(args[1], out implementationType)
            //    || !int.TryParse(args[2], out runs)
            //    )
            //{
            //    PrintUsage();
            //    return;
            //}

            //Console.WriteLine(new ComputerSpecifications().ToString());

            //var session = new PerformanceTestSession(scenarioType, implementationType, runs);

            //Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;

            //session.Run();

            //session.GenerateAndOpenReport();
            Console.ReadKey();
        }

        /// <summary>
        /// Returns a new <see cref="AppDomain"/> according to the specified criteria.
        /// Based on Tim Coulter's work: http://stackoverflow.com/questions/4145713/looking-for-a-practical-approach-to-sandboxing-net-plugins
        /// </summary>
        /// <param name="name">The name to be assigned to the new instance.</param>
        /// <param name="path">The root folder path in which assemblies will be resolved.</param>
        /// <param name="zone">A <see cref="SecurityZone"/> that determines the permission set to be assigned to this instance.</param>
        /// <returns></returns>
        public static AppDomain CreateSandboxDomain(
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


        private static void PrintUsage()
        {
            Console.WriteLine("Usage: Disruptor.PerfTests Scenario Implementation Runs");
            Console.WriteLine();
            PrintEnum(typeof(ScenarioType));
            Console.WriteLine();
            PrintEnum(typeof(ImplementationType));
            Console.WriteLine();
            Console.WriteLine("Runs: number of test run to do for each scenario and implementation");
            Console.WriteLine();
            Console.WriteLine("Example: Disruptor.PerfTests 0 0 3");
            Console.WriteLine("will run all performance test scenarios for all implementations 3 times.");
        }

        private static void PrintEnum(Type enumType)
        {
            var names = Enum.GetNames(enumType);
            var values = Enum.GetValues(enumType);

            Console.WriteLine(enumType.Name + " options:");

            for (var i = 0; i < names.Length; i++)
            {
                var name = names[i];
                var value = (int)values.GetValue(i);
                Console.WriteLine(" - {0} ({1})", value, name);
            }
        }
    }
}
