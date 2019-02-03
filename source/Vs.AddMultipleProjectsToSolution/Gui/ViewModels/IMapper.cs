using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public interface IMapper
    {
        Task<IFsItemViewModel[]> MapFilesToViewModelAsync(string rootDirectoryPath,
            IEnumerable<string> files,
            CancellationToken cancellationToken);
    }
}