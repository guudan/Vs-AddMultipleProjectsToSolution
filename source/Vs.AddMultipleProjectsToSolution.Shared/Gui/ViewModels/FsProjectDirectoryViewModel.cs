using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;
using Vs.AddMultipleProjectsToSolution.Annotations;
using Vs.AddMultipleProjectsToSolution.Gui.Utilities;

namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public class FsProjectDirectoryViewModel : IFsItemViewModel, IEquatable<FsProjectDirectoryViewModel>,
        IObservableObject
    {
        private readonly ObservableProperty<bool> _IsSelected;

        public FsProjectDirectoryViewModel([NotNull] string fileName, [NotNull] string filePath)
        {
            Name = fileName ?? throw new ArgumentNullException(nameof(fileName));
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _IsSelected = new ObservableProperty<bool>(this, true);
            ChangeSelectionCommand = new DelegateCommand(ChangeSelection);
        }

        public ICommand ChangeSelectionCommand { get; }

        public string FilePath { get; }

        public bool Equals(FsProjectDirectoryViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(FilePath, other.FilePath, StringComparison.OrdinalIgnoreCase);
        }

        public void Accept(IFsItemViewModelVisitor visitor)
        {
            if (visitor == null) throw new ArgumentNullException(nameof(visitor));
            visitor.Visit(this);
        }

        public IEnumerable<IFsItemViewModel> GetAllChildFileSystemItems()
        {
            return Enumerable.Empty<IFsItemViewModel>();
        }

        public bool? IsSelected
        {
            get => _IsSelected.GetValue();
            set => _IsSelected.SetValue(value ?? true);
        }

        public string Name { get; }

        public FsDirectoryViewModel Parent { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokeNotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FsProjectDirectoryViewModel) obj);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
        }

        private void ChangeSelection(object obj)
        {
            IsSelected = !(IsSelected ?? false);
            Parent?.UpdateSelectionState();
        }
    }
}