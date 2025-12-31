using ResoniteModLoader;
using FrooxEngine;
using FrooxEngine.CommonAvatar;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Avatar;
using Renderite.Shared;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    public partial class BodyNodeSlotInChildrenCaching
    {
        private static Slot CustomFindSlotForNodeInChildren(BodyNodeSlotInChildren instance, Slot targetSlot, BodyNode node)
        {
            Cache cache;
            BipedRig bipedRig;
            AvatarObjectSlot searchedAvatarObjectSlot = null!;

            // Returns early if the dictionary does not have the BodyNodeSlot tracked yet
            if (CachedBodyNodeSlotInChildrens.ContainsKey(instance)) cache = CachedBodyNodeSlotInChildrens[instance];
            else return null!;

	        if (targetSlot == null || node == BodyNode.NONE) return null!;

            // Stores for the first time the biped rig is searched to avoid searching again if it's null
            if (!cache.IsBipedRigSearched)
            {
                cache.CachedBipedRig = targetSlot.GetComponentInChildren<BipedRig>();
                bipedRig = cache.CachedBipedRig;
                CachedBodyNodeSlotInChildrens[instance].CachedBipedRig = cache.CachedBipedRig;
                CachedBodyNodeSlotInChildrens[instance].IsBipedRigSearched = true;

                // Subscribe a newly cached BipedRig to clear the cache if it is destroyed
                if (cache.CachedBipedRig != null && CachedBodyNodeSlotInChildrens[instance].SubscribedBipedRigs.Add(cache.CachedBipedRig))
                    CachedBodyNodeSlotInChildrens[instance].CachedBipedRig.Destroyed += (b) => { ClearCache(instance); };
            }
            else if (cache.CachedBipedRig == null) bipedRig = null!;
            else bipedRig = cache.CachedBipedRig;

	        Slot bone = (bipedRig != null) ? bipedRig.TryGetBone(node) : null!;

	        if (bone != null) return bone;

            // Check if the body node being searched has been searched already,
            // cache it if it hasn't, reuse the cached results if it has.
            if (!cache.SearchedAvatarObjectSlots.ContainsKey(node))
            {
                searchedAvatarObjectSlot = targetSlot.FindSlotForNodeInChildren(node);
                CachedBodyNodeSlotInChildrens[instance].SearchedAvatarObjectSlots.Add(node, searchedAvatarObjectSlot);

                if (searchedAvatarObjectSlot != null && CachedBodyNodeSlotInChildrens[instance].SubscribedSearchedAvatarObjectSlots.Add(searchedAvatarObjectSlot))
                    searchedAvatarObjectSlot.Destroyed += (a) => { ClearCache(instance); };
            }
            else searchedAvatarObjectSlot = cache.SearchedAvatarObjectSlots[node];

	        if (searchedAvatarObjectSlot != null) return searchedAvatarObjectSlot.Slot;

	        return null!;
        }
    }
}
