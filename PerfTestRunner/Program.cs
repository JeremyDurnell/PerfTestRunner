// TODO: make output directory configurable - or just move it to /output

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PerfTestRunner.Common.Runner;
using PerfTestRunner.Runner;

namespace PerfTestRunner
{
    public static class Program
    {
        static void Main(string[] args)
        {
            string pluginPath = null;

            bool helpMode = args != null && args.Length > 0 && char.ToLowerInvariant(args[0][0]) == 'h';

            if (helpMode)
            {
                if (args.Length > 1)
                {
                    pluginPath = args[1];
                }
                else
                {
                    PrintUsage(null, null);
                    return;
                }
            }
            else if (args != null && args.Length > 3)
            {
                pluginPath = args[3];
            }

            IList<TypeLocator> types = PluginFinder.FindPlugins(pluginPath).ToList();
            var distinctAttributes = types.SelectMany(t => t.Attributes).Distinct().ToArray();
            var distinctScenarios = distinctAttributes.Where(a => a is ScenarioTypeAttribute).Cast<ScenarioTypeAttribute>().ToArray();
            var distinctImplementations = distinctAttributes.Where(a => a is ImplementationTypeAttribute).Cast<ImplementationTypeAttribute>().ToArray();

            if (helpMode)
            {
                PrintUsage(distinctScenarios,distinctImplementations);
                return;
            }

            uint? scenarioIndex = ReadArg(0, args);
            uint? implementationIndex = ReadArg(1, args);
            uint? runs = ReadArg(2, args);

            PrintMenu("Scenarios", distinctScenarios);
            ScenarioTypeAttribute scenario = ReadMenuChoice("Scenario", distinctScenarios, scenarioIndex);
            Console.WriteLine(scenario == null ? "All" : scenario.Description);
            Console.WriteLine();

            PrintMenu("Implementations", distinctImplementations);
            ImplementationTypeAttribute implementation = ReadMenuChoice("Implementation", distinctImplementations, implementationIndex);
            Console.WriteLine(implementation == null ? "All" : implementation.Description);
            Console.WriteLine();

            runs = PromptUint("Runs", 1, 1000, runs);
            Console.WriteLine(runs);
            Console.WriteLine();

            var config = new Config(scenario, implementation, runs.Value, distinctScenarios, distinctImplementations, types, pluginPath);
            Console.WriteLine(new ComputerSpecifications().ToString());
            var session = new PerformanceTestSession(config);
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
            session.Run();
            session.GenerateAndOpenReport();
            Console.ReadKey();
        }

        private static uint? ReadArg(int index, params string[] args)
        {
            uint argValue;
            
            if (!uint.TryParse(args.Skip(index).Take(1).FirstOrDefault(), out argValue))
            {
                return null;
            }

            return argValue;
        }

        private static void PrintMenu<T>(string title, IList<T> attribs) where T : PerfTestAttribute
        {
            Console.WriteLine(title + ":");
            Console.WriteLine();
            Console.WriteLine("0 - All");

            for (int i = 1; i < attribs.Count + 1; i++)
            {
                Console.WriteLine(i + " - " + attribs[i - 1].Description);
            }

            Console.WriteLine();
        }

        private static T ReadMenuChoice<T>(string prompt, IList<T> attribs, uint? arg = null) where T : PerfTestAttribute
        {
            var choice = PromptUint(prompt, 0, (uint) attribs.Count, arg);
            return choice == 0 ? null : attribs[(int) (choice - 1)];
        }

        private static uint PromptUint(string prompt, uint min, uint max, uint? arg = null)
        {
            while (true)
            {
                Console.Write(prompt + ": ");

                if (arg.HasValue && arg >= min && arg <= max)
                {
                    Console.WriteLine(arg);
                    return arg.Value;
                }
                
                string entryString = (Console.ReadLine() ?? "").Trim();
                uint entryUint;

                if (uint.TryParse(entryString, out entryUint) && entryUint >= min && entryUint <= max)
                {
                    return entryUint;
                }


                Console.WriteLine("Invalid entry.  Please choose valid {0} between {1} and {2}.", prompt, min, max);
            }
        }

        private static void PrintUsage(ScenarioTypeAttribute[] distinctScenarios, ImplementationTypeAttribute[] distinctImplementations)
        {
            Console.WriteLine("Usage: Disruptor.PerfTests Scenario Implementation Runs Path");
            Console.WriteLine();

            if (distinctScenarios != null && distinctImplementations != null)
            {
                PrintMenu("Scenarios", distinctScenarios);
                Console.WriteLine();
                PrintMenu("Implementations", distinctImplementations);
                Console.WriteLine();
                Console.WriteLine("Runs: number of test run to do for each scenario and implementation");
                Console.WriteLine();
            }

            Console.WriteLine("Example: Disruptor.PerfTests 0 0 3 c:\\PerfTests");
            Console.WriteLine("will run all performance test scenarios for all implementations 3 times.");
        }
    }
}
