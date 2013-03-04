using System;

namespace PerfTestRunner.Common.Runner
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImplementationTypeAttribute : PerfTestAttribute
    {
        public ImplementationTypeAttribute(string desc)
        {
            Description = desc;
        }

        public bool Equals(ImplementationTypeAttribute other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other.Description, Description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as ImplementationTypeAttribute);
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