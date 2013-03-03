namespace Disruptor.PerfTests.Runner
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImplementationTypeAttribute : Attribute
    {
        public ImplementationTypeAttribute(string desc)
        {
            Description = desc;
        }

        public string Description { get; private set; }
    }
}