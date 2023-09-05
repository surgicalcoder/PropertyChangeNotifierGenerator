using GoLive.Generator.PropertyChangedNotifier.Utilities;
using GoLive.Saturn.Data.Entities;

namespace GoLive.Generator.PropertyChangedNotifier.Playground;

public partial class FourthItem : MultiscopedEntity<MainItem>
{
    private string blarg;
    [AddRefToScope]
    private Ref<MainItem> mainItem;
    
    [DoNotTrackChanges]
    public Dictionary<string, object> Changes = new();
    [DoNotTrackChanges]
    public bool EnableChangeTracking { get; set; }
}