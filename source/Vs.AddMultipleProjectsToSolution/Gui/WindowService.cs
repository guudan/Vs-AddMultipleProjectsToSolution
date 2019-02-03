using System;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using Vs.AddMultipleProjectsToSolution.Gui.ViewModels;

namespace Vs.AddMultipleProjectsToSolution.Gui
{
    public class WindowService : IWindowService
    {
        private static volatile WindowService _Instance;
        private static readonly object CreateInstanceLock = new object();

        private AddMultipleProjectsWindow _AddMultipleProjectsWindow;
        private LoadProjectsWindow _LoadProjectsWindow;

        private WindowService()
        {
        }

        public static WindowService Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (CreateInstanceLock)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new WindowService();
                        }
                    }
                }

                return _Instance;
            }
        }

        public bool CloseAddMultipleProjectsWindow()
        {
            return CloseWindow(_AddMultipleProjectsWindow);
        }

        public bool CloseLoadProjectsWindow()
        {
            return CloseWindow(_LoadProjectsWindow);
        }

        public void OpenAddMultipleProjectsWindow(AddMultipleProjectsConfigurationViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            _AddMultipleProjectsWindow = new AddMultipleProjectsWindow(viewModel);
            _AddMultipleProjectsWindow.Closed += _AddMultipleProjectsWindow_Closed;
            _AddMultipleProjectsWindow.ShowModal();
        }

        public void OpenLoadProjectsWindow(LoadProjectsWindowViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            _LoadProjectsWindow = new LoadProjectsWindow(viewModel);
            _LoadProjectsWindow.Closed += _LoadProjectsWindow_Closed;
            _LoadProjectsWindow.ShowModal();
        }

        public string OpenOpenFolderDialog()
        {
            var dialog = new CommonOpenFileDialog();
            // TODO: Location should default to the solution location
            dialog.IsFolderPicker = true;
            dialog.Multiselect = false;
            var folderName = dialog.ShowDialog() == CommonFileDialogResult.Ok ? dialog.FileName : null;
            return folderName;
        }

        public void SetAddMultipleProjectsWindowContent(
            AddMultipleProjectsProgressViewModel addProjectsProgressViewModel)
        {
            if (_AddMultipleProjectsWindow == null)
            {
                throw new InvalidOperationException("Add multiple projects window has to be open first.");
            }

            _AddMultipleProjectsWindow.Content =
                new AddMultipleProjectsProgressControlControl(addProjectsProgressViewModel);
        }

        public void SetAddMultipleProjectsWindowContent(
            AddMultipleProjectsConfigurationViewModel addProjectsConfigurationViewModel)
        {
            if (_AddMultipleProjectsWindow == null)
            {
                throw new InvalidOperationException("Add multiple projects window has to be open first.");
            }

            _AddMultipleProjectsWindow.Content =
                new AddMultipleProjectsConfigurationControl(addProjectsConfigurationViewModel);
        }

        private void _AddMultipleProjectsWindow_Closed(object sender, EventArgs e)
        {
            var window = _AddMultipleProjectsWindow;
            _AddMultipleProjectsWindow = null;
            window.Closed -= _AddMultipleProjectsWindow_Closed;
        }

        private void _LoadProjectsWindow_Closed(object sender, EventArgs e)
        {
            var window = _LoadProjectsWindow;
            _LoadProjectsWindow = null;
            window.Closed -= _LoadProjectsWindow_Closed;
        }

        private bool CloseWindow<T>(T window) where T : Window
        {
            if (window != null)
            {
                window.Close();
                return true;
            }

            return false;
        }
    }
}