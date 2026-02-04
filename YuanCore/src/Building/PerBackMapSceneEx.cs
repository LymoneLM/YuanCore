// 重责任场景建筑管理器

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace YuanCore.Building;

public class PerBackMapScene : MonoBehaviour
{
	public string PoisA_mouse;

	public string PoisB_mouse;

	private string SceneClass;

	private int SceneIndex;

	private bool isDestroyNPC;

	private string BuildID_CreatSelectLast;

	private GameObject perBuildTipA;

	private GameObject perBuildA;

	private bool isUpdateMemQ;

	private int LoadSpeed;

	private Transform _BuildShow;

	private Transform _BuildCity;

	private Transform _BuildShop;

	private bool Rebuild;

	private void Awake()
	{
		perBuildA = (GameObject)Resources.Load("PerBuildScene");
		perBuildTipA = (GameObject)Resources.Load("PerBuildTip");
		PoisA_mouse = "0";
		PoisB_mouse = "0";
		Rebuild = false;
		_BuildShow = transform.Find("BuildShow");
		_BuildCity = transform.Find("BuildCity");
		_BuildShop = transform.Find("BuildShop");
	}

	private void Start()
	{
		initData();
		initShow();
	}

	private void Update()
	{
		UpdateShow();
	}

	private void initData()
	{
		Mainload.TempMemberIndex_now = 0;
		isUpdateMemQ = false;
		SceneClass = Mainload.SceneID.Split('|')[0];
		SceneIndex = int.Parse(Mainload.SceneID.Split('|')[1]);
		BuildID_CreatSelectLast = "-1";
		Mainload.BuildPosiID_Now = "0|0";
		Mainload.BuildID_CreatNow = "null";
		isDestroyNPC = false;
		if (SceneClass == "Z")
		{
			string[] array = Mainload.NongZ_now[SceneIndex][int.Parse(Mainload.SceneID.Split('|')[2])][24].Split('|');
			Mainload.LastNonghuNum_Open = new List<float>
			{
				float.Parse(array[0]),
				float.Parse(array[1]),
				float.Parse(array[2])
			};
			SaveData.ReadBuildData(SceneClass, SceneIndex.ToString(), Mainload.SceneID.Split('|')[2]);
		}
		else if (SceneClass == "L")
		{
			SaveData.ReadBuildData(SceneClass, SceneIndex.ToString(), Mainload.SceneID.Split('|')[2]);
		}
		else if (SceneClass == "S")
		{
			SaveData.ReadBuildData(SceneClass, SceneIndex.ToString(), "0");
			if (Mainload.ShopData_updateTime[SceneIndex].Count <= 0)
			{
				for (int i = 0; i < Mainload.BuildInto_s.Count; i++)
				{
					Mainload.ShopData_updateTime[SceneIndex].Add(0);
					Mainload.Prop_shop_temp[SceneIndex].Add(new List<string>());
					Mainload.Horse_Shop_Temp[SceneIndex].Add(new List<List<string>>());
					Mainload.HuaiZhang_Shop_Temp[SceneIndex].Add(new List<List<string>>());
					Mainload.OtherTrade_shop_Temp[SceneIndex].Add(new List<List<string>>());
				}
			}
		}
		else if (!Mainload.isFirstGame)
		{
			SaveData.ReadBuildData(SceneClass, SceneIndex.ToString(), "0");
		}
		if (Mainload.SetData[5] == 0)
		{
			LoadSpeed = 50;
		}
		else if (Mainload.SetData[5] == 1)
		{
			LoadSpeed = 200;
		}
		else if (Mainload.SetData[5] == 2)
		{
			LoadSpeed = 500;
		}
		else if (Mainload.SetData[5] == 3)
		{
			LoadSpeed = 100000;
		}
	}

	private void initShow()
	{
		if (SceneClass == "M")
		{
			if (Mainload.BuildInto_m.Count <= 0)
			{
				Mainload.isCreatSceneFinish = true;
			}
			else
			{
				StartCoroutine(nameof(ReloadBuildingM));
			}
		}
		else if (SceneClass == "Z")
		{
			if (Mainload.BuildInto_z.Count <= 0)
			{
				Mainload.isCreatSceneFinish = true;
				int num = int.Parse(Mainload.SceneID.Split('|')[2]);
				string[] array = Mainload.NongZ_now[SceneIndex][num][2].Split('|');
				for (int i = 0; i < transform.Find("AddClickBack").Find("AllKuai").childCount; i++)
				{
					transform.Find("AddClickBack").Find("AllKuai").GetChild(i)
						.GetComponent<PerFeiWoRange>()
						.FeiWoNum = int.Parse(array[i]);
					transform.Find("AddClickBack").Find("AllKuai").GetChild(i)
						.GetComponent<PerFeiWoRange>()
						.FengDiIndex = SceneIndex;
					transform.Find("AddClickBack").Find("AllKuai").GetChild(i)
						.GetComponent<PerFeiWoRange>()
						.NongZIndex = num;
				}
			}
			else
			{
				StartCoroutine(nameof(ReloadBuildingZ));
			}
		}
		else if (SceneClass == "S")
		{
			if (Mainload.BuildInto_s.Count + Mainload.BuildInto_c.Count <= 0)
			{
				Mainload.isCreatSceneFinish = true;
			}
			else if (Mainload.BuildInto_s.Count <= 0)
			{
				StartCoroutine(nameof(ReloadBuildingC));
			}
			else
			{
				StartCoroutine(nameof(ReloadBuildingS));
			}
		}
		else if (SceneClass == "H")
		{
			if (Mainload.BuildInto_h.Count <= 0)
			{
				Mainload.isCreatSceneFinish = true;
			}
			else
			{
				StartCoroutine(nameof(ReloadBuildingH));
			}
		}
		else if (SceneClass == "L")
		{
			if (Mainload.BuildInto_l.Count <= 0)
			{
				Mainload.isCreatSceneFinish = true;
			}
			else
			{
				StartCoroutine(nameof(ReloadBuildingL));
			}
		}
	}

    private void AddBuilding(int index, string buildingBigClass, string positionAB)
    {
        var positionList = positionAB.Split('|');
        var positionA = int.Parse(positionList[0]);
        var positionB = int.Parse(positionList[1]);

        var obj = Instantiate(perBuildA, _BuildShow, true);
        var buildSceneComponent = obj.transform.GetComponent<PerBuildScene>();
        buildSceneComponent.BuildBigClass = buildingBigClass;
        buildSceneComponent.PosiA = positionA;
        buildSceneComponent.PosiB = positionB;
        obj.name = index.ToString();
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        obj.transform.localPosition = FormulaData.GetBuildPosi(positionA, positionB);
    }

	private IEnumerator ReloadBuildingM()
	{
		_BuildShow.DestroyAllChildren();
		for (var a = 0; a < Mainload.BuildInto_m.Count; a++)
        {
            AddBuilding(a, "M", Mainload.BuildInto_m[a][5]);

			if ((a + 1) % LoadSpeed == 0)
			{
				yield return null;
			}
		}
        Mainload.isCreatSceneFinish = true;
	}

	private IEnumerator ReloadBuildingZ()
	{
        _BuildShow.DestroyAllChildren();
        for (int a = 0; a < Mainload.BuildInto_z.Count; a++)
		{
            AddBuilding(a, "Z", Mainload.BuildInto_z[a][3]);

			if ((a + 1) % LoadSpeed == 0)
			{
				yield return null;
			}
		}

        int NongZIndex = int.Parse(Mainload.SceneID.Split('|')[2]);
        Mainload.isCreatSceneFinish = true;
        StopCoroutine(nameof(ReloadBuildingZ));
        string[] array = Mainload.NongZ_now[SceneIndex][NongZIndex][2].Split('|');
        for (int j = 0; j < transform.Find("AddClickBack").Find("AllKuai").childCount; j++)
        {
            transform.Find("AddClickBack").Find("AllKuai").GetChild(j)
                .GetComponent<PerFeiWoRange>()
                .FeiWoNum = int.Parse(array[j]);
            transform.Find("AddClickBack").Find("AllKuai").GetChild(j)
                .GetComponent<PerFeiWoRange>()
                .FengDiIndex = SceneIndex;
            transform.Find("AddClickBack").Find("AllKuai").GetChild(j)
                .GetComponent<PerFeiWoRange>()
                .NongZIndex = NongZIndex;
        }
        Invoke(nameof(JiaoTuiFeiwo), 0.1f);
	}

    //TODO：处理郡城S和C的特殊情况
	private IEnumerator ReloadBuildingS()
	{
		for (int i = 0; i < _BuildShop.childCount; i++)
		{
			Destroy(_BuildShop.GetChild(i).gameObject);
		}
		for (int a = 0; a < Mainload.BuildInto_s.Count; a++)
		{
			var AddbuildPosiA = int.Parse(Mainload.BuildInto_s[a][4].Split('|')[0]);
			var AddbuildPosiB = int.Parse(Mainload.BuildInto_s[a][4].Split('|')[1]);
			GameObject obj = Instantiate(perBuildA);
			obj.transform.GetComponent<PerBuildScene>().BuildBigClass = "S";
			obj.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
			obj.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
			obj.name = a.ToString();
			obj.transform.SetParent(_BuildShop);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			obj.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
			if (a >= Mainload.BuildInto_s.Count - 1)
			{
				StopCoroutine(nameof(ReloadBuildingS));
				if (Mainload.BuildInto_c.Count <= 0)
				{
					Mainload.isCreatSceneFinish = true;
				}
				else
				{
					StartCoroutine(nameof(ReloadBuildingC));
				}
			}
			if ((a + 1) % LoadSpeed == 0)
			{
				yield return null;
			}
		}
	}

	private IEnumerator ReloadBuildingC()
	{
		for (int i = 0; i < _BuildCity.childCount; i++)
		{
			Destroy(_BuildCity.GetChild(i).gameObject);
		}
		for (int a = 0; a < Mainload.BuildInto_c.Count; a++)
		{
			var AddbuildPosiA = int.Parse(Mainload.BuildInto_c[a][3].Split('|')[0]);
			var AddbuildPosiB = int.Parse(Mainload.BuildInto_c[a][3].Split('|')[1]);
			GameObject obj = Instantiate(perBuildA);
			obj.transform.GetComponent<PerBuildScene>().BuildBigClass = "C";
			obj.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
			obj.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
			obj.name = a.ToString();
			obj.transform.SetParent(_BuildCity);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			obj.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
			if (a >= Mainload.BuildInto_c.Count - 1)
			{
				Mainload.isCreatSceneFinish = true;
				StopCoroutine(nameof(ReloadBuildingC));
			}
			if ((a + 1) % LoadSpeed == 0)
			{
				yield return null;
			}
		}
	}

	private IEnumerator ReloadBuildingH()
	{
		_BuildShow.DestroyAllChildren();
		for (var a = 0; a < Mainload.BuildInto_h.Count; a++)
		{
            AddBuilding(a, "H", Mainload.BuildInto_h[a][3]);

			if ((a + 1) % LoadSpeed == 0)
			{
				yield return null;
			}
		}
        Mainload.isCreatSceneFinish = true;
	}

	private IEnumerator ReloadBuildingL()
	{
		_BuildShow.DestroyAllChildren();
		for (var a = 0; a < Mainload.BuildInto_l.Count; a++)
		{
            AddBuilding(a, "L", Mainload.BuildInto_l[a][3]);

			if ((a + 1) % LoadSpeed == 0)
			{
				yield return null;
			}
		}
        Mainload.isCreatSceneFinish = true;
	}

	private void UpdateShow()
	{
        // 肥沃度更新
		if (Mainload.isShiFeiMode && SceneClass == "Z" && Mainload.isChangeFeiWoNongZ)
		{
			Mainload.isChangeFeiWoNongZ = false;
			int index = int.Parse(Mainload.SceneID.Split('|')[2]);
			string[] array = Mainload.NongZ_now[SceneIndex][index][2].Split('|');
			for (int i = 0; i < transform.Find("AddClickBack").Find("AllKuai").childCount; i++)
			{
				transform.Find("AddClickBack").Find("AllKuai").GetChild(i)
					.GetComponent<PerFeiWoRange>()
					.FeiWoNum = int.Parse(array[i]);
				transform.Find("AddClickBack").Find("AllKuai").GetChild(i)
					.GetComponent<PerFeiWoRange>()
					.UpdateBuildFeiwo();
			}
		}

        // 建筑更新
		if (Mainload.isBuildMode || (Rebuild && Mainload.isBuildEdit))
		{
			Rebuild = false;
			if (BuildID_CreatSelectLast != Mainload.BuildID_Creatselect)
			{
				BuildID_CreatSelectLast = Mainload.BuildID_Creatselect;
				if (Mainload.isBuildMode)
				{
					for (int j = 0; j < transform.Find("BuildTip").childCount; j++)
					{
						Destroy(transform.Find("BuildTip").GetChild(j).gameObject);
					}
					GameObject obj = Instantiate(perBuildTipA);
					obj.transform.SetParent(transform.Find("BuildTip"));
					obj.transform.localScale = new Vector3(1f, 1f, 1f);
				}
			}
			if (Mainload.BuildID_CreatNow != "null")
			{
				var AddbuildPosiA = int.Parse(Mainload.BuildPosiID_CreatNow.Split('|')[0]);
				var AddbuildPosiB = int.Parse(Mainload.BuildPosiID_CreatNow.Split('|')[1]);
				if (SceneClass == "M")
				{
					FormulaData.AddBuildNew("M", SceneIndex, Mainload.BuildID_CreatNow, Mainload.BuildPosiID_CreatNow,
                        0, Mainload.BuildDirID_CreatSelect, isFamily: true, Mainload.BuildTaoZhuangID_SelectNow);
					GameObject obj2 = Instantiate(perBuildA);
					obj2.transform.GetComponent<PerBuildScene>().BuildBigClass = "M";
					obj2.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
					obj2.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
					obj2.name = (Mainload.BuildInto_m.Count - 1).ToString();
					obj2.transform.SetParent(_BuildShow);
					obj2.transform.localScale = new Vector3(1f, 1f, 1f);
					obj2.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
					Mainload.AddBuildNum_Now++;
				}
				else if (SceneClass == "Z")
				{
					int indexB = int.Parse(Mainload.SceneID.Split('|')[2]);
					FormulaData.AddBuildNew("Z", SceneIndex, Mainload.BuildID_CreatNow, Mainload.BuildPosiID_CreatNow,
                        indexB, Mainload.BuildDirID_CreatSelect, isFamily: true, Mainload.BuildTaoZhuangID_SelectNow);
					GameObject obj3 = Instantiate(perBuildA);
					obj3.transform.GetComponent<PerBuildScene>().BuildBigClass = "Z";
					obj3.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
					obj3.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
					obj3.name = (Mainload.BuildInto_z.Count - 1).ToString();
					obj3.transform.SetParent(_BuildShow);
					obj3.transform.localScale = new Vector3(1f, 1f, 1f);
					obj3.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
					Mainload.AddBuildNum_Now++;
				}
				else if (SceneClass == "S")
				{
					if (Mainload.BuildCityClass == "S0")
					{
						FormulaData.AddBuildNew("S", SceneIndex, Mainload.BuildID_CreatNow,
                            Mainload.BuildPosiID_CreatNow, 0, Mainload.BuildDirID_CreatSelect, isFamily: false,
                            Mainload.BuildTaoZhuangID_SelectNow);
						GameObject obj4 = Instantiate(perBuildA);
						obj4.transform.GetComponent<PerBuildScene>().BuildBigClass = "S";
						obj4.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
						obj4.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
						obj4.name = (Mainload.BuildInto_s.Count - 1).ToString();
						obj4.transform.SetParent(_BuildShop);
						obj4.transform.localScale = new Vector3(1f, 1f, 1f);
						obj4.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
						Mainload.AddBuildNum_Now = 0;
					}
					else if (Mainload.BuildCityClass == "S1")
					{
						FormulaData.AddBuildNew("S", SceneIndex, Mainload.BuildID_CreatNow,
                            Mainload.BuildPosiID_CreatNow, 0, Mainload.BuildDirID_CreatSelect, isFamily: true,
                            Mainload.BuildTaoZhuangID_SelectNow);
						GameObject obj5 = Instantiate(perBuildA);
						obj5.transform.GetComponent<PerBuildScene>().BuildBigClass = "S";
						obj5.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
						obj5.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
						obj5.name = (Mainload.BuildInto_s.Count - 1).ToString();
						obj5.transform.SetParent(_BuildShop);
						obj5.transform.localScale = new Vector3(1f, 1f, 1f);
						obj5.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
						Mainload.AddBuildNum_Now = 0;
					}
					else if (Mainload.BuildCityClass == "C")
					{
						FormulaData.AddBuildNew("C", SceneIndex, Mainload.BuildID_CreatNow,
                            Mainload.BuildPosiID_CreatNow, 0, Mainload.BuildDirID_CreatSelect, isFamily: false,
                            Mainload.BuildTaoZhuangID_SelectNow);
						GameObject obj6 = Instantiate(perBuildA);
						obj6.transform.GetComponent<PerBuildScene>().BuildBigClass = "C";
						obj6.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
						obj6.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
						obj6.name = (Mainload.BuildInto_c.Count - 1).ToString();
						obj6.transform.SetParent(_BuildCity);
						obj6.transform.localScale = new Vector3(1f, 1f, 1f);
						obj6.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
						Mainload.AddBuildNum_Now++;
					}
				}
				else if (SceneClass == "H")
				{
					FormulaData.AddBuildNew("H", SceneIndex, Mainload.BuildID_CreatNow, Mainload.BuildPosiID_CreatNow,
                        0, Mainload.BuildDirID_CreatSelect, isFamily: true, Mainload.BuildTaoZhuangID_SelectNow);
					GameObject obj7 = Instantiate(perBuildA);
					obj7.transform.GetComponent<PerBuildScene>().BuildBigClass = "H";
					obj7.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
					obj7.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
					obj7.name = (Mainload.BuildInto_h.Count - 1).ToString();
					obj7.transform.SetParent(_BuildShow);
					obj7.transform.localScale = new Vector3(1f, 1f, 1f);
					obj7.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
					Mainload.AddBuildNum_Now++;
				}
				else if (SceneClass == "L")
				{
					int indexB2 = int.Parse(Mainload.SceneID.Split('|')[2]);
					FormulaData.AddBuildNew("L", SceneIndex, Mainload.BuildID_CreatNow, Mainload.BuildPosiID_CreatNow,
                        indexB2, Mainload.BuildDirID_CreatSelect, isFamily: true, Mainload.BuildTaoZhuangID_SelectNow);
					GameObject obj8 = Instantiate(perBuildA);
					obj8.transform.GetComponent<PerBuildScene>().BuildBigClass = "L";
					obj8.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
					obj8.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
					obj8.name = (Mainload.BuildInto_l.Count - 1).ToString();
					obj8.transform.SetParent(_BuildShow);
					obj8.transform.localScale = new Vector3(1f, 1f, 1f);
					obj8.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
					Mainload.AddBuildNum_Now++;
				}
				if (Mainload.BuildIDAutoShape.Contains(Mainload.BuildID_Creatselect))
				{
					Mainload.BuildDirID_CreatSelect = TrueRandom.GetRanom(4).ToString();
				}
				Mainload.BuildID_CreatNow = "null";
			}
			if (Input.GetKeyUp(Mainload.FastKey[16]) && Mainload.AddBuildNum_Now > 0)
			{
				Mainload.AddBuildNum_Now--;
				if (SceneClass == "M")
				{
					FormulaData.RemoveBuildNew("M", SceneIndex, _BuildShow.childCount - 1, 0, TipShow: false, isCtrlZ: true);
					_BuildShow.GetChild(_BuildShow.childCount - 1).localPosition = new Vector3(-10000f, -10000f, 0f);
					Invoke(nameof(DestroyBuild), 0.05f);
				}
				else if (SceneClass == "Z")
				{
					int indexB3 = int.Parse(Mainload.SceneID.Split('|')[2]);
					FormulaData.RemoveBuildNew("Z", SceneIndex, indexB3, _BuildShow.childCount - 1, TipShow: false, isCtrlZ: true);
					_BuildShow.GetChild(_BuildShow.childCount - 1).localPosition = new Vector3(-10000f, -10000f, 0f);
					Invoke(nameof(DestroyBuild), 0.05f);
				}
				else if (SceneClass == "S")
				{
					FormulaData.RemoveBuildNew(_BuildCity.GetChild(_BuildCity.childCount - 1).GetComponent<PerBuildScene>().BuildBigClass, SceneIndex, _BuildCity.childCount - 1, 0, TipShow: false, isCtrlZ: true);
					_BuildCity.GetChild(_BuildCity.childCount - 1).localPosition = new Vector3(-10000f, -10000f, 0f);
					Invoke(nameof(DestroyBuildC), 0.05f);
				}
				else if (SceneClass == "H")
				{
					FormulaData.RemoveBuildNew("H", SceneIndex, _BuildShow.childCount - 1, 0, TipShow: false, isCtrlZ: true);
					_BuildShow.GetChild(_BuildShow.childCount - 1).localPosition = new Vector3(-10000f, -10000f, 0f);
					Invoke(nameof(DestroyBuild), 0.05f);
				}
				else if (SceneClass == "L")
				{
					int indexB4 = int.Parse(Mainload.SceneID.Split('|')[2]);
					FormulaData.RemoveBuildNew("L", SceneIndex, indexB4, _BuildShow.childCount - 1, TipShow: false, isCtrlZ: true);
					_BuildShow.GetChild(_BuildShow.childCount - 1).localPosition = new Vector3(-10000f, -10000f, 0f);
					Invoke(nameof(DestroyBuild), 0.05f);
				}
			}
		}
		else if (BuildID_CreatSelectLast != "-1")
		{
			BuildID_CreatSelectLast = "-1";
			if (transform.Find("BuildTip").childCount > 0)
			{
				transform.Find("BuildTip").GetChild(0).localScale = new Vector3(0f, 0f, 0f);
				transform.Find("BuildTip").GetChild(0).localPosition = new Vector3(1000000f, 1000000f, 0f);
				Invoke(nameof(DeleBuildTip), 0.05f);
			}
		}
		if (Mainload.isBuildEdit)
		{
			if (Input.GetKeyUp(Mainload.FastKey[16]) && Mainload.BuildData_Delete_Temp.Count > 0)
			{
				Rebuild = true;
				if (SceneClass == "M")
				{
					Mainload.BuildID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][1];
					Mainload.BuildPosiID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][5];
					Mainload.BuildDirID_CreatSelect = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][6];
					Mainload.BuildTaoZhuangID_SelectNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][7];
					float num = 0.5f;
					if (int.Parse(Mainload.BuildDirID_CreatSelect) < 0)
					{
						num = 0.2f;
					}
					FormulaData.ChangeCoins(-Mathf.RoundToInt(float.Parse(Mainload.AllBuilddata[int.Parse(Mainload.BuildID_CreatNow)][1].Split('|')[0]) * num));
					if (Mainload.BuildID_CreatNow == "0")
					{
						Mainload.Fudi_now[SceneIndex][39] = (int.Parse(Mainload.Fudi_now[SceneIndex][39]) + 1).ToString();
					}
					else if (Mainload.BuildID_CreatNow == "2" || Mainload.BuildID_CreatNow == "3" || Mainload.BuildID_CreatNow == "4")
					{
						Mainload.Fudi_now[SceneIndex][38] = (int.Parse(Mainload.Fudi_now[SceneIndex][38]) + 1).ToString();
					}
				}
				else if (SceneClass == "Z")
				{
					Mainload.BuildID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][1];
					Mainload.BuildPosiID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][3];
					Mainload.BuildDirID_CreatSelect = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][4];
					Mainload.BuildTaoZhuangID_SelectNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][10];
					FormulaData.ChangeCoins(-Mathf.RoundToInt(float.Parse(Mainload.AllBuilddata[int.Parse(Mainload.BuildID_CreatNow)][1].Split('|')[0]) * 0.5f));
				}
				else if (SceneClass == "S")
				{
					if (Mainload.BuildGroupdata[6][1].Contains(Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][1]))
					{
						Mainload.BuildID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][1];
						Mainload.BuildPosiID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][4];
						Mainload.BuildDirID_CreatSelect = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][5];
						Mainload.BuildTaoZhuangID_SelectNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][11];
						float num2 = 0.5f;
						if (int.Parse(Mainload.BuildDirID_CreatSelect) < 0)
						{
							num2 = 0.2f;
						}
						if (Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][6] == "-1")
						{
							Mainload.BuildCityClass = "S1";
							FormulaData.ChangeCoins(-Mathf.RoundToInt(float.Parse(Mainload.AllBuilddata[int.Parse(Mainload.BuildID_CreatNow)][1].Split('|')[0]) * num2));
						}
						else
						{
							Mainload.BuildCityClass = "S0";
							Mainload.CityData_now[SceneIndex][0][9] = (int.Parse(Mainload.CityData_now[SceneIndex][0][9]) - Mathf.RoundToInt(float.Parse(Mainload.AllBuilddata[int.Parse(Mainload.BuildID_CreatNow)][1].Split('|')[0]) * num2)).ToString();
						}
					}
					else
					{
						Mainload.BuildCityClass = "C";
						Mainload.BuildID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][1];
						Mainload.BuildPosiID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][3];
						Mainload.BuildDirID_CreatSelect = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][4];
						Mainload.BuildTaoZhuangID_SelectNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][6];
						float num3 = 0.5f;
						if (int.Parse(Mainload.BuildDirID_CreatSelect) < 0)
						{
							num3 = 0.2f;
						}
						Mainload.CityData_now[SceneIndex][0][9] = (int.Parse(Mainload.CityData_now[SceneIndex][0][9]) - Mathf.RoundToInt(float.Parse(Mainload.AllBuilddata[int.Parse(Mainload.BuildID_CreatNow)][1].Split('|')[0]) * num3)).ToString();
					}
				}
				else if (SceneClass == "H")
				{
					Mainload.BuildID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][1];
					Mainload.BuildPosiID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][3];
					Mainload.BuildDirID_CreatSelect = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][4];
					Mainload.BuildTaoZhuangID_SelectNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][5];
					FormulaData.CostKing_FromGuoKu(7, Mathf.RoundToInt(float.Parse(Mainload.AllBuilddata[int.Parse(Mainload.BuildID_CreatNow)][1].Split('|')[0]) * 0.5f));
				}
				else if (SceneClass == "L")
				{
					Mainload.BuildID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][1];
					Mainload.BuildPosiID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][3];
					Mainload.BuildDirID_CreatSelect = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][4];
					Mainload.BuildTaoZhuangID_SelectNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][5];
					FormulaData.ChangeCoins(-Mathf.RoundToInt(float.Parse(Mainload.AllBuilddata[int.Parse(Mainload.BuildID_CreatNow)][1].Split('|')[0]) * 0.5f));
				}
				Mainload.BuildData_Delete_Temp.RemoveAt(Mainload.BuildData_Delete_Temp.Count - 1);
			}
		}
		else if (Mainload.BuildData_Delete_Temp.Count > 0)
		{
			Mainload.BuildData_Delete_Temp = new List<List<string>>();
		}
		if (Mainload.isBuildMode || Mainload.isBuildEdit)
		{
			if (!transform.Find("BuildPosiGet").gameObject.activeSelf)
			{
				transform.Find("BuildPosiGet").gameObject.SetActive(value: true);
			}
			if (!isDestroyNPC)
			{
				isDestroyNPC = true;
				for (int k = 0; k < transform.Find("QNPC").childCount; k++)
				{
					Destroy(transform.Find("QNPC").GetChild(k).gameObject);
				}
			}
		}
		else
		{
			if (transform.Find("BuildPosiGet").gameObject.activeSelf)
			{
				transform.Find("BuildPosiGet").gameObject.SetActive(value: false);
			}
			isDestroyNPC = false;
		}
		if (!Mainload.isBuildEdit && !Mainload.isBuildMode && !Mainload.isShiFeiMode)
		{
			if (isUpdateMemQ)
			{
				return;
			}
			isUpdateMemQ = true;
			Mainload.QMemberID_Remove = new List<string>();
			Mainload.QMemberID_Change = new List<string>();
			if (SceneClass == "M")
			{
				Mainload.MemQShiJiaCanShow_Now = new List<string>();
				Mainload.PLNQCanShowNow = int.Parse(Mainload.Fudi_now[SceneIndex][2]);
				if (Mainload.PLNQCanShowNow > Mainload.MemberQShowNum[0])
				{
					Mainload.PLNQCanShowNow = Mainload.MemberQShowNum[0];
				}
				for (int l = 0; l < Mainload.Member_now.Count; l++)
				{
					if (int.Parse(Mainload.Member_now[l][6]) >= Mainload.OldFenjie[0] && Mainload.Member_now[l][3].Split('|')[0] == SceneIndex.ToString() && Mainload.Member_now[l][15] == "0")
					{
						string text = "0";
						if (int.Parse(Mainload.Member_now[l][6]) >= Mainload.OldFenjie[1])
						{
							text = "1";
						}
						string guanFuShowID_MemberQ = FormulaData.GetGuanFuShowID_MemberQ(Mainload.Member_now[l][12].Split('|')[0]);
						Mainload.MemQShiJiaCanShow_Now.Add(Mainload.Member_now[l][0] + "|-1|0|" + Mainload.Member_now[l][4].Split('|')[4] + "|" + text + "|" + Mainload.Member_now[l][1].Split('|')[1] + "|" + guanFuShowID_MemberQ);
						Mainload.PLNQCanShowNow++;
						if (Mainload.PLNQCanShowNow > Mainload.MemberQShowNum[0])
						{
							Mainload.PLNQCanShowNow = Mainload.MemberQShowNum[0];
						}
					}
				}
				for (int m = 0; m < Mainload.Member_qu.Count; m++)
				{
					if (int.Parse(Mainload.Member_qu[m][5]) >= Mainload.OldFenjie[0] && Mainload.Member_qu[m][4].Split('|')[0] == SceneIndex.ToString() && (Mainload.Member_qu[m][11] == "0" || Mainload.Member_qu[m][11] == "1"))
					{
						Mainload.MemQShiJiaCanShow_Now.Add(Mainload.Member_qu[m][0] + "|-1|1|" + Mainload.Member_qu[m][2].Split('|')[4] + "|1|" + Mainload.Member_qu[m][1].Split('|')[1] + "|-1");
						Mainload.PLNQCanShowNow++;
						if (Mainload.PLNQCanShowNow > Mainload.MemberQShowNum[0])
						{
							Mainload.PLNQCanShowNow = Mainload.MemberQShowNum[0];
						}
					}
				}
			}
			else if (SceneClass == "S")
			{
				Mainload.MemQShiJiaCanShow_Now = new List<string>();
				Mainload.PLNQCanShowNow = FormulaData.CityQMemberNumShow(int.Parse(Mainload.CityData_now[SceneIndex][0][10]));
				if (Mainload.PLNQCanShowNow > Mainload.MemberQShowNum[2])
				{
					Mainload.PLNQCanShowNow = Mainload.MemberQShowNum[2];
				}
				List<int> list = new List<int>();
				for (int n = 0; n < Mainload.ShiJia_Now.Count; n++)
				{
					if (Mainload.ShiJia_Now[n][0] == "0" && Mainload.ShiJia_Now[n][5].Split('|')[0] == SceneIndex.ToString())
					{
						list.Add(n);
					}
				}
				for (int num4 = 0; num4 < list.Count; num4++)
				{
					for (int num5 = 0; num5 < Mainload.Member_other[list[num4]].Count; num5++)
					{
						if (int.Parse(Mainload.Member_other[list[num4]][num5][3]) >= Mainload.OldFenjie[1] && Mainload.Member_other[list[num4]][num5][16] == "0" && TrueRandom.GetRanom(100) < 20)
						{
							string text2 = "0";
							if (int.Parse(Mainload.Member_other[list[num4]][num5][3]) >= Mainload.OldFenjie[1])
							{
								text2 = "1";
							}
							string guanFuShowID_MemberQ2 = FormulaData.GetGuanFuShowID_MemberQ(Mainload.Member_other[list[num4]][num5][9].Split('|')[0]);
							Mainload.MemQShiJiaCanShow_Now.Add(Mainload.Member_other[list[num4]][num5][0] + "|" + list[num4] + "|0|" + Mainload.Member_other[list[num4]][num5][2].Split('|')[4] + "|" + text2 + "|" + Mainload.Member_other[list[num4]][num5][1].Split('|')[1] + "|" + guanFuShowID_MemberQ2);
						}
					}
				}
				if (Mainload.Member_now.Count > 0)
				{
					int ranom = TrueRandom.GetRanom(Mainload.Member_now.Count);
					if (int.Parse(Mainload.Member_now[ranom][6]) >= Mainload.OldFenjie[1] && Mainload.Fudi_now[int.Parse(Mainload.Member_now[ranom][3].Split('|')[0])][0].Split('|')[0] == SceneIndex.ToString() && Mainload.Member_now[ranom][15] == "0")
					{
						string text3 = "0";
						if (int.Parse(Mainload.Member_now[ranom][6]) >= Mainload.OldFenjie[1])
						{
							text3 = "1";
						}
						string guanFuShowID_MemberQ3 = FormulaData.GetGuanFuShowID_MemberQ(Mainload.Member_now[ranom][12].Split('|')[0]);
						Mainload.MemQShiJiaCanShow_Now.Add(Mainload.Member_now[ranom][0] + "|-1|0|" + Mainload.Member_now[ranom][4].Split('|')[4] + "|" + text3 + "|" + Mainload.Member_now[ranom][1].Split('|')[1] + "|" + guanFuShowID_MemberQ3);
					}
				}
				if (Mainload.Member_qu.Count > 0)
				{
					int ranom2 = TrueRandom.GetRanom(Mainload.Member_qu.Count);
					if (int.Parse(Mainload.Member_qu[ranom2][5]) >= Mainload.OldFenjie[0] && Mainload.Fudi_now[int.Parse(Mainload.Member_qu[ranom2][4].Split('|')[0])][0].Split('|')[0] == SceneIndex.ToString() && Mainload.Member_qu[ranom2][11] == "0")
					{
						Mainload.MemQShiJiaCanShow_Now.Add(Mainload.Member_qu[ranom2][0] + "|-1|1|" + Mainload.Member_qu[ranom2][2].Split('|')[4] + "|1|" + Mainload.Member_qu[ranom2][1].Split('|')[1] + "|-1");
					}
				}
			}
			else if (SceneClass == "Z")
			{
				Mainload.PLNQCanShowNow = int.Parse(Mainload.NongZ_now[SceneIndex][int.Parse(Mainload.SceneID.Split('|')[2])][24].Split('|')[0]) + int.Parse(Mainload.NongZ_now[SceneIndex][int.Parse(Mainload.SceneID.Split('|')[2])][24].Split('|')[1]) + int.Parse(Mainload.NongZ_now[SceneIndex][int.Parse(Mainload.SceneID.Split('|')[2])][24].Split('|')[2]);
				if (Mainload.PLNQCanShowNow > Mainload.MemberQShowNum[1])
				{
					Mainload.PLNQCanShowNow = Mainload.MemberQShowNum[1];
				}
			}
			else if (SceneClass == "H")
			{
				Mainload.PLNQCanShowNow = Mainload.MemberQShowNum[3];
			}
			else if (SceneClass == "L")
			{
				Mainload.PLNQCanShowNow = int.Parse(Mainload.Mudi_now[SceneIndex][int.Parse(Mainload.SceneID.Split('|')[2])][9]);
				if (Mainload.PLNQCanShowNow > Mainload.MemberQShowNum[4])
				{
					Mainload.PLNQCanShowNow = Mainload.MemberQShowNum[4];
				}
			}
		}
		else if (isUpdateMemQ)
		{
			isUpdateMemQ = false;
		}
	}

	private void DestroyBuild()
	{
		Mainload.isNoBuild = false;
		Destroy(transform.Find("BuildShow").GetChild(transform.Find("BuildShow").childCount - 1).gameObject);
	}

	private void DestroyBuildC()
	{
		Mainload.isNoBuild = false;
		Destroy(transform.Find("BuildCity").GetChild(transform.Find("BuildCity").childCount - 1).gameObject);
	}

	private void DeleBuildTip()
	{
		for (int i = 0; i < transform.Find("BuildTip").childCount; i++)
		{
			Destroy(transform.Find("BuildTip").GetChild(i).gameObject);
		}
	}

	private void JiaoTuiFeiwo()
	{
		if (Mainload.isNeedJiaoDuiFeiwo)
		{
			Mainload.isNeedJiaoDuiFeiwo = false;
			Mainload.isShiFeiMode = true;
			Invoke(nameof(EndJiaoDuiFeiwo), 0.1f);
		}
	}

	private void EndJiaoDuiFeiwo()
	{
		Mainload.isShiFeiMode = false;
	}
}

