using System.Collections.ObjectModel;
using GoLive.Generator.PropertyChangedNotifier.Utilities;
using ObservableCollections;

namespace GoLive.Generator.ProperyChangedNotifier.Playground;

public partial class MainItem : Entity
{
    private string name;
    private string description;
    private ObservableList<string> strings;
    private ObservableList<string> anotherString;
}


/*public class ThirdTestItem : Entity
{
    public ObservableList<string> dt1 { get; set; }

    void Test()
    {
        dt1.CollectionChanged += delegate(in NotifyCollectionChangedEventArgs<string> args)
        {
            args.
        };
    }
}*/