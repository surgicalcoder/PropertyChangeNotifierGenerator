using System.ComponentModel;
using System.Runtime.CompilerServices;
using GoLive.Generator.PropertyChangedNotifier.Utilities;
using System.Collections.Specialized;
using FastMember;

namespace GoLive.Generator.ProperyChangedNotifier.Playground
{
    public partial class MainItem : INotifyPropertyChanged
    {
        public MainItem()
        {
            Strings = new();
            Strings.CollectionChanged += (in ObservableCollections.NotifyCollectionChangedEventArgs<String> eventArgs) => Changes.Upsert($"Strings.{eventArgs.NewStartingIndex}", eventArgs.NewItem);
            AnotherString = new();
            AnotherString.CollectionChanged += (in ObservableCollections.NotifyCollectionChangedEventArgs<String> eventArgs) => Changes.Upsert($"AnotherString.{eventArgs.NewStartingIndex}", eventArgs.NewItem);
        }

        TypeAccessor StringTypeAccessor = TypeAccessor.Create(typeof(string));
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            if (EnableChangeTracking)
            {
                OnPropertyChanged(propertyName);
                Changes.Upsert(propertyName, value);
            }

            return true;
        }

        public string Name { get => name; set => SetField(ref name, value); }
        public string Description { get => description; set => SetField(ref description, value); }
        public ObservableCollections.ObservableList<string> Strings { get => strings; set => SetField(ref strings, value); }
        public ObservableCollections.ObservableList<string> AnotherString { get => anotherString; set => SetField(ref anotherString, value); }
    }
}