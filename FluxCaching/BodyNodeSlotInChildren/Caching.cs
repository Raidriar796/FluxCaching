using Elements.Core;
using FrooxEngine;
using FrooxEngine.CommonAvatar;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Avatar;
using Renderite.Shared;
using ResoniteModLoader;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    public partial class BodyNodeSlotInChildrenCaching
    {
        // Instantiated to cache data to compare for changes
        private class Cache(BodyNodeSlotInChildren instance, Slot slot, BodyNode node)
        {
            public Slot CachedTargetSlot { get; set; } = slot;
            public BodyNode CachedNode { get; set; } = node;
            public Slot CachedSlot { get; set; } = CustomFindSlotForNodeInChildren(instance, slot, node);
            public AvatarObjectSlot CachedAvatarObjectSlot { get; set; } = null!;
            public RefID CachedEquippedAvatar { get; set; } = new();
            public BipedRig CachedBipedRig { get; set; } = new();

            public bool IsBodyNodeSearched { get; set; } = false;
            public bool IsBipedRigSearched { get; set; } = false;

            public Dictionary<BodyNode, AvatarObjectSlot> SearchedAvatarObjectSlots { get; } = [];

            public HashSet<AvatarObjectSlot> SubscribedAvatarObjectSlots { get; } = [];
            public HashSet<Slot> SubscribedSlots { get; } = [];
            public HashSet<BipedRig> SubscribedBipedRigs { get; } = [];
            public HashSet<AvatarObjectSlot> SubscribedSearchedAvatarObjectSlots { get; } = [];
        }

        // Stores the instance of the BodyNodeSlotInChildren with it's cached results
        private static readonly Dictionary<BodyNodeSlotInChildren, Cache> CachedBodyNodeSlotInChildrens = [];
    }
}
