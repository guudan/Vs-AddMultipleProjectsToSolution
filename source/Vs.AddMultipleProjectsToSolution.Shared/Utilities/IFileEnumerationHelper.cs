using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Vs.AddMultipleProjectsToSolution.Utilities
{
    public interface IFileEnumerationHelper
    {
        Task<IEnumerable<string>> FindFilesAsync(
            string directoryPath,
            string[] searchPatterns,
            CancellationToken cancellationToken,
            IProgress<int> numberOfFoundFilesProgress);
    }
}