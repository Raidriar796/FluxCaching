using ResoniteModLoader;
using FrooxEngine;
using FrooxEngine.CommonAvatar;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Avatar;
using Renderite.Shared;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    public partial class BodyNodeSlotCaching
    {
        private static Slot CustomGetBodyNodeSlot(BodyNodeSlot instance, User user, BodyNode node)
        {
            Cache cache;
            BipedRig bipedRig;
            AvatarObjectSlot avatarObjectSlot = null!;

            // Returns early if the dictionary does not have the BodyNodeSlot tracked yet
            if (CachedBodyNodeSlots.ContainsKey(instance)) cache = CachedBodyNodeSlots[instance];
            else return null!;

            // Recreation of the original GetBodyNodeSlot method's null checking
            Slot slot;

	        if (user == null) slot = null!;
	        else slot = (user.Root != null) ? user.Root.Slot : null!;

	        Slot root = slot;

	        if (root == null || node == BodyNode.NONE) return null!;

            // Stores for the first time the biped rig is searched to avoid searching again if it's null
            if (!cache.IsBipedRigSearched)
            {
                cache.CachedBipedRig = root.GetComponentInChildren<BipedRig>();
                bipedRig = cache.CachedBipedRig;
                CachedBodyNodeSlots[instance].CachedBipedRig = cache.CachedBipedRig;
                CachedBodyNodeSlots[instance].IsBipedRigSearched = true;

                if (cache.CachedBipedRig != null)
                {
                    CachedBodyNodeSlots[instance].CachedBipedRig.Destroyed += (b) =>
                    {
                        cache.CachedBipedRig = root.GetComponentInChildren<BipedRig>();
                        bipedRig = cache.CachedBipedRig;
                        CachedBodyNodeSlots[instance].CachedBipedRig = cache.CachedBipedRig;
                    };
                };
            }
            else if (cache.CachedBipedRig == null) bipedRig = null!;
            else bipedRig = cache.CachedBipedRig;

	        Slot bone = (bipedRig != null) ? bipedRig.TryGetBone(node) : null!;

	        if (bone != null) return bone;

            // Check if the body node being searched has been searched already,
            // cache it if it hasn't, reuse the cached results if it has.
            if (!cache.SearchedAvatarObjectSlots.ContainsKey(node))
            {
                avatarObjectSlot = root.FindSlotForNodeInChildren(node);
                CachedBodyNodeSlots[instance].SearchedAvatarObjectSlots.Add(node, avatarObjectSlot);

                if (avatarObjectSlot != null) avatarObjectSlot.Destroyed += (a) => { ClearCache(instance); };
            }
            else avatarObjectSlot = cache.SearchedAvatarObjectSlots[node];

	        if (avatarObjectSlot != null) return avatarObjectSlot.Slot;

	        return null!;
        }
    }
}
