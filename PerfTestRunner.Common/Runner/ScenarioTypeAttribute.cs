using System;

namespace PerfTestRunner.Common.Runner
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ScenarioTypeAttribute : PerfTestAttribute
    {
        public ScenarioTypeAttribute(string desc)
        {
            Description = desc;
        }

        public bool Equals(ScenarioTypeAttribute other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other.Description, Description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as ScenarioTypeAttribute);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (Description != null ? Description.GetHashCode() : 0);
            }
        }
    }
}