using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Vs.AddMultipleProjectsToSolution.Gui.ViewModels;

namespace Vs.AddMultipleProjectsToSolution.Gui
{
    public partial class AddMultipleProjectsProgressControlControl : UserControl
    {
        public AddMultipleProjectsProgressControlControl(AddMultipleProjectsProgressViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(CultureInfo.CurrentUICulture, "Invoked '{0}'", ToString()),
                "AddMultipleProjectsProgressControl");
        }
    }
}