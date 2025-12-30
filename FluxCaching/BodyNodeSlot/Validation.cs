using FrooxEngine;
using FrooxEngine.CommonAvatar;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Avatar;
using Renderite.Shared;
using ResoniteModLoader;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    public partial class BodyNodeSlotCaching
    {
        // Used to clear the slot cache when it becomes invalid
        private static void ClearCache(BodyNodeSlot instance)
        {
            CachedBodyNodeSlots.Remove(instance);
        }

        // If at any point a cache invalidation or other update occured, run the usual logic to fetch the body node slot
        // Additionally, events will be assigned to limit per update validation and to allow events to handle cache invalidation
        private static Slot GetSlotAndAssignEvents(BodyNodeSlot instance, User user, BodyNode node)
        {
            Slot slot = CustomGetBodyNodeSlot(instance, user, node);
            CachedBodyNodeSlots[instance].CachedSlot = slot;
            CachedBodyNodeSlots[instance].IsBodyNodeSearched = true;

            if (slot != null)
            {
                slot.Destroyed += (s) => { ClearCache(instance); };
                slot.ParentChanged += (s) => { ClearCache(instance); };
                
                ICollection<Slot> parentCollection = [];
                slot.GetAllParents(parentCollection, true);
                foreach (Slot parent in parentCollection)
                {
                    parent.Destroyed += (s) => { ClearCache(instance); };
                    parent.ParentChanged += (s) => { ClearCache(instance); };
                }
            }

            return slot!;
        }

        // Checks cached data for changes that cannot be assigned to events
        private static Slot CheckForChanges(BodyNodeSlot instance, User user, BodyNode node)
        {
            Cache cache;

            // Probably overkill null checks to exit early incase any of these are true
            if (user == null || user.IsDestroyed ||
                user.Root == null || user.Root.IsDestroyed ||
                user.Root.Slot == null || user.Root.Slot.IsDestroyed)
            {
                return null!;
            }

            // Creates a new Cache instance and adds it to the dictionary with the instance as the key
            if (!CachedBodyNodeSlots.ContainsKey(instance))
            {
                cache = new(instance, user, node);
                CachedBodyNodeSlots.Add(instance, cache);
            }
            // If the key already exists, simply reuse it
            else
            {
                cache = CachedBodyNodeSlots[instance];
            }

            Slot slot = cache.CachedSlot!;

            // Caches the slot if it hasn't been searched for
            // Checking against a bool to not search again if the slot returns null
            if (!cache.IsBodyNodeSearched) return GetSlotAndAssignEvents(instance, user, node);
            
            // Reassigns the user if the cached user doesn't match
            if (cache.CachedUser != user)
            {
                CachedBodyNodeSlots[instance].CachedUser = user;
                return GetSlotAndAssignEvents(instance, user, node);
            }

            // Reassigns the BodyNode if the cached BodyNode doesn't match
            if (cache.CachedNode != node)
            {
                CachedBodyNodeSlots[instance].CachedNode = node;
                return GetSlotAndAssignEvents(instance, user, node);
            }
            
            // Assigns the user's avatar object slot if it's not been assigned already
            if (cache.CachedAvatarObjectSlot == null)
            {
                cache.CachedAvatarObjectSlot = user.Root.Slot.GetComponent<AvatarObjectSlot>();

                if (cache.CachedAvatarObjectSlot != null)
                {
                    CachedBodyNodeSlots[instance].CachedAvatarObjectSlot = cache.CachedAvatarObjectSlot;
                    CachedBodyNodeSlots[instance].CachedAvatarObjectSlot.Equipped.OnValueChange += (v) => { ClearCache(instance); };
                    CachedBodyNodeSlots[instance].CachedAvatarObjectSlot.Destroyed += (v) => { ClearCache(instance); };
                    return GetSlotAndAssignEvents(instance, user, node);
                }
            }

            return slot!;
        }
    }
}
