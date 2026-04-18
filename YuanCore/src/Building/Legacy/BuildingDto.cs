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
    public float FieldSize;
    public string PlantTime;
    public int OutputAmountA;
    public string NextHarvestTime;
    public int WorkState;
    public int OutputAmountB;
}

public class BuildingDtoS : BuildingDto
{
    public int EmployeeCount;
    public string ShopOwnerID;
    public bool IsOpen;
    public int SalaryBase;
    public int AccumulatedProfit;
    public string LastUpdateTime;
    public int YuanbaoSpent;
}

public class BuildingDtoC : BuildingDto
{
    public int XueFengClass;
    public int TuitionCost;
    public string LastUpdateTime;
}

public class BuildingDtoL : BuildingDto
{
    public string DaiZangData;
}
