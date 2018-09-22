using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{

	public class Player : Agent {

		private static volatile Player mPlayerSingleton;

//		private static object objectLock = new System.Object();

		// 玩家角色单例
		public static Player mainPlayer{
			get{
				if (mPlayerSingleton == null) {
//					lock (objectLock) {
//						Player[] existPlayers = GameObject.FindObjectsOfType<Player>();
//						if (existPlayers != null) {
//							for (int i = 0; i < existPlayers.Length; i++) {
//								Destroy (existPlayers [i].gameObject);
//							}
//						}
//
//						ResourceLoader playerLoader = ResourceLoader.CreateNewResourceLoader ();
//
//						ResourceManager.Instance.LoadAssetsWithBundlePath<GameObject> (playerLoader, CommonData.mainStaticBundleName, () => {
//							mPlayerSingleton = playerLoader.gos[0].GetComponent<Player> ();
//							mPlayerSingleton.transform.SetParent (null);
//							mPlayerSingleton.ResetBattleAgentProperties (true);
//						},"Player");

						mPlayerSingleton = TransformManager.FindTransform ("Player").GetComponent<Player>();

						DontDestroyOnLoad (mPlayerSingleton);
//					}
				} 
				return mPlayerSingleton;
			}

		}




		public AttackSpeed originalAttackSpeed;//基础攻速
		public AttackSpeed attackSpeed;

		//玩家经验值
		//public int mExperience;
        public int experience;

		//玩家金钱
		//public int mTotalGold;
        public int totalGold;

		// 每次升级所需要的经验值
		public int upgradeExprience{
			get{
				return 2 * agentLevel * agentLevel + 100 * agentLevel;
			}
		}

		public override  float attackInterval{
			get{
				float interval = 1.1f;
				switch (attackSpeed) {
    				case AttackSpeed.Slow:
    					interval = 1.1f;
    					break;
    				case AttackSpeed.Medium:
    					interval = 0.8f;
    					break;
    				case AttackSpeed.Fast:
    					interval = 0.6f;
    					break;
    				case AttackSpeed.VeryFast:
    					interval = 0.4f;
    					break;
					case AttackSpeed.NoInterval:
						interval = 0f;
						break;
    			}
				return interval;
			}
		}

		public List<Item> allItemsInBag = new List<Item>();//背包中要显示的所有物品（已穿戴的装备和已解锁的卷轴将会从这个表中删除）

		public List<Consumables> allConsumablesInBag = new List<Consumables> ();

		public List<PropertyGemstone> allPropertyGemstonesInBag = new List<PropertyGemstone>();//背包中所有的属性宝石

		public List<SkillScroll> allSkillScrollsInBag = new List<SkillScroll>();//背包中所有的技能卷轴

		public List<SpecialItem> allSpecialItemsInBag = new List<SpecialItem>();//背包中所有的特殊物品

		public List<char> allCollectedCharacters = new List<char>();//所有收集到的字母碎片

        // 地图初始化记录
        public List<int> mapIndexRecord = new List<int>();

		public List<string> spellRecord = new List<string>();

		public int currentLevelIndex;

		public int maxUnlockLevelIndex;

		public List<int> unusedPuzzleIds = new List<int>();

		// 是否是新建的玩家
		public bool isNewPlayer;
      
		public int maxBagCount{ 
			get{
				int max = 1;
				if (BuyRecord.Instance.bag_2_unlocked)
                {
                    max = 2;
                }
				if(BuyRecord.Instance.bag_3_unlocked){
					max = 3;
				}
				if(BuyRecord.Instance.bag_4_unlocked){
					max = 4;
				}
				return max;
			} 
		}

		public List<Skill> allLearnedSkills = new List<Skill>();

		public List<SkillModel> allLearnedSkillsRecord = new List<SkillModel>();

		public int skillNumLeft;

		private int maxSkillCount = 6;


		// 记录当前版本信息,用于版本比对【格式：x.xx  例如：1.01 代表1.01版，  版本更新时版本号需比上一版大】
        public float currentVersion;

		// 探索界面遮罩的状态  【0:黑暗状态，使用黑暗动画】 【1:明亮状态，使用明亮动画】
		//public int exploreMaskStatus = 0;


        // 开宝箱的幸运度 
		// 0:  65%灰色装备 30%蓝色装备 5%金色装备
		// 1:  60%灰色装备 30%蓝色装备 10%金色装备
		public int luckInOpenTreasure;

        // 额外开宝箱幸运度【只增加开出金色宝箱的幸运度】
        // 每1点增加1%的开出金色装备的概率
		public int extraLuckInOpenTreasure;

        // 怪物掉落物品的幸运度
		// 0: 10%掉落物品
		// 1: 15%掉落物品
		public int luckInMonsterTreasure;

        // 额外怪物掉落物品幸运度
        // 每1点增加1%怪物掉落物品的概率
		public int extraLuckInMonsterTreasure;

        // 记录最大连续正确背诵单词的数量
		// 单词连续正确数量记录
		// 称号达成情况
		public int maxSimpleWordContinuousRightRecord;
		public int simpleWordContinuousRightRecord;      
        public bool[] titleQualificationsOfSimple;


        public int maxMediumWordContinuousRightRecord;
        public int mediumWordContinuousRightRecord;
        public bool[] titleQualificationsOfMedium;


        public int maxMasterWordContinuousRightRecord;
        public int masterWordContinuousRightRecord;
        public bool[] titleQualificationsOfMaster;

        // 所有已学习过的单词数量记录
		public int totalLearnedWordCount;

		// 所有未掌握单词数量记录
		public int totalUngraspWordCount;
              
		public bool needChooseDifficulty;

        // 一共击杀的怪物数量
		public int totaldefeatMonsterCount;

		public int learnedWordsCountInCurrentExplore;
		public int correctWordsCountInCurrentExplore;

		public string currentExploreStartDateString;

		public bool canSave;//是否可以存档【刚进入探索界面时不存档】

		// 记录到的存档点位置
		public Vector3 savePosition;

        // 记录到的在存档点的额朝向
		public MyTowards saveTowards;

		//public CurrentMapEventsRecord currentMapEventsRecord;

		public void SetUpPlayerWithPlayerData(PlayerData playerData){

			Debug.Log ("set up player data");

			if (playerData == null) {
				return;
			}

			this.currentVersion = playerData.currentVersion;

			this.agentName = playerData.agentName;
			this.agentLevel = playerData.agentLevel;


			//初始化基础信息
			this.originalMaxHealth = playerData.originalMaxHealth;
			this.originalMaxMana = playerData.originalMaxMana;

			this.originalAttack = playerData.originalAttack;
			this.originalMagicAttack = playerData.originalMagicAttack;

			this.originalAttackSpeed = playerData.originalAttackSpeed;
			this.originalMoveSpeed = playerData.originalMoveSpeed;

			this.originalArmor = playerData.originalArmor;
			this.originalMagicResist = playerData.originalMagicResist;
			this.originalArmorDecrease = playerData.originalArmorDecrease;
			this.originalMagicResistDecrease = playerData.originalMagicResistDecrease;

			this.originalCrit = playerData.originalCrit;
			this.originalDodge = playerData.originalDodge;

			this.originalCritHurtScaler = playerData.originalCritHurtScaler;
			this.originalPhysicalHurtScaler = playerData.originalPhysicalHurtScaler;
			this.originalMagicalHurtScaler = playerData.originalMagicalHurtScaler;

			this.originalExtraGold = playerData.originalExtraGold;
			this.originalExtraExperience = playerData.originalExtraExperience;

			this.originalHealthRecovery = playerData.originalHealthRecovery;
			this.originalMagicRecovery = playerData.originalMagicRecovery;


			//初始化实际信息
			this.maxHealth = playerData.maxHealth;
			this.maxMana = playerData.maxMana;

			this.health = playerData.health;
			this.mana = playerData.mana;

			this.attack = playerData.attack;
			this.magicAttack = playerData.magicAttack;

			this.attackSpeed = playerData.attackSpeed;
			this.moveSpeed = playerData.moveSpeed;

			this.armor = playerData.armor;
			this.magicResist = playerData.magicResist;
			this.armorDecrease = playerData.armorDecrease;
			this.magicResistDecrease = playerData.magicResistDecrease;

			this.crit = playerData.crit;
			this.dodge = playerData.dodge;

			this.critHurtScaler = playerData.critHurtScaler;
			this.physicalHurtScaler = playerData.physicalHurtScaler;
			this.magicalHurtScaler = playerData.magicalHurtScaler;

			this.extraGold = playerData.extraGold;
			this.extraExperience = playerData.extraExperience;

			this.healthRecovery = playerData.healthRecovery;
			this.magicRecovery = playerData.magicRecovery;


			this.allEquipmentsInBag = playerData.allEquipmentsInBag;
			this.allConsumablesInBag = playerData.allConsumablesInBag;
			this.allPropertyGemstonesInBag = playerData.allPropertyGemstonesInBag;
			this.allSkillScrollsInBag = playerData.allSkillScrollsInBag;
			this.allSpecialItemsInBag = playerData.allSpecialItemsInBag;

            this.mapIndexRecord = playerData.mapIndexRecord;

			this.allLearnedSkillsRecord = playerData.allLearnedSkillsRecord;
                     
			allEquipedEquipments = new Equipment[7];

			for(int i = 0;i<allEquipedEquipments.Length;i++){
				allEquipedEquipments [i] = new Equipment ();
			}


			for (int i = 0; i < allEquipmentsInBag.Count; i++) {

				Equipment e = allEquipmentsInBag [i];

				if (!e.equiped) {
					continue;
				}
				int equipmentTypeIndex = (int)e.equipmentType;

				if (e.equipmentType == EquipmentType.Ring && allEquipedEquipments [5].itemId >= 0) {
					allEquipedEquipments [6] = e;
				}else{
					allEquipedEquipments[equipmentTypeIndex] = e;
				}         
			}

			//for (int i = 0; i < allEquipmentsInBag.Count;i++){
			//	Equipment e = allEquipmentsInBag[i];
			//	if(e.itemId >=0){
			//		int equipmentTypeIndex = (int)(e.equipmentType);
			//		allEquipedEquipments[equipmentTypeIndex] = e;
			//	}
			//}
            
			this.currentLevelIndex = playerData.currentLevelIndex;
			this.maxUnlockLevelIndex = playerData.maxUnlockLevelIndex;

			//this.exploreMaskStatus = playerData.exploreMaskStatus;
                     
			this.totalGold = playerData.totalGold;
			this.experience = playerData.experience;

			this.isNewPlayer = playerData.isNewPlayer;
			this.needChooseDifficulty = playerData.needChooseDifficulty;

			this.skillNumLeft = playerData.skillNumLeft;

			this.luckInOpenTreasure = playerData.luckInOpenTreasure;
            this.luckInMonsterTreasure = playerData.luckInMonsterTreasure;
			this.extraLuckInOpenTreasure = 0;
			this.extraLuckInMonsterTreasure = 0;

			this.maxSimpleWordContinuousRightRecord = playerData.maxSimpleWordContinuousRightRecord;

			this.simpleWordContinuousRightRecord = playerData.simpleWordContinuousRightRecord;

			this.titleQualificationsOfSimple = playerData.titleQualificationsOfSimple;

			this.maxMediumWordContinuousRightRecord = playerData.maxMediumWordContinuousRightRecord;

			this.mediumWordContinuousRightRecord = playerData.mediumWordContinuousRightRecord;

			this.titleQualificationsOfMedium = playerData.titleQualificationsOfMedium;

			this.maxMasterWordContinuousRightRecord = playerData.maxMasterWordContinuousRightRecord;

			this.masterWordContinuousRightRecord = playerData.masterWordContinuousRightRecord;

			this.titleQualificationsOfMaster = playerData.titleQualificationsOfMaster;

			this.totalLearnedWordCount = LearningInfo.Instance.learnedWordCount;

			this.totalUngraspWordCount = LearningInfo.Instance.ungraspedWordCount;

			this.totaldefeatMonsterCount = playerData.totaldefeatMonsterCount;

			this.learnedWordsCountInCurrentExplore = playerData.learnedWordsCountInCurrentExplore;

			this.correctWordsCountInCurrentExplore = playerData.correctWordsCountInCurrentExplore;

			this.currentExploreStartDateString = playerData.currentExploreStartDateString;

			this.savePosition = playerData.savePosition;

			this.saveTowards = playerData.saveTowards;

			this.spellRecord = playerData.spellRecord;

			this.unusedPuzzleIds = playerData.unusedPuzzleIds;

			//this.currentMapEventsRecord = playerData.currentMapEventsRecord;
                     
			this.allStatus.Clear ();

			allItemsInBag = new List<Item> ();

			for (int i = 0; i < allEquipmentsInBag.Count; i++) {
				if (!allEquipmentsInBag [i].equiped) {
					allItemsInBag.Add (allEquipmentsInBag [i]);
				}
			}

			for (int i = 0; i < allConsumablesInBag.Count; i++) {
				allItemsInBag.Add(allConsumablesInBag[i]);
			}

			for (int i = 0; i < allPropertyGemstonesInBag.Count; i++) {
				allItemsInBag.Add (allPropertyGemstonesInBag [i]);
			}

			for (int i = 0; i < allSkillScrollsInBag.Count;i++){
				allItemsInBag.Add(allSkillScrollsInBag[i]);
			}

			for (int i = 0; i < allSpecialItemsInBag.Count;i++){
				allItemsInBag.Add(allSpecialItemsInBag[i]);
			}
            

			ClearAttachedSkills();

			for (int i = 0; i < allLearnedSkillsRecord.Count; i++)
            {

                SkillModel skillModel = allLearnedSkillsRecord[i];

                Skill skill = SkillGenerator.GenerateSkill(skillModel.skillId, skillModel.skillLevel);

                AddSkill(skill);
            }
            

			ResetBattleAgentProperties (false);

		}


		public void ClearAttachedSkills()
        {
            for (int i = 0; i < allLearnedSkills.Count; i++)
            {
                Destroy(allLearnedSkills[i].gameObject);
            }
            allLearnedSkills.Clear();
            attachedActiveSkills.Clear();
            attachedTriggeredSkills.Clear();
            attachedPermanentPassiveSkills.Clear();
        }

		public void ClearCollectedCharacters(){

			allCollectedCharacters.Clear();

		}

		public int GetAnUnusedPuzzleId(){
			
			if(unusedPuzzleIds.Count == 0){
				for (int i = 0; i < GameManager.Instance.gameDataCenter.allPuzzles.Count;i++){
					unusedPuzzleIds.Add(i);
				}
			}

			if(unusedPuzzleIds.Count > 0){
				int randomSeed = Random.Range(0, unusedPuzzleIds.Count);
				int puzzleId = unusedPuzzleIds[randomSeed];
				unusedPuzzleIds.RemoveAt(randomSeed);
				return puzzleId;
			}

			return -1;

		}


		public void InitializeMapIndex()
		{

			mapIndexRecord.Clear();

			List<int> indexSource = new List<int>();

			for (int i = 0; i < 49;i++){
				indexSource.Add(i);
			}

            // 前面45关按照每5关为一组，组内随机地图
			for (int i = 0; i < 9; i++){
				for (int j = 0; j < 5; j++){

					int randomSeed = Random.Range(0, 5-j);

					int mapIndex = indexSource[randomSeed];

					mapIndexRecord.Add(mapIndex);

					indexSource.RemoveAt(randomSeed);

				}            
			}

			// 第50关和第51关地图是固定的，需要单独处理
			for (int i = 0; i < 4;i++){
				int randomSeed = Random.Range(0, 4 - i);
				int mapIndex = indexSource[randomSeed];
				mapIndexRecord.Add(mapIndex);
				indexSource.RemoveAt(randomSeed);
			}

			mapIndexRecord.Add(49);
			mapIndexRecord.Add(50);

		}



        public int GetMapIndex()
        {
			if(mapIndexRecord.Count == 0){
				InitializeMapIndex();
			}

			return mapIndexRecord[currentLevelIndex];
        }

       


		public override PropertyChange ResetBattleAgentProperties (bool toOriginalState = false)
		{

			int maxHealthRecord = maxHealth;
			int healthRecord = health;
			int maxManaRecord = maxMana;
			int manaRecord = mana;
			int attackRecord = attack;
			int magicAttackRecord = magicAttack;
			int armorRecord = armor;
			int magicResistRecord = magicResist;
			int armorDecreaseRecord = armorDecrease;
			int magicResistDecreaseRecord = magicResistDecrease;
			float dodgeRecord = dodge;
			float critRecord = crit;
			int healthRecoveryRecord = healthRecovery;
			int magicRecoveryRecord = magicRecovery;
			int extraGoldRecord = extraGold;
			int extraExperienceRecord = extraExperience;

            // 方案一：
            // 因为人物属性都是>0的【逻辑上已进行强制】,原始属性+技能属性变化可能时负值，故需使用一个中间值进行存储
   //         int tempMaxHealth = originalMaxHealth + maxHealthChangeFromSkill;
   //         int tempMaxMana = originalMaxMana + maxManaChangeFromSkill;

   //         int tempAttack = originalAttack + attackChangeFromSkill;
   //         int tempMagicAttack = originalMagicAttack + magicAttackChangeFromSkill;

   //         int tempArmor = originalArmor + armorChangeFromSkill;
   //         int tempMagicResist = originalMagicResist + magicResistChangeFromSkill;

   //         int tempArmorDecrease = originalArmorDecrease + armorDecreaseChangeFromSkill;
			//int tempMagicResistDecrease = originalMagicResistDecrease + magicResistDecreaseChangeFromSkill;

			//attackSpeed = originalAttackSpeed;
   //         int tempMoveSpeed = originalMoveSpeed + moveSpeedChangeFromSkill;

			//float tempCrit = originalCrit + critChangeFromSkill;
   //         float tempDodge = originalDodge + dodgeChangeFromSkill;

   //         float tempCritHurtScaler = originalCritHurtScaler + critHurtScalerChangeFromSkill;
   //         float tempPhysicalHurtScaler = originalPhysicalHurtScaler + physicalHurtScalerChangeFromSkill;
   //         float tempMagicalHurtScaler = originalMagicalHurtScaler + magicalHurtScalerChangeFromSkill;

   //         int tempExtraGold = originalExtraGold + extraGoldChangeFromSkill;
   //         int tempExtraExperience = originalExtraExperience + extraExperienceChangeFromSkill;

   //         int tempHealthRecovery = originalHealthRecovery + healthRecoveryChangeFromSkill;
   //         int tempMagicRecovery = originalMagicRecovery + magicRecoveryChangeFromSkill;

			//this.extraLuckInOpenTreasure = 0;
			//this.extraLuckInMonsterTreasure = 0;

			//for (int i = 0; i < allEquipedEquipments.Length; i++) {

			//	Equipment eqp = allEquipedEquipments [i];

			//	if (eqp.itemId < 0) {
			//		continue;
			//	}

			//	tempMaxHealth += eqp.maxHealthGain + eqp.attachedPropertyGemstone.maxHealthGain;
			//	tempMaxMana += eqp.maxManaGain + eqp.attachedPropertyGemstone.maxManaGain;

			//	tempAttack += eqp.attackGain + eqp.attachedPropertyGemstone.attackGain;
			//	tempMagicAttack+= eqp.magicAttackGain + eqp.attachedPropertyGemstone.magicAttackGain;

			//	tempArmor += eqp.armorGain + eqp.attachedPropertyGemstone.armorGain;
			//	tempMagicResist += eqp.magicResistGain + eqp.attachedPropertyGemstone.magicResistGain;

			//	tempArmorDecrease += eqp.armorDecreaseGain + eqp.attachedPropertyGemstone.armorDecreaseGain;
			//	tempMagicResistDecrease += eqp.magicResistDecreaseGain + eqp.attachedPropertyGemstone.magicResistDecreaseGain;

			//	if (eqp.equipmentType == EquipmentType.Weapon) {
			//		attackSpeed = eqp.attackSpeed;
			//	}

			//	tempMoveSpeed += eqp.moveSpeedGain + eqp.attachedPropertyGemstone.moveSpeedGain;

			//	tempCrit += eqp.critGain + eqp.attachedPropertyGemstone.critGain / 100f;
			//	tempDodge += eqp.dodgeGain + eqp.attachedPropertyGemstone.dodgeGain / 100f;

			//	tempCritHurtScaler += eqp.critHurtScalerGain + eqp.attachedPropertyGemstone.critHurtScalerGain / 100f;
			//	tempPhysicalHurtScaler += eqp.physicalHurtScalerGain + eqp.attachedPropertyGemstone.physicalHurtScalerGain / 100f;
			//	tempMagicalHurtScaler += eqp.magicalHurtScalerGain + eqp.attachedPropertyGemstone.magicalHurtScalerGain / 100f;

			//	tempExtraGold += eqp.extraGoldGain + eqp.attachedPropertyGemstone.extraGoldGain;
			//	tempExtraExperience += eqp.extraExperienceGain + eqp.attachedPropertyGemstone.extraExperienceGain;

			//	tempHealthRecovery += eqp.healthRecoveryGain + eqp.attachedPropertyGemstone.healthRecoveryGain;
			//	tempMagicRecovery += eqp.magicRecoveryGain + eqp.attachedPropertyGemstone.magicRecoveryGain;
                            

			//}


			//maxHealth = tempMaxHealth;
			//maxMana = tempMaxMana;
			//attack = tempAttack;
			//magicAttack = tempMagicAttack;
			//armor = tempArmor;
			//magicResist = tempMagicResist;
			//armorDecrease = tempArmorDecrease;
			//magicResistDecrease = tempMagicResistDecrease;
			//moveSpeed = tempMoveSpeed;
			//crit = tempCrit;
			//dodge = tempDodge;
			//critHurtScaler = tempCritHurtScaler;
			//physicalHurtScaler = tempPhysicalHurtScaler;
			//magicalHurtScaler = tempMagicalHurtScaler;
			//extraGold = tempExtraGold;
			//extraExperience = tempExtraExperience;
			//healthRecovery = tempHealthRecovery;
			//magicRecovery = tempMagicRecovery;

            
			/// 方案二：不使用中间值存储的方案 
            // 先计算人物带上装备，考虑永久型技能影响后的属性，再计算主动技能和被动技能对属性的影响
			maxHealth = originalMaxHealth;
			maxMana = originalMaxMana;
			attack = originalAttack;
			magicAttack = originalMagicAttack;
			armor = originalArmor;
			magicResist = originalMagicResist;
			armorDecrease = originalArmorDecrease;
			magicResistDecrease = originalMagicResistDecrease;
			moveSpeed = originalMoveSpeed;
			attackSpeed = originalAttackSpeed;
			crit = originalCrit;
			dodge = originalDodge;
			critHurtScaler = originalCritHurtScaler;
			physicalHurtScaler = originalPhysicalHurtScaler;
			magicalHurtScaler = originalMagicalHurtScaler;
			extraGold = originalExtraGold;
			extraExperience = originalExtraExperience;
			healthRecovery = originalHealthRecovery;
			magicRecovery = originalMagicRecovery;

			this.extraLuckInOpenTreasure = 0;
            this.extraLuckInMonsterTreasure = 0;

			for (int i = 0; i < allEquipedEquipments.Length; i++)
            {

                Equipment eqp = allEquipedEquipments[i];

                if (eqp.itemId < 0)
                {
                    continue;
                }

                maxHealth += eqp.maxHealthGain;
                maxMana += eqp.maxManaGain;

                attack += eqp.attackGain;
                magicAttack += eqp.magicAttackGain;

                armor += eqp.armorGain ;
                magicResist += eqp.magicResistGain;

                armorDecrease += eqp.armorDecreaseGain;
                magicResistDecrease += eqp.magicResistDecreaseGain;

                if (eqp.equipmentType == EquipmentType.Weapon)
                {
                    attackSpeed = eqp.attackSpeed;
                }
                moveSpeed += eqp.moveSpeedGain;

                crit += eqp.critGain;
                dodge += eqp.dodgeGain;
                
                critHurtScaler += eqp.critHurtScalerGain;
                physicalHurtScaler += eqp.physicalHurtScalerGain;
                magicalHurtScaler += eqp.magicalHurtScalerGain;
                
                extraGold += eqp.extraGoldGain;
                extraExperience += eqp.extraExperienceGain;
                
                healthRecovery += eqp.healthRecoveryGain;
                magicRecovery += eqp.magicRecoveryGain;

				for (int j = 0; j < eqp.attachedPropertyGemstones.Count;j++){
					PropertyGemstone attachedPropertyGemstone = eqp.attachedPropertyGemstones[j];
					maxHealth += attachedPropertyGemstone.maxHealthGain;
                    maxMana += attachedPropertyGemstone.maxManaGain;

                    attack += attachedPropertyGemstone.attackGain;
                    magicAttack += attachedPropertyGemstone.magicAttackGain;

                    armor += attachedPropertyGemstone.armorGain;
                    magicResist += attachedPropertyGemstone.magicResistGain;

                    armorDecrease += attachedPropertyGemstone.armorDecreaseGain;
                    magicResistDecrease += attachedPropertyGemstone.magicResistDecreaseGain;

                    moveSpeed += attachedPropertyGemstone.moveSpeedGain;

                    crit += attachedPropertyGemstone.critGain / 100f;
                    dodge += attachedPropertyGemstone.dodgeGain / 100f;

                    critHurtScaler += attachedPropertyGemstone.critHurtScalerGain / 100f;
                    physicalHurtScaler += attachedPropertyGemstone.physicalHurtScalerGain / 100f;
                    magicalHurtScaler += attachedPropertyGemstone.magicalHurtScalerGain / 100f;

                    extraGold += attachedPropertyGemstone.extraGoldGain;
                    extraExperience += attachedPropertyGemstone.extraExperienceGain;

                    healthRecovery += attachedPropertyGemstone.healthRecoveryGain;
                    magicRecovery += attachedPropertyGemstone.magicRecoveryGain;
				}

            }

			for (int i = 0; i < attachedPermanentPassiveSkills.Count; i++)
            {
                PermanentPassiveSkill pps = attachedPermanentPassiveSkills[i];
                pps.AffectAgents(battleAgentCtr, null);
            }

			maxHealth += maxHealthChangeFromSkill;
            maxMana += maxManaChangeFromSkill;

            attack += attackChangeFromSkill;
            magicAttack += magicAttackChangeFromSkill;

            armor += armorChangeFromSkill;
            magicResist += magicResistChangeFromSkill;

            armorDecrease += armorDecreaseChangeFromSkill;
            magicResistDecrease += magicResistDecreaseChangeFromSkill;

            
            moveSpeed += moveSpeedChangeFromSkill;

            crit += critChangeFromSkill;
            dodge += dodgeChangeFromSkill;

			crit += float.Epsilon;
			dodge += float.Epsilon;

            critHurtScaler += critHurtScalerChangeFromSkill;
            physicalHurtScaler += physicalHurtScalerChangeFromSkill;
            magicalHurtScaler += magicalHurtScalerChangeFromSkill;

            extraGold += extraGoldChangeFromSkill;
            extraExperience += extraExperienceChangeFromSkill;

            healthRecovery += healthRecoveryChangeFromSkill;
            magicRecovery += magicRecoveryChangeFromSkill;
                     

			if (toOriginalState) {
				health = maxHealth;
				mana = maxMana;
			} else {
				health = Mathf.RoundToInt(healthRecord * (float)maxHealth / maxHealthRecord);
				mana = Mathf.RoundToInt(manaRecord * (float)maxMana / maxManaRecord);
			}

			int maxHealthChange = maxHealth - maxHealthRecord;
			int maxManaChange = maxMana - maxManaRecord;

			int attackChange = attack - attackRecord;
			int magicAttackChange = magicAttack - magicAttackRecord;

			int armorChange = armor - armorRecord;
			int magicResistChange = magicResist - magicResistRecord;

			int armorDecreaseChange = armorDecrease - armorDecreaseRecord;
			int magicResistDecreaseChange = magicResistDecrease - magicResistDecreaseRecord;

			float dodgeChange = dodge - dodgeRecord;
			float critChange = crit - critRecord;

			int healthRecoveryChange = healthRecovery - healthRecoveryRecord;
			int magicRecoveryChange = magicRecovery - magicRecoveryRecord;

			int extraGoldChange = extraGold - extraGoldRecord;
			int extraExperienceChange = extraExperience - extraExperienceRecord;

			return new PropertyChange (maxHealthChange, maxManaChange, attackChange, magicAttackChange,
				armorChange, magicResistChange,armorDecreaseChange,magicResistDecreaseChange,
				dodgeChange,critChange,healthRecoveryChange,magicRecoveryChange,extraGoldChange,extraExperienceChange);

		}



		/// <summary>
		/// 获取装备在人物装备栏内的序号，如果不在人物装备栏内，返回-1
		/// </summary>
		public int GetEquipmentIndexInPanel(Equipment equipment){
			int equipmentIndexInPanel = -1;
			for (int i = 0; i < allEquipedEquipments.Length; i++) {
				Equipment eqp = allEquipedEquipments [i];
				if (eqp.itemId < 0) {
					continue;
				}
				if (eqp == equipment) {
					equipmentIndexInPanel = i;
					break;
				}
			}

			return equipmentIndexInPanel;
		}

		/// <summary>
		/// 角色卸下装备
		/// </summary>
		/// <param name="equipment">Equipment.</param>
		/// <param name="equipmentIndexInPanel">Equipment index in panel.</param>
		public PropertyChange UnloadEquipment(Equipment equipment,int equipmentIndexInPanel,int indexInBag = -1){

			GameManager.Instance.soundManager.PlayAudioClip (CommonData.equipmentAudioName);

			equipment.equiped = false;

//			Debug.LogFormat ("卸下装备{0}/{1}", equipmentIndexInPanel,allEquipedEquipments.Length);

			BattlePlayerController bpCtr = battleAgentCtr as BattlePlayerController;

			if (indexInBag == -1) {
				allItemsInBag.Add (equipment);
			} else {
				allItemsInBag.Insert (indexInBag, equipment);
			}

			allEquipedEquipments [equipmentIndexInPanel] = new Equipment();

			PropertyChange pc = ResetBattleAgentProperties (false);

			if (ExploreManager.Instance != null) {
				ExploreManager manager = ExploreManager.Instance;
				manager.UpdatePlayerStatusPlane ();
				if (bpCtr.isInFight) {
					bpCtr.InitTriggeredPassiveSkillCallBacks (bpCtr, bpCtr.enemy);
				}
			}

         
			if(bpCtr.isInFight && equipment.equipmentType == EquipmentType.Weapon){
				HLHRoleAnimInfo animInfo = bpCtr.GetCurrentRoleAnimInfo();
				bpCtr.ResetAttackAfterInterval(animInfo, 0.4f);// 怪物的被攻击时的等待时间也是0.4
            }

			return pc;

		}
        




//		/// <summary>
//		/// 对应类型的装备是否可以装备（装备槽是否已经被占用）
//		/// </summary>
//		/// <returns><c>true</c>, if if has equiped was checked, <c>false</c> otherwise.</returns>
//		/// <param name="type">Type.</param>
//		public bool CheckCanEquiped(EquipmentType type){
//
//			bool canEquiped = false;
//
//			int index = (int)type;
//
//			if (index != 5) {
//				canEquiped = allEquipedEquipments [index].itemId == -1;
//			} else {
//				canEquiped = allEquipedEquipments [5].itemId == -1 || 
//					(BuyRecord.Instance.extraEquipmentSlotUnlocked && allEquipedEquipments [6].itemId == -1);
//			}
//
//			return canEquiped;
//
//		}

		/// <summary>
		/// 角色穿上装备
		/// </summary>
		/// <param name="equipment">Equipment.</param>
		/// <param name="equipmentIndexInPanel">Equipment index in panel.</param>
		public PropertyChange EquipEquipment(Equipment equipment,int equipmentIndexInPanel){

			GameManager.Instance.soundManager.PlayAudioClip (CommonData.equipmentAudioName);

			equipment.equiped = true;

//			Debug.LogFormat ("穿上装备{0}", equipmentIndexInPanel);

			allEquipedEquipments [equipmentIndexInPanel] = equipment;

			BattlePlayerController bpCtr = battleAgentCtr as BattlePlayerController;

			//if (equipment.attachedSkillId > 0) {
				
			//	if (equipment.attachedSkill == null) {

			//		Skill attachedSkill = SkillGenerator.GenerateSkill (equipment.attachedSkillId);

			//		equipment.attachedSkill = attachedSkill;

			//		AddSkill (attachedSkill);

			//	}else {
					
			//		Skill attachedSkill = equipment.attachedSkill;

			//		AddSkill (attachedSkill);
			//	}
			//}

			allItemsInBag.Remove (equipment);

			PropertyChange pc = ResetBattleAgentProperties (false);

			if (ExploreManager.Instance != null) {
				ExploreManager manager = ExploreManager.Instance;

				if (bpCtr.isInFight) {
					bpCtr.InitTriggeredPassiveSkillCallBacks (bpCtr, bpCtr.enemy);
				}
				manager.UpdatePlayerStatusPlane ();
			}

			if(bpCtr.isInFight && equipment.equipmentType == EquipmentType.Weapon){
				HLHRoleAnimInfo animInfo = bpCtr.GetCurrentRoleAnimInfo();
                bpCtr.ResetAttackAfterInterval(animInfo, 0.4f);// 怪物的被攻击时的等待时间也是0.4
			}
         
			return pc;

		}

		public void AddSkill(Skill skill){
			switch (skill.skillType) {
			case SkillType.Active:
				ActiveSkill activeSkl = skill as ActiveSkill;
				if(!attachedActiveSkills.Contains(activeSkl)){
					attachedActiveSkills.Add (activeSkl);
				}
				break;
			case SkillType.PermanentPassive:
				PermanentPassiveSkill ppSkl = skill as PermanentPassiveSkill;
				if (!attachedPermanentPassiveSkills.Contains (ppSkl)) {
					attachedPermanentPassiveSkills.Add (ppSkl);
				}
				break;
			case SkillType.TriggeredPassive:
				TriggeredPassiveSkill tpSkl = skill as TriggeredPassiveSkill;
				if (!attachedTriggeredSkills.Contains (tpSkl)) {
					attachedTriggeredSkills.Add (tpSkl);
				}
				break;
			}

			allLearnedSkills.Add(skill);

			skill.transform.SetParent (skillsContainer);

			skill.transform.localPosition = Vector3.zero;
			skill.transform.localRotation = Quaternion.identity;
			skill.transform.localScale = Vector3.one;
		}

		public void RemoveSkill(Skill skill){
			switch (skill.skillType) {
    			case SkillType.Active:
    				attachedActiveSkills.Remove (skill as ActiveSkill);
    				break;
    			case SkillType.PermanentPassive:
    				attachedPermanentPassiveSkills.Remove (skill as PermanentPassiveSkill);
    				break;
                case SkillType.TriggeredPassive:
    				attachedTriggeredSkills.Remove (skill as TriggeredPassiveSkill);
    				battleAgentCtr.RemoveTriggeredSkillExcutor (skill as TriggeredPassiveSkill);
    				break;
			}
		}

		public Equipment GetEquipedEquipment(int itemId){
			Equipment equipedEquipment = null;
			for (int i = 0; i < allEquipedEquipments.Length; i++) {
				Equipment tempEqp = allEquipedEquipments [i];
				if (tempEqp.itemId < 0) {
					continue;
				}
				if (allEquipedEquipments [i].itemId == itemId) {
					equipedEquipment = allEquipedEquipments [i];
					return  equipedEquipment;
				}
			}
			return equipedEquipment;
		}


		public bool CheckItemExistInBag(Item item){

			bool exist = false;

			for (int i = 0; i < allItemsInBag.Count; i++)
            {
				if (item is Equipment)
                {
					exist = item == allItemsInBag[i];
				}else{
					exist = item.itemId == allItemsInBag[i].itemId;
				}

				if(exist){
					break;
				}
            }

			return exist;

		}

		public int GetItemIndexInBag(Item item){

			int index = -1;

			for (int i = 0; i < allItemsInBag.Count; i++) {
				if (allItemsInBag [i] == item) {
					index = i;
					return index;
				}
			}

			return index;
		}


        /// <summary>
        /// 学习技能
        /// </summary>
        /// <param name="skill">Skill.</param>
		public PropertyChange LearnSkill(int skillId){

			Skill newSkill = SkillGenerator.GenerateSkill(skillId, 1);

			PropertyChange propertyChange = new PropertyChange();

   //         // 已学习技能id列表中记录技能id
			//allLearnedSkills.Add(skill);

			SkillModel skillModel = new SkillModel(skillId, 1);

			allLearnedSkillsRecord.Add(skillModel);

            // 人物添加技能
			AddSkill(newSkill);
         
			// 如果是永久性被动，则直接实现技能效果（永久提升属性)
			if (newSkill.skillType == SkillType.PermanentPassive)
            {
				propertyChange = ResetBattleAgentProperties(false);
            }

			return propertyChange;
		}
        
        /// <summary>
        /// 遗忘技能
        /// </summary>
        /// <param name="skill">Skill.</param>
		public PropertyChange ForgetSkill(Skill skill){

			PropertyChange propertyChange = new PropertyChange();

			SkillModel skillModel = allLearnedSkillsRecord.Find(delegate (SkillModel obj)
			{
				return obj.skillId == skill.skillId;
			});


			allLearnedSkills.Remove(skill);
			allLearnedSkillsRecord.Remove(skillModel);

			RemoveSkill(skill);

			skillNumLeft += skill.skillLevel * (skill.skillLevel - 1) / 4;

			// 如果是永久性被动，则直接实现技能效果（永久提升属性)
            if (skill.skillType == SkillType.PermanentPassive)
            {
				propertyChange = ResetBattleAgentProperties(false);
            }

			if(ExploreManager.Instance.battlePlayerCtr.isInFight){
				ExploreManager.Instance.expUICtr.RemoveActiveSkillButton(skill);
			}

			Destroy(skill.gameObject);

			return propertyChange;

		}

		public PropertyChange UpgradeSkill(Skill skill){

			PropertyChange propertyChange = new PropertyChange();
         
            // 扣除升级所需技能点
			skillNumLeft -= skill.skillLevel;

            // 技能等级提升1级
			skill.skillLevel++;

			SkillModel skillModel = allLearnedSkillsRecord.Find(delegate (SkillModel obj)
			{
				return obj.skillId == skill.skillId;
			});

			skillModel.skillLevel++;

			// 如果是永久性被动，则直接实现技能效果（永久提升属性)
			if(skill.skillType == SkillType.PermanentPassive){
				propertyChange = ResetBattleAgentProperties(false);
			}

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.skillUpgradeAudioName);

			return propertyChange;

		}

        /// <summary>
        /// 检查技能是否已经学习过
        /// </summary>
        /// <returns><c>true</c>, if skill has learned was checked, <c>false</c> otherwise.</returns>
        /// <param name="skill">Skill.</param>
		public bool CheckSkillHasLearned(Skill skill){
			
			bool skillLearned = false;

			for (int i = 0; i < allLearnedSkills.Count; i++)
            {
				Skill learnedSkill = allLearnedSkills[i];

				if (learnedSkill.skillId == skill.skillId)
                {
                    skillLearned = true;
                    break;
                }

            }

			//Debug.LogFormat("{0}-{1}", skill.skillName, skillLearned);

			return skillLearned;

		}

		public bool CheckSkillHasLearned(int skillId){

			bool SkillLearned = false;

			for (int i = 0; i < allLearnedSkills.Count;i++){

				Skill learnedSkill = allLearnedSkills[i];

				if(learnedSkill.skillId == skillId){               
					SkillLearned = true;
					break;

				}
			}

			return SkillLearned;

		}

        /// <summary>
		/// 检查玩家是否还可以学习新技 
        /// </summary>
        /// 玩家已经学满了6个技能,返回true；
		public bool CheckSkillFull(){
			return allLearnedSkills.Count >= maxSkillCount;

		}


		/// <summary>
		/// NPC的奖励如果是惩罚的话，检验是否可以满足惩罚的要求（为了方便，惩罚的数据也写在了奖励里面，数值为负的）
		/// </summary>
		/// <returns><c>true</c>, if can hand out was checked, <c>false</c> otherwise.</returns>
		/// <param name="type">惩罚类型.</param>
		/// <param name="value">【如果是金钱惩罚，对应交付的金钱】【如果是物品惩罚，对应交付物品的id】.</param>
		/// <param name="attachValue">【如果是物品惩罚，对应交付物品的数量】.</param>
		public bool CheckCanHandOut(HLHRewardType type,int value,int attachValue){

			bool canHandOut = true;

			switch (type) {
			//case HLHRewardType.Property:
			//case HLHRewardType.Experience:
				//break;
			case HLHRewardType.Gold:
				canHandOut = totalGold + value >= 0;
				break;
			case HLHRewardType.Item:
				Item itemInBag = allItemsInBag.Find (delegate(Item obj) {
					return obj.itemId == value;
				});
				if (itemInBag == null) {
					canHandOut = false;
				} else {
					canHandOut = itemInBag.itemCount + attachValue >= 0;
				}
				break;
			}
			return canHandOut;
		}


		/// <summary>
		/// 判断当前经验是否满足升级条件	
		/// </summary>
		/// <returns><c>true</c>, if up if experience enough was leveled, <c>false</c> otherwise.</returns>
		public bool LevelUpIfExperienceEnough(){

			bool levelUp = false;

			if (experience >= upgradeExprience) {
            
				int maxHealthRecord = maxHealth;
				originalMaxHealth += 10;
				maxHealth += 10;
				health = (int)(health * (float) maxHealth / maxHealthRecord);
				originalAttack++;
				attack++;
				originalArmor++;
				armor++;
				originalMagicAttack++;
				magicAttack++;
				originalMagicResist++;
				magicResist++;

				Player.mainPlayer.experience -= Player.mainPlayer.upgradeExprience;

				skillNumLeft++;
				agentLevel++;

				levelUp = true;
			}
			return levelUp;
		}

		/// <summary>
		/// 检查物品是否已经被玩家解锁
		/// </summary>
		/// <returns><c>true</c>, if item unlocked was checked, <c>false</c> otherwise.</returns>
		/// <param name="item">Item.</param>
//		public bool CheckItemUnlocked(int itemId){
//
//			for (int i = 0; i < allUnlockScrollsInBag.Count; i++) {
//				UnlockScroll unlockScroll = allUnlockScrollsInBag [i];
//				if (unlockScroll.unlocked && unlockScroll.unlockedItemId == itemId) {
//					return true;
//				}
//			}
//
//			return false;
//
//		}





		public bool CheckBagFull(Item item){

			if (item.itemType == ItemType.Equipment) {
				return allItemsInBag.Count >= maxBagCount * CommonData.singleBagItemVolume;
			} else {

				Item itemInBag = allItemsInBag.Find (delegate (Item obj) {
					return obj.itemId == item.itemId;
				});

				if (itemInBag == null) {
					return allItemsInBag.Count >= maxBagCount * CommonData.singleBagItemVolume;
				} 

				return false;
			}
		}



		/// <summary>
		/// 添加物品到背包中
		/// </summary>
		/// <param name="item">Item.</param>
		public void AddItem(Item item,int index = -1){

			if (item == null) {
				string error = "添加的物品为null";
				Debug.LogError (error);
				return;
			}

			switch(item.itemType){
    			case ItemType.Equipment:
    				for (int i = 0; i < item.itemCount; i++) {
    					Equipment equipment = item as Equipment;
    					allEquipmentsInBag.Add (equipment);
    					if (index == -1) {
    						allItemsInBag.Add (equipment);
    					} else {
    						allItemsInBag.Insert (index, equipment);
    					}
    				}
    				break;
    			// 如果是消耗品，且背包中已经存在该消耗品，则只合并数量
    			case ItemType.Consumables:
    				Consumables consumablesInBag = allConsumablesInBag.Find (delegate(Consumables obj) {
    					return obj.itemId == item.itemId;	
    				});
    				if (consumablesInBag != null) {
    					consumablesInBag.itemCount += item.itemCount;
    				} else {
    					consumablesInBag = item as Consumables;
    					allConsumablesInBag.Add (consumablesInBag);
    					if (index == -1) {
    						allItemsInBag.Add (consumablesInBag);
    					}else{
    						allItemsInBag.Insert (index, consumablesInBag);
    					}
    				}
    				break;
				case ItemType.PropertyGemstone:
					PropertyGemstone propertyGemstoneInBag = allPropertyGemstonesInBag.Find (delegate(PropertyGemstone obj) {
    					return obj.itemId == item.itemId;	
    				});
					if (propertyGemstoneInBag != null) {
						propertyGemstoneInBag.itemCount += item.itemCount;
    				} else {
						propertyGemstoneInBag = item as PropertyGemstone;
						allPropertyGemstonesInBag.Add (propertyGemstoneInBag);
    					if (index == -1) {
							allItemsInBag.Add (propertyGemstoneInBag);
    					} else {
							allItemsInBag.Insert (index, propertyGemstoneInBag);
    					}
    				}
    				break;
				case ItemType.SkillScroll:
					SkillScroll skillScroll = item as SkillScroll;
					allSkillScrollsInBag.Add(skillScroll);
					if (index == -1)
                    {
						allItemsInBag.Add(skillScroll);
                    }
                    else
                    {
						allItemsInBag.Insert(index, skillScroll);
                    }               
					break;
				case ItemType.SpecialItem:               
					SpecialItem specialItemInBag = allSpecialItemsInBag.Find(delegate (SpecialItem obj)
    				{
    					return obj.itemId == item.itemId;                  
    				});
					if (specialItemInBag != null) {
						specialItemInBag.itemCount += item.itemCount;
    				} else {
						specialItemInBag = item as SpecialItem;
						allSpecialItemsInBag.Add (specialItemInBag);
						if (index == -1)
                        {
							allItemsInBag.Add(specialItemInBag);
                        }
                        else
                        {
							allItemsInBag.Insert(index, specialItemInBag);
                        }
    				}               
    				break;               
			}
				
		}


		public bool RemoveItem(Item item,int removeCount){

			bool totallyRemoveFromBag = false;

			switch (item.itemType)
			{
				case ItemType.Equipment:

					Equipment equipment = allEquipmentsInBag.Find(delegate (Equipment obj)
					{
						return obj == item;
					});

					if (equipment == null)
					{
						Debug.Log("未找到物品");
						return false;
					}

					if (equipment.equiped)
					{
						for (int i = 0; i < allEquipedEquipments.Length; i++)
						{
							if (allEquipedEquipments[i] == equipment)
							{
								allEquipedEquipments[i] = new Equipment();
							}
						}
					}

					allEquipmentsInBag.Remove(equipment);

					allItemsInBag.Remove(equipment);

					totallyRemoveFromBag = true;

					break;
				// 如果是消耗品，且背包中已经存在该消耗品，则只合并数量
				case ItemType.Consumables:
					Consumables consumablesInBag = allConsumablesInBag.Find(delegate (Consumables obj)
					{
						return obj.itemId == item.itemId;
					});

					if (consumablesInBag == null)
					{
						Debug.Log("未找到物品");
						return false;
					}

					consumablesInBag.itemCount -= removeCount;

					if (consumablesInBag.itemCount <= 0)
					{
						allConsumablesInBag.Remove(consumablesInBag);
						allItemsInBag.Remove(consumablesInBag);
						totallyRemoveFromBag = true;
					}
					break;
				case ItemType.PropertyGemstone:
					PropertyGemstone propertyGemstoneInBag = allPropertyGemstonesInBag.Find(delegate (PropertyGemstone obj)
					{
						return obj.itemId == item.itemId;
					});

					if (propertyGemstoneInBag == null)
					{
						Debug.Log("未找到物品");
						return false;
					}

					propertyGemstoneInBag.itemCount -= removeCount;

					if (propertyGemstoneInBag.itemCount <= 0)
					{
						allPropertyGemstonesInBag.Remove(propertyGemstoneInBag);
						allItemsInBag.Remove(propertyGemstoneInBag);
						totallyRemoveFromBag = true;
					}
					break;
				case ItemType.SkillScroll:
					SkillScroll skillScrollInBag = allSkillScrollsInBag.Find(delegate (SkillScroll obj)
					{
						return obj.itemId == item.itemId;
					});
					if (skillScrollInBag == null)
					{
						Debug.Log("未找到物品");
						return false;
					}

					allSkillScrollsInBag.Remove(skillScrollInBag);
					allItemsInBag.Remove(skillScrollInBag);
                    totallyRemoveFromBag = true;
               
					break;
				case ItemType.SpecialItem:
					SpecialItem specialItem = allSpecialItemsInBag.Find(delegate (SpecialItem obj)
					{
						return obj.itemId == item.itemId;
					});               
					if (specialItem == null) {
    					Debug.Log ("未找到物品");
    					return false;
    				}

					specialItem.itemCount -= removeCount;

					if (specialItem.itemCount <= 0) {
						allSpecialItemsInBag.Remove (specialItem);
						allItemsInBag.Remove(specialItem);
    					totallyRemoveFromBag = true;
    				}
    				break;
			}
			return totallyRemoveFromBag;
		}


		public void ArrangeBagItems(){

			for (int i = 0; i < allItemsInBag.Count - 1; i++) {

				for (int j = 0; j < allItemsInBag.Count - i - 1; j++) {
					
					Item tempItemFormer = allItemsInBag [j];

					Item tempItemLatter = allItemsInBag [j + 1];

					if (tempItemFormer.itemId > tempItemLatter.itemId) {
						allItemsInBag [j] = tempItemLatter;
						allItemsInBag [j + 1] = tempItemFormer;
					}
				}
			}

		}

		/// <summary>
		/// 分解物品
		/// </summary>
		/// <returns>分解后获得的字母碎片</returns>
//		public List<char> ResolveItemAndGetCharacters(Item item,int resolveCount){
//
//			e.PlayAudioClip ("UI/sfx_UI_Resolve");
//
//			// 分解后得到的字母碎片
//			List<char> charactersReturn = new List<char> ();
//
//			// 物品英文名称转换为char数组
//			char[] charArray = item.itemNameInEnglish.ToCharArray ();
//
//			if (charArray.Length == 0) {
//				charArray = CommonData.wholeAlphabet;
//			}
//
//			// 每分解一个物品可以获得的字母碎片数量(解锁卷轴返回对应单词的所有字母，其余物品返回单词中的一个字母）
//			int charactersReturnCount = item.itemType == ItemType.UnlockScroll ? charArray.Length : 1;
//
//			// char数组转换为可以进行增减操作的list
//			List<char> charList = new List<char> ();
//
//			for (int i = 0; i < charArray.Length; i++) {
//				charList.Add (charArray [i]);
//			}
//
//			// 分解物品，背包中的字母碎片数量增加
//			for (int j = 0; j < resolveCount; j++) {
//
//				for (int i = 0; i < charactersReturnCount; i++) {
//
//					char character = ReturnRandomCharacters (ref charList);
//
//					int characterIndex = (int)character - CommonData.aInASCII;
//
//					charactersCount [characterIndex]++;
//
//					charactersReturn.Add (character);
//				}
//			}
//
//			// 被分解的物品减去分解数量，如果数量<=0,从背包中删除物品
//			RemoveItem(item,resolveCount);
//
//			return charactersReturn;
//
//		}

		/// <summary>
		/// 从单词的字母组成中随机返回一个字母
		/// </summary>
		/// <returns>The random characters.</returns>
		private char ReturnRandomCharacters(ref List<char> charList){

			int charIndex = Random.Range (0, charList.Count);

			char character = charList [charIndex];

			charList.RemoveAt (charIndex);

			return character;

		}




		/// <summary>
		/// Checks the unsufficient characters.
		/// </summary>
		/// <returns>The unsufficient characters.</returns>
		/// <param name="itemNameInEnglish">Item name in english.</param>
		public List<char> CheckUnsufficientCharacters(string itemNameInEnglish){

			char[] charactersArray = itemNameInEnglish.ToCharArray ();

			int[] charactersNeed = new int[26];

			List<char> unsufficientCharacters = new List<char> ();

			foreach (char c in charactersArray) {
				int index = (int)c - CommonData.aInASCII;
				charactersNeed [index]++;
			}

//			// 判断玩家字母碎片是否足够
//			for(int i = 0;i<charactersNeed.Length;i++){
//
//				if (charactersNeed [i] > Player.mainPlayer.charactersCount[i]) {
//
//					char c = (char)(i + CommonData.aInASCII);
//
//					unsufficientCharacters.Add (c);
//
//				}
//
//			}

			return unsufficientCharacters;

		}
			
		public List<Equipment> GetAllEquipedEquipment(){

			List<Equipment> equipedEquipments = new List<Equipment> ();

			for (int i = 0; i < allEquipedEquipments.Length; i++) {
				Equipment eqp = allEquipedEquipments [i];
				if (eqp.itemId >= 0) {
					equipedEquipments.Add (eqp);
				}
			}

			return equipedEquipments;
		}





		/// <summary>
		/// 随机返回数量，20%概率返回最大值*0.2，70%概率返回最大值* 0.5，10%概率返回最大值
		/// 最小返回1，最大返回5
		/// </summary>
		/// <returns>The random.</returns>
		/// <param name="max">Max.</param>
		private int MyRandom(int max){

			int seed = 0;

			int returnNum = 0;

			seed = Random.Range (1, 10);

			if (seed <= 2) {
				returnNum = (int)(max * 0.2f);
			} else if (seed <= 9) {
				returnNum = (int)(max * 0.5f);
			}else{
				returnNum = max;
			}

			if (returnNum < 1) {
				returnNum = 1;
			}

			if (returnNum > 5) {
				returnNum = 5;
			}

			return returnNum;

		}

	}



	[System.Serializable]
	public class PlayerData{

		// 记录当前版本信息,用于版本比对【格式：x.xx  例如：1.01 代表1.01版，  版本更新时版本号需比上一版大】
        public float currentVersion;

		public string agentName;

		public string agentIconName;

		public bool isActive = true;

		public int agentLevel;

		//*****初始信息********//
		public int originalMaxHealth;//基础最大生命值
		public int originalMaxMana;//基础最大魔法值
		public int originalAttack;//基础物理伤害
		public int originalMagicAttack;//基础魔法伤害
		public int originalArmor;//基础护甲
		public int originalMagicResist;//基础抗性
		public int originalArmorDecrease;//基础护甲穿刺
		public int originalMagicResistDecrease;//基础抗性穿刺
		public AttackSpeed originalAttackSpeed;//基础攻速
		public int originalMoveSpeed;//基础地图行走速度
		public float originalCrit;//基础暴击率
		public float originalDodge;//基础闪避率
		public int originalExtraGold;//基础额外金币
		public int originalExtraExperience;//基础额外经验
		public float originalPhysicalHurtScaler;//基础物理伤害系数
		public float originalMagicalHurtScaler;//基础魔法伤害系数
		public float originalCritHurtScaler;//基础暴击系数
		public int originalHealthRecovery;//基础生命回复系数
		public int originalMagicRecovery;//基础魔法回复系数
		//*****初始信息********//

		//*****实际信息********//
		public int maxHealth;//实际最大血量
		public int health;//实际生命
		public int maxMana;//实际最大魔法值
		public int mana;//实际魔法
		public int attack;//实际物理伤害
		public int magicAttack;//实际魔法伤害
		public int armor;//实际护甲
		public int magicResist;//实际抗性
		public int armorDecrease;
		public int magicResistDecrease;
		public AttackSpeed attackSpeed;//实际攻速
		public int moveSpeed;//实际行走速度
		public float crit;//实际暴击
		public float dodge;//实际闪避
		public int extraGold;//实际额外金钱
		public int extraExperience;//实际额外经验
		public float physicalHurtScaler;//实际物理伤害系数
		public float magicalHurtScaler;//实际魔法伤害系数
		public float critHurtScaler;//实际暴击系数
		public int healthRecovery;//实际生命回复系数
		public int magicRecovery;//实际魔法回复系数
		//*****实际信息********//

		public List<Equipment> allEquipmentsInBag;//背包中所有装备信息
		public List<Consumables> allConsumablesInBag;//背包中所有消耗品信息
		public List<PropertyGemstone> allPropertyGemstonesInBag;//背包中所有的技能宝石
		public List<SkillScroll> allSkillScrollsInBag;//背包中所有的技能卷轴
		public List<SpecialItem> allSpecialItemsInBag;//背包中所有的特殊物品
		public List<SkillModel> allLearnedSkillsRecord;

		public int maxUnlockLevelIndex;//最大解锁关卡序号
		public int currentLevelIndex;//当前所在关卡序号
        public List<int> mapIndexRecord = new List<int>();//关卡随机后的关卡序号记录
              
		public int experience;//人物经验值
		public int totalGold;//人物金币数量

		public bool isNewPlayer;

		public int skillNumLeft;

		// 探索界面遮罩的状态  【0:黑暗状态，使用黑暗动画】 【1:明亮状态，使用明亮动画】
        public int exploreMaskStatus = 0;

		// 开宝箱的幸运度 
        // 0:  65%灰色装备 30%蓝色装备 5%金色装备
        // 1:  60%灰色装备 30%蓝色装备 10%金色装备
        public int luckInOpenTreasure;

        // 怪物掉落物品的幸运度
        // 0: 10%掉落物品
        // 1: 15%掉落物品
        public int luckInMonsterTreasure;

		// 记录最大连续正确背诵单词的数量
        // 单词连续正确数量记录
        // 称号达成情况
        public int maxSimpleWordContinuousRightRecord;
        public int simpleWordContinuousRightRecord;
        public bool[] titleQualificationsOfSimple;


        public int maxMediumWordContinuousRightRecord;
        public int mediumWordContinuousRightRecord;
        public bool[] titleQualificationsOfMedium;


        public int maxMasterWordContinuousRightRecord;
        public int masterWordContinuousRightRecord;
        public bool[] titleQualificationsOfMaster;


# warning 下面这三个属性用于verision1.0->verison1.1更新时使用，后续版本更新时将下面这三个属性删除      
		public int maxWordContinuousRightRecord = 0;
		public int wordContinuousRightRecord = 0;
		public bool[] titleQualifications = {false,false,false,false,false,false};


		// 所有已学习过的单词数量记录
        public int totalLearnedWordCount;

        // 所有未掌握单词数量记录
        public int totalUngraspWordCount;

		public bool needChooseDifficulty;

        // 本次探索共击败的怪物数量
		public int totaldefeatMonsterCount;

		public int learnedWordsCountInCurrentExplore;
        public int correctWordsCountInCurrentExplore;

		public string currentExploreStartDateString;

		public List<string> spellRecord = new List<string>();

		public List<int> unusedPuzzleIds = new List<int>();

        // 记录到的存档点位置
		public Vector3 savePosition = -Vector3.one;

		public MyTowards saveTowards;

		//public CurrentMapEventsRecord currentMapEventsRecord;

		public PlayerData(Player player){

			this.currentVersion = player.currentVersion;

			this.agentName = player.agentName;
			this.agentLevel = player.agentLevel;


			this.originalMaxHealth = player.originalMaxHealth;
			this.originalMaxMana = player.originalMaxMana;

			this.originalAttack = player.originalAttack;
			this.originalMagicAttack = player.originalMagicAttack;

			this.originalAttackSpeed = player.originalAttackSpeed;
			this.originalMoveSpeed = player.originalMoveSpeed;

			this.originalArmor = player.originalArmor;
			this.originalMagicResist = player.originalMagicResist;
			this.originalArmorDecrease = player.originalArmorDecrease;
			this.originalMagicResistDecrease = player.originalMagicResistDecrease;

			this.originalCrit = player.originalCrit;
			this.originalDodge = player.originalDodge;

			this.originalCritHurtScaler = player.originalCritHurtScaler;
			this.originalPhysicalHurtScaler = player.originalPhysicalHurtScaler;
			this.originalMagicalHurtScaler = player.originalMagicalHurtScaler;

			this.originalExtraGold = player.originalExtraGold;
			this.originalExtraExperience = player.originalExtraExperience;

			this.originalHealthRecovery = player.originalHealthRecovery;
			this.originalMagicRecovery = player.originalMagicRecovery;


			this.maxHealth = player.maxHealth;
			this.maxMana = player.maxMana;

			this.health = player.health;
			this.mana = player.mana;

			this.attack = player.attack;
			this.magicAttack = player.magicAttack;

			this.attackSpeed = player.attackSpeed;
			this.moveSpeed = player.moveSpeed;

			this.armor = player.armor;
			this.magicResist = player.magicResist;
			this.armorDecrease = player.armorDecrease;
			this.magicResistDecrease = player.magicResistDecrease;

			this.crit = player.crit;
			this.dodge = player.dodge;

			this.critHurtScaler = player.critHurtScaler;
			this.physicalHurtScaler = player.physicalHurtScaler;
			this.magicalHurtScaler = player.magicalHurtScaler;

			this.extraGold = player.extraGold;
			this.extraExperience = player.extraExperience;

			this.healthRecovery = player.healthRecovery;
			this.magicRecovery = player.magicRecovery;


			this.allEquipmentsInBag = player.allEquipmentsInBag;
			//this.allEquipedEquipments = player.allEquipedEquipments;
			this.allConsumablesInBag = player.allConsumablesInBag;
			this.allPropertyGemstonesInBag = player.allPropertyGemstonesInBag;
			this.allSkillScrollsInBag = player.allSkillScrollsInBag;
			this.allSpecialItemsInBag = player.allSpecialItemsInBag;

			this.maxUnlockLevelIndex = player.maxUnlockLevelIndex;
			this.currentLevelIndex = player.currentLevelIndex;
            this.mapIndexRecord = player.mapIndexRecord;

			this.allLearnedSkillsRecord = player.allLearnedSkillsRecord;

         
			this.totalGold = player.totalGold;
			this.experience = player.experience;

			this.isNewPlayer = player.isNewPlayer;
			this.needChooseDifficulty = player.needChooseDifficulty;

			this.skillNumLeft = player.skillNumLeft;
			//this.exploreMaskStatus = player.exploreMaskStatus;

			this.luckInOpenTreasure = player.luckInOpenTreasure;
			this.luckInMonsterTreasure = player.luckInMonsterTreasure;

			this.maxSimpleWordContinuousRightRecord = player.maxSimpleWordContinuousRightRecord;

			this.simpleWordContinuousRightRecord = player.simpleWordContinuousRightRecord;

			this.titleQualificationsOfSimple = player.titleQualificationsOfSimple;

			this.maxMediumWordContinuousRightRecord = player.maxMediumWordContinuousRightRecord;

			this.mediumWordContinuousRightRecord = player.mediumWordContinuousRightRecord;

			this.titleQualificationsOfMedium = player.titleQualificationsOfMedium;

			this.maxMasterWordContinuousRightRecord = player.maxMasterWordContinuousRightRecord;

			this.masterWordContinuousRightRecord = player.masterWordContinuousRightRecord;

			this.titleQualificationsOfMaster = player.titleQualificationsOfMaster;

			this.totalLearnedWordCount = player.totalLearnedWordCount;

			this.totalUngraspWordCount = player.totalUngraspWordCount;



			this.totaldefeatMonsterCount = player.totaldefeatMonsterCount;

			this.learnedWordsCountInCurrentExplore = player.learnedWordsCountInCurrentExplore;

			this.correctWordsCountInCurrentExplore = player.correctWordsCountInCurrentExplore;

			this.currentExploreStartDateString = player.currentExploreStartDateString;

			this.savePosition = player.savePosition;

			this.saveTowards = player.saveTowards;

			this.spellRecord = player.spellRecord;

			this.unusedPuzzleIds = player.unusedPuzzleIds;

			//this.currentMapEventsRecord = player.currentMapEventsRecord;

		}
			
	}


}
