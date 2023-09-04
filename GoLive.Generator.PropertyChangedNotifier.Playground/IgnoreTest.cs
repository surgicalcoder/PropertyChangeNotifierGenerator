using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoLive.Generator.PropertyChangedNotifier.Utilities;
using GoLive.Saturn.Data.Entities;

namespace GoLive.Generator.ProperyChangedNotifier.Playground
{
    public partial class IgnoreTest : Entity
    {
        [DoNotTrackChanges]
        private string ignoreTestItem;
        [Readonly]
        private string readonlyTest;
        private string everythingElseTest;
        
        public Dictionary<string, object> Changes = new();
        public bool EnableChangeTracking { get; set; }
    }
}
