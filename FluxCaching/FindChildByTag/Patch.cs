using FrooxEngine;
using FrooxEngine.ProtoFlux;
using HarmonyLib;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Slots;
using Renderite.Shared;
using ResoniteModLoader;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    public partial class FindChildByTagCaching
    {
        [HarmonyPatch(typeof(FindChildByTag), "Compute")]
        private class FindChildByTagPatch
        {
            private static bool Prefix(ref FrooxEngineContext context, FindChildByTag __instance, ref string __result)
            {
                // Run original method if the mod is disabled
                if (!Config!.GetValue(enable) || !Config.GetValue(findChildByTagCaching)) return true;

                Slot targetSlot = ProtoFlux.Runtimes.Execution.ExecutionContextExtensions.ReadObject<Slot>(0, context);
                string targetTag = ProtoFlux.Runtimes.Execution.ExecutionContextExtensions.ReadObject<string>(1, context);
                int searchDepth = ProtoFlux.Runtimes.Execution.ExecutionContextExtensions.ReadValue<int>(2, context);
                // Run custom method instead of FindChild
                __result = CheckForChanges(__instance, targetSlot, targetTag, searchDepth);

                // Skip original method
	            return false;
            }
        }
    }
}
