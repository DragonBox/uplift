using System;

namespace Uplift.Common
{
    #region VersionStruct
    public struct VersionStruct
    {
        public int Major;
        public int? Minor, Patch, Optional;

        // Note - this is for using in sorting in comparison
        //        Ugly hack, but it works.
        //
        //        Feel free, to add IComparable interface.
        //        I gave up. (pkaminski)

        public int NumeralForm() {
            return Major * 10^9 +
                (Minor ?? 0) * 10^6 +
                (Patch ?? 0 ) * 10^3 +
                (Optional ?? 0);
        }

        public VersionStruct Next
        {
            get
            {
                VersionStruct result = this;
                if (Minor != null)
                {
                    if (Patch != null)
                    {
                        if (Optional != null) { result.Optional += 1; }
                        else { result.Patch += 1; }
                    }
                    else
                    {
                        result.Minor += 1;
                    }
                }
                else
                {
                    result.Major += 1;
                }
                return result;
            }
        }

        public static bool operator <(VersionStruct a, VersionStruct b)
        {
            if (a.Major != b.Major) return a.Major < b.Major;
            bool result = false;
            if (TryCompareInt(a.Minor, b.Minor, ref result)) return result;
            if (TryCompareInt(a.Patch, b.Patch, ref result)) return result;
            if (TryCompareInt(a.Optional, b.Optional, ref result)) return result;
            return false;
        }
        public static bool operator >(VersionStruct a, VersionStruct b) { return b < a; }
        public static bool operator <=(VersionStruct a, VersionStruct b) { return !(a > b); }
        public static bool operator >=(VersionStruct a, VersionStruct b) { return !(a < b); }

        public static bool operator ==(VersionStruct a, VersionStruct b) {
            return (
                a.Major == b.Major &&
                a.Minor == b.Minor &&
                a.Patch == b.Patch &&
                a.Optional == b.Optional
                );
        }
        public static bool operator !=(VersionStruct a, VersionStruct b) { return !(a == b); }
        public override int GetHashCode()
        {
            int result = Major;
            if (Minor != null) result = result & (int)Minor;
            if (Patch != null) result = result & (int)Patch;
            if (Optional != null) result = result & (int)Optional;
            return result;
        }
        public override bool Equals(object o)
        {
            return this == (VersionStruct)o;
        }

        private static bool TryCompareInt(int? a, int? b, ref bool result)
        {
            if (a != null)
            {
                if (b == null)
                {
                    result = false;
                    return true;
                }
                else if (a != b)
                {
                    result = a < b;
                    return true;
                }
            }
            else
            {
                // If a is null, versionA is X...Y
                // versionA is strictly lower than versionB if and only if versionB is X...Y.Z ie b is not null
                result = b != null;
                return true;
            }
            // Could not distinct versions
            return false;
        }

        public override string ToString()
        {
            string result = Major.ToString();
            if (Minor != null)
            {
                result += "." + Minor.ToString();
                if (Patch != null)
                {
                    result += "." + Patch.ToString();
                    if (Optional != null) { result += "." + Optional.ToString(); }
                }
            }
            return result;
        }
    }
    #endregion

    #region VersionRequirement
    public interface IVersionRequirement
    {
        bool IsMetBy(VersionStruct version);
        IVersionRequirement RestrictTo(IVersionRequirement other);
    }

    public static class VersionRequirementExtension
    {
        public static bool IsMetBy(this IVersionRequirement requirement, string version)
        {
            return requirement.IsMetBy(VersionParser.ParseVersion(version));
        }
    }

    // When no version requirement is specified
    public class NoRequirement : IVersionRequirement
    {
        public bool IsMetBy(VersionStruct version) { return true; }
        public IVersionRequirement RestrictTo(IVersionRequirement other) { return other; }
        public override string ToString() { return ""; }
    }

    // When minimal+ is specified
    public class MinimalVersionRequirement : IVersionRequirement
    {
        public VersionStruct minimal;

        public MinimalVersionRequirement(string minimal) : this(VersionParser.ParseIncompleteVersion(minimal)) { }
        public MinimalVersionRequirement(VersionStruct minimal)
        {
            this.minimal = minimal;
        }

        public bool IsMetBy(VersionStruct version)
        {
            return version >= minimal;
        }

        public IVersionRequirement RestrictTo(IVersionRequirement other)
        {
            if(other is NoRequirement)
            {
                return other.RestrictTo(this);
            }
            else if (other is MinimalVersionRequirement)
            {
                return minimal >= (other as MinimalVersionRequirement).minimal ? this : other;
            }
            else if (other is LoseVersionRequirement)
            {
                if (minimal <= (other as LoseVersionRequirement).stub) return other;
            }
            else if (other is BoundedVersionRequirement)
            {
                if (minimal <= (other as BoundedVersionRequirement).lowerBound) return other;
            }
            else if (other is ExactVersionRequirement)
            {
                if (minimal <= (other as ExactVersionRequirement).expected) return other;
            }
            throw new IncompatibleRequirementException(this, other);
        }

        public override string ToString()
        {
            return minimal.ToString() + "+";
        }
    }

    // When stub is specified
    public class LoseVersionRequirement : IVersionRequirement
    {
        public VersionStruct stub;
        private VersionStruct limit;

        public LoseVersionRequirement(string stub) : this(VersionParser.ParseIncompleteVersion(stub)) { }
        public LoseVersionRequirement(VersionStruct stub)
        {
            this.stub = stub;
            limit = stub.Next;
        }

        public bool IsMetBy(VersionStruct version)
        {
            return version >= stub && version < limit;
        }

        public IVersionRequirement RestrictTo(IVersionRequirement other)
        {
            if (other is NoRequirement || other is MinimalVersionRequirement)
            {
                return other.RestrictTo(this);
            }
            else if (other is LoseVersionRequirement)
            {
                if (IsMetBy((other as LoseVersionRequirement).stub)) return other;
                if ((other as LoseVersionRequirement).IsMetBy(stub)) return this;
            }
            else if(other is BoundedVersionRequirement)
            {
                if (IsMetBy((other as BoundedVersionRequirement).lowerBound)) return other;
            }
            else if(other is ExactVersionRequirement)
            {
                if (IsMetBy((other as ExactVersionRequirement).expected)) return other;
            }
            throw new IncompatibleRequirementException(this, other);
        }

        public override string ToString()
        {
            return stub.ToString();
        }
    }

    public class BoundedVersionRequirement : IVersionRequirement
    {
        public VersionStruct lowerBound;
        private VersionStruct upperBound;

        public BoundedVersionRequirement(string lowerBound) : this(VersionParser.ParseIncompleteVersion(lowerBound)) { }
        public BoundedVersionRequirement(VersionStruct lowerBound)
        {
            this.lowerBound = lowerBound;
            upperBound = lowerBound.Next;
        }

        public bool IsMetBy(VersionStruct version)
        {
            return version > lowerBound && version < upperBound;
        }

        public IVersionRequirement RestrictTo(IVersionRequirement other)
        {
            if (other is NoRequirement || other is MinimalVersionRequirement)
            {
                return other.RestrictTo(this);
            }
            else if(other is LoseVersionRequirement)
            {
                if (IsMetBy((other as LoseVersionRequirement).stub)) return other;
            }
            else if(other is BoundedVersionRequirement)
            {
                if (IsMetBy((other as BoundedVersionRequirement).lowerBound)) return other;
                if ((other as BoundedVersionRequirement).IsMetBy(lowerBound)) return this;
            }
            else if(other is ExactVersionRequirement)
            {
                if (IsMetBy((other as ExactVersionRequirement).expected)) return other;
            }
            throw new IncompatibleRequirementException(this, other);
        }

        public override string ToString()
        {
            return lowerBound.ToString() + ".*";
        }
    }

    public class ExactVersionRequirement : IVersionRequirement
    {
        public VersionStruct expected;

        public ExactVersionRequirement(string expected) : this(VersionParser.ParseIncompleteVersion(expected)) { }
        public ExactVersionRequirement(VersionStruct expected)
        {
            this.expected = expected;
        }

        public bool IsMetBy(VersionStruct version)
        {
            return expected.Equals(version);
        }

        public IVersionRequirement RestrictTo(IVersionRequirement other)
        {
            if (other is ExactVersionRequirement)
            {
                if (IsMetBy((other as ExactVersionRequirement).expected)) return this;
            }
            else
            {
                return other.RestrictTo(this);
            }
            throw new IncompatibleRequirementException(this, other);
        }

        public override string ToString()
        {
            return expected.ToString() + "!";
        }
    }
    #endregion

    #region Exception
    public class IncompatibleRequirementException : Exception
    {
        public IncompatibleRequirementException() : base("Incompatible requirements were identified") { }
        public IncompatibleRequirementException(string message) : base(message) { }
        public IncompatibleRequirementException(IVersionRequirement a, IVersionRequirement b)
            : this(string.Format("Requirements {0} and {1} are not compatible", a.ToString(), b.ToString())) { }
        public IncompatibleRequirementException(string format, params object[] args) : base(string.Format(format, args)) { }
        public IncompatibleRequirementException(string message, Exception innerException) : base(message, innerException) { }
        public IncompatibleRequirementException(string format, Exception innerException, params object[] args) : base(string.Format(format, args), innerException) { }
    }
    #endregion
}
