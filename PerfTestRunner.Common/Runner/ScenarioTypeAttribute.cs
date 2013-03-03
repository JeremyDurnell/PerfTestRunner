using System;

namespace PerfTestRunner.Common.Runner
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ScenarioTypeAttribute : Attribute
    {
        public ScenarioTypeAttribute(string desc)
        {
            Description = desc;
        }

        public string Description { get; private set; }
    }
}