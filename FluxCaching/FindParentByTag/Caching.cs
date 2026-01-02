using FrooxEngine;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Slots;
using ResoniteModLoader;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    public partial class FindParentByTagCaching
    {
        // Instantiated to cache data to compare for changes
        private class Cache(Slot targetSlot, string targetTag, int searchDepth)
        {
            public Slot CachedTargetSlot { get; set; } = targetSlot;
            public string CachedTargetTag { get; set; } = targetTag;
            public int CachedSearchedDepth { get; set; } = searchDepth;
            public Slot CachedSlot { get; set; } = null!;

            public HashSet<Slot> SubscribedSlots { get; } = [];
        }

        // Stores the instance of the FindParentByTag with it's cached results
        private static readonly Dictionary<FindParentByTag, Cache> CachedFindParentByTags = [];
    }
}
