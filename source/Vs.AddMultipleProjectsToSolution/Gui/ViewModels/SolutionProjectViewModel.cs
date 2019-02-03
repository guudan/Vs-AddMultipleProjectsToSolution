using System;
using System.ComponentModel;
using Vs.AddMultipleProjectsToSolution.Annotations;
using Vs.AddMultipleProjectsToSolution.Gui.Utilities;

namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public class SolutionProjectViewModel : ISolutionItemViewModel, IObservableObject
    {
        private readonly ObservableProperty<SolutionItemCreateStatus> _CreateStatus;
        private readonly ObservableProperty<string> _CreateStatusMessage;

        public SolutionProjectViewModel([NotNull] string projectName, [NotNull] string projectFilePath)
        {
            Name = projectName ?? throw new ArgumentNullException(nameof(projectName));
            ProjectFilePath = projectFilePath ?? throw new ArgumentNullException(nameof(projectFilePath));
            _CreateStatus = new ObservableProperty<SolutionItemCreateStatus>(this, SolutionItemCreateStatus.Pending);
            _CreateStatusMessage = new ObservableProperty<string>(this);
        }

        public string ProjectFilePath { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokeNotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SolutionItemCreateStatus CreateStatus
        {
            get => _CreateStatus.GetValue();
            set => _CreateStatus.SetValue(value);
        }

        public string CreateStatusMessage
        {
            get => _CreateStatusMessage.GetValue();
            set => _CreateStatusMessage.SetValue(value);
        }

        public string Name { get; }
    }
}