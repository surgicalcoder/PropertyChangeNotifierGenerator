using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace GoLive.Generator.Saturn;

public class MemberToGenerate
{
    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(IsCollection)}: {IsCollection}, {nameof(ReadOnly)}: {ReadOnly}, {nameof(WriteOnly)}: {WriteOnly}, {nameof(IsScoped)}: {IsScoped}, {nameof(LimitedViews)}: {LimitedViews?.Count}";
    }

    public string Name { get; set; }
    public ITypeSymbol Type { get; set; }

    public bool IsCollection { get; set; }
    public ITypeSymbol? CollectionType { get; set; }

    public bool ReadOnly { get; set; }
    public bool WriteOnly { get; set; }
    public bool IsScoped { get; set; }

    public List<LimitedViewToGenerate> LimitedViews { get; set; } = new();

    public List<MemberAttribute> AdditionalAttributes { get; set; } = new();
}

public class MemberAttribute
{
    public string Name { get; set; }
    public List<string> ConstructorParameters { get; set; } = new();
    public List<KeyValuePair<string, string>> NamedParameters { get; set; }= new();
}