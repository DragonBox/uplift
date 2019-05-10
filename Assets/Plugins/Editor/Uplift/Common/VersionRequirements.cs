// --- BEGIN LICENSE BLOCK ---
/*
 * Copyright (c) 2017-present WeWantToKnow AS
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
// --- END LICENSE BLOCK ---

using System;

namespace Uplift.Common
{
	#region Version
	public class Version : IComparable, ICloneable
	{
		public int Major;
		public int? Minor, Patch, Optional;

		// This is to conform struct->class transition
		public Version() { }

		public Version(int Major, int? Minor, int? Patch, int? Optional)
		{
			this.Major = Major;
			this.Minor = Minor;
			this.Patch = Patch;
			this.Optional = Optional;
		}

		public object Clone()
		{
			return new Version(this.Major, this.Minor, this.Patch, this.Optional);
		}

		public int CompareTo(object other)
		{
			if (other == null)
			{
				return 1;
			}

			Version otherVersion = other as Version;

			if (otherVersion == null)
			{
				// Not a Version object
				return 1;
			}

			if (otherVersion > this)
			{
				return -1;
			}
			else
			{
				return 1;
			}
		}

		public Version Next
		{
			get
			{
				Version result = this.Clone() as Version;

				if (Minor != null)
				{
					if (Patch != null)
					{
						if (Optional != null) { result.Optional += 1; } else { result.Patch += 1; }
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

		public static bool operator <(Version a, Version b)
		{
			if (a.Major != b.Major) return a.Major < b.Major;
			bool result = false;
			if (TryCompareInt(a.Minor, b.Minor, ref result)) return result;
			if (TryCompareInt(a.Patch, b.Patch, ref result)) return result;
			if (TryCompareInt(a.Optional, b.Optional, ref result)) return result;
			return false;
		}
		public static bool operator >(Version a, Version b) { return b < a; }
		public static bool operator <=(Version a, Version b) { return !(a > b); }
		public static bool operator >=(Version a, Version b) { return !(a < b); }

		public static bool operator ==(Version a, Version b)
		{

			// Null checking...
			if (ReferenceEquals(a, b))
			{
				return true;
			}
			if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
			{
				return false;
			}

			return (
				a.Major == b.Major &&
				a.Minor == b.Minor &&
				a.Patch == b.Patch &&
				a.Optional == b.Optional
			);
		}
		public static bool operator !=(Version a, Version b) { return !(a == b); }
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
			return this == (Version)o;
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
		bool IsMetBy(Version version);
		IVersionRequirement RestrictTo(IVersionRequirement other);
	}

	public static class VersionRequirementExtension
	{
		public static bool IsMetBy(this IVersionRequirement requirement, string version)
		{
			return requirement.IsMetBy(VersionParser.ParseIncompleteVersion(version));
		}
	}

	// When no version requirement is specified
	public class NoRequirement : IVersionRequirement
	{
		public bool IsMetBy(Version version) { return true; }
		public IVersionRequirement RestrictTo(IVersionRequirement other) { return other; }
		public override string ToString() { return ""; }
	}

	// When minimal+ is specified
	public class MinimalVersionRequirement : IVersionRequirement
	{
		public Version minimal;

		public MinimalVersionRequirement(string minimal) : this(VersionParser.ParseIncompleteVersion(minimal)) { }
		public MinimalVersionRequirement(Version minimal)
		{
			this.minimal = minimal;
		}

		public bool IsMetBy(Version version)
		{
			return version >= minimal;
		}

		public IVersionRequirement RestrictTo(IVersionRequirement other)
		{
			if (other is NoRequirement)
			{
				return other.RestrictTo(this);
			}
			else if (other is MinimalVersionRequirement)
			{
				return minimal >= (other as MinimalVersionRequirement).minimal ? this : other;
			}
			else if (other is RangeVersionRequirement)
			{
				if (minimal <= (other as RangeVersionRequirement).lower) return other;
				if (other.IsMetBy(minimal)) return new RangeVersionRequirement(
				   minimal,
				   (other as RangeVersionRequirement).upper);
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
	public class LoseVersionRequirement : RangeVersionRequirement
	{
		public LoseVersionRequirement(string stub) : this(VersionParser.ParseIncompleteVersion(stub)) { }
		public LoseVersionRequirement(Version stub) : base(stub, stub.Next) { }

		public override IVersionRequirement RestrictTo(IVersionRequirement other)
		{
			if (other is BoundedVersionRequirement)
			{
				if (IsMetBy((other as BoundedVersionRequirement).lower)) return other;
			}
			return base.RestrictTo(other);
		}

		public override string ToString()
		{
			return lower.ToString();
		}
	}

	public class BoundedVersionRequirement : RangeVersionRequirement
	{
		public BoundedVersionRequirement(string lowerBound) : this(VersionParser.ParseIncompleteVersion(lowerBound)) { }
		public BoundedVersionRequirement(Version lowerBound) : base(lowerBound, lowerBound.Next) { }

		public override bool IsMetBy(Version version)
		{
			return version > lower && version < upper;
		}

		public override IVersionRequirement RestrictTo(IVersionRequirement other)
		{
			if (other is LoseVersionRequirement)
			{
				return (other as LoseVersionRequirement).RestrictTo(this as BoundedVersionRequirement);
			}
			return base.RestrictTo(other);
		}

		public override string ToString()
		{
			return lower.ToString() + ".*";
		}
	}

	public class RangeVersionRequirement : IVersionRequirement
	{
		public Version lower;
		public Version upper;

		public RangeVersionRequirement(string lower, string upper) : this(
			VersionParser.ParseIncompleteVersion(lower),
			VersionParser.ParseIncompleteVersion(upper))
		{ }
		public RangeVersionRequirement(Version lower, Version upper)
		{
			if (lower >= upper) throw new ArgumentException("Upper version of a RangeVersionRequirement cannot be inferior to its lower version");
			this.lower = lower;
			this.upper = upper;
		}

		public virtual bool IsMetBy(Version version)
		{
			return version >= lower && version < upper;
		}

		public virtual IVersionRequirement RestrictTo(IVersionRequirement other)
		{
			if (other is NoRequirement || other is MinimalVersionRequirement)
			{
				return other.RestrictTo(this);
			}
			else if (other is RangeVersionRequirement)
			{
				var otherRange = other as RangeVersionRequirement;
				if (lower == otherRange.lower && upper == otherRange.upper) return this;
				// self include other?
				if (IsMetBy(otherRange.lower) && IsMetBy(otherRange.upper)) return other;
				// other include self?
				if (other.IsMetBy(lower) && other.IsMetBy(upper)) return this;
				// They are overlapping or not intersecting
				// overlap top?
				if (IsMetBy(otherRange.lower) && other.IsMetBy(upper)) return new RangeVersionRequirement(
				  otherRange.lower,
				  this.upper
			  );
				// overlap bottom?
				if (IsMetBy(otherRange.upper) && other.IsMetBy(lower)) return new RangeVersionRequirement(
				  this.lower,
				  otherRange.upper
			  );
			}
			else if (other is ExactVersionRequirement)
			{
				if (IsMetBy((other as ExactVersionRequirement).expected)) return other;
			}
			throw new IncompatibleRequirementException(this, other);
		}

		public override string ToString()
		{
			return lower.ToString() + "," + upper.ToString();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is RangeVersionRequirement)) return false;
			var otherRange = obj as RangeVersionRequirement;

			return upper == otherRange.upper && lower == otherRange.lower;
		}

		public override int GetHashCode()
		{
			return lower.GetHashCode() & upper.GetHashCode();
		}
	}

	public class ExactVersionRequirement : IVersionRequirement
	{
		public Version expected;

		public ExactVersionRequirement(string expected) : this(VersionParser.ParseIncompleteVersion(expected)) { }
		public ExactVersionRequirement(Version expected)
		{
			this.expected = expected;
		}

		public bool IsMetBy(Version version)
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
		public IncompatibleRequirementException(IVersionRequirement a, IVersionRequirement b) : this(string.Format("Requirements {0} and {1} are not compatible", a.ToString(), b.ToString())) { }
		public IncompatibleRequirementException(string format, params object[] args) : base(string.Format(format, args)) { }
		public IncompatibleRequirementException(string message, Exception innerException) : base(message, innerException) { }
		public IncompatibleRequirementException(string format, Exception innerException, params object[] args) : base(string.Format(format, args), innerException) { }
	}
	#endregion
}