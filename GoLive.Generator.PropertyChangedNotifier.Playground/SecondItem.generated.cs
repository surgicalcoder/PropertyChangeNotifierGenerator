using System.ComponentModel;
using System.Runtime.CompilerServices;
using GoLive.Generator.PropertyChangedNotifier.Utilities;
using System.Collections.Specialized;
using FastMember;

namespace GoLive.Generator.ProperyChangedNotifier.Playground
{
    public partial class SecondItem : INotifyPropertyChanged
    {
        public void GeneratedCtor()
        {
            ThingsContained = new();
            ThingsContained.ItemPropertyChanged += ThingsContainedOnItemPropertyChanged;
            ThingsContained.CollectionChanged += ThingsContainedOnCollectionChanged;
        }

        TypeAccessor MainItemTypeAccessor = TypeAccessor.Create(typeof(GoLive.Generator.ProperyChangedNotifier.Playground.MainItem));
        private void ThingsContainedOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Changes.Add($"ThingsContained.{e.OldStartingIndex}", e.NewItems);
        }

        private void ThingsContainedOnItemPropertyChanged(object? sender, ItemPropertyChangedEventArgs e)
        {
            Changes.Add($"ThingsContained.{e.CollectionIndex}.{e.PropertyName}", MainItemTypeAccessor[ThingsContained[e.CollectionIndex], e.PropertyName]);
        }

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

        public FullyObservableCollection<GoLive.Generator.ProperyChangedNotifier.Playground.MainItem> ThingsContained { get => thingsContained; set => SetField(ref thingsContained, value); }
    }
}