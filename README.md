# Add Multiple Projects To Solution V2

[![Build Status](https://dev.azure.com/guudan/GithubBuildAndRelease/_apis/build/status/Build%20Vs.AddMultipleProjectsToSolution?branchName=master)](https://dev.azure.com/guudan/GithubBuildAndRelease/_build/latest?definitionId=3&branchName=master)

<!-- Update the VS Gallery link after you upload the VSIX-->

Download this extension from the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=MaciejGudanowicz.AddMultipleProjectsToSolutionV2).
<!--
or get the [CI build](http://vsixgallery.com/extension/E298F226-867E-47D5-B1EE-D9D1377E03A5/).
-->

---------------------------------------

Visual Studio extension that allows adding multiple existing projects into 
the solution. Optionally creates the solution folder structure reflecting 
to the physical hierarchy on disk.

See the [change log](CHANGELOG.md) for changes and road map.

## Features

- Add mutliple projects to solution.
	- Load projects from given directory (recursive search by file extension).
    - Select which projects to add to the solution.
    - Define if solution folders hierarchy should be also created.

### Add multiple projects to solution
To add existing projects to the solution:
1. Right click the Solution node in Solution Explorer select Add -> Multiple Projects

![Click Add Multiple Command](docs/img/ClickAddMultipleCommand.png)

2. Click Load Projects From Folder

![Click Load From Folder](docs/img/ClickLoadFromFolder.png)

3. Select directory and click Add

![Select Folder For Loading Projects](docs/img/SelectFolderForLoadingProjects.png)

4. Found project will be displayed in the folder hierarchy in which they are defined on disk.
	- Select projects that you want to add to the solution. 
    - Define if solution folders should be created.
    - Click Add.

![Select Projects To Add](docs/img/SelectProjectsToAdd.png)

5. Review the structure of projects that will be created and press Start.

![Add Progress Review Configuration](docs/img/AddProgressReviewConfiguration.png)

6. Check results. 

The state of the operation is indicated by the icon next to the project.
When you will hover with mause over the icon the details will be displayed.

On the bottom of the window total number of errors and processed projects is displayed.

![Add Progress Review Results](docs/img/AddProgressReviewResults.png)

7. The result of the operation.

If you want to add projects without solution folders check configuration in step 4.

![Add Result With Solution Folders](docs/img/AddResultWithSolutionFolders.png)

## Contribute
Check out the [contribution guidelines](CONTRIBUTING.md)
if you want to contribute to this project.

## License
[MIT License](LICENSE)