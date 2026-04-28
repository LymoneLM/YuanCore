using HarmonyLib;
using UnityEngine;

namespace YuanCore.Building.Patches;

[HarmonyPatch(typeof(AllBuild))]
public class AllBuildPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("Awake")]
    public static bool ReplaceScript(AllBuild __instance)
    {
        __instance.enabled = false;
        var allBuild = __instance.gameObject.AddComponent<AllBuildEx>();
        var manager = __instance.gameObject.AddComponent<BuildingManager>();
        manager.SceneRoot = allBuild;
        return false;
    }
}
