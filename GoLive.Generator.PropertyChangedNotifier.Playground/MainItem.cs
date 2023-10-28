using GoLive.Generator.Saturn.Resources;
using GoLive.Saturn.Data.Entities;
using ObservableCollections;

namespace GoLive.Generator.PropertyChangedNotifier.Playground;

public partial class MainItem : Entity
{
    [AddToLimitedView("View1")]
    [AddToLimitedView("View2")]
    private string name;
    [AddToLimitedView("View2")]
    private string description;
    private ObservableList<string> strings;
    private ObservableList<string> anotherString;

    private string wibble3;
    
    public Dictionary<string, object> Changes = new();
    public bool EnableChangeTracking { get; set; }
}
