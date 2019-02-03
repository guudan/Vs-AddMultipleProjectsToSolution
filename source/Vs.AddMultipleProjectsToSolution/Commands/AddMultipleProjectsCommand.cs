using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Vs.AddMultipleProjectsToSolution.Gui;
using Vs.AddMultipleProjectsToSolution.Gui.ViewModels;
using Vs.AddMultipleProjectsToSolution.Utilities;
using Task = System.Threading.Tasks.Task;

namespace Vs.AddMultipleProjectsToSolution
{
    internal sealed class AddMultipleProjectsCommand
    {
        public static readonly Guid CommandSet = new Guid("52eefd8b-3e82-4d3a-a15f-5ef227b118a9");
        public const int SolutionContextCommandId = 0x0100;

        private readonly AsyncPackage _Package;

        private AddMultipleProjectsCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            _Package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var solutionContextCommandID = new CommandID(CommandSet, SolutionContextCommandId);
            var solutionContextMenuItem = new MenuCommand(Execute, solutionContextCommandID);
            commandService.AddCommand(solutionContextMenuItem);
        }

        public static AddMultipleProjectsCommand Instance { get; private set; }

        private IAsyncServiceProvider ServiceProvider => _Package;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in AddMultipleProjectsCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new AddMultipleProjectsCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var solution = NVsSolution.Create();
            var windowService = WindowService.Instance;
            var viewModel = new AddMultipleProjectsConfigurationViewModel(windowService, solution);
            windowService.OpenAddMultipleProjectsWindow(viewModel);
        }
    }
}