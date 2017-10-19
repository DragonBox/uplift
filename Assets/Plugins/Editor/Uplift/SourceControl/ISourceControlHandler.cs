namespace Uplift.SourceControl
{
    public interface ISourceControlHandler
    {
        void HandleDirectory(string pathToDirectory);
        void HandleFile(string pathToFile);
    }
}