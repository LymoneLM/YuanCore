using System;

namespace YuanCore.Building;

public static class SceneIDResolver
{
    public static (string SceneType, int SceneSize) GetSceneType(string sceneID)
    {
        var parts = sceneID.Split('|');
        var sceneType = parts[0];
        var index = int.Parse(parts[1]);
        var index2 = parts.Length > 2 ? int.Parse(parts[2]) : 0;

        var sceneSize = sceneType switch
        {
            "M" => int.Parse(Mainload.Fudi_now[index][37]),
            "Z" => int.Parse(Mainload.NongZ_now[index][index2][5]),
            "S" => index,
            "F" => -1,
            "H" => index,
            "L" => int.Parse(Mainload.Mudi_now[index][index2][2]),
            _ => throw new ArgumentOutOfRangeException()
        };
        return (sceneType, sceneSize);
    }
}
