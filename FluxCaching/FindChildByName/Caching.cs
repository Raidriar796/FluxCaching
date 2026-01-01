using Elements.Core;
using FrooxEngine;
using FrooxEngine.CommonAvatar;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Slots;
using Renderite.Shared;
using ResoniteModLoader;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    public partial class FindChildByNameCaching
    {
        // Instantiated to cache data to compare for changes
        private class Cache(Slot targetSlot, string name, bool matchSubstring, bool ignoreCase, int searchDepth)
        {
            public Slot CachedTargetSlot { get; set; } = targetSlot;
            public string CachedName { get; set; } = name;
            public bool CachedMatchSubstring { get; set; } = matchSubstring;
            public bool CachedIgnoreCase { get; set; } = ignoreCase;
            public int CachedSearchedDepth { get; set; } = searchDepth;
            public Slot CachedSlot { get; set; } = null!;

            public HashSet<Slot> SubscribedSlots { get; } = [];
        }

        // Stores the instance of the BodyNodeSlot with it's cached results
        private static readonly Dictionary<FindChildByName, Cache> CachedFindChildByNames = [];
    }
}
