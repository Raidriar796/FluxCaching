using FrooxEngine;
using FrooxEngine.ProtoFlux;
using HarmonyLib;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Slots;
using ResoniteModLoader;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    public partial class FindParentByTagCaching
    {
        [HarmonyPatch(typeof(FindParentByTag), "Compute")]
        private class FindParentByTagPatch
        {
            private static bool Prefix(ref FrooxEngineContext context, FindParentByTag __instance, ref Slot __result)
            {
                // Run original method if the mod is disabled
                if (!Config!.GetValue(enable) || !Config.GetValue(findParentByTagCaching)) return true;

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
