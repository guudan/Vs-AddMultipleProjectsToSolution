using System;
using System.Runtime.CompilerServices;
using Vs.AddMultipleProjectsToSolution.Annotations;

namespace Vs.AddMultipleProjectsToSolution.Gui.Utilities
{
    public class ObservableProperty<T>
    {
        private readonly IObservableObject _Owner;
        private T _Value;

        public ObservableProperty([NotNull] IObservableObject owner) : this(owner, default(T))
        {
        }

        public ObservableProperty([NotNull] IObservableObject owner, T initialValue)
        {
            _Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _Value = initialValue;
        }

        public T GetValue()
        {
            return _Value;
        }

        public void SetValue(T newValue, [CallerMemberName] string propertyName = "")
        {
            if (!Equals(_Value, newValue))
            {
                _Value = newValue;
                _Owner.InvokeNotifyPropertyChanged(propertyName);
            }
        }

        public void SetValue(T newValue, params string[] propertiesNames)
        {
            if (!Equals(_Value, newValue))
            {
                _Value = newValue;
                foreach (var propertyName in propertiesNames)
                {
                    _Owner.InvokeNotifyPropertyChanged(propertyName);
                }
            }
        }
    }
}