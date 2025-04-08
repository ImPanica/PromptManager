using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PromptManager.App.Models
{
    public class Section : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private ObservableCollection<Prompt> _prompts = new();

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public ObservableCollection<Prompt> Prompts
        {
            get => _prompts;
            set
            {
                if (_prompts != value)
                {
                    _prompts = value;
                    OnPropertyChanged(nameof(Prompts));
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