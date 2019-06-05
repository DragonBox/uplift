using Uplift.Common;
using Uplift.Packages;
using Uplift.Schemas;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine;

namespace Uplift.DependencyResolution
{
	[TestFixture]
	public class TestDependencyResolution
	{
		//Create dependency list / repo

		static Dictionary<string, string[]> packages = new Dictionary<string, string[]>();

		void FillDependencies()
		{
			DependencyDefinition A = new DependencyDefinition();
			A.Name = "A";
			A.Version = "1.1.5";
			DependencyDefinition B = new DependencyDefinition();
			B.Name = "B";
			B.Version = "1.1.3";
			DependencyDefinition C = new DependencyDefinition();
			C.Name = "C";
			C.Version = "1.1.6";
			DependencyDefinition D = new DependencyDefinition();
			D.Name = "D";
			D.Version = "1.1.0";

			originalDependencies.Add(A);
			originalDependencies.Add(B);
			originalDependencies.Add(C);
			originalDependencies.Add(D);
		}

		void FillRepo()
		{
			packages["A"] = new string[] { "1.1.0", "1.1.5", "1.2.0" };
			packages["B"] = new string[] { "1.1.0", "1.1.3" };
			packages["C"] = new string[] { "1.1.0", "1.1.6" };
			packages["D"] = new string[] { "1.1.0" };
		}

		/*
			packages : 
			- A 1.0
				- B 1.0+
			- A 1.5
				- B 1.2+
			- A 2.0
				- C 1.0+
			- B 1.0
			- B 1.3
			- C 1.0
			- C 1.6
			- D 1.0
		 */

		List<DependencyDefinition> originalDependencies = new List<DependencyDefinition>();

		DependencyGraph baseGraph = new DependencyGraph();
		PackageRepoStub packageRepoStub;

		[Test]
		public void TestInitialization()
		{

			Debug.Log("Filling repo");
			FillRepo();
			Debug.Log("Filling Dependencies");
			FillDependencies();
			packageRepoStub = new PackageRepoStub(packages);
			Debug.Log("Creating resolver");
			Resolver resolver = new Resolver(originalDependencies, baseGraph);
			resolver.packageRepoStub = packageRepoStub;
			Debug.Log("Pushing initial state");
			resolver.pushInitialState();

			Assert.IsTrue(true);
		}

		//Launch resolution
	}
}