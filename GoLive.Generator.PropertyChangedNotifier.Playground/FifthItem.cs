using GoLive.Generator.Saturn.Resources;
using GoLive.Saturn.Data.Entities;

namespace GoLive.Generator.PropertyChangedNotifier.Playground;

public partial class FifthItem : Entity
{
    [AddToLimitedView("PublicView")]
    private string username;
    private string password;
}