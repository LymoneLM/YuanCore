// 重责任场景建筑管理器

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanCore.Building;

public class PerBackMapSceneEx : MonoBehaviour
{
	private string _sceneClass;

	private int _sceneIndex;

	private bool _isDestroyNpc;

	private string _buildIDCreatSelectLast;

	private GameObject _perBuildTipA;

	private GameObject _perBuildA;

	private bool _isUpdateMemQ;

	private int _loadSpeed;

	private Transform _buildShow;

	private Transform _buildCity;

	private Transform _buildShop;

	private bool _rebuild;

	private void Awake()
	{
		_perBuildA = (GameObject)Resources.Load("PerBuildScene");
		_perBuildTipA = (GameObject)Resources.Load("PerBuildTip");
		_rebuild = false;
		_buildShow = transform.Find("BuildShow");
		_buildCity = transform.Find("BuildCity");
		_buildShop = transform.Find("BuildShop");
	}

	private void Start()
	{
		InitData();
		InitShow();
	}

	private void Update()
	{
		UpdateShow();
	}

	private void InitData()
	{
		Mainload.TempMemberIndex_now = 0;
		_isUpdateMemQ = false;
		_sceneClass = Mainload.SceneID.Split('|')[0];
		_sceneIndex = int.Parse(Mainload.SceneID.Split('|')[1]);
		_buildIDCreatSelectLast = "-1";
		Mainload.BuildPosiID_Now = "0|0";
		Mainload.BuildID_CreatNow = "null";
		_isDestroyNpc = false;
		if (_sceneClass == "Z")
		{
			string[] array = Mainload.NongZ_now[_sceneIndex][int.Parse(Mainload.SceneID.Split('|')[2])][24].Split('|');
			Mainload.LastNonghuNum_Open = new List<float>
			{
				float.Parse(array[0]),
				float.Parse(array[1]),
				float.Parse(array[2])
			};
			SaveData.ReadBuildData(_sceneClass, _sceneIndex.ToString(), Mainload.SceneID.Split('|')[2]);
		}
		else if (_sceneClass == "L")
		{
			SaveData.ReadBuildData(_sceneClass, _sceneIndex.ToString(), Mainload.SceneID.Split('|')[2]);
		}
		else if (_sceneClass == "S")
		{
			SaveData.ReadBuildData(_sceneClass, _sceneIndex.ToString(), "0");
			if (Mainload.ShopData_updateTime[_sceneIndex].Count <= 0)
			{
				for (int i = 0; i < Mainload.BuildInto_s.Count; i++)
				{
					Mainload.ShopData_updateTime[_sceneIndex].Add(0);
					Mainload.Prop_shop_temp[_sceneIndex].Add(new List<string>());
					Mainload.Horse_Shop_Temp[_sceneIndex].Add(new List<List<string>>());
					Mainload.HuaiZhang_Shop_Temp[_sceneIndex].Add(new List<List<string>>());
					Mainload.OtherTrade_shop_Temp[_sceneIndex].Add(new List<List<string>>());
				}
			}
		}
		else if (!Mainload.isFirstGame)
		{
			SaveData.ReadBuildData(_sceneClass, _sceneIndex.ToString(), "0");
		}
		if (Mainload.SetData[5] == 0)
		{
			_loadSpeed = 50;
		}
		else if (Mainload.SetData[5] == 1)
		{
			_loadSpeed = 200;
		}
		else if (Mainload.SetData[5] == 2)
		{
			_loadSpeed = 500;
		}
		else if (Mainload.SetData[5] == 3)
		{
			_loadSpeed = 100000;
		}
	}

	private void InitShow()
	{
		if (_sceneClass == "M")
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
		else if (_sceneClass == "Z")
		{
			if (Mainload.BuildInto_z.Count <= 0)
			{
				Mainload.isCreatSceneFinish = true;
				int num = int.Parse(Mainload.SceneID.Split('|')[2]);
				string[] array = Mainload.NongZ_now[_sceneIndex][num][2].Split('|');
				for (int i = 0; i < transform.Find("AddClickBack").Find("AllKuai").childCount; i++)
				{
					transform.Find("AddClickBack").Find("AllKuai").GetChild(i)
						.GetComponent<PerFeiWoRange>()
						.FeiWoNum = int.Parse(array[i]);
					transform.Find("AddClickBack").Find("AllKuai").GetChild(i)
						.GetComponent<PerFeiWoRange>()
						.FengDiIndex = _sceneIndex;
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
		else if (_sceneClass == "S")
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
		else if (_sceneClass == "H")
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
		else if (_sceneClass == "L")
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

        var obj = Instantiate(_perBuildA, _buildShow, true);
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
		_buildShow.DestroyAllChildren();
		for (var a = 0; a < Mainload.BuildInto_m.Count; a++)
        {
            AddBuilding(a, "M", Mainload.BuildInto_m[a][5]);

			if ((a + 1) % _loadSpeed == 0)
			{
				yield return null;
			}
		}
        Mainload.isCreatSceneFinish = true;
	}

	private IEnumerator ReloadBuildingZ()
	{
        _buildShow.DestroyAllChildren();
        for (int a = 0; a < Mainload.BuildInto_z.Count; a++)
		{
            AddBuilding(a, "Z", Mainload.BuildInto_z[a][3]);

			if ((a + 1) % _loadSpeed == 0)
			{
				yield return null;
			}
		}

        int NongZIndex = int.Parse(Mainload.SceneID.Split('|')[2]);
        Mainload.isCreatSceneFinish = true;
        StopCoroutine(nameof(ReloadBuildingZ));
        string[] array = Mainload.NongZ_now[_sceneIndex][NongZIndex][2].Split('|');
        for (int j = 0; j < transform.Find("AddClickBack").Find("AllKuai").childCount; j++)
        {
            transform.Find("AddClickBack").Find("AllKuai").GetChild(j)
                .GetComponent<PerFeiWoRange>()
                .FeiWoNum = int.Parse(array[j]);
            transform.Find("AddClickBack").Find("AllKuai").GetChild(j)
                .GetComponent<PerFeiWoRange>()
                .FengDiIndex = _sceneIndex;
            transform.Find("AddClickBack").Find("AllKuai").GetChild(j)
                .GetComponent<PerFeiWoRange>()
                .NongZIndex = NongZIndex;
        }
        Invoke(nameof(JiaoTuiFeiwo), 0.1f);
	}

    //TODO：处理郡城S和C的特殊情况
	private IEnumerator ReloadBuildingS()
	{
		for (int i = 0; i < _buildShop.childCount; i++)
		{
			Destroy(_buildShop.GetChild(i).gameObject);
		}
		for (int a = 0; a < Mainload.BuildInto_s.Count; a++)
		{
			var AddbuildPosiA = int.Parse(Mainload.BuildInto_s[a][4].Split('|')[0]);
			var AddbuildPosiB = int.Parse(Mainload.BuildInto_s[a][4].Split('|')[1]);
			GameObject obj = Instantiate(_perBuildA);
			obj.transform.GetComponent<PerBuildScene>().BuildBigClass = "S";
			obj.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
			obj.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
			obj.name = a.ToString();
			obj.transform.SetParent(_buildShop);
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
			if ((a + 1) % _loadSpeed == 0)
			{
				yield return null;
			}
		}
	}

	private IEnumerator ReloadBuildingC()
	{
		for (int i = 0; i < _buildCity.childCount; i++)
		{
			Destroy(_buildCity.GetChild(i).gameObject);
		}
		for (int a = 0; a < Mainload.BuildInto_c.Count; a++)
		{
			var AddbuildPosiA = int.Parse(Mainload.BuildInto_c[a][3].Split('|')[0]);
			var AddbuildPosiB = int.Parse(Mainload.BuildInto_c[a][3].Split('|')[1]);
			GameObject obj = Instantiate(_perBuildA);
			obj.transform.GetComponent<PerBuildScene>().BuildBigClass = "C";
			obj.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
			obj.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
			obj.name = a.ToString();
			obj.transform.SetParent(_buildCity);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			obj.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
			if (a >= Mainload.BuildInto_c.Count - 1)
			{
				Mainload.isCreatSceneFinish = true;
				StopCoroutine(nameof(ReloadBuildingC));
			}
			if ((a + 1) % _loadSpeed == 0)
			{
				yield return null;
			}
		}
	}

	private IEnumerator ReloadBuildingH()
	{
		_buildShow.DestroyAllChildren();
		for (var a = 0; a < Mainload.BuildInto_h.Count; a++)
		{
            AddBuilding(a, "H", Mainload.BuildInto_h[a][3]);

			if ((a + 1) % _loadSpeed == 0)
			{
				yield return null;
			}
		}
        Mainload.isCreatSceneFinish = true;
	}

	private IEnumerator ReloadBuildingL()
	{
		_buildShow.DestroyAllChildren();
		for (var a = 0; a < Mainload.BuildInto_l.Count; a++)
		{
            AddBuilding(a, "L", Mainload.BuildInto_l[a][3]);

			if ((a + 1) % _loadSpeed == 0)
			{
				yield return null;
			}
		}
        Mainload.isCreatSceneFinish = true;
	}

	private void UpdateShow()
	{
        // 肥沃度更新
		if (Mainload.isShiFeiMode && _sceneClass == "Z" && Mainload.isChangeFeiWoNongZ)
		{
			Mainload.isChangeFeiWoNongZ = false;
			int index = int.Parse(Mainload.SceneID.Split('|')[2]);
			string[] array = Mainload.NongZ_now[_sceneIndex][index][2].Split('|');
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
		if (Mainload.isBuildMode || (_rebuild && Mainload.isBuildEdit))
		{
			_rebuild = false;
			if (_buildIDCreatSelectLast != Mainload.BuildID_Creatselect)
			{
				_buildIDCreatSelectLast = Mainload.BuildID_Creatselect;
				if (Mainload.isBuildMode)
				{
					for (int j = 0; j < transform.Find("BuildTip").childCount; j++)
					{
						Destroy(transform.Find("BuildTip").GetChild(j).gameObject);
					}
					GameObject obj = Instantiate(_perBuildTipA);
					obj.transform.SetParent(transform.Find("BuildTip"));
					obj.transform.localScale = new Vector3(1f, 1f, 1f);
				}
			}
			if (Mainload.BuildID_CreatNow != "null")
			{
				var AddbuildPosiA = int.Parse(Mainload.BuildPosiID_CreatNow.Split('|')[0]);
				var AddbuildPosiB = int.Parse(Mainload.BuildPosiID_CreatNow.Split('|')[1]);
				if (_sceneClass == "M")
				{
					FormulaData.AddBuildNew("M", _sceneIndex, Mainload.BuildID_CreatNow, Mainload.BuildPosiID_CreatNow,
                        0, Mainload.BuildDirID_CreatSelect, isFamily: true, Mainload.BuildTaoZhuangID_SelectNow);
					GameObject obj2 = Instantiate(_perBuildA);
					obj2.transform.GetComponent<PerBuildScene>().BuildBigClass = "M";
					obj2.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
					obj2.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
					obj2.name = (Mainload.BuildInto_m.Count - 1).ToString();
					obj2.transform.SetParent(_buildShow);
					obj2.transform.localScale = new Vector3(1f, 1f, 1f);
					obj2.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
					Mainload.AddBuildNum_Now++;
				}
				else if (_sceneClass == "Z")
				{
					int indexB = int.Parse(Mainload.SceneID.Split('|')[2]);
					FormulaData.AddBuildNew("Z", _sceneIndex, Mainload.BuildID_CreatNow, Mainload.BuildPosiID_CreatNow,
                        indexB, Mainload.BuildDirID_CreatSelect, isFamily: true, Mainload.BuildTaoZhuangID_SelectNow);
					GameObject obj3 = Instantiate(_perBuildA);
					obj3.transform.GetComponent<PerBuildScene>().BuildBigClass = "Z";
					obj3.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
					obj3.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
					obj3.name = (Mainload.BuildInto_z.Count - 1).ToString();
					obj3.transform.SetParent(_buildShow);
					obj3.transform.localScale = new Vector3(1f, 1f, 1f);
					obj3.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
					Mainload.AddBuildNum_Now++;
				}
				else if (_sceneClass == "S")
				{
					if (Mainload.BuildCityClass == "S0")
					{
						FormulaData.AddBuildNew("S", _sceneIndex, Mainload.BuildID_CreatNow,
                            Mainload.BuildPosiID_CreatNow, 0, Mainload.BuildDirID_CreatSelect, isFamily: false,
                            Mainload.BuildTaoZhuangID_SelectNow);
						GameObject obj4 = Instantiate(_perBuildA);
						obj4.transform.GetComponent<PerBuildScene>().BuildBigClass = "S";
						obj4.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
						obj4.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
						obj4.name = (Mainload.BuildInto_s.Count - 1).ToString();
						obj4.transform.SetParent(_buildShop);
						obj4.transform.localScale = new Vector3(1f, 1f, 1f);
						obj4.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
						Mainload.AddBuildNum_Now = 0;
					}
					else if (Mainload.BuildCityClass == "S1")
					{
						FormulaData.AddBuildNew("S", _sceneIndex, Mainload.BuildID_CreatNow,
                            Mainload.BuildPosiID_CreatNow, 0, Mainload.BuildDirID_CreatSelect, isFamily: true,
                            Mainload.BuildTaoZhuangID_SelectNow);
						GameObject obj5 = Instantiate(_perBuildA);
						obj5.transform.GetComponent<PerBuildScene>().BuildBigClass = "S";
						obj5.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
						obj5.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
						obj5.name = (Mainload.BuildInto_s.Count - 1).ToString();
						obj5.transform.SetParent(_buildShop);
						obj5.transform.localScale = new Vector3(1f, 1f, 1f);
						obj5.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
						Mainload.AddBuildNum_Now = 0;
					}
					else if (Mainload.BuildCityClass == "C")
					{
						FormulaData.AddBuildNew("C", _sceneIndex, Mainload.BuildID_CreatNow,
                            Mainload.BuildPosiID_CreatNow, 0, Mainload.BuildDirID_CreatSelect, isFamily: false,
                            Mainload.BuildTaoZhuangID_SelectNow);
						GameObject obj6 = Instantiate(_perBuildA);
						obj6.transform.GetComponent<PerBuildScene>().BuildBigClass = "C";
						obj6.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
						obj6.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
						obj6.name = (Mainload.BuildInto_c.Count - 1).ToString();
						obj6.transform.SetParent(_buildCity);
						obj6.transform.localScale = new Vector3(1f, 1f, 1f);
						obj6.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
						Mainload.AddBuildNum_Now++;
					}
				}
				else if (_sceneClass == "H")
				{
					FormulaData.AddBuildNew("H", _sceneIndex, Mainload.BuildID_CreatNow, Mainload.BuildPosiID_CreatNow,
                        0, Mainload.BuildDirID_CreatSelect, isFamily: true, Mainload.BuildTaoZhuangID_SelectNow);
					GameObject obj7 = Instantiate(_perBuildA);
					obj7.transform.GetComponent<PerBuildScene>().BuildBigClass = "H";
					obj7.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
					obj7.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
					obj7.name = (Mainload.BuildInto_h.Count - 1).ToString();
					obj7.transform.SetParent(_buildShow);
					obj7.transform.localScale = new Vector3(1f, 1f, 1f);
					obj7.transform.localPosition = FormulaData.GetBuildPosi(AddbuildPosiA, AddbuildPosiB);
					Mainload.AddBuildNum_Now++;
				}
				else if (_sceneClass == "L")
				{
					int indexB2 = int.Parse(Mainload.SceneID.Split('|')[2]);
					FormulaData.AddBuildNew("L", _sceneIndex, Mainload.BuildID_CreatNow, Mainload.BuildPosiID_CreatNow,
                        indexB2, Mainload.BuildDirID_CreatSelect, isFamily: true, Mainload.BuildTaoZhuangID_SelectNow);
					GameObject obj8 = Instantiate(_perBuildA);
					obj8.transform.GetComponent<PerBuildScene>().BuildBigClass = "L";
					obj8.transform.GetComponent<PerBuildScene>().PosiA = AddbuildPosiA;
					obj8.transform.GetComponent<PerBuildScene>().PosiB = AddbuildPosiB;
					obj8.name = (Mainload.BuildInto_l.Count - 1).ToString();
					obj8.transform.SetParent(_buildShow);
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
				if (_sceneClass == "M")
				{
					FormulaData.RemoveBuildNew("M", _sceneIndex, _buildShow.childCount - 1, 0, TipShow: false, isCtrlZ: true);
					_buildShow.GetChild(_buildShow.childCount - 1).localPosition = new Vector3(-10000f, -10000f, 0f);
					Invoke(nameof(DestroyBuild), 0.05f);
				}
				else if (_sceneClass == "Z")
				{
					int indexB3 = int.Parse(Mainload.SceneID.Split('|')[2]);
					FormulaData.RemoveBuildNew("Z", _sceneIndex, indexB3, _buildShow.childCount - 1, TipShow: false, isCtrlZ: true);
					_buildShow.GetChild(_buildShow.childCount - 1).localPosition = new Vector3(-10000f, -10000f, 0f);
					Invoke(nameof(DestroyBuild), 0.05f);
				}
				else if (_sceneClass == "S")
				{
					FormulaData.RemoveBuildNew(_buildCity.GetChild(_buildCity.childCount - 1).GetComponent<PerBuildScene>().BuildBigClass, _sceneIndex, _buildCity.childCount - 1, 0, TipShow: false, isCtrlZ: true);
					_buildCity.GetChild(_buildCity.childCount - 1).localPosition = new Vector3(-10000f, -10000f, 0f);
					Invoke(nameof(DestroyBuildC), 0.05f);
				}
				else if (_sceneClass == "H")
				{
					FormulaData.RemoveBuildNew("H", _sceneIndex, _buildShow.childCount - 1, 0, TipShow: false, isCtrlZ: true);
					_buildShow.GetChild(_buildShow.childCount - 1).localPosition = new Vector3(-10000f, -10000f, 0f);
					Invoke(nameof(DestroyBuild), 0.05f);
				}
				else if (_sceneClass == "L")
				{
					int indexB4 = int.Parse(Mainload.SceneID.Split('|')[2]);
					FormulaData.RemoveBuildNew("L", _sceneIndex, indexB4, _buildShow.childCount - 1, TipShow: false, isCtrlZ: true);
					_buildShow.GetChild(_buildShow.childCount - 1).localPosition = new Vector3(-10000f, -10000f, 0f);
					Invoke(nameof(DestroyBuild), 0.05f);
				}
			}
		}
		else if (_buildIDCreatSelectLast != "-1")
		{
			_buildIDCreatSelectLast = "-1";
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
				_rebuild = true;
				if (_sceneClass == "M")
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
						Mainload.Fudi_now[_sceneIndex][39] = (int.Parse(Mainload.Fudi_now[_sceneIndex][39]) + 1).ToString();
					}
					else if (Mainload.BuildID_CreatNow == "2" || Mainload.BuildID_CreatNow == "3" || Mainload.BuildID_CreatNow == "4")
					{
						Mainload.Fudi_now[_sceneIndex][38] = (int.Parse(Mainload.Fudi_now[_sceneIndex][38]) + 1).ToString();
					}
				}
				else if (_sceneClass == "Z")
				{
					Mainload.BuildID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][1];
					Mainload.BuildPosiID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][3];
					Mainload.BuildDirID_CreatSelect = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][4];
					Mainload.BuildTaoZhuangID_SelectNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][10];
					FormulaData.ChangeCoins(-Mathf.RoundToInt(float.Parse(Mainload.AllBuilddata[int.Parse(Mainload.BuildID_CreatNow)][1].Split('|')[0]) * 0.5f));
				}
				else if (_sceneClass == "S")
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
							Mainload.CityData_now[_sceneIndex][0][9] = (int.Parse(Mainload.CityData_now[_sceneIndex][0][9]) - Mathf.RoundToInt(float.Parse(Mainload.AllBuilddata[int.Parse(Mainload.BuildID_CreatNow)][1].Split('|')[0]) * num2)).ToString();
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
						Mainload.CityData_now[_sceneIndex][0][9] = (int.Parse(Mainload.CityData_now[_sceneIndex][0][9]) - Mathf.RoundToInt(float.Parse(Mainload.AllBuilddata[int.Parse(Mainload.BuildID_CreatNow)][1].Split('|')[0]) * num3)).ToString();
					}
				}
				else if (_sceneClass == "H")
				{
					Mainload.BuildID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][1];
					Mainload.BuildPosiID_CreatNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][3];
					Mainload.BuildDirID_CreatSelect = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][4];
					Mainload.BuildTaoZhuangID_SelectNow = Mainload.BuildData_Delete_Temp[Mainload.BuildData_Delete_Temp.Count - 1][5];
					FormulaData.CostKing_FromGuoKu(7, Mathf.RoundToInt(float.Parse(Mainload.AllBuilddata[int.Parse(Mainload.BuildID_CreatNow)][1].Split('|')[0]) * 0.5f));
				}
				else if (_sceneClass == "L")
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
			if (!_isDestroyNpc)
			{
				_isDestroyNpc = true;
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
			_isDestroyNpc = false;
		}
		if (!Mainload.isBuildEdit && !Mainload.isBuildMode && !Mainload.isShiFeiMode)
		{
			if (_isUpdateMemQ)
			{
				return;
			}
			_isUpdateMemQ = true;
			Mainload.QMemberID_Remove = new List<string>();
			Mainload.QMemberID_Change = new List<string>();
			if (_sceneClass == "M")
			{
				Mainload.MemQShiJiaCanShow_Now = new List<string>();
				Mainload.PLNQCanShowNow = int.Parse(Mainload.Fudi_now[_sceneIndex][2]);
				if (Mainload.PLNQCanShowNow > Mainload.MemberQShowNum[0])
				{
					Mainload.PLNQCanShowNow = Mainload.MemberQShowNum[0];
				}
				for (int l = 0; l < Mainload.Member_now.Count; l++)
				{
					if (int.Parse(Mainload.Member_now[l][6]) >= Mainload.OldFenjie[0] && Mainload.Member_now[l][3].Split('|')[0] == _sceneIndex.ToString() && Mainload.Member_now[l][15] == "0")
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
					if (int.Parse(Mainload.Member_qu[m][5]) >= Mainload.OldFenjie[0] && Mainload.Member_qu[m][4].Split('|')[0] == _sceneIndex.ToString() && (Mainload.Member_qu[m][11] == "0" || Mainload.Member_qu[m][11] == "1"))
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
			else if (_sceneClass == "S")
			{
				Mainload.MemQShiJiaCanShow_Now = new List<string>();
				Mainload.PLNQCanShowNow = FormulaData.CityQMemberNumShow(int.Parse(Mainload.CityData_now[_sceneIndex][0][10]));
				if (Mainload.PLNQCanShowNow > Mainload.MemberQShowNum[2])
				{
					Mainload.PLNQCanShowNow = Mainload.MemberQShowNum[2];
				}
				List<int> list = new List<int>();
				for (int n = 0; n < Mainload.ShiJia_Now.Count; n++)
				{
					if (Mainload.ShiJia_Now[n][0] == "0" && Mainload.ShiJia_Now[n][5].Split('|')[0] == _sceneIndex.ToString())
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
					if (int.Parse(Mainload.Member_now[ranom][6]) >= Mainload.OldFenjie[1] && Mainload.Fudi_now[int.Parse(Mainload.Member_now[ranom][3].Split('|')[0])][0].Split('|')[0] == _sceneIndex.ToString() && Mainload.Member_now[ranom][15] == "0")
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
					if (int.Parse(Mainload.Member_qu[ranom2][5]) >= Mainload.OldFenjie[0] && Mainload.Fudi_now[int.Parse(Mainload.Member_qu[ranom2][4].Split('|')[0])][0].Split('|')[0] == _sceneIndex.ToString() && Mainload.Member_qu[ranom2][11] == "0")
					{
						Mainload.MemQShiJiaCanShow_Now.Add(Mainload.Member_qu[ranom2][0] + "|-1|1|" + Mainload.Member_qu[ranom2][2].Split('|')[4] + "|1|" + Mainload.Member_qu[ranom2][1].Split('|')[1] + "|-1");
					}
				}
			}
			else if (_sceneClass == "Z")
			{
				Mainload.PLNQCanShowNow = int.Parse(Mainload.NongZ_now[_sceneIndex][int.Parse(Mainload.SceneID.Split('|')[2])][24].Split('|')[0]) + int.Parse(Mainload.NongZ_now[_sceneIndex][int.Parse(Mainload.SceneID.Split('|')[2])][24].Split('|')[1]) + int.Parse(Mainload.NongZ_now[_sceneIndex][int.Parse(Mainload.SceneID.Split('|')[2])][24].Split('|')[2]);
				if (Mainload.PLNQCanShowNow > Mainload.MemberQShowNum[1])
				{
					Mainload.PLNQCanShowNow = Mainload.MemberQShowNum[1];
				}
			}
			else if (_sceneClass == "H")
			{
				Mainload.PLNQCanShowNow = Mainload.MemberQShowNum[3];
			}
			else if (_sceneClass == "L")
			{
				Mainload.PLNQCanShowNow = int.Parse(Mainload.Mudi_now[_sceneIndex][int.Parse(Mainload.SceneID.Split('|')[2])][9]);
				if (Mainload.PLNQCanShowNow > Mainload.MemberQShowNum[4])
				{
					Mainload.PLNQCanShowNow = Mainload.MemberQShowNum[4];
				}
			}
		}
		else if (_isUpdateMemQ)
		{
			_isUpdateMemQ = false;
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

