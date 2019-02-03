using System;
using System.Collections;
using System.Collections.Generic;
using Vs.AddMultipleProjectsToSolution.Annotations;

namespace Vs.AddMultipleProjectsToSolution.Gui.ViewModels
{
    public class FsItemCollection : IList<IFsItemViewModel>
    {
        private readonly FsDirectoryViewModel _Directory;
        private readonly List<IFsItemViewModel> _FileSystemItems = new List<IFsItemViewModel>();

        public FsItemCollection([NotNull] FsDirectoryViewModel directory)
        {
            _Directory = directory ?? throw new ArgumentNullException(nameof(directory));
        }

        public void Add(IFsItemViewModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.Parent = _Directory;
            _FileSystemItems.Add(item);
        }

        public void Clear()
        {
            _FileSystemItems.Clear();
        }

        public bool Contains(IFsItemViewModel item)
        {
            return _FileSystemItems.Contains(item);
        }

        public void CopyTo(IFsItemViewModel[] array, int arrayIndex)
        {
            _FileSystemItems.CopyTo(array, arrayIndex);
        }

        public int Count => _FileSystemItems.Count;
        public bool IsReadOnly => false;

        public bool Remove(IFsItemViewModel item)
        {
            return _FileSystemItems.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _FileSystemItems.GetEnumerator();
        }

        public IEnumerator<IFsItemViewModel> GetEnumerator()
        {
            return _FileSystemItems.GetEnumerator();
        }

        public int IndexOf(IFsItemViewModel item)
        {
            return _FileSystemItems.IndexOf(item);
        }

        public void Insert(int index, IFsItemViewModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.Parent = _Directory;
            _FileSystemItems.Insert(index, item);
        }

        public IFsItemViewModel this[int index]
        {
            get => _FileSystemItems[index];
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                value.Parent = _Directory;
                _FileSystemItems[index] = value;
            }
        }

        public void RemoveAt(int index)
        {
            _FileSystemItems.RemoveAt(index);
        }
    }
}