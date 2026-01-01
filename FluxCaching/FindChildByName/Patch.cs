using FrooxEngine;
using FrooxEngine.ProtoFlux;
using HarmonyLib;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Slots;
using Renderite.Shared;
using ResoniteModLoader;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    public partial class FindChildByNameCaching
    {
        [HarmonyPatch(typeof(FindChildByName), "Compute")]
        private class BodyNodeSlotPatch
        {
            private static bool Prefix(ref FrooxEngineContext context, FindChildByName __instance, ref Slot __result)
            {
                // Run original method if the mod is disabled
                if (!Config!.GetValue(enable) || !Config.GetValue(findChildByNameCaching)) return true;

                Slot targetSlot = ProtoFlux.Runtimes.Execution.ExecutionContextExtensions.ReadObject<Slot>(0, context);
                // if (targetSlot == null)
                // {
                //     __result = null!;
                //     return false;
                // }
                string name = ProtoFlux.Runtimes.Execution.ExecutionContextExtensions.ReadObject<string>(1, context);
                bool matchSubstring = ProtoFlux.Runtimes.Execution.ExecutionContextExtensions.ReadValue<bool>(2, context);
                bool ignoreCase = ProtoFlux.Runtimes.Execution.ExecutionContextExtensions.ReadValue<bool>(3, context);
                int searchDepth = ProtoFlux.Runtimes.Execution.ExecutionContextExtensions.ReadValue<int>(4, context);
                // Run custom method instead of FindChild
                __result = CheckForChanges(__instance, targetSlot, name, matchSubstring, ignoreCase, searchDepth);

                // Skip original method
	            return false;
            }
        }
    }
}
