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
        __instance.gameObject.AddComponent<AllBuildEx>();
        __instance.gameObject.AddComponent<BuildingManager>();
        return false;
    }
}
