using FrooxEngine;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Slots;
using ResoniteModLoader;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    public partial class FindChildByNameCaching
    {
        // Used to clear the slot cache when it becomes invalid
        private static void ClearCache(FindChildByName instance)
        {
            Cache cache = CachedFindChildByNames[instance];

            // SOME form of clearing no longer used entries in the main dictionary,
            // hopefully this is replaced later, there's likely a better way to do this
            foreach (FindChildByName findChildByName in CachedFindChildByNames.Keys)
            {
                if (CachedFindChildByNames[findChildByName] == null)
                   CachedFindChildByNames.Remove(findChildByName);
            }

            cache.CachedTargetSlot = null!;
            cache.CachedName = null!;
            cache.CachedSlot = null!;
        }

        // If at any point a cache invalidation or other update occured, run the usual logic to fetch the body node slot
        // Additionally, events will be assigned to limit per update validation and to allow events to handle cache invalidation
        private static Slot GetSlotAndAssignEvents(FindChildByName instance, Slot targetSlot, string name, bool matchSubstring, bool ignoreCase, int searchDepth)
        {
            if (targetSlot == null) return null!;

            Cache cache = CachedFindChildByNames[instance];
            Slot slot = targetSlot.FindChild(name, matchSubstring, ignoreCase, searchDepth);
            cache.CachedSlot = slot;

            // This gets every parent and subscribes events to invalidate the cache
            ICollection<Slot> slotCollection = [];
            targetSlot.GetAllParents(slotCollection, false);
            foreach (Slot tempSlot in slotCollection)
            {
                if (cache.SubscribedSlots.Add(tempSlot))
                {
                    tempSlot.Destroyed += (s) => { ClearCache(instance); };
                }
            }

            // This gets every child and subscribes events to invalidate the cache
            targetSlot.GetAllChildren(slotCollection, true);
            foreach (Slot tempSlot in slotCollection)
            {
                if (cache.SubscribedSlots.Add(tempSlot))
                {
                    tempSlot.ChildAdded += (s, ss) => { ClearCache(instance); };
                    tempSlot.ChildRemoved += (s, ss) => { ClearCache(instance); };
                    tempSlot.NameChanged += (s) => { ClearCache(instance); };
                    tempSlot.ParentChanged += (s) => { ClearCache(instance); };
                    tempSlot.Destroyed += (s) => { ClearCache(instance); };
                }
            }

            return slot;
        }

        // Checks cached data for changes that cannot be assigned to events
        private static Slot CheckForChanges(FindChildByName instance, Slot targetSlot, string name, bool matchSubstring, bool ignoreCase, int searchDepth)
        {
            Cache cache;
            bool shouldUpdate = false;

            // Creates a new Cache instance and adds it to the dictionary with the instance as the key
            if (!CachedFindChildByNames.ContainsKey(instance))
            {
                cache = new(targetSlot, name, matchSubstring, ignoreCase, searchDepth);
                CachedFindChildByNames.Add(instance, cache);
            }
            // If the key already exists, simply reuse it
            else
            {
                cache = CachedFindChildByNames[instance];
            }
            
            Slot slot = cache.CachedSlot;
            
            if (cache.CachedTargetSlot != targetSlot)
            {
                cache.CachedTargetSlot = targetSlot;
                shouldUpdate = true;
            }

            if (cache.CachedName != name)
            {
                cache.CachedName = name;
                shouldUpdate = true;
            }

            if (cache.CachedMatchSubstring != matchSubstring)
            {
                cache.CachedMatchSubstring = matchSubstring;
                shouldUpdate = true;
            }

            if (cache.CachedIgnoreCase != ignoreCase)
            {
                cache.CachedIgnoreCase = ignoreCase;
                shouldUpdate = true;
            }

            if (cache.CachedSearchedDepth != searchDepth)
            {
                cache.CachedSearchedDepth = searchDepth;
                shouldUpdate = true;
            }

            if (shouldUpdate || slot == null) return GetSlotAndAssignEvents(instance, targetSlot, name, matchSubstring, ignoreCase, searchDepth);

            return slot;
        }
    }
}
