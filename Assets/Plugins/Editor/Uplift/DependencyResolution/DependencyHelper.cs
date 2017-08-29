using Uplift.Schemas;

namespace Uplift.DependencyResolution
{
    public class DependencyHelper
    {
        public delegate void ConflictChecker(ref DependencyNode existing, DependencyNode compared);
    }

    [System.Serializable]
    public class MissingDependencyException : System.Exception
    {
        public MissingDependencyException() { }
        public MissingDependencyException(string message) : base(message) { }
        public MissingDependencyException(string message, System.Exception inner) : base(message, inner) { }
        protected MissingDependencyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
