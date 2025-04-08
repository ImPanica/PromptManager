using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PromptManager.App.ViewModels
{
    public partial class AddSectionViewModel : ObservableObject
    {
        [ObservableProperty] private string _sectionName = string.Empty;

        public event EventHandler? SaveCompleted;

        public string ResultSectionName { get; private set; } = string.Empty;

        [RelayCommand]
        private void Save()
        {
            if (string.IsNullOrWhiteSpace(SectionName))
            {
                MessageBox.Show("Please enter a section name.", "Validation Error", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            ResultSectionName = SectionName;
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