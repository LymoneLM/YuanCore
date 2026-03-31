using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace YuanCore.Core;

[BepInPlugin(MODGUID, MODNAME, VERSION)]
public class YuanCorePlugin : BaseUnityPlugin
{
    public const string MODNAME = "YuanCore";
    public const string MODGUID = "cc.lymone.HoL." + MODNAME;
    public const string VERSION = "0.1.0";

    internal new static ManualLogSource Logger;
    internal static Harmony Harmony = new(MODGUID);

    private void Awake()
    {
        Logger = base.Logger;
        Harmony.PatchAll();
    }
}
