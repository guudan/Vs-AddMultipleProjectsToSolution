using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;
using Vs.AddMultipleProjectsToSolution.Annotations;
using Vs.AddMultipleProjectsToSolution.Gui.Utilities;
using Vs.AddMultipleProjectsToSolution.Utilities;

namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public class LoadProjectsWindowViewModel : INotifyPropertyChanged, IObservableObject
    {
        private static readonly string[] DefaultProjectTypesExtensions =
        {
            "*.csproj",
            "*.fsproj",
            "*.vbproj",
            "*.shproj",
            "*.sqlproj"
        };

        private readonly IFileEnumerationHelper _FileEnumerationHelper;
        private readonly ObservableProperty<string> _FolderPath;
        private readonly ObservableProperty<bool> _IsLoading;
        private readonly ObservableProperty<string> _LoadingStatus;
        [NotNull] private readonly Action<IEnumerable<IFsItemViewModel>> _ProjectsLoadedCallback;
        private readonly ObservableProperty<string> _ProjectTypeExtensions;
        private readonly IMapper _ViewModelMapper;

        private readonly IWindowService _WindowService;
        private CancellationTokenSource _LoadingCancelationTokenSource;

        public LoadProjectsWindowViewModel(
            [NotNull] IWindowService windowService,
            [NotNull] Action<IEnumerable<IFsItemViewModel>> projectsLoadedCallback)
            : this(windowService, projectsLoadedCallback, FileEnumerationHelper.Instance, Mapper.Instance)
        {
        }

        public LoadProjectsWindowViewModel(
            [NotNull] IWindowService windowService,
            [NotNull] Action<IEnumerable<IFsItemViewModel>> projectsLoadedCallback,
            [NotNull] IFileEnumerationHelper fileEnumerationHelper,
            [NotNull] IMapper viewModelMapper)
        {
            _WindowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
            _ProjectsLoadedCallback =
                projectsLoadedCallback ?? throw new ArgumentNullException(nameof(projectsLoadedCallback));
            _FileEnumerationHelper =
                fileEnumerationHelper ?? throw new ArgumentNullException(nameof(fileEnumerationHelper));
            _ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            _FolderPath = new ObservableProperty<string>(this, string.Empty);
            _ProjectTypeExtensions = new ObservableProperty<string>(this, DefaultProjectTypeExtensionsString);
            _IsLoading = new ObservableProperty<bool>(this, false);
            _LoadingStatus = new ObservableProperty<string>(this, string.Empty);
            LoadProjectsCommand = new DelegateCommand(LoadProjects, o => CanExecuteLoadProjects);
            SelectFolderCommand = new DelegateCommand(SelectFolder);
            CancelCommand = new DelegateCommand(CloseLoadProjectsWindow);
            CancelLoadingCommand = new DelegateCommand(CancelLoading, o => IsLoading);
        }

        public ICommand CancelCommand { get; }
        public ICommand CancelLoadingCommand { get; }

        public bool CanExecuteLoadProjects =>
            !string.IsNullOrWhiteSpace(ProjectTypeExtensions) &&
            !string.IsNullOrWhiteSpace(FolderPath);

        public string FolderPath
        {
            get => _FolderPath.GetValue();
            set => _FolderPath.SetValue(value);
        }

        public bool IsLoading
        {
            get => _IsLoading.GetValue();
            set => _IsLoading.SetValue(value);
        }

        public string LoadingStatus
        {
            get => _LoadingStatus.GetValue();
            set => _LoadingStatus.SetValue(value);
        }

        public ICommand LoadProjectsCommand { get; }

        public string ProjectTypeExtensions
        {
            get => _ProjectTypeExtensions.GetValue();
            set => _ProjectTypeExtensions.SetValue(value);
        }

        public ICommand SelectFolderCommand { get; }

        private static string DefaultProjectTypeExtensionsString => string.Join("; ", DefaultProjectTypesExtensions);

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokeNotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CancelLoading(object obj)
        {
            _LoadingCancelationTokenSource?.Cancel();
            _WindowService.CloseLoadProjectsWindow();
        }

        private void CloseLoadProjectsWindow(object obj)
        {
            _WindowService.CloseLoadProjectsWindow();
        }

        private string[] GetProjectFilesExtensions()
        {
            return ProjectTypeExtensions
                .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();
        }

        private async void LoadProjects(object obj)
        {
            if (IsLoading)
            {
                throw new InvalidOperationException("Loading already in progress.");
            }

            _LoadingCancelationTokenSource = new CancellationTokenSource();
            try
            {
                LoadingStatus = "Searching for project files started.";
                IsLoading = true;
                var progressUpdater = new Progress<int>(numberOfFoundProjects =>
                {
                    LoadingStatus = $"Searching for project files. Already found {numberOfFoundProjects} projects.";
                });
                var projectFilesExtensions = GetProjectFilesExtensions();
                var folder = FolderPath;
                var files = await _FileEnumerationHelper.FindFilesAsync(
                    folder, projectFilesExtensions, _LoadingCancelationTokenSource.Token,
                    progressUpdater);

                LoadingStatus = "Searching completed. Preparing data for display.";
                var fsItemViewModels = await _ViewModelMapper
                    .MapFilesToViewModelAsync(folder, files, _LoadingCancelationTokenSource.Token);
                if (_LoadingCancelationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                _ProjectsLoadedCallback(fsItemViewModels);
                _WindowService.CloseLoadProjectsWindow();
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                IsLoading = false;
                _LoadingCancelationTokenSource.Dispose();
                _LoadingCancelationTokenSource = null;
            }
        }

        private void SelectFolder(object obj)
        {
            var selectedFolder = _WindowService.OpenOpenFolderDialog();
            if (!string.IsNullOrWhiteSpace(selectedFolder))
            {
                FolderPath = selectedFolder;
            }
        }
    }
}