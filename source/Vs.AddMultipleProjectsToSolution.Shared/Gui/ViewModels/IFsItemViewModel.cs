using System.Collections.Generic;

namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public interface IFsItemViewModel
    {
        bool? IsSelected { get; set; }
        string Name { get; }

        FsDirectoryViewModel Parent { get; set; }

        void Accept(IFsItemViewModelVisitor visitor);

        IEnumerable<IFsItemViewModel> GetAllChildFileSystemItems();
    }
}