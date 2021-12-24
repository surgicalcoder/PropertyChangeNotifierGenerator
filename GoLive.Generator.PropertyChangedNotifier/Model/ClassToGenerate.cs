using System.Collections.Generic;

namespace GoLive.Generator.PropertyChangedNotifier.Model
{
    public class ClassToGenerate
    {
        public ClassToGenerate()
        {
            Members = new List<MemberToGenerate>();
        }

        public string Name { get; set; }
        public List<MemberToGenerate> Members { get; set; }
        public string Filename { get; set; }
        public string Namespace { get; set; }
    }
}