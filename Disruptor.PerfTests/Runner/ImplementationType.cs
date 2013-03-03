namespace Disruptor.PerfTests.Runner
{
    using System;

    public enum ImplementationType
    {
        All = 0,
        Disruptor = 1,
        DisruptorWithAffinity = 2,
        BlockingCollection = 3
    }

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