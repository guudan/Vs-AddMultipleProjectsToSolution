using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public sealed class Mapper : IMapper
    {
        private Mapper()
        {
        }

        public static Mapper Instance { get; } = new Mapper();

        public Task<IFsItemViewModel[]> MapFilesToViewModelAsync(
            string rootDirectoryPath,
            IEnumerable<string> files,
            CancellationToken cancellationToken)
        {
            return Task.Run(() => MapFilesToViewModel(rootDirectoryPath, files, cancellationToken).ToArray(), cancellationToken);
        }

        private string GetFileNameWithoutExtension(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        private IEnumerable<IFsItemViewModel> MapFilesToViewModel(
            string rootDirectoryPath,
            IEnumerable<string> files,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(rootDirectoryPath))
            {
                throw new ArgumentException($"Value of the {nameof(rootDirectoryPath)} must be not empty string.",
                    nameof(rootDirectoryPath));
            }

            if (files == null) throw new ArgumentNullException(nameof(files));

            var directorySeparatorChars = new[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar};
            var parentPathParts =
                rootDirectoryPath.Split(directorySeparatorChars, StringSplitOptions.RemoveEmptyEntries);
            FsDirectoryViewModel parentDirectory = null;
            foreach (var filePath in files)
            {
                if (filePath == null)
                {
                    throw new ArgumentException("One of passed file paths is null.");
                }

                if (!filePath.StartsWith(rootDirectoryPath, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException($"Path {filePath} is not child of the {rootDirectoryPath}.");
                }

                var filePathParts = filePath
                    .Split(directorySeparatorChars, StringSplitOptions.RemoveEmptyEntries)
                    .Skip(parentPathParts.Length)
                    .ToArray();

                if (filePathParts.Length == 1)
                {
                    var fileName = GetFileNameWithoutExtension(filePathParts[0]);
                    yield return new FsProjectDirectoryViewModel(fileName, filePath);
                }
                else if (filePathParts.Length > 1)
                {
                    if (parentDirectory == null)
                    {
                        var parentDirectoryName = parentPathParts[parentPathParts.Length - 1];
                        parentDirectory = new FsDirectoryViewModel(parentDirectoryName);
                    }

                    var processedDirectoryParent = parentDirectory;
                    for (var i = 0; i < filePathParts.Length - 2; i++)
                    {
                        var subDirectoryName = filePathParts[i];
                        var subDirectory = processedDirectoryParent
                            .ChildItems
                            .OfType<FsDirectoryViewModel>()
                            .SingleOrDefault(d =>
                                string.Equals(d.Name, subDirectoryName, StringComparison.OrdinalIgnoreCase));
                        if (subDirectory == null)
                        {
                            subDirectory = new FsDirectoryViewModel(subDirectoryName);
                            processedDirectoryParent.ChildItems.Add(subDirectory);
                        }

                        processedDirectoryParent = subDirectory;
                    }

                    var projectFileName = GetFileNameWithoutExtension(filePathParts[filePathParts.Length - 1]);
                    var projectDirectory = new FsProjectDirectoryViewModel(projectFileName, filePath);
                    processedDirectoryParent.ChildItems.Add(projectDirectory);
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            if (parentDirectory != null)
            {
                yield return parentDirectory;
            }
        }
    }
}