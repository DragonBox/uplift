using System;
using System.IO;

namespace UpliftTesting.Helpers
{
    class Helper
    {
        public static string GetLocalXMLFile(string[] path)
        {
            string[] temp = new string[path.Length + 1];
            temp[0] = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            Array.Copy(path, 0, temp, 1, path.Length);

            return string.Join(Path.DirectorySeparatorChar.ToString(), temp);
        }
    }
}
