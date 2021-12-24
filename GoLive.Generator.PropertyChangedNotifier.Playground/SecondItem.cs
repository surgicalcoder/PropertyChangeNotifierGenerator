using System.Collections.ObjectModel;
using System.Collections.Specialized;
using GoLive.Generator.PropertyChangedNotifier.Utilities;

namespace GoLive.Generator.ProperyChangedNotifier.Playground
{

    public partial class SecondItem : Entity
    {
        private string item1;

        public SecondItem()
        {
            thingsContained = new();
            thingsContained.ItemPropertyChanged += ThingsContainedOnItemPropertyChanged;
            thingsContained.CollectionChanged += ThingsContainedOnCollectionChanged;
        }

        private void ThingsContainedOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            
        }

        private void ThingsContainedOnItemPropertyChanged(object? sender, ItemPropertyChangedEventArgs e)
        {
            e.
        }

        private FullyObservableCollection<MainItem> thingsContained;
    }
}