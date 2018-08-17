using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{

	public class LearnTitleQualification
    {
        public int totalWordsCount;
        public float totalCorrectPercentage;
        public int continuousCorrectWordCount;
		public string title;
		public string qualificationNeed;
		public int rewardGold;

		public LearnTitleQualification(int totalWordsCount, float totalCorrectPercentage, int continuousCorrectWordCount, string title,string qualificationNeed, int rewardGold)
		{
			this.totalWordsCount = totalWordsCount;
			this.totalCorrectPercentage = totalCorrectPercentage;
			this.continuousCorrectWordCount = continuousCorrectWordCount;
			this.qualificationNeed = qualificationNeed;
			this.title = title;
			this.rewardGold = rewardGold;
		}
		

    }


	public delegate void CallBack();
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

		public static KVPair[] originDataArr = {
			new KVPair("BuyRecord","/Data/BuyRecord.json"),
			new KVPair("GameLevelDatas","/Data/GameLevelDatas.json"),
			new KVPair("GameSettings","/Data/GameSettings.json"),
			new KVPair("MyGameDB","/Data/MyGameDB.db"),
			new KVPair("EquipmentDatas","/Data/GameItems/EquipmentDatas.json"),
			new KVPair("ConsumablesDatas","/Data/GameItems/ConsumablesDatas.json"),
			new KVPair("PropertyGemstoneDatas","/Data/GameItems/PropertyGemstoneDatas.json"),
			new KVPair("SkillScrollDatas","/Data/GameItems/SkillScrollDatas.json"),
			new KVPair("SpecialItemDatas","/Data/GameItems/SpecialItemDatas.json"),
			new KVPair("SpellItemDatas","/Data/GameItems/SpellItemDatas.json"),
			new KVPair("OriginalPlayerData","/Data/OriginalPlayerData.json"),
			new KVPair("Level","/Data/MapData/Level"),
			new KVPair("NPC","/Data/NPCs"),
            new KVPair("ProverbData","/Data/HLHSentenceAndPoemData.json"),
			new KVPair("MapEventsRecord","/Data/MapEventsRecord.json"),
			new KVPair("DiaryData","/Data/DiaryData.json")
		};


		public static string assetBundleRootName = "AssetBundle";
        
		public static string gameLevelDataFilePath = persistDataPath + "/GameLevelDatas.json";
		public static string equipmentDataFilePath = persistDataPath + "/GameItems/EquipmentDatas.json";
		public static string consumablesDataFilePath = persistDataPath + "/GameItems/ConsumablesDatas.json";
		public static string propertyGemstoneDataFilePath = persistDataPath + "/GameItems/PropertyGemstoneDatas.json";
		public static string skillScrollDataFilePath = persistDataPath + "/GameItems/SkillScrollDatas.json";
		public static string specialItemDataFilePath = persistDataPath + "/GameItems/SpecialItemDatas.json";
		public static string spellItemDataFilePath = persistDataPath + "/GameItems/SpellItemDatas.json";
        public static string proverbsDataFilePath = persistDataPath + "/HLHSentenceAndPoemData.json";
		public static string diaryDataFilePath = persistDataPath + "/DiaryData.json";
		public static string npcsDataDirPath = persistDataPath + "/NPCs";

		public static string buyRecordFilePath = persistDataPath + "/BuyRecord.json";
		public static string chatRecordsFilePath = persistDataPath + "/ChatRecords.json";
		public static string mapEventsRecordFilePath = persistDataPath + "/MapEventsRecord.json";



		public static string dataBaseName = "MyGameDB.db";


		public static string simpleWordsTable = "SIMPLE";
		public static string mediumWordsTabel = "MEDIUM";
		public static string masterWordsTabel = "MASTER";

		public static string instanceContainerName = "InstanceContainer";
		public static string poolContainerName = "PoolsContainer";

		public static string exploreScenePoolContainerName = CommonData.poolContainerName + "/ExploreScenePoolContainer";
		public static string bagCanvasPoolContainerName = CommonData.poolContainerName + "/BagCanvasPoolContainer";
		public static string learnCanvasPoolContainerName = CommonData.poolContainerName + "/LearnCanvasPoolContainer";


		public static string homeCanvasBundleName = "home/canvas";
		public static string recordCanvasBundleName = "record/canvas";
		public static string bagCanvasBundleName = "bag/canvas";
		public static string settingCanvasBundleName = "setting/canvas";
		public static string spellCanvasBundleName = "spell/canvas";
		public static string exploreSceneBundleName = "explore/scene";
		public static string loadingCanvasBundleName = "loading/canvas";
		public static string guideCanvasBundleName = "guide/canvas";
		public static string learnCanvasBundleName = "learn/canvas";
		public static string npcCanvasBundleName = "npc/canvas";
		public static string shareCanvasBundleName = "share/canvas";
		public static string allMonstersBundleName = "explore/monsters";
		public static string allMapNpcBundleName = "explore/npcs";
		public static string allEffectsBundleName = "skills/effects";
		public static string finalChapterCanvasBundleName = "finalchapter/canvas";

		public static string allEquipmentSpritesBundleName = "item/equipment_icons";
		public static string allConsumablesSpritesBundleName = "item/consumables_icons";
		public static string allPropertyGemstoneSpritesBundleName = "item/propertygemstone_icons";
		public static string allSkillScrollSpritesBundleName = "item/skillscroll_icons";
		public static string allSpecialItemSpritesBundleName = "item/specialitem_icons";
		public static string allMapSpritesBundleName = "explore/mapicons";
		public static string allCharacterSpritesBundleName = "item/character_icons";
		public static string allSkillsBundleName = "skills/skills";
		public static string allSkillSpritesBundleName = "skills/icons";
		public static string allUISpritesBundleName = "ui/common";

		public static string mapTileset_1_BundleName = "explore/maptileset_1";
		public static string mapTileset_2_BundleName = "explore/maptileset_2";
		public static string mapTileset_3_BundleName = "explore/maptileset_3";
		public static string mapTileset_4_BundleName = "explore/maptileset_4";
		public static string mapTileset_5_BundleName = "explore/maptileset_5";
	

		public static string skillsContainerName = CommonData.instanceContainerName + "/AllSkills";
		public static string monstersContainerName = CommonData.instanceContainerName + "/AllMonsters";

		public static string exploreDarktMaskAnimName = "dark";
		public static string exploreLightMaskAnimName = "light";

        public static string regularEffectAnimName = "default";

		public static string roleIdleAnimName = "wait";
		public static string roleWalkAnimName = "walk";
		public static string roleRunAnimName = "run";
		public static string roleDieAnimName = "death";


		public static string roleAttackAnimName = "attack";
		public static string rolePhysicalSkillAnimName = "physical_skill";
		public static string roleMagicalSkillAnimName = "magical_skill";
		public static string roleAttackIntervalAnimName = "interval";

		//******* 玩家使用武器攻击的动画名称 *********//
		public static string playerAttackBareHandName = "attack_default";
		public static string playerAttackWithSwordName = "attack_sword";
		public static string playerAttackWithStaffName = "attack_staff";
		public static string playerAttackWithAxeName = "attack_axe";
		public static string playerAttackWithDraggerName = "attack_dragger";

		//******* 玩家使用物理伤害型技能攻击的动画名称 *********//
		public static string playerPhysicalSkillBareHandName = "skill1_default";
		public static string playerPhysicalSkillWithSwordName = "skill1_sword";
		public static string playerPhysicalSkillWithStaffName = "skill1_staff";
		public static string playerPhysicalSkillWithAxeName = "skill1_axe";
		public static string playerPhysicalSkillWithDraggerName = "skill1_dragger";

		//******* 玩家使用魔法伤害型技能攻击的动画名称 *********//
		public static string playerMagicalSkillBareHandName = "skill2_default";
		public static string playerMagicalSkillWithSwordName = "skill2_sword";
		public static string playerMagicalSkillWithStaffName = "skill2_staff";
		public static string playerMagicalSkillWithAxeName = "skill2_axe";
		public static string playerMagicalSkillWithDraggerName = "skill2_dragger";

		//******* 玩家攻击间隔的动画名称 *********//
		public static string playerIntervalBareHandName = "interval_default";
		public static string playerIntervalWithSwordName = "interval_sword";
		public static string playerIntervalWithStaffName = "interval_staff";
		public static string playerIntervalWithAxeName = "interval_axe";
		public static string playerIntervalWithDraggerName = "interval_dragger";


		// 特效名称
		public static string healthHealEffecttName = "state_health_heal";
		public static string magicHealEffectName = "state_magic_heal";
		public static string levelUpEffectName = "state_level_up";
		public static string yinShenEffectName = "state_stealth";
		public static string poisonedEffectName = "state_poisoned";
		public static string burnedEffectName = "state_burned";
		public static string frozenEffectName = "state_frozen";
		public static string paralyzedEffectName = "state_paralyzed";
		public static string healthAddUpEffectName = "state_health_add_up";
		public static string armorUpEffectName = "state_armor_up";
		public static string attackUpEffectName = "state_attack_up";
		public static string magicAttackUpEffectName = "state_magic_attack_up";
		public static string magicResistUpEffectName = "state_resist_up";
		public static string dodgeUpEffectName = "state_dodge_up";
		public static string critUpEffectName = "state_crit_up";
		public static string attackDownEffectName = "state_attack_down";
		public static string armorDownEffectName = "state_armor_down";
		public static string magicAttackDownEffectName = "state_magic_attack_down";
		public static string magicResistDownEffectName = "state_resist_down";
		public static string dodgeDownEffectName = "state_dodge_down";
		public static string critDownEffectName = "state_crit_down";
		public static string skillPointUpEffectName = "state_skill_point_up";
			


  //      // 技能 音效 名称
		//public static string poisonedAudioName = "MapEvents/sfx_poisonTrap";
		//public static string burnedAudioName = "MapEvents/sfx_fireTrap";



		// 物品 音效 名称
		public static string gemstoneAudioName = "Item/sfx_gemstone";
		public static string dianjinAudioName = "Item/sfx_chongZhu";
		public static string chongzhuAudioName = "Item/sfx_chongZhu";
		public static string xiaoMoAudioName = "Item/sfx_gemstone";
		public static string yinShenAudioName = "Item/sfx_yinShen";
		public static string equipmentAudioName = "Item/sfx_equipment";
		public static string siYeCaoAudioName = "Other/state_attribute_up";
		public static string xingYunYuMaoAudioName = "Other/state_attribute_up";

		// 其他 音效 名称
		public static string levelUpAudioName = "Other/state_level_up";
		public static string skillUpgradeAudioName = "Other/sfx_skillUpgrade";
		public static string propertyPromotionAudioName = "Skill/state_attribute_up";
		public static string propertyDecreaseAudioName = "Skill/state_attribute_down";
		public static string footstepAudioName = "Other/sfx_footstep";
		public static string goldAudioName = "Other/sfx_gold";
		public static string playerDieAudioName = "Other/sfx_playerDie";
		public static string drinkAudioName = "Item/sfx_drink";
		public static string eatAudoiName = "Item/sfx_eat";
        


		// 地图事件 音效 名称
		public static string doorAudioName = "MapEvents/sfx_door";
        public static string potAudioName = "MapEvents/sfx_pot";
        public static string bucketAudioName = "MapEvents/sfx_bucket";
        public static string treasureBoxAudioName = "MapEvents/sfx_treasurebox";
        public static string crystalAudioName = "MapEvents/sfx_crystal";
		public static string merchantAudioName = "MapEvents/sfx_merchant";
		public static string billboardAudioName = "MapEvents/sfx_billboard";
		public static string thornTrapAudioName = "MapEvents/sfx_thornTrap";
		public static string fireTrapAudioName = "MapEvents/sfx_fire";
		public static string poisonTrapAudioName = "MapEvents/sfx_poison";
		public static string switchAudioName = "MapEvents/sfx_switch";
		public static string keyAudioName = "MapEvents/sfx_key";
		public static string lockOffAudioName = "MapEvents/sfx_lockOff";
		public static string exitAudioName = "MapEvents/sfx_exit";
		public static string lockedDoorAudioName = "MapEvents/sfx_lockedDoor";

		// UI 音效 名称
		public static string correctTintAudioName = "UI/sfx_UI_correctTint";
		public static string wrongTintAudioName = "UI/sfx_UI_wrongTint";
		public static string buttonClickAudioName = "UI/sfx_UI_click";
		public static string paperAudioName = "UI/sfx_UI_paper";
		public static string addSkillAudioName= "UI/sfx_UI_xiangqianjineng";
		public static string bagAudioName = "UI/sfx_UI_Bag";
		public static string dropItemAudioName = "UI/sfx_UI_dropItem";

		public static int aInASCII = (int)('a');

		public static float HWScalerOfCurrentScreen = Camera.main.pixelHeight / Camera.main.pixelWidth;

		// 当前屏幕分辨率和预设屏幕分辨率之间的转换比例
		public static float scalerToPresetResulotion = 1920f / Camera.main.pixelHeight;

		// 当前屏幕宽高比与设计屏幕宽高比之间的比例
		public static float scalerToPresetHW = Camera.main.pixelHeight * 1080f / Camera.main.pixelWidth / 1920f;

		public static float scalerToPresetW = Camera.main.pixelWidth / 1080f;
		public static float scalerToPresetH = Camera.main.pixelHeight / 1920f;


		public static int singleBagItemVolume = 21;
		public static int singleWordsRecordsPageVolume = 7;

		public static int maxLevel = 50;

		public static char diamond = (char)6;
		public static int totalFadeStep = 20;

		public static Color selectedColor = new Color (243f/255, 152f/255, 0);
		public static Color deselectedColor = new Color (166f/255, 147f/255, 124f/225);

		public static Color grayEquipmentColor = new Color(222f / 255, 202f / 255, 170f / 255);
		public static Color blueEquipmentColor = new Color(0, 219f / 255, 217f / 255);
		public static Color goldEquipmentColor = new Color(243f / 255, 152f / 255, 0);
		public static Color purpleEquipmentColor = new Color(195f / 255, 25f / 255, 186f / 255);

		public static Color tabBarTitleNormalColor = new Color(166f / 255, 147f / 255, 124f / 255);
		public static Color tabBarTitleSelectedColor = new Color(243f / 255, 152f / 255, 0);

		public static Color regularTextColor = new Color(222f / 255, 202f / 255, 170f / 255);


		public static string homeBgmName = "Castle";
		public static string exploreBgmName = "Explore";

		public static char[] wholeAlphabet = new char[]{'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'};

		public static Vector2 iphoneXResolution = new Vector2(1125,2436);


		public static string[] gameTints = {
            "每个NPC都有不同的技能，试试不同的搭配吧",
            "并不是每个怪物都需要消灭，合理分配资源才能走到最后",
            "城堡里有很多上了锁的门，需要用对应的钥匙才能打开",
            "切换难度将会重置游戏进度，一定要慎重选择哦",
            "尽可能去探索每一个房间，这会使你获得意想不到的收获",
            "不要错过每个可以获取金币的机会，在城堡里金币才是硬通货",
            "较低等级的关卡是提升实力和储备资源的好地方"
        };

		// 获得称号所需的条件
        public static LearnTitleQualification[] learnTitleQualifications = {
            new LearnTitleQualification(30,0,0,"单词菜鸟","· 背诵单词30个",50),
			new LearnTitleQualification(100,0.7f,0,"单词学徒","背诵单词100个\n· 正确率70%以上",100),
			new LearnTitleQualification(500,0.8f,0,"单词达人","背诵单词500个\n· 正确率80%以上",200),
			new LearnTitleQualification(1500,0.85f,50,"单词学霸","背诵单词1500个\n· 连续正确背诵单词50个\n· 正确率85%以上",400),
			new LearnTitleQualification(2500,0.9f,150,"单词学者","背诵单词2500个\n· 连续正确背诵单词150个\n· 正确率90%以上",1000),
			new LearnTitleQualification(3000,0.95f,300,"单词大师","背诵单词3000个\n· 连续正确背诵单词300个\n· 正确率95%以上",2000)
        };
              


	}




	public enum WordType{
		Simple,
		Medium,
		Master
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