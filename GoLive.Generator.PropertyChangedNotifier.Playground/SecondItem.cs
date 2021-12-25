using System.Collections.ObjectModel;
using System.Collections.Specialized;
using FastMember;
using GoLive.Generator.PropertyChangedNotifier.Utilities;

namespace GoLive.Generator.ProperyChangedNotifier.Playground
{

    public partial class SecondItem : Entity
    {
        public SecondItem()
        {
            GeneratedCtor(); // needed
        }

        private string item1;
        private FullyObservableCollection<MainItem> thingsContained;
    }
}