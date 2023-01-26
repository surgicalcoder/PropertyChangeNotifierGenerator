using System.Collections.ObjectModel;
using System.Collections.Specialized;
using FastMember;
using GoLive.Generator.PropertyChangedNotifier.Utilities;
using ObservableCollections;

namespace GoLive.Generator.ProperyChangedNotifier.Playground
{
    public partial class SecondItem : Entity
    {
        private string item1;
        private ObservableList<MainItem> thingsContained;
    }
}