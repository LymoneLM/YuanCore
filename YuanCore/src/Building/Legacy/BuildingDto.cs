using UnityEngine;

namespace YuanCore.Building;

public class BuildingDto
{
    public string Uid;
    public int BuildingLevel;
    public int TaoZhuangID;
    public int BuildingID;
    public BuildingRotation Rotation;
    public bool IsRuined;
    public Vector2Int GridPosition;
}

public class BuildingDtoM : BuildingDto
{
    public int ServantCount;
    public int MonthlyPaymentLevel;
}

public class BuildingDtoZ : BuildingDto
{
    public float FertilityRate;
    public Vector2Int PlantTime;
    public int OutputAmountA;
    public Vector2Int LastUpdateTime;
    public int WorkState;
    public int OutputAmountB;
}

public class BuildingDtoS : BuildingDto
{
    public int EmployeeCount;
    public int ShopOwnerID;
    public bool IsSelling;
    public int SalaryBase;
    public int AccumulatedProfit;
    public Vector2Int LastUpdateTime;
    public int YuanbaoSpent;
}

public class BuildingDtoC : BuildingDto
{
    public int XueFengClass;
    public int MonthlyCost;
    public Vector2Int LastUpdateTime;
}

public class BuildingDtoL : BuildingDto
{
    public Deceased? DeceasedData;

    public struct Deceased(int generation, Gender gender, string name, int age)
    {
        public int Generation = generation;
        public Gender Gender = gender;
        public string Name = name;
        public int Age = age;

        public static Deceased? Parse(string str)
        {
            var parts = str.Split('|');
            if (str == "null" || parts.Length < 4)
                return null;

            return new Deceased(
                int.Parse(parts[0]),
                (Gender)int.Parse(parts[1]),
                parts[2],
                int.Parse(parts[3])
            );
        }

        public static string ToString(Deceased? deceased)
        {
            return deceased == null ? "null" : deceased.ToString();
        }

        public override string ToString()
        {
            return $"{Generation}|{(int)Gender}|{Name}|{Age}";
        }
    }
}



