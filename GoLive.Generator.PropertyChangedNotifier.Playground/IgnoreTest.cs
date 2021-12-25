using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoLive.Generator.PropertyChangedNotifier.Utilities;

namespace GoLive.Generator.ProperyChangedNotifier.Playground
{
    public partial class IgnoreTest : Entity
    {
        [DoNotTrackChanges]
        private string ignoreTest;
        [ReadonlyAtribute]
        private string readonlyTest;
        private string everythingElseTest;
    }
}
