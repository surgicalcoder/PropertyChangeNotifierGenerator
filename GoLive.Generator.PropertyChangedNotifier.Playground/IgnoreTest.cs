using GoLive.Generator.Saturn.Resources;
using GoLive.Saturn.Data.Entities;

namespace GoLive.Generator.PropertyChangedNotifier.Playground
{
    public partial class IgnoreTest : Entity
    {
        [DoNotTrackChanges]
        private string ignoreTestItem;
        [Readonly]
        private string readonlyTest;
        private string everythingElseTest;
        
        /*public Dictionary<string, object> Changes = new();
        public bool EnableChangeTracking { get; set; }*/
    }
}
