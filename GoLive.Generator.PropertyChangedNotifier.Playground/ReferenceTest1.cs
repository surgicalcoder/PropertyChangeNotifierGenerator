using FastMember;
using GoLive.Generator.Saturn.Resources;
using GoLive.Saturn.Data.Entities;

namespace GoLive.Generator.PropertyChangedNotifier.Playground;

public partial class ReferenceTestScope : Entity
{
    private string name;
}

public partial class ReferenceTest1 : MultiscopedEntity<ReferenceTestScope>
{
    [AddRefToScope]
    private Ref<ReferenceTest2> test2;
    [AddRefToScope]
    private Ref<ReferenceTest3> test3;
    
}


public partial class ReferenceTest2 : Entity
{
    private string testName;
}

public partial class ReferenceTest3 : Entity
{
    private string anotherName;
}