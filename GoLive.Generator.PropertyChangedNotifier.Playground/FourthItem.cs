using System.Text.Json.Serialization;
using GoLive.Generator.Saturn.Resources;
using GoLive.Saturn.Data.Entities;

namespace GoLive.Generator.PropertyChangedNotifier.Playground;

public partial class FourthItem : MultiscopedEntity<MainItem>
{
    [AddToLimitedView("View1")]
    [AddToLimitedView("View2")]
    private string blarg;

    [AddToLimitedView("View1", UseLimitedView = "PublicView")]
    private FifthItem fifth;
    
    [AddRefToScope]
    [System.Text.Json.Serialization.JsonIgnoreAttribute(Condition = 0)]
    private Ref<MainItem> mainItem;
    
    /*[DoNotTrackChanges]
    public Dictionary<string, object> Changes = new();
    [DoNotTrackChanges]
    public bool EnableChangeTracking { get; set; }*/
}