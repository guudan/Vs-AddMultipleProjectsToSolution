using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;
using Vs.AddMultipleProjectsToSolution.Annotations;
using Vs.AddMultipleProjectsToSolution.Gui.Utilities;
using Vs.AddMultipleProjectsToSolution.Utilities;

namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public class AddMultipleProjectsConfigurationViewModel : IObservableObject
    {
        private readonly ObservableProperty<bool> _AreThereAnyProjectsLoaded;
        private readonly ObservableProperty<bool?> _CreateSolutionFolders;
        private readonly INVsSolution _Solution;
        private readonly IWindowService _WindowService;

        public AddMultipleProjectsConfigurationViewModel(
            [NotNull] IWindowService windowService,
            [NotNull] INVsSolution solution)
        {
            _WindowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
            _Solution = solution ?? throw new ArgumentNullException(nameof(solution));
            _CreateSolutionFolders = new ObservableProperty<bool?>(this, true);
            _AreThereAnyProjectsLoaded = new ObservableProperty<bool>(this);
            AddProjectsCommand = new DelegateCommand(AddProjects, arg => AreThereAnyProjectsLoaded);
            LoadProjectsCommand = new DelegateCommand(OpenLoadProjectsWindow);
            CancelCommand = new DelegateCommand(CloseAddMultipleProjectsWindow);
            ChangeCreateSolutionFoldersCommand = new DelegateCommand(ChangeCreateSolutionFolders);
        }

        public ICommand AddProjectsCommand { get; }

        public bool AreThereAnyProjectsLoaded
        {
            get => _AreThereAnyProjectsLoaded.GetValue();
            set => _AreThereAnyProjectsLoaded.SetValue(value);
        }

        public ICommand CancelCommand { get; }
        public ICommand ChangeCreateSolutionFoldersCommand { get; }

        public bool? CreateSolutionFolders
        {
            get => _CreateSolutionFolders.GetValue();
            set => _CreateSolutionFolders.SetValue(value);
        }

        public ICommand LoadProjectsCommand { get; }

        public ObservableCollection<IFsItemViewModel> ProjectTree { get; } =
            new ObservableCollection<IFsItemViewModel>();

        private IEnumerable<IFsItemViewModel> AllItems =>
            ProjectTree.Concat(ProjectTree.SelectMany(i => i.GetAllChildFileSystemItems()));

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokeNotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddProjects(object obj)
        {
            var solutionItemHierarchyBuilder = new FsItemBuildSolutionItemHierarchyVisitor();
            var solutionItemHierarchy = solutionItemHierarchyBuilder.BuildSolutionItemHierarchy(ProjectTree);
            var addProjectsProgress =
                new AddMultipleProjectsProgressViewModel(_WindowService, _Solution, this, solutionItemHierarchy);
            _WindowService.SetAddMultipleProjectsWindowContent(addProjectsProgress);
        }

        private void ChangeCreateSolutionFolders(object obj)
        {
            var newValue = CreateSolutionFolders != true;
            SetCreateSolutionFoldersValue(newValue);
        }

        private void CloseAddMultipleProjectsWindow(object obj)
        {
            _WindowService.CloseAddMultipleProjectsWindow();
        }

        private void OpenLoadProjectsWindow(object obj)
        {
            var addProjectsFolderViewModel = new LoadProjectsWindowViewModel(_WindowService, ProjectsLoadedCallback);
            _WindowService.OpenLoadProjectsWindow(addProjectsFolderViewModel);
        }

        private void ProjectsLoadedCallback(IEnumerable<IFsItemViewModel> projectsTree)
        {
            ProjectTree.Clear();
            foreach (var fileSystemItem in projectsTree)
            {
                ProjectTree.Add(fileSystemItem);
            }

            AreThereAnyProjectsLoaded = ProjectTree.Any();

            var createSolutionFolderValue = CreateSolutionFolders ?? true;
            SetCreateSolutionFoldersValue(createSolutionFolderValue);
        }

        private void SetCreateSolutionFoldersValue(bool newValue)
        {
            CreateSolutionFolders = newValue;
            var changeItemsVisitor = new FsItemChangeCreateSolutionFolderVisitor(newValue);
            foreach (var fileSystemItem in AllItems)
            {
                fileSystemItem.Accept(changeItemsVisitor);
            }
        }
    }
}