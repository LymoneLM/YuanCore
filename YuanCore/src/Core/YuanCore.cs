using BepInEx;
using BepInEx.Logging;

namespace YuanCore.Core;

[BepInPlugin(MODGUID, MODNAME, VERSION)]
public class YuanCorePlugin : BaseUnityPlugin
{
    public const string MODNAME = "YuanCore";
    public const string MODGUID = "cc.lymone.HoL." + MODNAME;
    public const string VERSION = "0.1.0";

    internal new static ManualLogSource Logger;

    public void Awake()
    {
        Logger = base.Logger;
    }
}
