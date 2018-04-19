using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{



	public delegate void CallBack ();
	public delegate void ChooseCallBack(bool arg);

//	public delegate void CallBack<T>(T[] parameters);

	public delegate void ExploreEventHandler (Transform colliderTrans);


	public struct CommonData{

		public static string originDataPath = Application.streamingAssetsPath + "/Data";
		public static string persistDataPath = Application.persistentDataPath + "/Data";

		//针对安卓客户端的初始化目录结构写法
		//字典数组中诺列了所有的数据
//		public static Dictionary<string, string>[] originDataArr = {
//
//			
//
//			new Dictionary<string, string>(){{"fileName","BuyRecord"},{"filePath","/Data/BuyRecord.json"}},
//			new Dictionary<string, string>(){{"fileName","GameLevelDatas"},{"filePath","/Data/GameLevelDatas.json"}},
//			new Dictionary<string, string>(){{"fileName","GameSettings"},{"filePath","/Data/GameSettings.json"}},
//			new Dictionary<string, string>(){{"fileName","MyGameDB"},{"filePath","/Data/MyGameDB.db"}},
//			new Dictionary<string, string>(){{"fileName","NewItemDatas"},{"filePath","/Data/EquipmentDatas.json"}},
//			new Dictionary<string, string>(){{"fileName","OriginalPlayerData"},{"filePath","/Data/OriginalPlayerData.json"}},
//
//			//这里的level从0-29，应该循环
//			new Dictionary<string, string>(){{"fileName","Level"},{"filePath","/Data/MapData/Level"}},
//			new Dictionary<string, string>(){{"fileName","Trader_TraderMan"},{"filePath","/Data/NPCs/Trader_TraderMan.json"}}
//
//		};

		public static KVPair[] originDataArr = new KVPair[]{
			new KVPair("BuyRecord","/Data/BuyRecord.json"),
			new KVPair("GameLevelDatas","/Data/GameLevelDatas.json"),
			new KVPair("GameSettings","/Data/GameSettings.json"),
			new KVPair("MyGameDB","/Data/MyGameDB.db"),
			new KVPair("EquipmentDatas","/Data/GameItems/EquipmentDatas.json"),
			new KVPair("ConsumablesDatas","/Data/GameItems/ConsumablesDatas.json"),
			new KVPair("SkillGemstoneDatas","/Data/GameItems/SkillGemstoneDatas.json"),
			new KVPair("OriginalPlayerData","/Data/OriginalPlayerData.json"),
			new KVPair("Level","/Data/MapData/Level"),
			new KVPair("NPC","/Data/NPCs"),
		};


		public static string assetBundleRootName = "AssetBundle";

		public static string effectsDataFilePath = persistDataPath + "/TestEffectString.txt";
		public static string gameLevelDataFilePath = persistDataPath + "/GameLevelDatas.json";
		public static string equipmentDataFilePath = persistDataPath + "/GameItems/EquipmentDatas.json";
		public static string consumablesDataFilePath = persistDataPath + "/GameItems/ConsumablesDatas.json";
		public static string skillGemstoneDataFilePath = persistDataPath + "/GameItems/SkillGemstoneDatas.json";
		public static string materialsDataFilePath = persistDataPath + "/Materials.json";
		public static string npcsDataDirPath = persistDataPath + "/NPCs";

		public static string buyRecordFilePath = persistDataPath + "/BuyRecord.json";



		public static string dataBaseName = "MyGameDB.db";

		public static string NMETTable = "NMET";
		public static string CET46Table = "CET46";
		public static string GRETable = "GRE";
		public static string TOEFLTable = "TOEFL";

		public static string instanceContainerName = "InstanceContainer";
		public static string poolContainerName = "PoolsContainer";

		public static string exploreScenePoolContainerName = CommonData.poolContainerName + "/ExploreScenePoolContainer";
		public static string bagCanvasPoolContainerName = CommonData.poolContainerName + "/BagCanvasPoolContainer";
		public static string learnCanvasPoolContainerName = CommonData.poolContainerName + "/LearnCanvasPoolContainer";


		public static string homeCanvasBundleName = "home/canvas";
		public static string recordCanvasBundleName = "record/canvas";
		public static string unlockedItemsCanvasBundleName = "unlockeditems/canvas";
		public static string bagCanvasBundleName = "bag/canvas";
		public static string settingCanvasBundleName = "setting/canvas";
		public static string spellCanvasBundleName = "spell/canvas";
		public static string exploreSceneBundleName = "explore/scene";
		public static string produceCanvasBundleName = "produce/canvas";
		public static string learnCanvasBundleName = "learn/canvas";
		public static string allMonstersBundleName = "explore/monsters";
		public static string allMapNpcBundleName = "explore/npcs";
		public static string allEffectsBundleName = "explore/effects";

		public static string allEquipmentSpritesBundleName = "item/equipment_icons";
		public static string allConsumablesSpritesBundleName = "item/consumables_icons";
		public static string allSkillGemstoneSpritesBundleName = "item/gemstone_icons";
		public static string allMapSpritesBundleName = "explore/mapicons";
		public static string allSkillsBundleName = "skills/skills";
		public static string allSkillSpritesBundleName = "skills/icons";
		public static string allUISpritesBundleName = "ui/common";
	

		public static string skillsContainerName = CommonData.instanceContainerName + "/AllSkills";
		public static string monstersContainerName = CommonData.instanceContainerName + "/AllMonsters";


		public static string roleIdleAnimName = "wait";
		public static string roleWalkAnimName = "walk";
		public static string roleRunAnimName = "run";
		public static string roleAttackAnimName = "attack";
		public static string roleAttackIntervalAnimName = "interval";
		public static string roleDieAnimName = "death";

		public static int aInASCII = (int)('a');

		// 当前屏幕分辨率和预设屏幕分辨率之间的转换比例
		public static float scalerToPresetResulotion = 1920f / Camera.main.pixelHeight;

		public static int singleBagItemVolume = 24;

		public static char diamond = (char)6;
		public static int totalFadeStep = 20;

		public static Vector3 selectedColor = new Vector3 (238f/255, 206f/255, 149f/255);
		public static Vector3 deselectedColor = new Vector3 (63f/255, 31f/255, 0);

		public static string homeBgmName = "Castle";
		public static string exploreBgmName = "Explore";

		public static char[] wholeAlphabet = new char[]{'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'};

		public static Vector2 iphoneXResolution = new Vector2(1125,2436);

	}




	public enum WordType{
		NMET,
		CET46,
		TOEFL,
		GRE
	}


	// 附加信息层对应的附加信息
	public enum MapEventType{
		Exit,
		Crystal,
		Trader,
		NPC,
		Door,
		Buck,
		Pot,
		TreasureBox,
		Stone,
		Tree,
		Switch,
		TrapOff,
		TrapOn,
		Monster,
		FireTrap,
		Billboard,
		SecretRoom,
		MovableBox,
		PressSwitch,
		Docoration
	}


	// Using Serializable allows us to embed a class with sub properties in the inspector.
	[System.Serializable]
	public class Count
	{
		public int minimum; 			//Minimum value for our Count class.
		public int maximum; 			//Maximum value for our Count class.


		//Assignment constructor.
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}

		public int GetAValueWithinRange(){
			int value = Random.Range (minimum, maximum + 1);
			return value;
		}

	}


		
}