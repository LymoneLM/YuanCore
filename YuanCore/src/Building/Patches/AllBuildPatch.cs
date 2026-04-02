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
        var abc = __instance.gameObject.GetComponent<AllBuild>();
        Object.DestroyImmediate(abc);
        __instance.gameObject.AddComponent<AllBuildEx>();
        return false;
    }
}
