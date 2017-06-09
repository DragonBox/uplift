using System;
using System.IO;

namespace Uplift.Common
{
    public class TemporaryDirectory : IDisposable {

        public readonly string Path;
        protected bool disposed;

        public TemporaryDirectory() {
            System.IO.Path.GetTempPath();
            string dirname = System.IO.Path.GetTempFileName();
            File.Delete(dirname);
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), dirname);
            Directory.CreateDirectory(Path);
        }

        ~ TemporaryDirectory()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                Directory.Delete(Path, true);
                disposed = true;
            }
        }
    }
}