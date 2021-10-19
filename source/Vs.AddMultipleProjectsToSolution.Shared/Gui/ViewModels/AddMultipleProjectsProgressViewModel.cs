using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.VisualStudio.PlatformUI;
using Vs.AddMultipleProjectsToSolution.Annotations;
using Vs.AddMultipleProjectsToSolution.Gui.Utilities;
using Vs.AddMultipleProjectsToSolution.Utilities;

namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public partial class AddMultipleProjectsProgressViewModel : IObservableObject
    {
        private readonly ObservableProperty<bool> _AreThereAnyProjectsSelected;
        private readonly AddMultipleProjectsConfigurationViewModel _ConfigurationViewModel;
        private readonly ObservableProperty<bool> _HasProjectAddCompleted;
        private readonly ObservableProperty<int> _NumberOfCreatedProjects;
        private readonly ObservableProperty<int> _NumberOfCreatedSolutionFolders;
        private readonly ObservableProperty<int> _NumberOfCreatedSolutionItems;
        private readonly ObservableProperty<int> _NumberOfErrors;
        [NotNull] private readonly INVsSolution _Solution;
        private readonly ObservableProperty<bool> _WasProjectAddStarted;
        private readonly IWindowService _WindowService;
        private CancellationTokenSource _AddOperationCancellationTokenSource;

        public AddMultipleProjectsProgressViewModel(
            [NotNull] IWindowService windowService,
            [NotNull] INVsSolution solution,
            AddMultipleProjectsConfigurationViewModel configurationViewModel,
            [NotNull] SolutionItemHierarchy solutionItemHierarchy)
        {
            if (solutionItemHierarchy == null) throw new ArgumentNullException(nameof(solutionItemHierarchy));
            _WindowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
            _Solution = solution ?? throw new ArgumentNullException(nameof(solution));
            _ConfigurationViewModel = configurationViewModel;
            SolutionItemHierarchy =
                solutionItemHierarchy.SolutionItems;
            SolutionItemCount = solutionItemHierarchy.NumberOfItemsToCreate;
            _HasProjectAddCompleted = new ObservableProperty<bool>(this);
            _WasProjectAddStarted = new ObservableProperty<bool>(this);
            _AreThereAnyProjectsSelected =
                new ObservableProperty<bool>(this, solutionItemHierarchy.SolutionItems.Any());
            _NumberOfCreatedProjects = new ObservableProperty<int>(this);
            _NumberOfCreatedSolutionFolders = new ObservableProperty<int>(this);
            _NumberOfCreatedSolutionItems = new ObservableProperty<int>(this);
            _NumberOfErrors = new ObservableProperty<int>(this);
            StartCommand = new DelegateCommand(Start, arg => AreThereAnyProjectsSelected);
            CancelCommand = new DelegateCommand(Cancel);
            CloseCommand = new DelegateCommand(Close, arg => HasProjectAddCompleted);
            GoBackToConfigurationCommand = new DelegateCommand(GoBackToConfiguration);
        }

        public bool AreThereAnyProjectsSelected
        {
            get => _AreThereAnyProjectsSelected.GetValue();
            set => _AreThereAnyProjectsSelected.SetValue(value);
        }

        public ICommand CancelCommand { get; }
        public ICommand CloseCommand { get; }

        public ICommand GoBackToConfigurationCommand { get; }

        public bool HasProjectAddCompleted
        {
            get => _HasProjectAddCompleted.GetValue();
            set => _HasProjectAddCompleted.SetValue(value);
        }

        public int NumberOfCreatedProjects
        {
            get => _NumberOfCreatedProjects.GetValue();
            set => _NumberOfCreatedProjects.SetValue(value);
        }

        public int NumberOfCreatedSolutionFolders
        {
            get => _NumberOfCreatedSolutionFolders.GetValue();
            set => _NumberOfCreatedSolutionFolders.SetValue(value);
        }

        public int NumberOfCreatedSolutionItems
        {
            get => _NumberOfCreatedSolutionItems.GetValue();
            set => _NumberOfCreatedSolutionItems.SetValue(value);
        }

        public int NumberOfErrors
        {
            get => _NumberOfErrors.GetValue();
            set => _NumberOfErrors.SetValue(value);
        }

        public SolutionItemsCount SolutionItemCount { get; }


        public IList<ISolutionItemViewModel> SolutionItemHierarchy { get; }

        public ICommand StartCommand { get; }

        public bool WasProjectAddStarted
        {
            get => _WasProjectAddStarted.GetValue();
            set => _WasProjectAddStarted.SetValue(value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokeNotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Cancel(object obj)
        {
            if (WasProjectAddStarted)
            {
                _AddOperationCancellationTokenSource?.Cancel();
            }
            else
            {
                _WindowService.CloseAddMultipleProjectsWindow();
            }
        }

        private void Close(object obj)
        {
            _WindowService.CloseAddMultipleProjectsWindow();
        }

        private INVsProjectHierarchy CreateDirectory(
            INVsProjectHierarchy parent, SolutionDirectoryViewModel directory)
        {
            // TODO: Add logging of the error to the output window
            try
            {
                directory.CreateStatus = SolutionItemCreateStatus.InProgress;
                var solutionFolder =
                    parent.GetSolutionFolder(directory.Name) ??
                    parent.AddSolutionFolder(directory.Name);
                directory.CreateStatus = SolutionItemCreateStatus.Added;
                return solutionFolder;
            }
            catch (Exception e)
            {
                directory.CreateStatus = SolutionItemCreateStatus.Failed;
                directory.CreateStatusMessage = e.Message;
                NumberOfErrors++;
                return null;
            }
            finally
            {
                NumberOfCreatedSolutionFolders++;
                NumberOfCreatedSolutionItems++;
            }
        }

        private INVsProject CreateProject(INVsProjectHierarchy parent, SolutionProjectViewModel project)
        {
            // TODO: Add logging of the error to the output window
            try
            {
                project.CreateStatus = SolutionItemCreateStatus.InProgress;
                var solutionProject =
                    parent.GetProjectByFilePath(project.ProjectFilePath);
                if (solutionProject == null)
                {
                    try
                    {
                        solutionProject = parent.AddExistingProject(project.ProjectFilePath);
                    }
                    catch
                    {
                        var projectInDifferentLocation =
                            _Solution.GetProjectFromAnywhereInSolution(project.ProjectFilePath);
                        projectInDifferentLocation?.Remove();
                        solutionProject = parent.AddExistingProject(project.ProjectFilePath);
                    }
                }

                project.CreateStatus = SolutionItemCreateStatus.Added;
                return solutionProject;
            }
            catch (Exception e)
            {
                project.CreateStatus = SolutionItemCreateStatus.Failed;
                project.CreateStatusMessage = e.Message;
                NumberOfErrors++;
                return null;
            }
            finally
            {
                NumberOfCreatedProjects++;
                NumberOfCreatedSolutionItems++;
            }
        }

        private void FillProcessingStack(
            Stack<ProcessingContext> itemsToProcess,
            INVsProjectHierarchy parentItem,
            IEnumerable<ISolutionItemViewModel> solutionItems)
        {
            foreach (var solutionItem in solutionItems)
            {
                itemsToProcess.Push(new ProcessingContext(parentItem, solutionItem));
            }
        }

        private void GoBackToConfiguration(object obj)
        {
            _WindowService.SetAddMultipleProjectsWindowContent(_ConfigurationViewModel);
        }

        private async Task PerformProcessingAsync(CancellationToken cancellationToken)
        {
            var itemsToProcess = new Stack<ProcessingContext>();
            FillProcessingStack(itemsToProcess, _Solution, SolutionItemHierarchy);

            while (itemsToProcess.Any())
            {
                var item = itemsToProcess.Pop();
                switch (item.SolutionItem)
                {
                    case SolutionDirectoryViewModel directory:
                        var createdSolutionFolder = CreateDirectory(item.Parent, directory);
                        if (createdSolutionFolder != null)
                        {
                            FillProcessingStack(itemsToProcess, createdSolutionFolder, directory.ChildItems);
                        }

                        break;
                    case SolutionProjectViewModel project:
                        CreateProject(item.Parent, project);
                        break;
                    default:
                        throw new NotSupportedException($"{item.SolutionItem.GetType()} is not supported.");
                }

                cancellationToken.ThrowIfCancellationRequested();
                await Dispatcher.Yield(DispatcherPriority.Background);
            }
        }

        private async void Start(object obj)
        {
            WasProjectAddStarted = true;
            _AddOperationCancellationTokenSource = new CancellationTokenSource();
            try
            {
                await PerformProcessingAsync(_AddOperationCancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                HasProjectAddCompleted = true;
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}