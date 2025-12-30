using FrooxEngine;
using FrooxEngine.ProtoFlux;
using HarmonyLib;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Avatar;
using Renderite.Shared;
using ResoniteModLoader;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    public partial class BodyNodeSlotCaching
    {
        [HarmonyPatch(typeof(BodyNodeSlot), "Compute")]
        private class BodyNodeSlotPatch
        {
            private static bool Prefix(ref FrooxEngineContext context, BodyNodeSlot __instance, ref Slot __result)
            {
                // Run original method if the mod is disabled
                if (!Config!.GetValue(enable)) return true;

                // Recreation of the original Compute method
                User user;

                if (__instance.Source.Source == null) user = context.World.LocalUser;

                else user = ProtoFlux.Runtimes.Execution.ExecutionContextExtensions.ReadObject<User>(0, context);

                BodyNode node = ProtoFlux.Runtimes.Execution.ExecutionContextExtensions.ReadValue<BodyNode>(1, context);
                // Run custom method instead of GetBodyNodeSlot
                __result = CheckForChanges(__instance, user, node);

                // Skip original method
	            return false;
            }
        }
    }
}
