using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PromptManager.App.Models;

namespace PromptManager.App.ViewModels
{
    public partial class EditPromptViewModel : ObservableObject
    {
        private readonly Prompt _prompt;

        [ObservableProperty] private string _actionText = string.Empty;

        [ObservableProperty] private string _promptText = string.Empty;

        public event EventHandler? SaveCompleted;

        public EditPromptViewModel(Prompt prompt)
        {
            _prompt = prompt;
            ActionText = prompt.Act;
            PromptText = prompt.PromptText;
        }

        [RelayCommand]
        private void Save()
        {
            if (string.IsNullOrWhiteSpace(ActionText) || string.IsNullOrWhiteSpace(PromptText))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            _prompt.Act = ActionText;
            _prompt.PromptText = PromptText;
            DialogResult = true;
            SaveCompleted?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogResult = false;
            SaveCompleted?.Invoke(this, EventArgs.Empty);
        }

        public bool DialogResult { get; private set; }
    }
}