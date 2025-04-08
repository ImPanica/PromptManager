using System.Windows;
using PromptManager.App.ViewModels;
using Wpf.Ui.Controls;

namespace PromptManager.App.Views
{
    public partial class AddSectionWindow : FluentWindow
    {
        public string SectionName => (DataContext as AddSectionViewModel)?.ResultSectionName ?? string.Empty;

        public AddSectionWindow()
        {
            InitializeComponent();
            var viewModel = new AddSectionViewModel();
            viewModel.SaveCompleted += (s, e) => Close();
            DataContext = viewModel;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (DataContext is AddSectionViewModel viewModel)
            {
                DialogResult = viewModel.DialogResult;
            }
        }
    }
} 