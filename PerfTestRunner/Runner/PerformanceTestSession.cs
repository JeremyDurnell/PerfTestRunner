using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PerfTestRunner.Runner
{
    internal class PerformanceTestSession
    {
        private readonly string _scenarioType;
        private readonly string _implementationType;
        private readonly uint _runs;
        private readonly IList<Scenario> _scenarios = new List<Scenario>();
        private readonly ComputerSpecifications _computerSpecifications;

        public PerformanceTestSession(Config config)
        {
            _computerSpecifications = new ComputerSpecifications();
            _runs = config.Runs;
            _scenarioType = config.Scenario == null ? "All" : config.Scenario.Description;
            _implementationType = config.Implementation == null ? "All" : config.Implementation.Description;

            Console.WriteLine("Scenario={0}, Implementation={1}, Runs={2}"
                , _scenarioType
                , _implementationType
                , config.Runs);
            
            if (config.Scenario == null)
            {
                foreach (var scenario in config.KnownScenarios)
                {
                    _scenarios.Add(new Scenario(scenario,_computerSpecifications.NumberOfCores, config));
                }
            }
            else
            {
               _scenarios.Add(new Scenario(config.Scenario, _computerSpecifications.NumberOfCores, config));
            }
        }

        public void Run()
        {
            foreach (var scenario in _scenarios)
            {
                scenario.Run();
            }
        }

        private string BuildReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">")
                .AppendLine("<html>")
                .AppendLine("	<head>")
                .AppendLine("		<title>Disruptor-net - Test Report</title>")
                .AppendLine("	</head>")
                .AppendLine("	<body>")
                .AppendLine("        Local time: " + DateTime.Now + "<br>")
                .AppendLine("        UTC time: " + DateTime.UtcNow);

            sb.AppendLine("        <h2>Host configuration</h2>");
            _computerSpecifications.AppendHtml(sb);

            if(_computerSpecifications.NumberOfCores < 4)
            {
                sb.AppendFormat("        <b><font color='red'>Your computer has {0} physical core(s) but most of the tests require at least 4 cores</font></b><br>", _computerSpecifications.NumberOfCores);
            }
            if (_computerSpecifications.IsHyperThreaded)
            {
                sb.AppendLine("        <b><font color='red'>Hyperthreading can degrade performance, you should turn it off.</font></b><br>");
            }

            sb.AppendLine("        <h2>Test configuration</h2>")
                .AppendLine("        Scenarios: " + _scenarioType + "<br>")
                .AppendLine("        Implementations: " + _implementationType + "<br>")
                .AppendLine("        Runs: " + _runs + "<br>");

            sb.AppendLine("        <h2>Test results</h2>")
                .AppendLine("        Best results of " + _runs + " run(s).<br>")
                .AppendLine("        <br>");

            //TODO
            sb.AppendLine("        <h2>Detailed test results</h2>");
            sb.AppendLine("        <table border=\"1\">");
            sb.AppendLine("            <tr>");
            sb.AppendLine("                <td>Scenario</td>");
            sb.AppendLine("                <td>Implementation</td>");
            sb.AppendLine("                <td>Run</td>");
            sb.AppendLine("                <td>Operations per second</td>");
            sb.AppendLine("                <td>Duration (ms)</td>");
            sb.AppendLine("                <td># GC (0-1-2)</td>");
            sb.AppendLine("                <td>Comments");
            sb.AppendLine("            </tr>");

            foreach (var scenario in _scenarios)
            {
                scenario.AppendDetailedHtmlReport(sb);
            }

            sb.AppendLine("        </table>");

            return sb.ToString();
        }

        public void GenerateAndOpenReport()
        {
            var path = Path.Combine(Environment.CurrentDirectory,
                                    "TestReport-" + DateTime.UtcNow.ToString("yyyy-MM-dd hh-mm-ss") + ".html");

            File.WriteAllText(path, BuildReport());

            Process.Start(path);
        }
    }
}