using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
namespace  GoLive.Generator.PropertyChangedNotifier.Utilities {
   
using System;

public static class DictionaryExt
{
    public static void Upsert<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class ReadonlyAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class WriteOnlyAttribute : Attribute { }


[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class DoNotTrackChangesAttribute : Attribute { }


[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class AddRefToScopeAttribute : Attribute { }


[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
public class AddToLimitedViewAttribute : Attribute
{
    public AddToLimitedViewAttribute(string Name)
    {
        this.Name = Name;
    }
    public string Name { get; set; }
}

}
