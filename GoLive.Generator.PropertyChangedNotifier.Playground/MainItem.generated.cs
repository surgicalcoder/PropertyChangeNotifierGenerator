using System.ComponentModel;
using System.Runtime.CompilerServices;
using GoLive.Generator.PropertyChangedNotifier.Utilities;

namespace GoLive.Generator.ProperyChangedNotifier.Playground
{
    public partial class MainItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            Changes.Add(propertyName, value);
            return true;
        }

        public string Name { get => name; set => SetField(ref name, value); }

        public string Description { get => description; set => SetField(ref description, value); }
    }
}