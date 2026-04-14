using System;
using System.Collections.Generic;
using UnityEngine;

// <summary>
// AI写的建筑数据传输对象 (DTO) 定义，不好说能不能用  
// 总之先传上去，免得忘了

namespace YuanCore.Building
{
    // --- 1. 基础 DTO (所有建筑共有) ---
    public class BuildingDto
    {
        public string Uid;
        public int BuildingLevel;
        public int TaoZhuangID;
        public int BuildingID;
        public BuildingRotation Rotation; // 假设你定义了这个枚举
        public bool IsRuined;
        public Vector2Int GridPosition;
    }

    // --- 2. 宅邸系列 DTO (M) ---
    public class BuildingDtoM : BuildingDto
    {
        public int ServantCount;          // [3] 仆人数量
        public int MonthlyPaymentLevel;   // [4] 月例等级
    }

    // --- 3. 城市商铺/附属 DTO (S) ---
    public class BuildingDtoS : BuildingDto
    {
        public int AffiliationId;         // [3] 附加数据1 (推测：所属势力ID)
        public int InteractionFlag;       // [6] 附加数据2 (条件判断标志)
        public bool IsSelling;            // [7] 出售标志
        public int ShopFunds;             // [8] 商铺资金 / 繁荣度
        public int UnknownData4;          // [9] 附加数据4
        public Vector2Int Size;           // [10] 建筑尺寸
        public int UnknownData7;          // [12] 附加数据7
    }

    // --- 4. 城市民居/自定义 DTO (C) ---
    public class BuildingDtoC : BuildingDto
    {
        public int UnknownData1;          // [5] 附加数据1
        public int UnknownData2;          // [7] 附加数据2
        public Vector2Int Size;           // [8] 建筑尺寸
    }

    // --- 5. 农庄/资源点 DTO (Z) ---
    public class BuildingDtoZ : BuildingDto
    {
        public float ProductivityModifier; // [5] 土地肥力/生产倍率
        public Vector2Int PlantTime;       // [6] 种植时间/阶段
        public int PrimaryOutputAmount;    // [7] 主产物积累量
        public Vector2Int LastUpdateTime;  // [8] 上次结算时间
        public bool HasWorkersAssigned;    // [9] 农户安排开关
        public int SecondaryOutputAmount;  // [11] 副产物积累量
    }

    // (H 皇城 和 L 陵墓直接使用基类 BuildingDto，无需子类)


    // --- 6. 终极数据解析工厂 ---
    public class BuildingDataParser
    {
        /// <summary>
        /// 将原版的字符串列表根据系列标识（M, S, C, Z, H, L）解析为强类型 DTO
        /// </summary>
        public static BuildingDto Parse(List<string> rawData, string seriesType)
        {
            if (rawData == null || rawData.Count < 6)
            {
                Debug.LogWarning($"[BuildingDataParser] 建筑数据异常或过短。系列: {seriesType}");
                return null;
            }

            BuildingDto baseDto = null;

            // 动态索引：用于处理不同系列中基础数据被特殊数据挤压产生的位置偏移
            int posIdx = 0;   // 坐标索引
            int stateIdx = 0; // 状态索引
            int setIdx = 0;   // 套装ID索引

            // 第一步：根据系列实例化对应的子类，并解析其特有数据
            switch (seriesType.ToUpper())
            {
                case "M": // 宅邸 (长度 8)
                    var dtoM = new BuildingDtoM();
                    dtoM.ServantCount = ParseIntSafe(rawData, 3);
                    dtoM.MonthlyPaymentLevel = ParseIntSafe(rawData, 4);
                    baseDto = dtoM;
                    
                    posIdx = 5; stateIdx = 6; setIdx = 7;
                    break;

                case "S": // 商铺 (长度 13)
                    var dtoS = new BuildingDtoS();
                    dtoS.AffiliationId = ParseIntSafe(rawData, 3);
                    dtoS.InteractionFlag = ParseIntSafe(rawData, 6);
                    dtoS.IsSelling = (rawData.Count > 7 && rawData[7] == "1");
                    dtoS.ShopFunds = ParseIntSafe(rawData, 8);
                    dtoS.UnknownData4 = ParseIntSafe(rawData, 9);
                    dtoS.Size = ParseVector2(rawData, 10);
                    dtoS.UnknownData7 = ParseIntSafe(rawData, 12);
                    baseDto = dtoS;

                    posIdx = 4; stateIdx = 5; setIdx = 11;
                    break;

                case "C": // 自定义/民居 (长度 9)
                    var dtoC = new BuildingDtoC();
                    dtoC.UnknownData1 = ParseIntSafe(rawData, 5);
                    dtoC.UnknownData2 = ParseIntSafe(rawData, 7);
                    dtoC.Size = ParseVector2(rawData, 8);
                    baseDto = dtoC;

                    posIdx = 3; stateIdx = 4; setIdx = 6;
                    break;

                case "Z": // 农庄/资源 (长度 12)
                    var dtoZ = new BuildingDtoZ();
                    dtoZ.ProductivityModifier = ParseFloatSafe(rawData, 5);
                    dtoZ.PlantTime = ParseVector2(rawData, 6);
                    dtoZ.PrimaryOutputAmount = ParseIntSafe(rawData, 7);
                    dtoZ.LastUpdateTime = ParseVector2(rawData, 8);
                    dtoZ.HasWorkersAssigned = (rawData.Count > 9 && rawData[9] != "0");
                    dtoZ.SecondaryOutputAmount = ParseIntSafe(rawData, 11);
                    baseDto = dtoZ;

                    posIdx = 3; stateIdx = 4; setIdx = 10;
                    break;

                case "H": // 皇城 (长度 6)
                case "L": // 陵墓 (长度 6)
                    baseDto = new BuildingDto();
                    posIdx = 3; stateIdx = 4; setIdx = 5;
                    break;

                default:
                    Debug.LogError($"[BuildingDataParser] 未知的建筑系列标识: {seriesType}");
                    return null;
            }

            // 第二步：灌入所有建筑共有的基础数据
            baseDto.Uid = rawData[0];
            baseDto.BuildingID = ParseIntSafe(rawData, 1);
            baseDto.BuildingLevel = ParseIntSafe(rawData, 2);
            
            // 动态索引数据
            baseDto.GridPosition = ParseVector2(rawData, posIdx);
            baseDto.TaoZhuangID = ParseIntSafe(rawData, setIdx);

            // 解析 State (破损与旋转)
            int state = ParseIntSafe(rawData, stateIdx);
            baseDto.IsRuined = (state < 0);
            // 假设旋转枚举是用绝对值表示的，如有其他逻辑可在此修改
            baseDto.Rotation = (BuildingRotation)Mathf.Abs(state); 

            return baseDto;
        }

        // --- 辅助安全解析方法 ---

        private static int ParseIntSafe(List<string> data, int index)
        {
            if (index >= 0 && index < data.Count && int.TryParse(data[index], out int result))
            {
                return result;
            }
            return 0; // 默认值
        }

        private static float ParseFloatSafe(List<string> data, int index)
        {
            if (index >= 0 && index < data.Count && float.TryParse(data[index], out float result))
            {
                return result;
            }
            return 0f;
        }

        private static Vector2Int ParseVector2(List<string> data, int index)
        {
            if (index >= 0 && index < data.Count && !string.IsNullOrEmpty(data[index]))
            {
                string[] parts = data[index].Split('|');
                if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                {
                    return new Vector2Int(x, y);
                }
            }
            return Vector2Int.zero;
        }
    }

    // 占位用的枚举，如果你的项目里已经有了请删掉这个
    public enum BuildingRotation
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }
}