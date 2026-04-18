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
                Uid                 = row[0],
                BuildingID          = int.Parse(row[1]),
                BuildingLevel       = int.Parse(row[2]),
                ServantCount        = int.Parse(row[3]),
                MonthlyPaymentLevel = int.Parse(row[4]),
                GridPosition        = ParseGrid(row[5]),
                Rotation            = rotate,
                IsRuined            = isRuin,
                TaoZhuangID         = int.Parse(row[7]),
            });
        }
        return result;
    }

    private static List<BuildingDto> AdaptZ(int sceneIndex, int subSceneIndex)
    {
        var result = new List<BuildingDto>(Mainload.BuildInto_z.Count);
        foreach (var row in Mainload.BuildInto_z)
        {
            var (rotate, isRuin) = ParseRotation(row[4]);
            result.Add(new BuildingDtoZ
            {
                Uid             = row[0],
                BuildingID      = int.Parse(row[1]),
                BuildingLevel   = int.Parse(row[2]),
                GridPosition    = ParseGrid(row[3]),
                Rotation        = rotate,
                IsRuined        = isRuin,
                FieldSize       = float.Parse(row[5]),
                PlantTime       = row[6],
                OutputAmountA   = int.Parse(row[7]),
                NextHarvestTime = row[8],
                WorkState       = int.Parse(row[9]),
                TaoZhuangID     = int.Parse(row[10]),
                OutputAmountB   = int.Parse(row[11]),
            });
        }
        return result;
    }

    private static List<BuildingDto> AdaptS(int sceneIndex)
    {
        var result = new List<BuildingDto>(Mainload.BuildInto_s.Count + Mainload.BuildInto_c.Count);

        foreach (var row in Mainload.BuildInto_s)
        {
            var (rotation, isRuined) = ParseRotation(row[5]);
            result.Add(new BuildingDtoS
            {
                Uid               = row[0],
                BuildingID        = int.Parse(row[1]),
                BuildingLevel     = int.Parse(row[2]),
                EmployeeCount     = int.Parse(row[3]),
                GridPosition      = ParseGrid(row[4]),
                Rotation          = rotation,
                IsRuined          = isRuined,
                ShopOwnerID       = row[6],
                IsOpen            = row[7] == "1",
                SalaryBase        = int.Parse(row[8]),
                AccumulatedProfit = int.Parse(row[9]),
                LastUpdateTime    = row[10],
                TaoZhuangID       = int.Parse(row[11]),
                YuanbaoSpent      = int.Parse(row[12]),
            });
        }

        foreach (var row in Mainload.BuildInto_c)
        {
            var (rotation, isRuined) = ParseRotation(row[4]);
            result.Add(new BuildingDtoC
            {
                Uid            = row[0],
                BuildingID     = int.Parse(row[1]),
                BuildingLevel  = int.Parse(row[2]),
                GridPosition   = ParseGrid(row[3]),
                Rotation       = rotation,
                IsRuined       = isRuined,
                XueFengClass   = int.Parse(row[5]),
                TaoZhuangID    = int.Parse(row[6]),
                TuitionCost    = int.Parse(row[7]),
                LastUpdateTime = row[8],
            });
        }

        return result;
    }

    private static List<BuildingDto> AdaptH(int sceneIndex)
    {
        var result = new List<BuildingDto>(Mainload.BuildInto_h.Count);
        foreach (var row in Mainload.BuildInto_h)
        {
            var (rotation, isRuined) = ParseRotation(row[4]);
            result.Add(new BuildingDto
            {
                Uid           = row[0],
                BuildingID    = int.Parse(row[1]),
                BuildingLevel = int.Parse(row[2]),
                GridPosition  = ParseGrid(row[3]),
                Rotation      = rotation,
                IsRuined      = isRuined,
                TaoZhuangID   = int.Parse(row[5]),
            });
        }
        return result;
    }

    private static List<BuildingDto> AdaptL(int sceneIndex, int subSceneIndex)
    {
        var result = new List<BuildingDto>(Mainload.BuildInto_l.Count);
        foreach (var row in Mainload.BuildInto_l)
        {
            var (rotation, isRuined) = ParseRotation(row[4]);
            result.Add(new BuildingDtoL
            {
                Uid          = row[0],
                BuildingID   = int.Parse(row[1]),
                BuildingLevel = int.Parse(row[2]),
                GridPosition = ParseGrid(row[3]),
                Rotation     = rotation,
                IsRuined     = isRuined,
                TaoZhuangID  = int.Parse(row[5]),
                DaiZangData  = row[6] == "null" ? null : row[6],
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
