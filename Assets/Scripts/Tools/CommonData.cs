﻿using System.Collections;
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
			new KVPair("PropertyGemstoneDatas","/Data/GameItems/PropertyGemstoneDatas.json"),
			new KVPair("SkillScrollDatas","/Data/GameItems/SkillScrollDatas.json"),
			new KVPair("SpecialItemDatas","/Data/GameItems/SpecialItemDatas.json"),
			new KVPair("SpellItemDatas","/Data/GameItems/SpellItemDatas.json"),
			new KVPair("OriginalPlayerData","/Data/OriginalPlayerData.json"),
			new KVPair("Level","/Data/MapData/Level"),
			new KVPair("NPC","/Data/NPCs"),
            new KVPair("ProverbData","/Data/HLHSentenceAndPoemData.json")         
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
		public static string npcsDataDirPath = persistDataPath + "/NPCs";

		public static string buyRecordFilePath = persistDataPath + "/BuyRecord.json";
		public static string chatRecordsFilePath = persistDataPath + "/ChatRecords.json";



		public static string dataBaseName = "MyGameDB.db";

//		public static string NMETTable = "NMET";
//		public static string CET46Table = "CET46";
//		public static string GRETable = "GRE";
//		public static string TOEFLTable = "TOEFL";

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
		public static string allPropertyGemstoneSpritesBundleName = "item/propertygemstone_icons";
		public static string allSkillScrollSpritesBundleName = "item/skillscroll_icons";
		public static string allSpecialItemSpritesBundleName = "item/specialitem_icons";
		public static string allMapSpritesBundleName = "explore/mapicons";
		public static string allSkillsBundleName = "skills/skills";
		public static string allSkillSpritesBundleName = "skills/icons";
		public static string allUISpritesBundleName = "ui/common";
	

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
		public static string healthHealEffecttName = "01_health_heal";
		public static string magicHealEffectName = "02_magic_heal";
		public static string levelUpEffectName = "03_level_up";
		public static string stealthEffectName = "04_stealth";
		public static string poisonedEffectName = "05_poisoned";
		public static string burnedEffectName = "06_burned";
		public static string propertyDecreaseEffectName = "07_attribute_down";
		public static string bleedingEffectName = "08_bleeding";
		public static string shieldEffectName = "09_shiled";
		public static string frozenEffectName = "10_frozen";
		public static string paralizedEffectName = "11_paralized";
		public static string propertyIncreaseEffectName = "12_attribute_up";


        // 技能 音效 名称
        public static string healthHealAudiotName = "01_health_heal";
        public static string magicHealAudioName = "02_magic_heal";
        
        public static string stealthAudioName = "04_stealth";
        public static string poisonedAudioName = "05_poisoned";
        public static string burnedAudioName = "06_burned";
        public static string propertyDecreaseAudioName = "07_attribute_down";
        public static string bleedingAudioName = "08_bleeding";
        public static string shieldAudioName = "09_shiled";
        public static string frozenAudioName = "10_frozen";
        public static string paralizedAudioName = "11_paralized";
        public static string propertyIncreaseAudioName = "12_attribute_up";


		// 物品 音效 名称
		public static string gemstoneAudioName = "Item/sfx_gemstone";
		public static string chongzhuAudioName = "Item/sfx_chongzhu";
		public static string xiaoMoAudioName = "";
		public static string yinShenAudioName = "";
		public static string xiangQianJiNengAudioName = "Item/sfx_xiangqianjineng";
		public static string equipmentAudioName = "Item/sfx_equipment";

		// 其他 音效 名称
		public static string levelUpAudioName = "Other/sfx_levelup";
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

		// UI 音效 名称
		public static string correctTintAudioName = "UI/sfx_UI_correctTint";
		public static string wrongTintAudioName = "UI/sfx_UI_wrongTint";
		public static string buttonClickAudioName = "UI/sfx_UI_click";
		public static string paperAudioName = "UI/sfx_UI_paper";
		public static string addSkillAudioName= "UI/sfx_UI_xiangqianjineng";
		public static string bagAudioName = "UI/sfx_UI_bag";
		public static string dropItemAudioName = "UI/sfx_UI_dropItem";

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