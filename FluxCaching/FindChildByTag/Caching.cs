using FrooxEngine;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Slots;
using ResoniteModLoader;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    public partial class FindChildByTagCaching
    {
        // Instantiated to cache data to compare for changes
        private class Cache(Slot targetSlot, string targetTag, int searchDepth)
        {
            public Slot CachedTargetSlot { get; set; } = targetSlot;
            public string CachedTargetTag { get; set; } = targetTag;
            public int CachedSearchedDepth { get; set; } = searchDepth;
            public string CachedTag { get; set; } = null!;

            public HashSet<Slot> SubscribedSlots { get; } = [];
        }

        // Stores the instance of the FindChildByTag with it's cached results
        private static readonly Dictionary<FindChildByTag, Cache> CachedFindChildByTags = [];
    }
}
