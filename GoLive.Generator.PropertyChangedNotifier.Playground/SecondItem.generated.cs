using System.ComponentModel;
using System.Runtime.CompilerServices;
using GoLive.Generator.PropertyChangedNotifier.Utilities;

namespace GoLive.Generator.ProperyChangedNotifier.Playground
{
    public partial class SecondItem : INotifyPropertyChanged
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

        public string Item1 { get => item1; set => SetField(ref item1, value); }

        public System.Collections.ObjectModel.ObservableCollection<GoLive.Generator.ProperyChangedNotifier.Playground.MainItem> ThingsContained { get => thingsContained; set => SetField(ref thingsContained, value); }
    }
}