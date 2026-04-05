using System;
using System.Collections.Generic;
using UnityEngine;

namespace YuanCore.Building;

public static class BuildingDataAdapter
{
    public static List<BuildingDto> Load(string sceneID)
    {
        var parts = sceneID.Split('|');
        var sceneClass = parts[0];
        var sceneIndex = int.Parse(parts[1]);
        var subSceneIndex = parts.Length > 2 ? int.Parse(parts[2]) : 0;

        PrepareLegacySceneData(sceneClass, sceneIndex, subSceneIndex);

        return sceneClass switch
        {
            "M" => AdaptM(sceneIndex),
            "Z" => AdaptZ(sceneIndex, subSceneIndex),
            "S" => AdaptS(sceneIndex),
            "H" => AdaptH(sceneIndex),
            "L" => AdaptL(sceneIndex, subSceneIndex),
            _ => []
        };
    }

    private static void PrepareLegacySceneData(string sceneClass, int sceneIndex, int subSceneIndex)
    {
        if (sceneClass == "Z")
        {
            string[] array = Mainload.NongZ_now[sceneIndex][subSceneIndex][24].Split('|');
            Mainload.LastNonghuNum_Open =
            [
                float.Parse(array[0]),
                float.Parse(array[1]),
                float.Parse(array[2])
            ];
            SaveData.ReadBuildData(sceneClass, sceneIndex.ToString(), subSceneIndex.ToString());
        }
        else if (sceneClass == "L")
        {
            SaveData.ReadBuildData(sceneClass, sceneIndex.ToString(), subSceneIndex.ToString());
        }
        else if (sceneClass == "S")
        {
            SaveData.ReadBuildData(sceneClass, sceneIndex.ToString(), "0");

            if (Mainload.ShopData_updateTime[sceneIndex].Count > 0)
                return;
            for (var i = 0; i < Mainload.BuildInto_s.Count; i++)
            {
                Mainload.ShopData_updateTime[sceneIndex].Add(0);
                Mainload.Prop_shop_temp[sceneIndex].Add([]);
                Mainload.Horse_Shop_Temp[sceneIndex].Add([]);
                Mainload.HuaiZhang_Shop_Temp[sceneIndex].Add([]);
                Mainload.OtherTrade_shop_Temp[sceneIndex].Add([]);
            }
        }
        else if (!Mainload.isFirstGame)
        {
            SaveData.ReadBuildData(sceneClass, sceneIndex.ToString(), "0");
        }
    }

    private static List<BuildingDto> AdaptM(int sceneIndex)
    {
        var result = new List<BuildingDto>(Mainload.BuildInto_m.Count);
        foreach (var row in Mainload.BuildInto_m)
        {
            var (rotate, isRuin) = ParseRotation(row[6]);
            result.Add(new BuildingDtoM
            {
                Uid = row[0],
                BuildingLevel = int.Parse(row[2]),
                TaoZhuangID = int.Parse(row[7]),
                BuildingID = int.Parse(row[1]),
                Rotation = rotate,
                IsRuined = isRuin,
                GridPosition = ParseGrid(row[5]),

                ServantCount = int.Parse(row[3]),
                MonthlyPaymentLevel = int.Parse(row[4]),
            });
        }
        return result;
    }

    private static List<BuildingDto> AdaptZ(int sceneIndex, int subSceneIndex)
    {
        var result = new List<BuildingDto>(Mainload.BuildInto_z.Count);
        for (int i = 0; i < Mainload.BuildInto_z.Count; i++)
        {
            var row = Mainload.BuildInto_z[i];
            result.Add(new BuildingDto
            {
                Uid = $"Z:{sceneIndex}:{subSceneIndex}:{i}",
                BuildingID = int.Parse(row[1]),
                TaoZhuangID = int.Parse(row[10]),
            });
        }
        return result;
    }

    private static List<BuildingDto> AdaptS(int sceneIndex)
    {
        var result = new List<BuildingDto>(Mainload.BuildInto_s.Count + Mainload.BuildInto_c.Count);

        for (int i = 0; i < Mainload.BuildInto_s.Count; i++)
        {
            var row = Mainload.BuildInto_s[i];
            result.Add(new BuildingDto
            {
                Uid = $"S:{sceneIndex}:S:{i}",
                BuildingID = int.Parse(row[1]),
                TaoZhuangID = int.Parse(row[11]),
            });
        }

        for (int i = 0; i < Mainload.BuildInto_c.Count; i++)
        {
            var row = Mainload.BuildInto_c[i];
            result.Add(new BuildingDto
            {
                Uid = $"S:{sceneIndex}:C:{i}",
                BuildingID = int.Parse(row[1]),
                TaoZhuangID = int.Parse(row[6]),
            });
        }

        return result;
    }

    private static List<BuildingDto> AdaptH(int sceneIndex)
    {
        var result = new List<BuildingDto>(Mainload.BuildInto_h.Count);
        for (int i = 0; i < Mainload.BuildInto_h.Count; i++)
        {
            var row = Mainload.BuildInto_h[i];
            result.Add(new BuildingDto
            {
                Uid = $"H:{sceneIndex}:{i}",
                BuildingID = int.Parse(row[1]),
                TaoZhuangID = int.Parse(row[5]),
            });
        }
        return result;
    }

    private static List<BuildingDto> AdaptL(int sceneIndex, int subSceneIndex)
    {
        var result = new List<BuildingDto>(Mainload.BuildInto_l.Count);
        for (int i = 0; i < Mainload.BuildInto_l.Count; i++)
        {
            var row = Mainload.BuildInto_l[i];
            result.Add(new BuildingDto
            {
                Uid = $"L:{sceneIndex}:{subSceneIndex}:{i}",
                BuildingID = int.Parse(row[1]),
                TaoZhuangID = int.Parse(row[5]),
            });
        }
        return result;
    }

    private static Vector2Int ParseGrid(string value)
    {
        var arr = value.Split('|');
        return new Vector2Int(int.Parse(arr[0]), int.Parse(arr[1]));
    }

    private static (BuildingRotation, bool) ParseRotation(string raw)
    {
        var value = int.Parse(raw);
        return value < 0 ?
            ((BuildingRotation)(-value-1), true) :
            ((BuildingRotation)value, false);
    }
}
