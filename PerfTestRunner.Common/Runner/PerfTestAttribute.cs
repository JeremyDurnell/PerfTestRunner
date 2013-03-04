using System;

namespace PerfTestRunner.Common.Runner
{
    [Serializable]
    public class PerfTestAttribute : Attribute
    {
        public string Description { get; protected set; }
    }
}