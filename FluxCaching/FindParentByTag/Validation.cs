using FrooxEngine;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Slots;
using ResoniteModLoader;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    public partial class FindParentByTagCaching
    {
        // Used to clear the slot cache when it becomes invalid
        private static void ClearCache(FindParentByTag instance, Slot targetSlot, string targetTag, int searchDepth)
        {
            // SOME form of clearing no longer used entries in the main dictionary,
            // hopefully this is replaced later, there's likely a better way to do this
            foreach (FindParentByTag findChildByName in CachedFindParentByTags.Keys)
            {
                if (CachedFindParentByTags[findChildByName] == null)
                   CachedFindParentByTags.Remove(findChildByName);
            }

            if (!CachedFindParentByTags.ContainsKey(instance)) return;

            CachedFindParentByTags[instance].cache = new(targetSlot, targetTag, searchDepth);
        }

        // If at any point a cache invalidation or other update occured, run the usual logic to fetch the body node slot
        // Additionally, events will be assigned to limit per update validation and to allow events to handle cache invalidation
        private static Slot GetSlotAndAssignEvents(FindParentByTag instance, Slot targetSlot, string targetTag, int searchDepth)
        {
            if (targetSlot == null) return null!;

            Data data = CachedFindParentByTags[instance];
            Cache cache = data.cache;
            Slot slot = targetSlot.FindParent(s => s.Tag == targetTag, searchDepth);
            cache.CachedSlot = slot;

            ICollection<Slot> slotCollection = [];

            // This gets every parent and subscribes events to invalidate the cache
            targetSlot.GetAllParents(slotCollection, true);
            foreach (Slot tempSlot in slotCollection)
            {
                if (data.SubscribedSlots.Add(tempSlot))
                {
                    tempSlot.ChildAdded += (s, ss) => { ClearCache(instance, targetSlot, targetTag, searchDepth); };
                    tempSlot.ChildRemoved += (s, ss) => { ClearCache(instance, targetSlot, targetTag, searchDepth); };
                    tempSlot.Tag_Field.OnValueChange += (v) => { ClearCache(instance, targetSlot, targetTag, searchDepth); };
                    tempSlot.ParentChanged += (s) => { ClearCache(instance, targetSlot, targetTag, searchDepth); };
                    tempSlot.Destroyed += (s) => { ClearCache(instance, targetSlot, targetTag, searchDepth); };
                }
            }

            return slot;
        }

        // Checks cached data for changes that cannot be assigned to events
        private static Slot CheckForChanges(FindParentByTag instance, Slot targetSlot, string targetTag, int searchDepth)
        {
            Cache cache;
            bool shouldUpdate = false;

            // Creates a new Cache instance and adds it to the dictionary with the instance as the key
            if (!CachedFindParentByTags.ContainsKey(instance))
            {
                cache = new(targetSlot, targetTag, searchDepth);
                Data data = new(cache);
                CachedFindParentByTags.Add(instance, data);
            }
            // If the key already exists, simply reuse it
            else cache = CachedFindParentByTags[instance].cache;
            
            Slot slot = cache.CachedSlot;

            if (cache.CachedTargetSlot != targetSlot)
            {
                cache.CachedTargetSlot = targetSlot;
                shouldUpdate = true;
            }

            if (cache.CachedTargetTag != targetTag)
            {
                cache.CachedTargetTag = targetTag;
                shouldUpdate = true;
            }

            if (cache.CachedSearchedDepth != searchDepth)
            {
                cache.CachedSearchedDepth = searchDepth;
                shouldUpdate = true;
            }

            if (shouldUpdate || slot == null) return GetSlotAndAssignEvents(instance, targetSlot, targetTag, searchDepth);

            return slot;
        }
    }
}
