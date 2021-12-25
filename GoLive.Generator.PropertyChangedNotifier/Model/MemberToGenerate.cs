using Microsoft.CodeAnalysis;

namespace GoLive.Generator.PropertyChangedNotifier.Model
{
    public class MemberToGenerate
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public bool IsCollection { get; set; }
        public ITypeSymbol? CollectionType { get; set; }

        public bool ReadOnly { get; set; }
    }
}