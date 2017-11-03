using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Uplift.Common;

using System.Diagnostics;
using System.Text;
using System.Linq;

namespace BuildTool {

	public class BuildLibraryData {
		public string[] References;
		public string[] Files;
		public int SdkLevel;
		public string OutFile;
	}
}