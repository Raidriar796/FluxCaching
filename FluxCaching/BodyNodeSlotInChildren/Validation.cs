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
        // Used to clear the slot cache when it becomes invalid
        private static void ClearCache(BodyNodeSlotInChildren instance)
        {
            // SOME form of clearing no longer used entries in the main dictionary,
            // hopefully this is replaced later, there's likely a better way to do this
            foreach (BodyNodeSlotInChildren bodyNodeSlot in CachedBodyNodeSlotInChildrens.Keys)
            {
                if (CachedBodyNodeSlotInChildrens[bodyNodeSlot] == null)
                   CachedBodyNodeSlotInChildrens.Remove(bodyNodeSlot);
            }

            CachedBodyNodeSlotInChildrens[instance].CachedTargetSlot = null!;
            CachedBodyNodeSlotInChildrens[instance].CachedSlot = null!;
            CachedBodyNodeSlotInChildrens[instance].CachedAvatarObjectSlot = null!;
            CachedBodyNodeSlotInChildrens[instance].CachedBipedRig = null!;

            CachedBodyNodeSlotInChildrens[instance].IsBodyNodeSearched = false;
            CachedBodyNodeSlotInChildrens[instance].IsBipedRigSearched = false;

            CachedBodyNodeSlotInChildrens[instance].SearchedAvatarObjectSlots.Clear();
        }

        // If at any point a cache invalidation or other update occured, run the usual logic to fetch the body node slot
        // Additionally, events will be assigned to limit per update validation and to allow events to handle cache invalidation
        private static Slot GetSlotAndAssignEvents(BodyNodeSlotInChildren instance, Slot targetSlot, BodyNode node)
        {
            Slot slot = CustomFindSlotForNodeInChildren(instance, targetSlot, node);
            CachedBodyNodeSlotInChildrens[instance].CachedSlot = slot;
            CachedBodyNodeSlotInChildrens[instance].IsBodyNodeSearched = true;

            // Subscribe the found slot and all of it's parents up to the targetSlot root if they haven't been already
            if (slot != null)
            {
                ICollection<Slot> parentCollection = [];
                slot.GetAllParents(parentCollection, true);
                foreach (Slot tempSlot in parentCollection)
                {
                    if (CachedBodyNodeSlotInChildrens[instance].SubscribedSlots.Add(tempSlot))
                    {
                        tempSlot.Destroyed += (s) => { ClearCache(instance); };
                        tempSlot.ParentChanged += (s) => { ClearCache(instance); };
                    }
                }
            }

            return slot!;
        }

        // Checks cached data for changes that cannot be assigned to events
        private static Slot CheckForChanges(BodyNodeSlotInChildren instance, Slot targetSlot, BodyNode node)
        {
            Cache cache;
            bool shouldUpdate = false;

            // Null checks to exit early incase any of these are true
            if (targetSlot == null || targetSlot.IsDestroyed) return null!;

            // Creates a new Cache instance and adds it to the dictionary with the instance as the key
            if (!CachedBodyNodeSlotInChildrens.ContainsKey(instance))
            {
                cache = new(instance, targetSlot, node);
                CachedBodyNodeSlotInChildrens.Add(instance, cache);
            }
            // If the key already exists, simply reuse it
            else cache = CachedBodyNodeSlotInChildrens[instance];

            Slot slot = cache.CachedSlot!;

            // Caches the slot if it hasn't been searched for
            // Checking against a bool to not search again if the slot returns null
            if (!cache.IsBodyNodeSearched) shouldUpdate = true;
            
            // Reassigns the cached slot if the target slot doesn't match
            if (cache.CachedTargetSlot != targetSlot)
            {
                CachedBodyNodeSlotInChildrens[instance].CachedTargetSlot = targetSlot;
                shouldUpdate = true;
            }

            // Reassigns the BodyNode if the cached BodyNode doesn't match
            if (cache.CachedNode != node)
            {
                CachedBodyNodeSlotInChildrens[instance].CachedNode = node;
                shouldUpdate = true;
            }

            // Assigns the user's avatar object slot if it's not been assigned already
            if (cache.CachedAvatarObjectSlot == null)
            {
                cache.CachedAvatarObjectSlot = targetSlot.GetComponentInChildren<AvatarObjectSlot>();

                if (cache.CachedAvatarObjectSlot != null)
                {
                    CachedBodyNodeSlotInChildrens[instance].CachedAvatarObjectSlot = cache.CachedAvatarObjectSlot;

                    // Prevents resubscribing previously cached AvatarObjectSlots
                    if (CachedBodyNodeSlotInChildrens[instance].SubscribedAvatarObjectSlots.Add(cache.CachedAvatarObjectSlot))
                    {
                        CachedBodyNodeSlotInChildrens[instance].CachedAvatarObjectSlot.Equipped.OnValueChange += (v) => { ClearCache(instance); };
                        CachedBodyNodeSlotInChildrens[instance].CachedAvatarObjectSlot.Destroyed += (v) => { ClearCache(instance); };
                    }

                    shouldUpdate = true;
                }
            }

            if (shouldUpdate) return GetSlotAndAssignEvents(instance, targetSlot, node);

            return slot!;
        }
    }
}
