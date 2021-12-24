using System.Collections.ObjectModel;
using System.Collections.Specialized;
using FastMember;
using GoLive.Generator.PropertyChangedNotifier.Utilities;

namespace GoLive.Generator.ProperyChangedNotifier.Playground
{

    public partial class SecondItem : Entity
    {
        private string item1;

        public SecondItem()
        {
            ThingsContained = new();
            ThingsContained.ItemPropertyChanged += ThingsContainedOnItemPropertyChanged;
            ThingsContained.CollectionChanged += ThingsContainedOnCollectionChanged;
        }

        private void ThingsContainedOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Changes.Add($"thingsContained.{e.OldStartingIndex}", e.NewItems);
        }

        TypeAccessor mainItemTypeAccesor = TypeAccessor.Create(typeof(MainItem));

        private void ThingsContainedOnItemPropertyChanged(object? sender, ItemPropertyChangedEventArgs e)
        {
            Changes.Add($"thingsContained.{e.CollectionIndex}.{e.PropertyName}", mainItemTypeAccesor[thingsContained[e.CollectionIndex], e.PropertyName]);
        }

        private FullyObservableCollection<MainItem> thingsContained;
    }
}