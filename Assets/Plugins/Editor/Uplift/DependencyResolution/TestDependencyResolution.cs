using Uplift.Common;
using Uplift.Packages;
using Uplift.Schemas;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine;
using System.Text;

namespace Uplift.DependencyResolution
{
	[TestFixture]
	public class TestDependencyResolution
	{
		//Create dependency list / repo
		//TODO twice the same requirement
		static Dictionary<string, Upset[]> packages = new Dictionary<string, Upset[]>();

		//TODO Create function fulfillRequirements(originalDependencies, Upset[]) ==> To check algorithm
		void FillDependencies()
		{
			DependencyDefinition A = new DependencyDefinition();
			A.Name = "A";
			A.Version = "1.1.5+";
			DependencyDefinition B = new DependencyDefinition();
			B.Name = "B";
			B.Version = "1.1.3+";
			DependencyDefinition C = new DependencyDefinition();
			C.Name = "C";
			C.Version = "1.1.6+";
			DependencyDefinition D = new DependencyDefinition();
			D.Name = "D";
			D.Version = "1.1.0+";

			originalDependencies.Push(A);
			originalDependencies.Push(B);
			originalDependencies.Push(C);
			originalDependencies.Push(D);
		}

		/*	Packages : 
			- A 1.1.0
				- B 1.1.0+
			- A 1.5
				- B 1.1.2+
			- A 2.0
				- C 1.1.0+
			- B 1.1.0
			- B 1.1.3
			- C 1.1.0
			- C 1.1.6
			- D 1.1.0
		 */

		void FillRepo()
		{
			Upset A110 = new Upset();
			A110.PackageName = "A";
			A110.PackageVersion = "1.1.0";
			A110.Dependencies = new DependencyDefinition[1];
			A110.Dependencies[0] = new DependencyDefinition();
			A110.Dependencies[0].Name = "B";
			A110.Dependencies[0].Version = "1.1.0+";

			Upset A116 = new Upset();
			A116.PackageName = "A";
			A116.PackageVersion = "1.1.6";
			A116.Dependencies = new DependencyDefinition[1];
			A116.Dependencies[0] = new DependencyDefinition();
			A116.Dependencies[0].Name = "B";
			A116.Dependencies[0].Version = "1.1.2+";

			Upset A120 = new Upset();
			A120.PackageName = "A";
			A120.PackageVersion = "1.2.0";
			A120.Dependencies = new DependencyDefinition[1];
			A120.Dependencies[0] = new DependencyDefinition();
			A120.Dependencies[0].Name = "C";
			A120.Dependencies[0].Version = "1.1.0+";

			Upset B110 = new Upset();
			B110.PackageName = "B";
			B110.PackageVersion = "1.1.0";

			Upset B113 = new Upset();
			B113.PackageName = "B";
			B113.PackageVersion = "1.1.3";

			Upset C110 = new Upset();
			C110.PackageName = "C";
			C110.PackageVersion = "1.1.0";

			Upset C116 = new Upset();
			C116.PackageName = "C";
			C116.PackageVersion = "1.1.6";

			Upset D110 = new Upset();
			D110.PackageName = "D";
			D110.PackageVersion = "1.1.0";

			Upset E110 = new Upset();
			E110.PackageName = "E";
			E110.PackageVersion = "1.1.0";
			E110.Dependencies = new DependencyDefinition[4];
			E110.Dependencies[0] = new DependencyDefinition();
			E110.Dependencies[0].Name = "A";
			E110.Dependencies[0].Version = "1.1.0+";
			E110.Dependencies[1] = new DependencyDefinition();
			E110.Dependencies[1].Name = "B";
			E110.Dependencies[1].Version = "1.1.0+";
			E110.Dependencies[2] = new DependencyDefinition();
			E110.Dependencies[2].Name = "C";
			E110.Dependencies[2].Version = "1.1.0+";
			E110.Dependencies[3] = new DependencyDefinition();
			E110.Dependencies[3].Name = "D";
			E110.Dependencies[3].Version = "1.1.0+";

			Upset F110 = new Upset();
			F110.PackageName = "F";
			F110.PackageVersion = "1.1.0";
			F110.Dependencies = new DependencyDefinition[2];
			F110.Dependencies[0] = new DependencyDefinition();
			F110.Dependencies[0].Name = "A";
			F110.Dependencies[0].Version = "1.1.0+";
			F110.Dependencies[1] = new DependencyDefinition();
			F110.Dependencies[1].Name = "B";
			F110.Dependencies[1].Version = "1.1.0+";

			Upset G110 = new Upset();
			G110.PackageName = "G";
			G110.PackageVersion = "1.1.0";
			G110.Dependencies = new DependencyDefinition[2];
			G110.Dependencies[0] = new DependencyDefinition();
			G110.Dependencies[0].Name = "C";
			G110.Dependencies[0].Version = "1.1.0+";
			G110.Dependencies[1] = new DependencyDefinition();
			G110.Dependencies[1].Name = "D";
			G110.Dependencies[1].Version = "1.1.0+";

			Upset H255 = new Upset();
			H255.PackageName = "H";
			H255.PackageVersion = "2.5.5";

			packages["A"] = new Upset[] { A110, A116, A120 };
			packages["B"] = new Upset[] { B110, B113 };
			packages["C"] = new Upset[] { C110, C116 };
			packages["D"] = new Upset[] { D110 };
			packages["E"] = new Upset[] { E110 };
			packages["F"] = new Upset[] { F110 };
			packages["G"] = new Upset[] { G110 };
			packages["H"] = new Upset[] { H255 };

		}
		Stack<DependencyDefinition> originalDependencies = new Stack<DependencyDefinition>();
		DependencyGraph baseGraph = new DependencyGraph();
		PackageRepoStub packageRepoStub;

		[SetUp]
		public void InitializationRepoStub()
		{
			Debug.Log("==== Initialization ====");
			Debug.Log("-> Filling repo");
			FillRepo();
			packageRepoStub = new PackageRepoStub(packages);
			Debug.Log(packageRepoStub);
		}

		[Test]
		public void TestInitialization()
		{
			Debug.Log("-> Filling Original Dependencies");
			FillDependencies();

			Debug.Log("-> Creating resolver");
			Resolver resolver = new Resolver(originalDependencies, baseGraph);
			resolver.packageRepoStub = packageRepoStub;
			Debug.Log("=================");
			Assert.DoesNotThrow(() => { resolver.SolveDependencies(); });
		}

		[Test]
		public void NoRequirement()
		{
			Resolver resolver = new Resolver(originalDependencies, baseGraph);
			resolver.packageRepoStub = packageRepoStub;

			Assert.DoesNotThrow(() => { resolver.SolveDependencies(); });
		}

		#region No Dependency

		[Test]
		public void SingleRequirementWithNoDependency()
		{
			DependencyDefinition B = new DependencyDefinition();
			B.Name = "B";
			B.Version = "1.1.0+";

			originalDependencies.Push(B);

			Resolver resolver = new Resolver(originalDependencies, baseGraph);
			resolver.packageRepoStub = packageRepoStub;

			Assert.DoesNotThrow(() => { resolver.SolveDependencies(); });
		}

		[Test]
		public void SeveralRequirementWithNoDependency()
		{
			DependencyDefinition B = new DependencyDefinition();
			B.Name = "B";
			B.Version = "1.1.0+";

			DependencyDefinition C = new DependencyDefinition();
			C.Name = "C";
			C.Version = "1.1.0+";

			DependencyDefinition D = new DependencyDefinition();
			D.Name = "D";
			D.Version = "1.1.0+";

			originalDependencies.Push(B);
			originalDependencies.Push(C);
			originalDependencies.Push(D);

			Resolver resolver = new Resolver(originalDependencies, baseGraph);
			resolver.packageRepoStub = packageRepoStub;

			Assert.DoesNotThrow(() => { resolver.SolveDependencies(); });
		}

		#endregion

		#region With Dependencies
		[Test]
		public void SingleRequirementWithSingleDependency()
		{
			DependencyDefinition A = new DependencyDefinition();
			A.Name = "A";
			A.Version = "1.1.0+";

			originalDependencies.Push(A);

			Resolver resolver = new Resolver(originalDependencies, baseGraph);
			resolver.packageRepoStub = packageRepoStub;

			Assert.DoesNotThrow(() => { resolver.SolveDependencies(); });
		}
		[Test]
		public void SingleRequirementWithSeveralDependencies()
		{
			DependencyDefinition E = new DependencyDefinition();
			E.Name = "E";
			E.Version = "1.1.0+";

			originalDependencies.Push(E);

			Resolver resolver = new Resolver(originalDependencies, baseGraph);
			resolver.packageRepoStub = packageRepoStub;

			Assert.DoesNotThrow(() => { resolver.SolveDependencies(); });
		}
		[Test]
		public void SeveralRequirementWithSeveralDependencies()
		{
			DependencyDefinition F = new DependencyDefinition();
			F.Name = "F";
			F.Version = "1.1.0+";

			DependencyDefinition G = new DependencyDefinition();
			G.Name = "G";
			G.Version = "1.1.0+";

			originalDependencies.Push(F);
			originalDependencies.Push(G);

			Resolver resolver = new Resolver(originalDependencies, baseGraph);
			resolver.packageRepoStub = packageRepoStub;

			Assert.DoesNotThrow(() => { resolver.SolveDependencies(); });
		}
		#endregion

		#region Dependencies
		[Test]
		public void DependencyOnExactVersion()
		{
			DependencyDefinition H = new DependencyDefinition();
			H.Name = "H";
			H.Version = "2.5.5";
			originalDependencies.Push(H);

			Resolver resolver = new Resolver(originalDependencies, baseGraph);
			resolver.packageRepoStub = packageRepoStub;

			Assert.DoesNotThrow(() => { resolver.SolveDependencies(); });
		}

		[Test]
		public void DependencyOnLowerPatchVersion()
		{
			DependencyDefinition H = new DependencyDefinition();
			H.Name = "H";
			H.Version = "2.5.3+";
			originalDependencies.Push(H);

			Resolver resolver = new Resolver(originalDependencies, baseGraph);
			resolver.packageRepoStub = packageRepoStub;

			Assert.DoesNotThrow(() => { resolver.SolveDependencies(); });
		}

		[Test]
		public void DependencyOnUpperPatchVersion()
		{
			DependencyDefinition H = new DependencyDefinition();
			H.Name = "H";
			H.Version = "2.5.7+";
			originalDependencies.Push(H);

			Resolver resolver = new Resolver(originalDependencies, baseGraph);
			resolver.packageRepoStub = packageRepoStub;

			Assert.DoesNotThrow(() => { resolver.SolveDependencies(); });
		}
		#endregion

		[Test]
		public void DependencyOnLowerMinorVersion()
		{
			DependencyDefinition H = new DependencyDefinition();
			H.Name = "H";
			H.Version = "2.4.5+";
			originalDependencies.Push(H);

			Resolver resolver = new Resolver(originalDependencies, baseGraph);
			resolver.packageRepoStub = packageRepoStub;

			Assert.DoesNotThrow(() => { resolver.SolveDependencies(); });
		}

		[Test]
		public void DependencyOnUpperMinorVersion()
		{
			DependencyDefinition H = new DependencyDefinition();
			H.Name = "H";
			H.Version = "2.6.5+";
			originalDependencies.Push(H);

			Resolver resolver = new Resolver(originalDependencies, baseGraph);
			resolver.packageRepoStub = packageRepoStub;

			Assert.DoesNotThrow(() => { resolver.SolveDependencies(); });
		}

		[Test]
		public void DependencyOnLowerMajorVersion()
		{
			DependencyDefinition H = new DependencyDefinition();
			H.Name = "H";
			H.Version = "1.5.5+";
			originalDependencies.Push(H);

			Resolver resolver = new Resolver(originalDependencies, baseGraph);
			resolver.packageRepoStub = packageRepoStub;

			Assert.DoesNotThrow(() => { resolver.SolveDependencies(); });
		}

		[Test]
		public void DependencyOnUpperMajorVersion()
		{
			DependencyDefinition H = new DependencyDefinition();
			H.Name = "H";
			H.Version = "3.5.5+";
			originalDependencies.Push(H);

			Resolver resolver = new Resolver(originalDependencies, baseGraph);
			resolver.packageRepoStub = packageRepoStub;

			Assert.DoesNotThrow(() => { resolver.SolveDependencies(); });
		}
	}
}