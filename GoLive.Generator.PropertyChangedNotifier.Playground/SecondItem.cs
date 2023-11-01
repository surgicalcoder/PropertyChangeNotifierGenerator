using GoLive.Saturn.Data.Entities;
using ObservableCollections;

namespace GoLive.Generator.PropertyChangedNotifier.Playground
{
    public partial class SecondItem : Entity
    {
        private string item1;
        private ObservableList<MainItem> thingsContained;
        
        /*public Dictionary<string, object> Changes = new();
        public bool EnableChangeTracking { get; set; }*/
    }
}