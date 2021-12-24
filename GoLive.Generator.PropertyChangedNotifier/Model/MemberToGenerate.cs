namespace GoLive.Generator.PropertyChangedNotifier.Model
{
    public class MemberToGenerate
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public bool IsCollection { get; set; }
        public string CollectionType { get; set; }
    }
}