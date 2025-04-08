using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PromptManager.App.Models;
using PromptManager.App.Services;
using PromptManager.App.Views;
using Wpf.Ui.Appearance;

namespace PromptManager.App.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly CsvService _csvService;
        private readonly NotificationService _notificationService;

        [ObservableProperty] private Section? _selectedSection;

        [ObservableProperty] private string _searchText = string.Empty;

        [ObservableProperty] private ObservableCollection<Section> _sections = new();

        [ObservableProperty] private bool _isDarkTheme;

        public MainViewModel()
        {
            _csvService = new CsvService();
            _notificationService = new NotificationService();
            ApplicationThemeManager.Apply(ApplicationTheme.Light);
            _isDarkTheme = false;
            LoadSections();
        }

        partial void OnIsDarkThemeChanged(bool value)
        {
            ApplicationThemeManager.Apply(value ? ApplicationTheme.Dark : ApplicationTheme.Light);
        }

        [RelayCommand]
        private void ChangeTheme(string theme)
        {
            if (Enum.TryParse<ApplicationTheme>(theme, out var themeType))
            {
                ApplicationThemeManager.Apply(themeType);
                IsDarkTheme = themeType == ApplicationTheme.Dark;
            }
        }

        public IEnumerable<Prompt> FilteredPrompts
        {
            get
            {
                if (SelectedSection == null) return Enumerable.Empty<Prompt>();

                var prompts = SelectedSection.Prompts;
                if (string.IsNullOrWhiteSpace(SearchText)) return prompts;

                return prompts.Where(p =>
                    p.Act.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    p.PromptText.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }
        }

        private void LoadSections()
        {
            try
            {
                var sections = _csvService.LoadAllSections();
                Sections = new ObservableCollection<Section>(sections);
                if (Sections.Any())
                {
                    SelectedSection = Sections.First();
                    _notificationService.ShowSuccess("Sections loaded successfully");
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Error loading sections: {ex.Message}");
            }
        }

        [RelayCommand]
        private void CopyPrompt(object? parameter)
        {
            if (parameter is not Prompt prompt) return;

            try
            {
                Clipboard.SetText(prompt.PromptText);
                _notificationService.ShowSuccess("Prompt copied to clipboard");
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Error copying prompt: {ex.Message}");
            }
        }

        [RelayCommand]
        private void EditPrompt(object? parameter)
        {
            if (parameter is not Prompt prompt) return;

            try
            {
                var dialog = new EditPromptWindow(prompt);
                dialog.Owner = Application.Current.MainWindow;
                if (dialog.ShowDialog() == true)
                {
                    _notificationService.ShowSuccess("Prompt updated successfully");
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Error editing prompt: {ex.Message}");
            }
        }

        [RelayCommand]
        private void SavePrompt(object? parameter)
        {
            if (parameter is not Prompt prompt || SelectedSection == null) return;

            try
            {
                _csvService.SaveSection(SelectedSection);
                _notificationService.ShowSuccess("Changes saved successfully");
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Error saving changes: {ex.Message}");
            }
        }

        [RelayCommand]
        private void DeletePrompt(object? parameter)
        {
            if (parameter is not Prompt prompt || SelectedSection == null) return;

            try
            {
                var result = MessageBox.Show(
                    "Are you sure you want to delete this prompt?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SelectedSection.Prompts.Remove(prompt);
                    _csvService.SaveSection(SelectedSection);
                    _notificationService.ShowSuccess("Prompt deleted successfully");
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Error deleting prompt: {ex.Message}");
            }
        }

        [RelayCommand]
        private void AddSection()
        {
            try
            {
                var dialog = new AddSectionWindow();
                dialog.Owner = Application.Current.MainWindow;
                if (dialog.ShowDialog() == true)
                {
                    var newSection = new Section { Name = dialog.SectionName };
                    Sections.Add(newSection);
                    SelectedSection = newSection;
                    _notificationService.ShowSuccess("Section added successfully");
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Error adding section: {ex.Message}");
            }
        }

        [RelayCommand]
        private void AddPrompt()
        {
            if (SelectedSection == null)
            {
                _notificationService.ShowError("Please select a section first");
                return;
            }

            try
            {
                var newPrompt = new Prompt();
                var dialog = new EditPromptWindow(newPrompt);
                dialog.Owner = Application.Current.MainWindow;
                if (dialog.ShowDialog() == true)
                {
                    SelectedSection.Prompts.Add(newPrompt);
                    _csvService.SaveSection(SelectedSection);
                    _notificationService.ShowSuccess("Prompt added successfully");
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Error adding prompt: {ex.Message}");
            }
        }

        [RelayCommand]
        private void DeleteSection(object? parameter)
        {
            if (parameter is not Section section) return;

            try
            {
                var result = MessageBox.Show(
                    "Are you sure you want to delete this section? All prompts in this section will be deleted.",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Sections.Remove(section);
                    _csvService.DeleteSection(section);
                    if (Sections.Any())
                    {
                        SelectedSection = Sections.First();
                    }
                    _notificationService.ShowSuccess("Section deleted successfully");
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Error deleting section: {ex.Message}");
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            OnPropertyChanged(nameof(FilteredPrompts));
        }

        partial void OnSelectedSectionChanged(Section? value)
        {
            OnPropertyChanged(nameof(FilteredPrompts));
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter is T t ? t : default) ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute(parameter is T t ? t : default);
        }
    }
}