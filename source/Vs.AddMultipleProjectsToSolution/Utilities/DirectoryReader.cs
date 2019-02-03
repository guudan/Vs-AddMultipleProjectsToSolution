using System.Collections.Generic;
using System.IO;

namespace Vs.AddMultipleProjectsToSolution.Utilities
{
    public sealed class DirectoryReader : IDirectoryReader
    {
        private DirectoryReader()
        {
        }

        public static DirectoryReader Instance { get; } = new DirectoryReader();

        public bool Exists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public IEnumerable<string> GetDirectoryFilesRecursive(string directoryPath, string searchPattern)
        {
            return Directory.EnumerateFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
        }
    }
}