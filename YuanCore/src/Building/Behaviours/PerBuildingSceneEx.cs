using System.Collections.Generic;
using UnityEngine;

namespace YuanCore.Building;

public class PerBuildingSceneEx : MonoBehaviour
{
	public int PosiA;

	public int PosiB;

	public int buildstateID;

	public int buildstateID_Last;

	public string BuildBigClass;

	public string buildClassID;

	public bool isAnPaiNonghu;

	private string BuildTaoZhuangID;

	private string BuildShiliID;

	private string AllbuildClassID;

	private List<bool> isPengAll;

	private int BuildStateIDChange;

	private bool isCanChangeState;

	private int SceneID;

	private void Start()
	{
		InitData();
		InitShow();
	}

	private void InitData()
	{
		isAnPaiNonghu = false;
		isPengAll = new List<bool> { false, false, false, false };
		if (BuildBigClass == "M")
		{
			BuildTaoZhuangID = Mainload.BuildInto_m[int.Parse(base.name)][7];
			BuildShiliID = Mainload.BuildInto_m[int.Parse(base.name)][0];
			buildClassID = Mainload.BuildInto_m[int.Parse(base.name)][1];
			buildstateID = int.Parse(Mainload.BuildInto_m[int.Parse(base.name)][6]);
			AllbuildClassID = Mainload.AllBuilddata[int.Parse(buildClassID)][5];
		}
		else if (BuildBigClass == "Z")
		{
			BuildTaoZhuangID = Mainload.BuildInto_z[int.Parse(base.name)][10];
			BuildShiliID = Mainload.BuildInto_z[int.Parse(base.name)][0];
			buildClassID = Mainload.BuildInto_z[int.Parse(base.name)][1];
			AllbuildClassID = Mainload.AllBuilddata[int.Parse(buildClassID)][5];
			buildstateID = int.Parse(Mainload.BuildInto_z[int.Parse(base.name)][4]);
		}
		else if (BuildBigClass == "S")
		{
			BuildTaoZhuangID = Mainload.BuildInto_s[int.Parse(base.name)][11];
			BuildShiliID = Mainload.BuildInto_s[int.Parse(base.name)][0];
			buildClassID = Mainload.BuildInto_s[int.Parse(base.name)][1];
			AllbuildClassID = Mainload.AllBuilddata[int.Parse(buildClassID)][5];
			buildstateID = int.Parse(Mainload.BuildInto_s[int.Parse(base.name)][5]);
		}
		else if (BuildBigClass == "C")
		{
			BuildTaoZhuangID = Mainload.BuildInto_c[int.Parse(base.name)][6];
			BuildShiliID = Mainload.BuildInto_c[int.Parse(base.name)][0];
			buildClassID = Mainload.BuildInto_c[int.Parse(base.name)][1];
			AllbuildClassID = Mainload.AllBuilddata[int.Parse(buildClassID)][5];
			buildstateID = int.Parse(Mainload.BuildInto_c[int.Parse(base.name)][4]);
		}
		else if (BuildBigClass == "H")
		{
			BuildTaoZhuangID = Mainload.BuildInto_h[int.Parse(base.name)][5];
			BuildShiliID = Mainload.BuildInto_h[int.Parse(base.name)][0];
			buildClassID = Mainload.BuildInto_h[int.Parse(base.name)][1];
			buildstateID = int.Parse(Mainload.BuildInto_h[int.Parse(base.name)][4]);
			AllbuildClassID = Mainload.AllBuilddata[int.Parse(buildClassID)][5];
		}
		else if (BuildBigClass == "L")
		{
			BuildTaoZhuangID = Mainload.BuildInto_l[int.Parse(base.name)][5];
			BuildShiliID = Mainload.BuildInto_l[int.Parse(base.name)][0];
			buildClassID = Mainload.BuildInto_l[int.Parse(base.name)][1];
			AllbuildClassID = Mainload.AllBuilddata[int.Parse(buildClassID)][5];
			buildstateID = int.Parse(Mainload.BuildInto_l[int.Parse(base.name)][4]);
		}
		SceneID = int.Parse(Mainload.SceneID.Split('|')[1]);
	}

	private void InitShow()
	{
		ZhuanBuild(buildstateID);
	}

	public void FixBuild(bool isAuto)
	{
		int siblingIndex = base.transform.GetSiblingIndex();
		bool flag = false;
		if (BuildBigClass == "M")
		{
			if (FormulaData.GetCoinsNum() >= FormulaData.FixBuildCost(int.Parse(buildClassID)))
			{
				if (buildClassID == "10" || buildClassID == "182")
				{
					FormulaData.TaskOrder_AddNum(12, 1);
				}
				else if ((int.Parse(buildClassID) >= 11 && int.Parse(buildClassID) <= 16) || (int.Parse(buildClassID) >= 137 && int.Parse(buildClassID) <= 146))
				{
					FormulaData.TaskOrder_AddNum(13, 1);
				}
				else if ((int.Parse(buildClassID) >= 19 && int.Parse(buildClassID) <= 22) || (int.Parse(buildClassID) >= 147 && int.Parse(buildClassID) <= 150))
				{
					FormulaData.TaskOrder_AddNum(37, 1);
				}
				else if (buildClassID == "9")
				{
					FormulaData.TaskOrder_AddNum(41, 1);
				}
				FormulaData.ChangeCoins(-FormulaData.FixBuildCost(int.Parse(buildClassID)));
				AllbuildClassID = Mainload.AllBuilddata[int.Parse(buildClassID)][5];
				flag = true;
				FormulaData.BuildM_AddData(SceneID, siblingIndex, 0, isBuild: true, OnlyReuceData: true);
				Mainload.BuildInto_m[siblingIndex][6] = InitBuildData.Build_ToFix(Mainload.BuildInto_m[siblingIndex][6]);
				buildstateID = int.Parse(Mainload.BuildInto_m[siblingIndex][6]);
				buildstateID_Last = buildstateID;
				FormulaData.BuildM_AddData(SceneID, siblingIndex, 1, isBuild: true, OnlyReuceData: false);
				if (!isAuto)
				{
					Mainload.Tip_Show.Add(new List<string>
					{
						"0",
						AllText.Text_TipShow[264][Mainload.SetData[4]].Replace("@", FormulaData.FixBuildCost(int.Parse(buildClassID)).ToString())
					});
				}
			}
			else
			{
				Mainload.Tip_Show.Add(new List<string>
				{
					"1",
					AllText.Text_TipShow[8][Mainload.SetData[4]]
				});
			}
		}
		else if (BuildBigClass == "S")
		{
			if (int.Parse(Mainload.CityData_now[SceneID][0][9]) >= FormulaData.FixBuildCost(int.Parse(buildClassID)))
			{
				Mainload.CityData_now[SceneID][0][9] = (int.Parse(Mainload.CityData_now[SceneID][0][9]) - FormulaData.FixBuildCost(int.Parse(buildClassID))).ToString();
				FormulaData.BuildS_AddData(SceneID, siblingIndex, 0, 0, OnlyReuceData: true, isInit: false);
				Mainload.BuildInto_s[siblingIndex][5] = InitBuildData.Build_ToFix(Mainload.BuildInto_s[siblingIndex][5]);
				buildstateID = int.Parse(Mainload.BuildInto_s[siblingIndex][5]);
				buildstateID_Last = buildstateID;
				Mainload.BuildInto_s[siblingIndex][7] = "0";
				AllbuildClassID = Mainload.AllBuilddata[int.Parse(buildClassID)][5];
				flag = true;
				FormulaData.BuildS_AddData(SceneID, siblingIndex, 1, 0, OnlyReuceData: false, isInit: false);
				if (!isAuto)
				{
					Mainload.Tip_Show.Add(new List<string>
					{
						"0",
						AllText.Text_TipShow[262][Mainload.SetData[4]].Replace("@", FormulaData.FixBuildCost(int.Parse(buildClassID)).ToString())
					});
				}
			}
			else
			{
				Mainload.isNoCoinsForFastFixBuild = true;
				Mainload.Tip_Show.Add(new List<string>
				{
					"1",
					AllText.Text_TipShow[11][Mainload.SetData[4]]
				});
			}
		}
		else if (BuildBigClass == "C")
		{
			if (int.Parse(Mainload.CityData_now[SceneID][0][9]) >= FormulaData.FixBuildCost(int.Parse(buildClassID)))
			{
				Mainload.CityData_now[SceneID][0][9] = (int.Parse(Mainload.CityData_now[SceneID][0][9]) - FormulaData.FixBuildCost(int.Parse(buildClassID))).ToString();
				FormulaData.BuildC_AddData(SceneID, siblingIndex, 0, isBuild: true, OnlyReuceData: true);
				Mainload.BuildInto_c[siblingIndex][4] = InitBuildData.Build_ToFix(Mainload.BuildInto_c[siblingIndex][4]);
				buildstateID = int.Parse(Mainload.BuildInto_c[siblingIndex][4]);
				buildstateID_Last = buildstateID;
				AllbuildClassID = Mainload.AllBuilddata[int.Parse(buildClassID)][5];
				flag = true;
				FormulaData.BuildC_AddData(SceneID, siblingIndex, 1, isBuild: true, OnlyReuceData: false);
				if (!isAuto)
				{
					Mainload.Tip_Show.Add(new List<string>
					{
						"0",
						AllText.Text_TipShow[262][Mainload.SetData[4]].Replace("@", FormulaData.FixBuildCost(int.Parse(buildClassID)).ToString())
					});
				}
			}
			else
			{
				Mainload.Tip_Show.Add(new List<string>
				{
					"1",
					AllText.Text_TipShow[11][Mainload.SetData[4]]
				});
			}
		}
		else if (BuildBigClass == "L")
		{
			if (FormulaData.GetCoinsNum() >= FormulaData.FixBuildCost(int.Parse(buildClassID)))
			{
				FormulaData.ChangeCoins(-FormulaData.FixBuildCost(int.Parse(buildClassID)));
				AllbuildClassID = Mainload.AllBuilddata[int.Parse(buildClassID)][5];
				flag = true;
				Mainload.BuildInto_l[siblingIndex][4] = InitBuildData.Build_ToFix(Mainload.BuildInto_l[siblingIndex][4]);
				buildstateID = int.Parse(Mainload.BuildInto_l[siblingIndex][4]);
				buildstateID_Last = buildstateID;
				if (!isAuto)
				{
					Mainload.Tip_Show.Add(new List<string>
					{
						"0",
						AllText.Text_TipShow[264][Mainload.SetData[4]].Replace("@", FormulaData.FixBuildCost(int.Parse(buildClassID)).ToString())
					});
				}
			}
			else
			{
				Mainload.Tip_Show.Add(new List<string>
				{
					"1",
					AllText.Text_TipShow[8][Mainload.SetData[4]]
				});
			}
		}
		if (flag)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Object.Destroy(base.transform.GetChild(i).gameObject);
			}
			GameObject gameObject = Object.Instantiate((GameObject)Resources.Load("AllBuild/" + BuildTaoZhuangID + "/Scene/" + buildClassID + "/" + buildstateID));
			gameObject.name = buildstateID.ToString();
			if (int.Parse(AllbuildClassID) >= 0)
			{
				gameObject.transform.GetComponent<PerBuildShow>().buildShiliID = BuildShiliID;
				gameObject.transform.GetComponent<PerBuildShow>().buildClassID = buildClassID;
				gameObject.transform.GetComponent<PerBuildShow>().AllbuildClassID = AllbuildClassID;
				gameObject.transform.GetComponent<PerBuildShow>().BuildBigClass = BuildBigClass;
			}
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			GameObject obj = Object.Instantiate((GameObject)Resources.Load("SmokeA"));
			obj.transform.SetParent(base.transform);
			obj.transform.localScale = new Vector3(1.8f, 1.8f, 1f);
			obj.transform.localPosition = new Vector3(0f, 0.8f, 0f);
		}
	}

	public void ZhuanBuild(int buildState_ID)
	{
		buildstateID = buildState_ID;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Object.Destroy(base.transform.GetChild(i).gameObject);
		}
		GameObject gameObject = Object.Instantiate((GameObject)Resources.Load("AllBuild/" + BuildTaoZhuangID + "/Scene/" + buildClassID + "/" + buildState_ID));
		gameObject.name = buildState_ID.ToString();
		if (int.Parse(AllbuildClassID) >= 0)
		{
			gameObject.transform.GetComponent<PerBuildShow>().buildShiliID = BuildShiliID;
			gameObject.transform.GetComponent<PerBuildShow>().buildClassID = buildClassID;
			gameObject.transform.GetComponent<PerBuildShow>().AllbuildClassID = AllbuildClassID;
			gameObject.transform.GetComponent<PerBuildShow>().BuildBigClass = BuildBigClass;
		}
		gameObject.transform.SetParent(base.transform);
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		if (int.Parse(AllbuildClassID) >= 0)
		{
			Invoke("GetZuDang", 0.1f);
		}
	}

	private void GetZuDang()
	{
		bool flag = false;
		for (int i = 0; i < base.transform.GetChild(0).Find("UI").Find("AllDrag")
			.childCount; i++)
		{
			if (base.transform.GetChild(0).Find("UI").Find("AllDrag")
				.GetChild(i)
				.GetComponent<DragBT>()
				.isZuDang)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			buildstateID_Last = buildstateID;
		}
	}

	public void GetPeng(int id, bool isPeng)
	{
		isPengAll[id] = isPeng;
		isCanChangeState = true;
		Invoke("ChangeState", 0.15f);
	}

	private void ChangeState()
	{
		if (!isCanChangeState)
		{
			return;
		}
		isCanChangeState = false;
		BuildStateIDChange = 0;
		if (!isPengAll[0] && !isPengAll[1] && !isPengAll[2] && !isPengAll[3])
		{
			if (buildstateID_Last == 0 || buildstateID_Last == 1 || buildstateID_Last == 2 || buildstateID_Last == 3)
			{
				BuildStateIDChange = buildstateID_Last;
			}
			else
			{
				BuildStateIDChange = 0;
			}
		}
		else if (isPengAll[0] && !isPengAll[1] && !isPengAll[2] && !isPengAll[3])
		{
			if (buildstateID_Last == 1 || buildstateID_Last == 3)
			{
				BuildStateIDChange = buildstateID_Last;
			}
			else
			{
				BuildStateIDChange = 1;
			}
		}
		else if (!isPengAll[0] && isPengAll[1] && !isPengAll[2] && !isPengAll[3])
		{
			if (buildstateID_Last == 0 || buildstateID_Last == 2)
			{
				BuildStateIDChange = buildstateID_Last;
			}
			else
			{
				BuildStateIDChange = 0;
			}
		}
		else if (!isPengAll[0] && !isPengAll[1] && isPengAll[2] && !isPengAll[3])
		{
			if (buildstateID_Last == 1 || buildstateID_Last == 3)
			{
				BuildStateIDChange = buildstateID_Last;
			}
			else
			{
				BuildStateIDChange = 1;
			}
		}
		else if (!isPengAll[0] && !isPengAll[1] && !isPengAll[2] && isPengAll[3])
		{
			if (buildstateID_Last == 0 || buildstateID_Last == 2)
			{
				BuildStateIDChange = buildstateID_Last;
			}
			else
			{
				BuildStateIDChange = 0;
			}
		}
		else if (isPengAll[0] && !isPengAll[1] && isPengAll[2] && !isPengAll[3])
		{
			if (buildstateID_Last == 1 || buildstateID_Last == 3)
			{
				BuildStateIDChange = buildstateID_Last;
			}
			else
			{
				BuildStateIDChange = 1;
			}
		}
		else if (!isPengAll[0] && isPengAll[1] && !isPengAll[2] && isPengAll[3])
		{
			if (buildstateID_Last == 0 || buildstateID_Last == 2)
			{
				BuildStateIDChange = buildstateID_Last;
			}
			else
			{
				BuildStateIDChange = 0;
			}
		}
		else if (isPengAll[0] && !isPengAll[1] && !isPengAll[2] && isPengAll[3])
		{
			BuildStateIDChange = 4;
		}
		else if (!isPengAll[0] && !isPengAll[1] && isPengAll[2] && isPengAll[3])
		{
			BuildStateIDChange = 5;
		}
		else if (!isPengAll[0] && isPengAll[1] && isPengAll[2] && !isPengAll[3])
		{
			BuildStateIDChange = 6;
		}
		else if (isPengAll[0] && isPengAll[1] && !isPengAll[2] && !isPengAll[3])
		{
			BuildStateIDChange = 7;
		}
		else if (!isPengAll[0] && isPengAll[1] && isPengAll[2] && isPengAll[3])
		{
			BuildStateIDChange = 8;
		}
		else if (isPengAll[0] && isPengAll[1] && isPengAll[2] && !isPengAll[3])
		{
			BuildStateIDChange = 9;
		}
		else if (isPengAll[0] && isPengAll[1] && !isPengAll[2] && isPengAll[3])
		{
			BuildStateIDChange = 10;
		}
		else if (isPengAll[0] && !isPengAll[1] && isPengAll[2] && isPengAll[3])
		{
			BuildStateIDChange = 11;
		}
		else if (isPengAll[0] && isPengAll[1] && isPengAll[2] && isPengAll[3])
		{
			BuildStateIDChange = 12;
		}
		if (buildstateID_Last != BuildStateIDChange)
		{
			ZhuanBuild(BuildStateIDChange);
		}
	}
}
