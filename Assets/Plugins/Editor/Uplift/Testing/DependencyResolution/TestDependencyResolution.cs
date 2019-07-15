using Uplift.Common;
using Uplift.Packages;
using Uplift.Schemas;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine;
using System.Text;
using System.Linq;

namespace Uplift.DependencyResolution
{
	[TestFixture]
	public class TestDependencyResolution
	{
		// TODO 
		// - Create dependency list / repo
		// - Test with twice the same requirement
		// - Test for cyclic dependencies
		// - Test when 1 node has 2 disctinct restrictor parents
		// 	  * 1 when 2 parents exist
		//		 * test when conflicting node has 2 parents
		//	  * 1 when 1 parent doesn't exist anymore
		// - Test exception handling

		static Dictionary<string, Upset[]> packages = new Dictionary<string, Upset[]>();

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

			//Package to test rewind
			Upset I110 = new Upset();
			I110.PackageName = "I";
			I110.PackageVersion = "1.1.0";

			Upset I150 = new Upset();
			I150.PackageName = "I";
			I150.PackageVersion = "1.5.0";

			Upset J110 = new Upset();
			J110.PackageName = "J";
			J110.PackageVersion = "1.1.0";
			J110.Dependencies = new DependencyDefinition[1];
			J110.Dependencies[0] = new DependencyDefinition();
			J110.Dependencies[0].Name = "K";
			J110.Dependencies[0].Version = "1.1.0+";

			Upset K110 = new Upset();
			K110.PackageName = "K";
			K110.PackageVersion = "1.1.0";
			K110.Dependencies = new DependencyDefinition[1];
			K110.Dependencies[0] = new DependencyDefinition();
			K110.Dependencies[0].Name = "H";
			K110.Dependencies[0].Version = "2.5.5+";

			Upset K150 = new Upset();
			K150.PackageName = "K";
			K150.PackageVersion = "1.5.0";
			K150.Dependencies = new DependencyDefinition[1];
			K150.Dependencies[0] = new DependencyDefinition();
			K150.Dependencies[0].Name = "I";
			K150.Dependencies[0].Version = "1.1.0";

			Upset L120 = new Upset();
			L120.PackageName = "L";
			L120.PackageVersion = "1.2.0";
			L120.Dependencies = new DependencyDefinition[1];
			L120.Dependencies[0] = new DependencyDefinition();
			L120.Dependencies[0].Name = "H";
			L120.Dependencies[0].Version = "2.5.5+";

			Upset L150 = new Upset();
			L150.PackageName = "L";
			L150.PackageVersion = "1.5.0";
			L150.Dependencies = new DependencyDefinition[1];
			L150.Dependencies[0] = new DependencyDefinition();
			L150.Dependencies[0].Name = "K";
			L150.Dependencies[0].Version = "1.5.0";

			Upset M110 = new Upset();
			M110.PackageName = "M";
			M110.PackageVersion = "1.1.0";
			M110.Dependencies = new DependencyDefinition[1];
			M110.Dependencies[0] = new DependencyDefinition();
			M110.Dependencies[0].Name = "J";
			M110.Dependencies[0].Version = "1.1.0+";

			Upset M150 = new Upset();
			M150.PackageName = "M";
			M150.PackageVersion = "1.5.0";
			M150.Dependencies = new DependencyDefinition[1];
			M150.Dependencies[0] = new DependencyDefinition();
			M150.Dependencies[0].Name = "L";
			M150.Dependencies[0].Version = "1.5.0";

			Upset N110 = new Upset();
			N110.PackageName = "N";
			N110.PackageVersion = "1.1.0";
			N110.Dependencies = new DependencyDefinition[1];
			N110.Dependencies[0] = new DependencyDefinition();
			N110.Dependencies[0].Name = "P";
			N110.Dependencies[0].Version = "1.1.0+";

			Upset O110 = new Upset();
			O110.PackageName = "O";
			O110.PackageVersion = "1.1.0";
			O110.Dependencies = new DependencyDefinition[1];
			O110.Dependencies[0] = new DependencyDefinition();
			O110.Dependencies[0].Name = "P";
			O110.Dependencies[0].Version = "1.2.0+";

			Upset P110 = new Upset();
			P110.PackageName = "P";
			P110.PackageVersion = "1.1.0";

			Upset P120 = new Upset();
			P120.PackageName = "P";
			P120.PackageVersion = "1.2.0";

			Upset P150 = new Upset();
			P150.PackageName = "P";
			P150.PackageVersion = "1.5.0";

			packages["A"] = new Upset[] { A110, A116, A120 };
			packages["B"] = new Upset[] { B110, B113 };
			packages["C"] = new Upset[] { C110, C116 };
			packages["D"] = new Upset[] { D110 };
			packages["E"] = new Upset[] { E110 };
			packages["F"] = new Upset[] { F110 };
			packages["G"] = new Upset[] { G110 };
			packages["H"] = new Upset[] { H255 };
			packages["I"] = new Upset[] { I110, I150 };
			packages["J"] = new Upset[] { J110 };
			packages["K"] = new Upset[] { K110, K150 };
			packages["L"] = new Upset[] { L120, L150 };
			packages["M"] = new Upset[] { M110, M150 };
			packages["N"] = new Upset[] { N110 };
			packages["O"] = new Upset[] { O110 };
			packages["P"] = new Upset[] { P110, P120, P150 };
		}
		Stack<DependencyDefinition> originalDependencies = new Stack<DependencyDefinition>();
		DependencyGraph baseGraph = new DependencyGraph();
		PackageListStub packageListStub;
		Resolver resolver;
		[SetUp]
		public void InitializationRepoStub()
		{
			Debug.Log("==== Initialization ====");
			Debug.Log("-> Filling repo");
			FillRepo();
			packageListStub = new PackageListStub(packages);
			resolver = new Resolver(baseGraph, packageListStub);
			Debug.Log(packageListStub);
		}

		[Test]
		public void NoRequirement()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Upset[] expected = { };
			Assert.IsTrue(CheckResolverResults(expected, resolver.SolveDependencies(originalDependencies.ToArray())));
		}

		#region No Dependency

		[Test]
		public void RequirementSingleWithNoDependency()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition B = new DependencyDefinition();
			B.Name = "B";
			B.Version = "1.1.0+";

			originalDependencies.Push(B);

			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Upset[] expected = { packages["B"][1] };//B113
			Assert.IsTrue(CheckResolverResults(expected, resolver.SolveDependencies(originalDependencies.ToArray())));
		}

		[Test]
		public void RequirementSeveralWithNoDependency()
		{
			originalDependencies = new Stack<DependencyDefinition>();
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

			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Upset[] expected = { packages["B"][1],	//B113
								 packages["C"][1],	//C116
								 packages["D"][0] };//D110
			Assert.IsTrue(CheckResolverResults(expected, resolver.SolveDependencies(originalDependencies.ToArray())));
		}

		#endregion

		#region With Dependencies
		[Test]
		public void RequirementSingleWithSingleDependency()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition A = new DependencyDefinition();
			A.Name = "A";
			A.Version = "1.1.0+";

			originalDependencies.Push(A);

			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Upset[] expected = { packages["A"][2],	 //A120
								 packages["C"][1] }; //C116
			Assert.IsTrue(CheckResolverResults(expected, resolver.SolveDependencies(originalDependencies.ToArray())));
		}
		[Test]
		public void RequirementSingleWithSeveralDependencies()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition E = new DependencyDefinition();
			E.Name = "E";
			E.Version = "1.1.0+";

			originalDependencies.Push(E);

			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Upset[] expected = { packages["A"][2],	//A120
								 packages["B"][1],	//B113
								 packages["C"][1],	//C116
								 packages["D"][0],	//D110
								 packages["E"][0] };//E110
			Assert.IsTrue(CheckResolverResults(expected, resolver.SolveDependencies(originalDependencies.ToArray())));
		}
		[Test]
		public void RequirementSeveralWithSeveralDependencies()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition F = new DependencyDefinition();
			F.Name = "F";
			F.Version = "1.1.0+";

			DependencyDefinition G = new DependencyDefinition();
			G.Name = "G";
			G.Version = "1.1.0+";

			originalDependencies.Push(F);
			originalDependencies.Push(G);

			Resolver resolver = new Resolver(baseGraph, packageListStub);

			Upset[] expected = { packages["A"][2],	//A120
								 packages["B"][1],	//B113
								 packages["C"][1],	//C116
								 packages["D"][0],	//D110
								 packages["F"][0],	//F110
								 packages["G"][0] };//G110
			Assert.IsTrue(CheckResolverResults(expected, resolver.SolveDependencies(originalDependencies.ToArray())));
		}
		#endregion

		#region Dependencies
		[Test]
		public void DependencyOnExactVersion()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition H = new DependencyDefinition();
			H.Name = "H";
			H.Version = "2.5.5";
			originalDependencies.Push(H);

			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Upset[] expected = { packages["H"][0] };  //H255
			Assert.IsTrue(CheckResolverResults(expected, resolver.SolveDependencies(originalDependencies.ToArray())));
		}

		[Test]
		public void DependencyOnLowerPatchVersion()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition H = new DependencyDefinition();
			H.Name = "H";
			H.Version = "2.5.3+";
			originalDependencies.Push(H);
			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Upset[] expected = { packages["H"][0] };  //H255
			Assert.IsTrue(CheckResolverResults(expected, resolver.SolveDependencies(originalDependencies.ToArray())));
		}

		[Test]
		public void DependencyOnUpperPatchVersion()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition H = new DependencyDefinition();
			H.Name = "H";
			H.Version = "2.5.7+";
			originalDependencies.Push(H);
			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Assert.DoesNotThrow(() => { resolver.SolveDependencies(originalDependencies.ToArray()); });
		}
		#endregion

		[Test]
		public void DependencyOnLowerMinorVersion()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition H = new DependencyDefinition();
			H.Name = "H";
			H.Version = "2.4.5+";
			originalDependencies.Push(H);

			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Upset[] expected = { packages["H"][0] };  //H255
			Assert.IsTrue(CheckResolverResults(expected, resolver.SolveDependencies(originalDependencies.ToArray())));
		}

		[Test]
		public void DependencyOnUpperMinorVersion()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition H = new DependencyDefinition();
			H.Name = "H";
			H.Version = "2.6.5+";
			originalDependencies.Push(H);

			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Assert.DoesNotThrow(() => { resolver.SolveDependencies(originalDependencies.ToArray()); });
		}

		[Test]
		public void DependencyOnLowerMajorVersion()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition H = new DependencyDefinition();
			H.Name = "H";
			H.Version = "1.5.5+";
			originalDependencies.Push(H);
			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Upset[] expected = { packages["H"][0] };  //H255
			Assert.IsTrue(CheckResolverResults(expected, resolver.SolveDependencies(originalDependencies.ToArray())));
		}

		[Test]
		public void DependencyOnUpperMajorVersion()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition H = new DependencyDefinition();
			H.Name = "H";
			H.Version = "3.5.5+";
			originalDependencies.Push(H);
			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Assert.DoesNotThrow(() => { resolver.SolveDependencies(originalDependencies.ToArray()); });
		}

		#region Conflict management and unwind
		[Test]
		public void UnwindSimple()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition I = new DependencyDefinition();
			I.Name = "I";
			I.Version = "1.5.0";

			DependencyDefinition J = new DependencyDefinition();
			J.Name = "J";
			J.Version = "1.1.0+";

			originalDependencies.Push(I);
			originalDependencies.Push(J);

			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Upset[] expected = {packages["I"][1],	//I150
								packages["J"][0],	//J110
								packages["K"][0],	//K110
								packages["H"][0]};  //H255

			Assert.IsTrue(CheckResolverResults(expected, resolver.SolveDependencies(originalDependencies.ToArray())));
		}

		[Test]
		public void UnwindSimpleOnParent()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition I = new DependencyDefinition();
			I.Name = "I";
			I.Version = "1.5.0";

			DependencyDefinition L = new DependencyDefinition();
			L.Name = "L";
			L.Version = "1.2.0+";

			originalDependencies.Push(L);
			originalDependencies.Push(I);

			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Upset[] expected = {packages["I"][1],	//I150
								packages["L"][0],	//L120
								packages["H"][0]};  //H255

			Assert.IsTrue(CheckResolverResults(expected, resolver.SolveDependencies(originalDependencies.ToArray())));
		}

		[Test]
		public void UnwindDouble()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition I = new DependencyDefinition();
			I.Name = "I";
			I.Version = "1.5.0";

			DependencyDefinition M = new DependencyDefinition();
			M.Name = "M";
			M.Version = "1.1.0+";

			originalDependencies.Push(M);
			originalDependencies.Push(I);

			Resolver resolver = new Resolver(baseGraph, packageListStub);

			Upset[] expected = {packages["I"][1],	//I150
								packages["J"][0],	//J110
								packages["K"][0],	//K110
								packages["H"][0],	//H255
								packages["M"][0]};  //M110

			Assert.IsTrue(CheckResolverResults(expected, resolver.SolveDependencies(originalDependencies.ToArray())));
		}

		#endregion

		[Test]
		public void PossibilitySetRestriction()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition N = new DependencyDefinition();
			N.Name = "N";
			N.Version = "1.1.0+";

			DependencyDefinition O = new DependencyDefinition();
			O.Name = "O";
			O.Version = "1.1.0+";

			originalDependencies.Push(N);
			originalDependencies.Push(O);

			Resolver resolver = new Resolver(baseGraph, packageListStub);

			Upset[] expected = { packages["N"][0],	//N110
								 packages["O"][0],	//O110
								 packages["P"][2] };//P150
			Assert.IsTrue(CheckResolverResults(expected, resolver.SolveDependencies(originalDependencies.ToArray())));
		}

		private bool CheckResolverResults(Upset[] expected, List<PackageRepo> results)
		{
			List<Upset> resultsUpset = new List<Upset>();
			resultsUpset = results.Select(repo => repo.Package).ToList();

			if (expected.Length == resultsUpset.Count)
			{
				return !resultsUpset.Any(pkg => !expected.Contains(pkg));
			}
			else
			{
				return false;
			}
		}
	}
}