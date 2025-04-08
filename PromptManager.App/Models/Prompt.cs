using System.ComponentModel;
using System.Runtime.CompilerServices;
using CsvHelper.Configuration.Attributes;

namespace PromptManager.App.Models
{
    public class Prompt : INotifyPropertyChanged
    {
        private string _act = string.Empty;
        private string _promptText = string.Empty;

        [Name("act")]
        public string Act
        {
            get => _act;
            set
            {
                if (_act != value)
                {
                    _act = value;
                    OnPropertyChanged(nameof(Act));
                }
            }
        }

        [Name("prompt")]
        public string PromptText
        {
            get => _promptText;
            set
            {
                if (_promptText != value)
                {
                    _promptText = value;
                    OnPropertyChanged(nameof(PromptText));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}