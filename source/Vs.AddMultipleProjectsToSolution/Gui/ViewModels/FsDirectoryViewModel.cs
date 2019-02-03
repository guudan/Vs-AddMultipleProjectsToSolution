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
    public class FsDirectoryViewModel : IFsItemViewModel, IEquatable<FsDirectoryViewModel>, IObservableObject
    {
        private readonly ObservableProperty<bool> _CreateSolutionFolder;
        private readonly ObservableProperty<bool?> _IsSelected;

        public FsDirectoryViewModel([NotNull] string directoryName)
        {
            Name = directoryName ?? throw new ArgumentNullException(nameof(directoryName));
            ChildItems = new FsItemCollection(this);
            _IsSelected = new ObservableProperty<bool?>(this, true);
            _CreateSolutionFolder = new ObservableProperty<bool>(this, true);
            ChangeSelectionCommand = new DelegateCommand(ChangeSelection);
        }

        public ICommand ChangeSelectionCommand { get; }

        public FsItemCollection ChildItems { get; }

        public bool CreateSolutionFolder
        {
            get => _CreateSolutionFolder.GetValue();
            set => _CreateSolutionFolder.SetValue(value, nameof(CreateSolutionFolder), nameof(ShouldCreateSolutionFolder));
        }

        public bool Equals(FsDirectoryViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
                   ChildItems.Count == other.ChildItems.Count &&
                   ChildItems.All(other.ChildItems.Contains);
        }

        public void Accept(IFsItemViewModelVisitor visitor)
        {
            if (visitor == null) throw new ArgumentNullException(nameof(visitor));
            visitor.Visit(this);
        }

        public IEnumerable<IFsItemViewModel> GetAllChildFileSystemItems()
        {
            return ChildItems.Concat(ChildItems.SelectMany(c => c.GetAllChildFileSystemItems()));
        }

        public bool? IsSelected
        {
            get => _IsSelected.GetValue();
            set => _IsSelected.SetValue(value, nameof(IsSelected), nameof(ShouldCreateSolutionFolder));
        }

        public bool ShouldCreateSolutionFolder
            => IsSelected != false && CreateSolutionFolder;

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
            return Equals((FsDirectoryViewModel) obj);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
        }

        public void UpdateSelectionState()
        {
            var allSelected = true;
            var anySelected = false;

            for (var i = 0; i < ChildItems.Count && (allSelected || !anySelected); i++)
            {
                var currentItem = ChildItems[i];
                allSelected &= currentItem.IsSelected ?? false;
                anySelected |= currentItem.IsSelected ?? true;
            }

            switch (new {allSelected, anySelected})
            {
                case var s when s.allSelected:
                    IsSelected = true;
                    break;
                case var s when s.anySelected:
                    IsSelected = null;
                    break;
                default:
                    IsSelected = false;
                    break;
            }

            Parent?.UpdateSelectionState();
        }

        private void ChangeSelection(object obj)
        {
            IsSelected = !(IsSelected ?? false);
            foreach (var child in GetAllChildFileSystemItems())
            {
                child.IsSelected = IsSelected;
            }

            Parent?.UpdateSelectionState();
        }
    }
}