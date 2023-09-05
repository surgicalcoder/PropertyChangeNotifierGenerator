﻿using GoLive.Saturn.Data.Entities;
using ObservableCollections;

namespace GoLive.Generator.PropertyChangedNotifier.Playground;

public partial class MainItem : Entity
{
    private string name;
    private string description;
    private ObservableList<string> strings;
    private ObservableList<string> anotherString;
    
    
    public Dictionary<string, object> Changes = new();
    public bool EnableChangeTracking { get; set; }
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