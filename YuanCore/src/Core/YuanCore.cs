using BepInEx;

namespace YuanCore.Core;

[BepInPlugin(MODGUID, MODNAME, VERSION)]
public class YuanCore : BaseUnityPlugin
{
    public const string MODNAME = "YuanCore";
    public const string MODGUID = "cc.lymone.HoL." + MODNAME;
    public const string VERSION = "0.1.0";
}
