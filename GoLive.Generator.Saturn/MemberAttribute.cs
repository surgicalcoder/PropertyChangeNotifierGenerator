using System.Collections.Generic;

namespace GoLive.Generator.Saturn;

public class MemberAttribute
{
    public string Name { get; set; }
    public List<string> ConstructorParameters { get; set; } = new();
    public List<KeyValuePair<string, string>> NamedParameters { get; set; }= new();
}