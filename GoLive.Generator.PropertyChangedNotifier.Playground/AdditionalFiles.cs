using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
namespace GoLive.Generator.PropertyChangedNotifier.Utilities { 
   
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


[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class DoNotTrackChangesAttribute : Attribute { }
}
