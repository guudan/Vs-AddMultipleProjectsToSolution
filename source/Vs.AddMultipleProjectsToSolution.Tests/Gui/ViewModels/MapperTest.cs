using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vs.AddMultipleProjectsToSolution.Gui.ViewModels;
using Xunit;

namespace Vs.AddMultipleProjectsToSolution.Tests.Gui.ViewModels
{
    public class MapperTest
    {
        [Fact]
        public async Task MapFilesToViewModelAsync_ForGivenMultipleProjectPaths_CreatesTreeOfProjectDirectories()
        {
            var rootDirectory = @"c:\test\input_directory";
            var projectFiles = new
            {
                FirstProject = @"c:\test\input_directory\Project1.csproj",
                SecondProject =  @"c:\test\input_directory\ChildDirWith2Projects\Project2\Project2.csproj",
                ThirdProject = @"c:\test\input_directory\ChildDirWith2Projects\Project3\Project3.csproj",
                FourthProject = @"c:\test\input_directory\Project4\Project4.csproj",
                FifthProject = @"c:\test\input_directory\Project4\Project5.csproj",
                SixthProject = @"c:\test\input_directory\Project4\Project6\Project6"
            };
           
            var expectedObjectTree = new List<IFsItemViewModel>();
            expectedObjectTree.Add(new FsProjectDirectoryViewModel("Project1", projectFiles.FirstProject));

            var inputDirectory = new FsDirectoryViewModel("input_directory");

            var childDirWith2Projects = new FsDirectoryViewModel("ChildDirWith2Projects");
            childDirWith2Projects.ChildItems.Add(new FsProjectDirectoryViewModel("Project2", projectFiles.SecondProject));
            childDirWith2Projects.ChildItems.Add(new FsProjectDirectoryViewModel("Project3", projectFiles.ThirdProject));
            inputDirectory.ChildItems.Add(childDirWith2Projects);

            inputDirectory.ChildItems.Add(new FsProjectDirectoryViewModel("Project4", projectFiles.FourthProject));
            inputDirectory.ChildItems.Add(new FsProjectDirectoryViewModel("Project5", projectFiles.FifthProject));

            var project4Directory = new FsDirectoryViewModel("Project4");
            project4Directory.ChildItems.Add(new FsProjectDirectoryViewModel("Project6", projectFiles.SixthProject));
            inputDirectory.ChildItems.Add(project4Directory);

            expectedObjectTree.Add(inputDirectory);
            
            var projectList = new[]
            {
                projectFiles.FirstProject, projectFiles.SecondProject, projectFiles.ThirdProject,
                projectFiles.FourthProject, projectFiles.FifthProject, projectFiles.SixthProject
            };

            var mapper = Mapper.Instance;
            var resultObjectTree = await mapper.MapFilesToViewModelAsync(rootDirectory, projectList, CancellationToken.None);
            
            foreach (var expectedTreeItem in expectedObjectTree)
            {
                Assert.Contains(expectedTreeItem, resultObjectTree);
            }
            Assert.Equal(expectedObjectTree.Count, resultObjectTree.Length);
        }

        [Fact]
        public async Task MapFilesToViewModelAsync_FilePathEqualsRootDirectory_FileSkipped()
        {
            var rootDirectory = @"c:\test\input_directory";
            var mapper = Mapper.Instance;
            var resultingObjectTree =
                await mapper.MapFilesToViewModelAsync(rootDirectory, new[] {rootDirectory}, CancellationToken.None);
            Assert.Empty(resultingObjectTree);
        }

        [Fact]
        public async Task MapFilesToViewModelAsync_FilePathNull_ThrowsException()
        {
            var rootDirectory = @"c:\test\input_directory";
            var mapper = Mapper.Instance;
            await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                mapper.MapFilesToViewModelAsync(rootDirectory, new string[] { null }, CancellationToken.None));
        }

        [Fact]
        public async Task MapFilesToViewModelAsync_FilesListNull_ThrowsException()
        {
            var rootDirectory = @"c:\test\input_directory";
            var mapper = Mapper.Instance;
            await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                mapper.MapFilesToViewModelAsync(rootDirectory, null, CancellationToken.None));
        }

        [Fact]
        public async Task MapFilesToViewModelAsync_FileNotBelowRootFolder_ThrowsException()
        {
            var rootDirectory = @"c:\test\input_directory";
            var testFilePath = @"c:\test\some_other_directory\Project1.csproj";
            var mapper = Mapper.Instance;
            await Assert.ThrowsAnyAsync<ArgumentException>(()=>
                mapper.MapFilesToViewModelAsync(rootDirectory, new[] {testFilePath}, CancellationToken.None));
        }

        [Fact]
        public async Task MapFilesToViewModelAsync_RootFolderNull_ThrowsException()
        {
            var testFilePath = @"c:\test\some_other_directory\Project1.csproj";
            var mapper = Mapper.Instance;
            await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                mapper.MapFilesToViewModelAsync(null, new[] { testFilePath }, CancellationToken.None));
        }

        [Fact]
        public async Task MapFilesToViewModelAsync_RootFolderEmpty_ThrowsException()
        {
            var testFilePath = @"c:\test\some_other_directory\Project1.csproj";
            var mapper = Mapper.Instance;
            await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                mapper.MapFilesToViewModelAsync(string.Empty, new[] { testFilePath }, CancellationToken.None));
        }
    }
}
