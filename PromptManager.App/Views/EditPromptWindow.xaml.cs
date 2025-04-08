using System.Windows;
using PromptManager.App.Models;
using PromptManager.App.ViewModels;
using Wpf.Ui.Controls;

namespace PromptManager.App.Views
{
    public partial class EditPromptWindow : FluentWindow
    {
        public EditPromptWindow(Prompt prompt)
        {
            InitializeComponent();
            var viewModel = new EditPromptViewModel(prompt);
            viewModel.SaveCompleted += (s, e) => Close();
            DataContext = viewModel;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (DataContext is EditPromptViewModel viewModel)
            {
                DialogResult = viewModel.DialogResult;
            }
        }
    }
} 