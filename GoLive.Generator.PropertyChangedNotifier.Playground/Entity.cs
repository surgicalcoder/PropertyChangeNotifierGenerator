namespace GoLive.Generator.ProperyChangedNotifier.Playground;

public abstract class Entity
{
    public string Id { get; set; }

    public Dictionary<string, dynamic> Changes = new();
}