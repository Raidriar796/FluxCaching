using ResoniteModLoader;

namespace FluxCaching;

public partial class FluxCaching : ResoniteMod
{
    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> enable =
        new("enable", "Enable FluxCaching", () => true);

    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> bodyNodeSlotCaching =
        new("bodyNodeSlotCaching", "BodyNodeSlot Caching", () => true);

    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> bodyNodeSlotInChildrenCaching =
        new("bodyNodeSlotInChildrenCaching", "BodyNodeSlotInChildren Caching", () => true);

    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> findChildByNameCaching =
        new("findChildByNameCaching", "FindChildByName Caching", () => true);
}
