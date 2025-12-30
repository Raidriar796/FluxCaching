using HarmonyLib;
using ResoniteModLoader;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    public override string Name => "FluxCaching";
    public override string Author => "Raidriar796";
    public override string Version => "0.2.0";
    public override string Link => "https://github.com/Raidriar796/FluxCaching";
    private static ModConfiguration? Config;

    public override void OnEngineInit()
    {
        Harmony harmony = new("net.raidriar796.FluxCaching");
        Config = GetConfiguration();
        Config?.Save(true);
        harmony.PatchAll();
    }
}
