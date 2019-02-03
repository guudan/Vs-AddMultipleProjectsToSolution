using System;
using System.Collections.Generic;
using System.ComponentModel;
using Vs.AddMultipleProjectsToSolution.Annotations;
using Vs.AddMultipleProjectsToSolution.Gui.Utilities;

namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public class SolutionDirectoryViewModel : ISolutionItemViewModel, IObservableObject
    {
        private readonly ObservableProperty<SolutionItemCreateStatus> _CreateStatus;
        private readonly ObservableProperty<string> _CreateStatusMessage;

        public SolutionDirectoryViewModel([NotNull] string directoryName)
        {
            Name = directoryName ?? throw new ArgumentNullException(nameof(directoryName));
            _CreateStatus = new ObservableProperty<SolutionItemCreateStatus>(this);
            _CreateStatusMessage = new ObservableProperty<string>(this);
        }

        public IList<ISolutionItemViewModel> ChildItems { get; } = new List<ISolutionItemViewModel>();

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