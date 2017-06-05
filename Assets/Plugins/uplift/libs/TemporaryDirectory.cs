using System.IO;
public class TemporaryDirectory {

    public readonly string Path;

    public TemporaryDirectory() {
        System.IO.Path.GetTempPath();
        string dirname = System.IO.Path.GetTempFileName();
        File.Delete(dirname);
        this.Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), dirname);
        Directory.CreateDirectory(Path);
    }

    public void Destroy() {
        Directory.Delete(Path, true);
    }
}