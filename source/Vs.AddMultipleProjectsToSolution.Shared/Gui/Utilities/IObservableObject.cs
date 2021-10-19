using System.ComponentModel;

namespace Vs.AddMultipleProjectsToSolution.Gui.Utilities
{
    public interface IObservableObject : INotifyPropertyChanged
    {
        void InvokeNotifyPropertyChanged(string propertyName);
    }
}