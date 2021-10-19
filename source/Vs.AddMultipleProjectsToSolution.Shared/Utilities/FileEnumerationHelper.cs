using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vs.AddMultipleProjectsToSolution.Annotations;

namespace Vs.AddMultipleProjectsToSolution.Utilities
{
    public class FileEnumerationHelper : IFileEnumerationHelper
    {
        private readonly IDirectoryReader _DirectoryReader;

        public FileEnumerationHelper([NotNull] IDirectoryReader directoryReader)
        {
            _DirectoryReader = directoryReader ?? throw new ArgumentNullException(nameof(directoryReader));
        }

        public static FileEnumerationHelper Instance { get; }
            = new FileEnumerationHelper(DirectoryReader.Instance);

        public async Task<IEnumerable<string>> FindFilesAsync(
            string directoryPath,
            string[] searchPatterns,
            CancellationToken cancellationToken,
            IProgress<int> numberOfFoundFilesProgress)
        {
            var fileCount = new Counter();
            var task = Task.Run(() =>
            {
                if (!_DirectoryReader.Exists(directoryPath))
                {
                    return (IEnumerable<string>) new string[0];
                }

                return searchPatterns
                    .AsParallel()
                    .WithCancellation(cancellationToken)
                    .WithMergeOptions(ParallelMergeOptions.AutoBuffered)
                    .SelectMany(p => _DirectoryReader
                        .GetDirectoryFilesRecursive(directoryPath, p)
                        .Aggregate(new List<string>(), (list, file) =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            var currentCounter = fileCount.Add(1);
                            if (currentCounter % 5 == 0)
                            {
                                numberOfFoundFilesProgress.Report(currentCounter);
                            }

                            list.Add(file);
                            return list;
                        })
                    )
                    .ToArray();
            }, cancellationToken);
            var items = await task;
            numberOfFoundFilesProgress.Report(fileCount.Value);
            return items;
        }
    }
}