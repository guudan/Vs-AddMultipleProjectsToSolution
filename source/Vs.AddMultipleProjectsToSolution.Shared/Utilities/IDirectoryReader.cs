using System.Collections.Generic;

namespace Vs.AddMultipleProjectsToSolution.Utilities
{
    public interface IDirectoryReader
    {
        bool Exists(string directoryPath);
        IEnumerable<string> GetDirectoryFilesRecursive(string directoryPath, string searchPattern);
    }
}