using Elements.Core;
using FrooxEngine;
using FrooxEngine.CommonAvatar;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Avatar;
using Renderite.Shared;
using ResoniteModLoader;

namespace FastBodyNodeSlot;

public partial class FastBodyNodeSlot : ResoniteMod
{
    // Instantiated to cache data to compare for changes
    private class CachedResults(User user, BodyNode node)
    {
        public User CachedUser { get; set; } = user;
        public BodyNode CachedNode { get; set; } = node;
        public Slot CachedSlot { get; set; } = user.GetBodyNodeSlot(node);
        public Slot CachedParent { get; set; } = null!;
        public AvatarObjectSlot CachedAvatarObjectSlot { get; set; } = null!;
        public RefID CachedEquippedAvatar { get; set; } = new();
    }

    // Stores the instance of the BodyNodeSlot with it's cached results
    private static readonly Dictionary<BodyNodeSlot, CachedResults> CachedBodyNodeSlots = [];
}
