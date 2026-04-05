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
