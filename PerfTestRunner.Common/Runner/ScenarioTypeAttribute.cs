namespace Disruptor.PerfTests.Runner
{
    using System;

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