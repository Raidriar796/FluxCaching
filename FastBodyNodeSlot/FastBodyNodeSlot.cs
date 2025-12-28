using HarmonyLib;
using ResoniteModLoader;

namespace FastBodyNodeSlot;

public partial class FastBodyNodeSlot : ResoniteMod
{
    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> enable =
        new("enable", "Enable FastBodyNodeSlot", () => true);

    public override string Name => "FastBodyNodeSlot";
    public override string Author => "Raidriar796";
    public override string Version => "0.1.0";
    public override string Link => "https://github.com/Raidriar796/FastBodyNodeSlot";
    private static ModConfiguration? Config;

    public override void OnEngineInit()
    {
        Harmony harmony = new("net.raidriar796.FastBodyNodeSlot");
        Config = GetConfiguration();
        Config?.Save(true);
        harmony.PatchAll();
    }

}
