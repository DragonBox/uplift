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

			Upset Q100 = new Upset();
			Q100.PackageName = "Q";
			Q100.PackageVersion = "1.0.0";
			Q100.Dependencies = new DependencyDefinition[1];
			Q100.Dependencies[0] = new DependencyDefinition();
			Q100.Dependencies[0].Name = "H";
			Q100.Dependencies[0].Version = "2.5.5";

			Upset Q105 = new Upset();
			Q105.PackageName = "Q";
			Q105.PackageVersion = "1.0.5";

			Upset Q110 = new Upset();
			Q110.PackageName = "Q";
			Q110.PackageVersion = "1.1.0";
			Q110.Dependencies = new DependencyDefinition[1];
			Q110.Dependencies[0] = new DependencyDefinition();
			Q110.Dependencies[0].Name = "S";
			Q110.Dependencies[0].Version = "1.2.0+";

			Upset R100 = new Upset();
			R100.PackageName = "R";
			R100.PackageVersion = "1.0.0";
			R100.Dependencies = new DependencyDefinition[1];
			R100.Dependencies[0] = new DependencyDefinition();
			R100.Dependencies[0].Name = "I";
			R100.Dependencies[0].Version = "1.1.0";

			Upset R110 = new Upset();
			R110.PackageName = "R";
			R110.PackageVersion = "1.1.0";
			R110.Dependencies = new DependencyDefinition[1];
			R110.Dependencies[0] = new DependencyDefinition();
			R110.Dependencies[0].Name = "S";
			R110.Dependencies[0].Version = "1.5.0+";

			Upset S110 = new Upset();
			S110.PackageName = "S";
			S110.PackageVersion = "1.1.0";

			Upset S120 = new Upset();
			S120.PackageName = "S";
			S120.PackageVersion = "1.2.0";

			Upset S150 = new Upset();
			S150.PackageName = "S";
			S150.PackageVersion = "1.5.0";

			Upset T100 = new Upset();
			T100.PackageName = "T";
			T100.PackageVersion = "1.0.0";
			T100.Dependencies = new DependencyDefinition[1];
			T100.Dependencies[0] = new DependencyDefinition();
			T100.Dependencies[0].Name = "Q";
			T100.Dependencies[0].Version = "1.0.0+";

			Upset U100 = new Upset();
			U100.PackageName = "U";
			U100.PackageVersion = "1.0.0";
			U100.Dependencies = new DependencyDefinition[1];
			U100.Dependencies[0] = new DependencyDefinition();
			U100.Dependencies[0].Name = "Q";
			U100.Dependencies[0].Version = "1.0.0";

			Upset V100 = new Upset();
			V100.PackageName = "V";
			V100.PackageVersion = "1.0.0";
			V100.Dependencies = new DependencyDefinition[1];
			V100.Dependencies[0] = new DependencyDefinition();
			V100.Dependencies[0].Name = "W";
			V100.Dependencies[0].Version = "1.0.0";

			Upset W100 = new Upset();
			W100.PackageName = "W";
			W100.PackageVersion = "1.0.0";
			W100.Dependencies = new DependencyDefinition[1];
			W100.Dependencies[0] = new DependencyDefinition();
			W100.Dependencies[0].Name = "V";
			W100.Dependencies[0].Version = "1.0.0";

			Upset X100 = new Upset();
			X100.PackageName = "X";
			X100.PackageVersion = "1.0.0";
			X100.Dependencies = new DependencyDefinition[1];
			X100.Dependencies[0] = new DependencyDefinition();
			X100.Dependencies[0].Name = "S";
			X100.Dependencies[0].Version = "1.1.0";

			Upset Y100 = new Upset();
			Y100.PackageName = "Y";
			Y100.PackageVersion = "1.0.0";
			Y100.Dependencies = new DependencyDefinition[1];
			Y100.Dependencies[0] = new DependencyDefinition();
			Y100.Dependencies[0].Name = "S";
			Y100.Dependencies[0].Version = "1.2.0";

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
			packages["Q"] = new Upset[] { Q100, Q105, Q110 };
			packages["R"] = new Upset[] { R100, R110 };
			packages["S"] = new Upset[] { S110, S120, S150 };
			packages["T"] = new Upset[] { T100 };
			packages["U"] = new Upset[] { U100 };
			packages["V"] = new Upset[] { V100 };
			packages["W"] = new Upset[] { W100 };
			packages["X"] = new Upset[] { X100 };
			packages["Y"] = new Upset[] { Y100 };
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

		#region simple tests

		[Test]
		public void NoRequirement()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Upset[] expected = { };
			Assert.IsTrue(CheckResolverResults(expected));
		}
		[Test]
		public void InitWithPackageRepos()
		{
			DependencyDefinition[] originalDependencies = new DependencyDefinition[3];
			DependencyDefinition A = new DependencyDefinition();
			A.Name = "A";
			A.Version = "1.1.0+";
			DependencyDefinition B = new DependencyDefinition();
			B.Name = "B";
			B.Version = "1.1.0+";
			DependencyDefinition C = new DependencyDefinition();
			C.Name = "C";
			C.Version = "1.1.0+";

			Resolver resolver = new Resolver(baseGraph, packageListStub);

			originalDependencies[0] = A;
			originalDependencies[1] = B;
			originalDependencies[2] = C;

			List<PackageRepo> startingRepos = new List<PackageRepo>();

			startingRepos.AddRange(packageListStub.GetPackageRepo("A").Where(pr => pr.Package.PackageVersion == "1.1.0"));
			startingRepos.AddRange(packageListStub.GetPackageRepo("B").Where(pr => pr.Package.PackageVersion == "1.1.0"));
			startingRepos.AddRange(packageListStub.GetPackageRepo("C").Where(pr => pr.Package.PackageVersion == "1.1.0"));

			foreach (var item in startingRepos)
			{
				Debug.Log(item.Package.PackageName + " " + item.Package.PackageVersion);
			}

			Assert.IsTrue(CheckResolverResults(startingRepos.Select(pr => pr.Package).ToArray(),
											   resolver.SolveDependencies(originalDependencies, startingRepos.ToArray())));
		}
		[Test]
		public void TwiceTheSameRequirement()
		{
			originalDependencies = new Stack<DependencyDefinition>();

			DependencyDefinition H1 = new DependencyDefinition();
			H1.Name = "H";
			H1.Version = "2.5.5";

			DependencyDefinition H2 = new DependencyDefinition();
			H2.Name = "H";
			H2.Version = "2.5.5";

			originalDependencies.Push(H1);
			originalDependencies.Push(H2);

			Resolver resolver = new Resolver(baseGraph, packageListStub);

			Upset[] expected = { packages["H"][0] }; //H255

			Assert.Throws<IncompatibleRequirementException>(() => resolver.SolveDependencies(originalDependencies.ToArray()));
		}

		#endregion

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
			Assert.IsTrue(CheckResolverResults(expected));
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
			Assert.IsTrue(CheckResolverResults(expected));
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
			Assert.IsTrue(CheckResolverResults(expected));
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
			Assert.IsTrue(CheckResolverResults(expected));
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
			Assert.IsTrue(CheckResolverResults(expected));
		}
		#endregion

		#region Tweaking package versions
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
			Assert.IsTrue(CheckResolverResults(expected));
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
			Assert.IsTrue(CheckResolverResults(expected));
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
			Assert.Throws<IncompatibleRequirementException>(() => resolver.SolveDependencies(originalDependencies.ToArray()));
		}

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
			Assert.IsTrue(CheckResolverResults(expected));
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
			Assert.Throws<IncompatibleRequirementException>(() => resolver.SolveDependencies(originalDependencies.ToArray()));
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
			Assert.IsTrue(CheckResolverResults(expected));
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
			Assert.Throws<IncompatibleRequirementException>(() => resolver.SolveDependencies(originalDependencies.ToArray()));
		}

		#endregion

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

			Assert.IsTrue(CheckResolverResults(expected));
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

			Assert.IsTrue(CheckResolverResults(expected));
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

			Assert.IsTrue(CheckResolverResults(expected));
		}

		#endregion

		#region Node restrictors

		[Test]
		public void SeveralRestrictorOnNode()
		{
			originalDependencies = new Stack<DependencyDefinition>();
			DependencyDefinition Q = new DependencyDefinition();
			Q.Name = "Q";
			Q.Version = "1.1.0+";

			DependencyDefinition R = new DependencyDefinition();
			R.Name = "R";
			R.Version = "1.1.0+";

			originalDependencies.Push(Q);
			originalDependencies.Push(R);

			Resolver resolver = new Resolver(baseGraph, packageListStub);

			Upset[] expected = {packages["Q"][2],	//Q110
								packages["R"][1],	//R110
								packages["S"][2],	//S150
							   };

			Assert.IsTrue(CheckResolverResults(expected));
		}

		[Test]
		public void NodeWithDeletedRestrictor()
		{
			originalDependencies = new Stack<DependencyDefinition>();

			DependencyDefinition R = new DependencyDefinition();
			R.Name = "R";
			R.Version = "1.1.0+";

			DependencyDefinition Q = new DependencyDefinition();
			Q.Name = "Q";
			Q.Version = "1.0.0+";

			DependencyDefinition S = new DependencyDefinition();
			S.Name = "S";
			S.Version = "1.5.0+";

			DependencyDefinition U = new DependencyDefinition();
			U.Name = "U";
			U.Version = "1.0.0+";

			originalDependencies.Push(U);
			originalDependencies.Push(S);
			originalDependencies.Push(Q);
			originalDependencies.Push(R);

			Resolver resolver = new Resolver(baseGraph, packageListStub);

			Upset[] expected = {packages["Q"][0],	//Q105
								packages["R"][1],	//R110
								packages["S"][2],	//S150
							    packages["H"][0],	//H255
								packages["U"][0],	//U100
							   };

			Assert.IsTrue(CheckResolverResults(expected));
		}

		[Test]
		public void ConflictingNodesWithRestrictors()
		{
			originalDependencies = new Stack<DependencyDefinition>();

			DependencyDefinition S = new DependencyDefinition();
			S.Name = "S";
			S.Version = "1.1.0";

			DependencyDefinition Q = new DependencyDefinition();
			Q.Name = "Q";
			Q.Version = "1.0.0+";

			DependencyDefinition R = new DependencyDefinition();
			R.Name = "R";
			R.Version = "1.0.0+";

			originalDependencies.Push(S);
			originalDependencies.Push(Q);
			originalDependencies.Push(R);

			Resolver resolver = new Resolver(baseGraph, packageListStub);

			Upset[] expected = {packages["Q"][1],	//Q105
								packages["R"][0],	//R100
								packages["S"][0],	//S100
								packages["I"][0],	//I110
							   };

			Assert.IsTrue(CheckResolverResults(expected));

		}

		[Test]
		public void DoubleRestrictorOnNode()
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
			Assert.IsTrue(CheckResolverResults(expected));
		}

		#endregion

		#region cyclic dependencies
		[Test]
		public void ExplicitCyclicDependencies()
		{
			originalDependencies = new Stack<DependencyDefinition>();

			DependencyDefinition V = new DependencyDefinition();
			V.Name = "V";
			V.Version = "1.0.0";

			DependencyDefinition W = new DependencyDefinition();
			W.Name = "W";
			W.Version = "1.0.0";

			originalDependencies.Push(V);
			originalDependencies.Push(W);

			Resolver resolver = new Resolver(baseGraph, packageListStub);

			Upset[] expected = {packages["V"][0],	//V100
								packages["W"][0],	//W100
								};

			Assert.IsTrue(CheckResolverResults(expected));
		}

		[Test]
		public void BasicCyclicDependencies()
		{
			originalDependencies = new Stack<DependencyDefinition>();

			DependencyDefinition V = new DependencyDefinition();
			V.Name = "V";
			V.Version = "1.0.0";

			originalDependencies.Push(V);

			Resolver resolver = new Resolver(baseGraph, packageListStub);

			Upset[] expected = {packages["V"][0],	//V100
								packages["W"][0],	//W100
								};

			Assert.IsTrue(CheckResolverResults(expected));
		}
		#endregion

		#region Exception Handling

		[Test]
		public void IncompatibleOriginalDependencies()
		{
			originalDependencies = new Stack<DependencyDefinition>();

			DependencyDefinition S1 = new DependencyDefinition();
			S1.Name = "S";
			S1.Version = "1.1.0";

			DependencyDefinition S2 = new DependencyDefinition();
			S2.Name = "S";
			S2.Version = "1.2.0";

			originalDependencies.Push(S1);
			originalDependencies.Push(S2);

			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Assert.Throws<IncompatibleRequirementException>(() => resolver.SolveDependencies(originalDependencies.ToArray()));
		}

		[Test]
		public void IncompatibilityWithOrginalDependency()
		{
			originalDependencies = new Stack<DependencyDefinition>();

			DependencyDefinition S = new DependencyDefinition();
			S.Name = "S";
			S.Version = "1.1.0";

			DependencyDefinition R = new DependencyDefinition();
			R.Name = "R";
			R.Version = "1.1.0";

			originalDependencies.Push(S);
			originalDependencies.Push(R);

			Resolver resolver = new Resolver(baseGraph, packageListStub);
			//resolver.SolveDependencies(originalDependencies.ToArray());
			Assert.Throws<IncompatibleRequirementException>(() => resolver.SolveDependencies(originalDependencies.ToArray()));
		}

		[Test]
		public void IncompatibleDependencies()
		{
			originalDependencies = new Stack<DependencyDefinition>();

			DependencyDefinition X = new DependencyDefinition();
			X.Name = "X";
			X.Version = "1.0.0";

			DependencyDefinition Y = new DependencyDefinition();
			Y.Name = "Y";
			Y.Version = "1.0.0";

			originalDependencies.Push(X);
			originalDependencies.Push(Y);

			Resolver resolver = new Resolver(baseGraph, packageListStub);
			Assert.Throws<IncompatibleRequirementException>(() => resolver.SolveDependencies(originalDependencies.ToArray()));
		}

		#endregion
		private bool CheckResolverResults(Upset[] expected)
		{
			return CheckResolverResults(expected, resolver.SolveDependencies(originalDependencies.ToArray()));
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