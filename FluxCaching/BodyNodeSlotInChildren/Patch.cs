using FrooxEngine;
using FrooxEngine.ProtoFlux;
using HarmonyLib;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Avatar;
using Renderite.Shared;
using ResoniteModLoader;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    public partial class BodyNodeSlotInChildrenCaching
    {
        [HarmonyPatch(typeof(BodyNodeSlotInChildren), "Compute")]
        private class BodyNodeSlotInChildrenPatch
        {
            private static bool Prefix(ref FrooxEngineContext context, BodyNodeSlotInChildren __instance, ref Slot __result)
            {
                // Run original method if the mod is disabled
                if (!Config!.GetValue(enable) || !Config.GetValue(bodyNodeSlotInChildrenCaching)) return true;

                Slot slot = ProtoFlux.Runtimes.Execution.ExecutionContextExtensions.ReadObject<Slot>(0, context);
                BodyNode node = ProtoFlux.Runtimes.Execution.ExecutionContextExtensions.ReadValue<BodyNode>(1, context);

                // Run custom method instead of GetBodyNodeSlotInChildren
                __result = CheckForChanges(__instance, slot, node);

                // Skip original method
	            return false;
            }
        }
    }
}
