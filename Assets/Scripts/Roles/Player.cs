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
				return 50 * agentLevel * (agentLevel + 1);
			}
		}

		public override  float attackInterval{
			get{
				float interval = 1.0f;
				switch (attackSpeed) {
    				case AttackSpeed.Slow:
    					interval = 1.0f;
    					break;
    				case AttackSpeed.Medium:
    					interval = 0.9f;
    					break;
    				case AttackSpeed.Fast:
    					interval = 0.8f;
    					break;
    				case AttackSpeed.VeryFast:
    					interval = 0.6f;
    					break;
    			}
				return interval;
			}
		}

		public List<Item> allItemsInBag = new List<Item>();//背包中要显示的所有物品（已穿戴的装备和已解锁的卷轴将会从这个表中删除）
		public List<Consumables> allConsumablesInBag = new List<Consumables> ();
		public List<TaskItem> allTaskItemsInBag = new List<TaskItem> ();
//		public List<UnlockScroll> allUnlockScrollsInBag = new List<UnlockScroll>();//所有背包中的解锁卷轴
//		public List<CraftingRecipe> allCraftingRecipesInBag = new List<CraftingRecipe>();//所有背包中的合成配方
		public List<SkillGemstone> allSkillGemstonesInBag = new List<SkillGemstone>();//背包中所有的技能宝石

		public List<HLHTask> inProgressTasks = new List<HLHTask> ();

        // 地图初始化记录
        public List<int> mapIndexRecord = new List<int>();

		public int robTime;

//		public int maxUnlockLevelIndex;

		public int currentLevelIndex;

		// 是否是新建的玩家
		public bool isNewPlayer = true;

//		public bool hasCompass = false;

		private int maxBagCount{ get{ return BuyRecord.Instance.extraBagUnlocked ? 4 : 3; } }
//		private int singleBagVolume = 24;

		public void SetUpPlayerWithPlayerData(PlayerData playerData){

			Debug.Log ("set up player data");

			if (playerData == null) {
				return;
			}

			this.agentName = playerData.agentName;
//			this.agentIconName = playerData.agentIconName;
			this.agentLevel = playerData.agentLevel;
//			this.isActive = false;


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
			this.allSkillGemstonesInBag = playerData.allSkillGemstonesInBag;

            this.mapIndexRecord = playerData.mapIndexRecord;

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

				if (equipmentTypeIndex == 0 && allEquipedEquipments [0].itemId >= 0) {
					allEquipedEquipments [6] = e;
				}

				allEquipedEquipments [equipmentTypeIndex] = e;


			}

//			this.maxUnlockLevelIndex = playerData.maxUnlockLevelIndex;
			this.currentLevelIndex = playerData.currentLevelIndex;
//			this.currentLevelIndex = 0;

			this.inProgressTasks = playerData.inProgressTasks;

			this.totalGold = playerData.totalGold;
			this.experience = playerData.experience;
			this.robTime = playerData.robTime;

			this.isNewPlayer = playerData.isNewPlayer;

			this.attachedTriggeredSkills.Clear ();
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

			for (int i = 0; i < allSkillGemstonesInBag.Count; i++) {
				allItemsInBag.Add (allSkillGemstonesInBag [i]);
			}

//			for (int i = 0; i < allUnlockScrollsInBag.Count; i++) {
//				if (!allUnlockScrollsInBag [i].unlocked) {
//					allItemsInBag.Add (allUnlockScrollsInBag [i]);
//				}
//			}

//			for (int i = 0; i < allCraftingRecipesInBag.Count; i++) {
//				allItemsInBag.Add(allCraftingRecipesInBag[i]);
//			}

			for(int i = 0;i<skillsContainer.childCount;i++) {
				Destroy (skillsContainer.GetChild (i).gameObject);
				attachedActiveSkills.Clear ();
				attachedTriggeredSkills.Clear ();
				attachedPermanentPassiveSkills.Clear ();
			}

			for (int i = 0; i < allEquipedEquipments.Length; i++) {

				Equipment equipment = allEquipedEquipments [i];

				if (equipment.itemId < 0) {
					continue;
				}

				if (equipment.attachedSkillId <= 0) {
					continue;
				}
					
				Skill skill = SkillGenerator.GenerateTriggeredSkill (equipment.attachedSkillId);

				AddSkill (skill);

				equipment.attachedSkill = skill;
			}

			ResetBattleAgentProperties (false);

		}


        public int GetRandomMapIndex()
        {

			return currentLevelIndex;

			if(Player.mainPlayer.currentLevelIndex == 49){
				return 49;
			}

			if(mapIndexRecord.Count == 0){
				//mapIndexRecord.Clear();
				int currentLevelGrade = Player.mainPlayer.currentLevelIndex / 10;
				for (int i = 0; i < 10; i++){
					mapIndexRecord.Add(currentLevelGrade * 10 + i);
				}
			}         

			int mapIndex = mapIndexRecord[Random.Range(0, mapIndexRecord.Count)];

			mapIndexRecord.Remove(mapIndex);

			return mapIndex;
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

            maxHealth = originalMaxHealth + maxHealthChangeFromSkill;
            maxMana = originalMaxMana + maxManaChangeFromSkill;

            attack = originalAttack + attackChangeFromSkill;
            magicAttack = originalMagicAttack + magicAttackChangeFromSkill;

            armor = originalArmor + armorChangeFromSkill;
            magicResist = originalMagicResist + magicResistChangeFromSkill;

            armorDecrease = originalArmorDecrease + armorDecreaseChangeFromSkill;
            magicResistDecrease = originalMagicResistDecrease + magicResistChangeFromSkill;

			attackSpeed = originalAttackSpeed;
            moveSpeed = originalMoveSpeed + moveSpeedChangeFromSkill;

            crit = originalCrit + critChangeFromSkill;
            dodge = originalDodge + dodgeChangeFromSkill;

            critHurtScaler = originalCritHurtScaler + critHurtScalerChangeFromSkill;
            physicalHurtScaler = originalPhysicalHurtScaler + physicalHurtScalerChangeFromSkill;
            magicalHurtScaler = originalMagicalHurtScaler + magicalHurtScalerChangeFromSkill;

            extraGold = originalExtraGold + extraGoldChangeFromSkill;
            extraExperience = originalExtraExperience + extraExperienceChangeFromSkill;

            healthRecovery = originalHealthRecovery + healthRecoveryChangeFromSkill;
            magicRecovery = originalMagicRecovery + magicRecoveryChangeFromSkill;

			//shenLuTuTengScaler = 0;
			//poisonHurtScaler = 1f;

			for (int i = 0; i < allEquipedEquipments.Length; i++) {

				Equipment eqp = allEquipedEquipments [i];

				if (eqp.itemId < 0) {
					continue;
				}

				maxHealth += eqp.maxHealthGain;
				maxMana += eqp.maxManaGain;

				attack += eqp.attackGain;
				magicAttack += eqp.magicAttackGain;

				armor += eqp.armorGain;
				magicResist += eqp.magicResistGain;

				armorDecrease += eqp.armorDecreaseGain;
				magicResistDecrease += eqp.magicResistDecreaseGain;

				if (eqp.equipmentType == EquipmentType.Weapon) {
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


				Skill equipmentAttachedSkill = eqp.attachedSkill;
				if(equipmentAttachedSkill != null && equipmentAttachedSkill.skillType == SkillType.PermanentPassive){
					PermanentPassiveSkill pps = equipmentAttachedSkill as PermanentPassiveSkill;
					pps.AffectAgents (battleAgentCtr, null);
				}

			}




			if (toOriginalState) {
				health = maxHealth;
				mana = maxMana;
				//isDead = false;
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

			Skill attachedSkill = equipment.attachedSkill;

			if (attachedSkill != null) {
				if (!bpCtr.isInFight) {
					RemoveSkill (attachedSkill);
					equipment.DestroyAttachedSkillGameObject ();
				} else {
					RemoveSkill (attachedSkill);
				}
			}
				
//			Equipment emptyEquipment = new Equipment();

			allEquipedEquipments [equipmentIndexInPanel] = new Equipment();

			PropertyChange pc = ResetBattleAgentProperties (false);

			if (ExploreManager.Instance != null) {
				ExploreManager manager = ExploreManager.Instance;
				manager.UpdatePlayerStatusPlane ();
				if (bpCtr.isInFight) {
					bpCtr.InitTriggeredPassiveSkillCallBacks (bpCtr, bpCtr.enemy);
				}
			}

			HLHRoleAnimInfo animInfo = bpCtr.GetCurrentRoleAnimInfo ();

			bpCtr.PlayRoleAnimByTime(animInfo.roleAnimName,animInfo.roleAnimTime,animInfo.playTimes,animInfo.animEndCallback);

			return pc;

		}


		public void DestroyEquipmentInBagAttachedSkills(){

			for (int j = 0; j < allEquipmentsInBag.Count; j++) {
				Equipment equipment = allEquipmentsInBag [j];
				if (!equipment.equiped && equipment.attachedSkill != null) {
					Skill attachedSkill = equipment.attachedSkill;
					Destroy (attachedSkill.gameObject);
					equipment.attachedSkill = null;
				}
			}

		}

		public void PlayerPropertyChange(PropertyType type,int change){
			switch (type) {
			case PropertyType.MaxHealth:
				int maxHealthRecord = maxHealth;
				originalMaxHealth += change;
				maxHealth += change;
				health = (int)(health * (float) maxHealth / maxHealthRecord);
				break;
			case PropertyType.Health:
				health += change;
				break;
			case PropertyType.MaxMana:
				int maxManaRecord = maxMana;
				originalMaxMana += change;
				maxMana += change;
				mana = (int)(mana * (float)maxMana / maxManaRecord);
				break;
			case PropertyType.Attack:
				originalAttack += change;
				attack += change;
				break;
			case PropertyType.MagicAttack:
				originalMagicAttack += change;
				magicAttack += change;
				break;
			case PropertyType.Armor:
				originalArmor += change;
				armor += change;
				break;
			case PropertyType.MagicResist:
				originalMagicResist += change;
				magicResist += change;
				break;
			case PropertyType.ArmorDecrease:
				originalArmorDecrease += change;
				armorDecrease += change;
				break;
			case PropertyType.MagicResistDecrease:
				originalMagicResistDecrease += change;
				magicResistDecrease += change;
				break;
			case PropertyType.MoveSpeed:
				originalMoveSpeed += change;
				moveSpeed += change;
				break;
			case PropertyType.Dodge:
				float changeInFloat = (float)change / 100;
				originalDodge += changeInFloat;
				dodge += changeInFloat;
				break;
			case PropertyType.Crit:
				changeInFloat = (float)change / 100;
				originalCrit += changeInFloat;
				crit += changeInFloat;
				break;
			case PropertyType.CritHurtScaler:
				changeInFloat = (float)change / 100;
				originalCritHurtScaler += changeInFloat;
				critHurtScaler += changeInFloat;
				break;
			case PropertyType.ExtraGold:
				originalExtraGold += change;
				extraGold += change;
				break;
			case PropertyType.ExtraExperience:
				originalExtraExperience += change;
				extraExperience += change;
				break;
			case PropertyType.HealthRecovery:
				originalHealthRecovery += change;
				healthRecovery += change;
				break;
			case PropertyType.MagicRecovery:
				originalMagicRecovery += change;
				magicRecovery += change;
				break;
			case PropertyType.HealthPunish:
				maxHealthRecord = maxHealth;
				originalMaxHealth += change * robTime;
				maxHealth += change * robTime;
				health = (int)(health * (float)maxHealth / maxHealthRecord);
				robTime = 0;
				break;
			case PropertyType.AttackPunish:
				originalAttack += change * robTime;
				attack += change * robTime;
				robTime = 0;
				break;
			}
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

			if (equipment.attachedSkillId > 0) {
				
				if (equipment.attachedSkill == null) {

					Skill attachedSkill = SkillGenerator.GenerateTriggeredSkill (equipment.attachedSkillId);

					equipment.attachedSkill = attachedSkill;

					AddSkill (attachedSkill);

				}else {
					
					Skill attachedSkill = equipment.attachedSkill;

					AddSkill (attachedSkill);
				}
			}

			allItemsInBag.Remove (equipment);

			PropertyChange pc = ResetBattleAgentProperties (false);

			if (ExploreManager.Instance != null) {
				ExploreManager manager = ExploreManager.Instance;

				if (bpCtr.isInFight) {
					bpCtr.InitTriggeredPassiveSkillCallBacks (bpCtr, bpCtr.enemy);
				}
				manager.UpdatePlayerStatusPlane ();
			}

			HLHRoleAnimInfo animInfo = bpCtr.GetCurrentRoleAnimInfo ();

			bpCtr.PlayRoleAnimByTime(animInfo.roleAnimName,animInfo.roleAnimTime,animInfo.playTimes,animInfo.animEndCallback);

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
		/// NPC的奖励如果是惩罚的话，检验是否可以满足惩罚的要求（为了方便，惩罚的数据也写在了奖励里面，数值为负的）
		/// </summary>
		/// <returns><c>true</c>, if can hand out was checked, <c>false</c> otherwise.</returns>
		/// <param name="type">惩罚类型.</param>
		/// <param name="value">【如果是金钱惩罚，对应交付的金钱】【如果是物品惩罚，对应交付物品的id】.</param>
		/// <param name="attachValue">【如果是物品惩罚，对应交付物品的数量】.</param>
		public bool CheckCanHandOut(HLHRewardType type,int value,int attachValue){

			bool canHandOut = true;

			switch (type) {
			case HLHRewardType.Property:
			case HLHRewardType.Experience:
				break;
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




		public bool CheckTaskExistFromTaskId(int taskId){

			bool taskExist = false;
			for (int i = 0; i < inProgressTasks.Count; i++) {
				HLHTask inProgressTask = inProgressTasks [i];
				if (inProgressTask.taskId == taskId) {
					taskExist = true;
					break;
				}
			}
			return taskExist;

		}

		public bool CheckTaskExistFromTriggeredDialogGroupId(int dialogGroupId){
			bool taskExist = false;
			for (int i = 0; i < inProgressTasks.Count; i++) {
				HLHTask inProgressTask = inProgressTasks [i];
				if (inProgressTask.dialogGroupId == dialogGroupId) {
					taskExist = true;
					break;
				}
			}
			return taskExist;
		}

		/// <summary>
		/// 检查任务是否完成
		/// </summary>
		/// <returns><c>true</c>, if task finish was checked, <c>false</c> otherwise.</returns>
		/// <param name="player">Player.</param>
		/// <param name="taskId">Task identifier.</param>
		public bool CheckTaskFinish(HLHTask task){

			bool isTaskFinish = false;

			int tempTaskItemId = task.taskItemId;

			Item taskItem = allItemsInBag.Find (delegate(Item obj) {
				return obj.itemId == tempTaskItemId;
			});

			if (taskItem != null) {
				isTaskFinish = taskItem.itemCount >= task.taskItemCount;
			}

			return isTaskFinish;

		}


		/// <summary>
		/// 接受任务
		/// </summary>
		/// <param name="task">Task.</param>
		public void ReceiveTask(HLHTask task){
			bool taskExist = CheckTaskExistFromTaskId (task.taskId);
			if (!taskExist) {
				inProgressTasks.Add (task);
			}
		}

		/// <summary>
		/// 提交任务
		/// </summary>
		/// <param name="task">Task.</param>
		public void HandInTask(HLHTask task){

			inProgressTasks.Remove (task);

		}

		/// <summary>
		/// 移除任务
		/// </summary>
		/// <param name="task">Task.</param>
		public void FinishTask(HLHTask task){
			
			for (int i = 0; i < inProgressTasks.Count; i++) {
				
				HLHTask inProgressTask = inProgressTasks [i];

				if (inProgressTask.taskId == task.taskId) {

					Item taskItem = allItemsInBag.Find (delegate(Item obj) {
						return obj.itemId == task.taskItemId;
					});

					if (taskItem != null) {
						RemoveItem (taskItem, task.taskItemCount);
					}

					inProgressTasks.Remove (inProgressTask);

					break;
				}
			}
		}

		public void RefreshTasksWhenEnterNextLevel(){

			for (int i = 0; i < inProgressTasks.Count; i++) {

				HLHTask task = inProgressTasks [i];

				if (task.isCurrentLevelTask) {

					inProgressTasks.RemoveAt (i);

				}
			}

		}

		/// <summary>
		/// 判断当前经验是否满足升级条件	
		/// </summary>
		/// <returns><c>true</c>, if up if experience enough was leveled, <c>false</c> otherwise.</returns>
		public bool LevelUpIfExperienceEnough(){

			bool levelUp = false;

			if (experience >= upgradeExprience) {
		
				agentLevel++;

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
			case ItemType.Gemstone:
				SkillGemstone skillGemstoneInBag = allSkillGemstonesInBag.Find (delegate(SkillGemstone obj) {
					return obj.itemId == item.itemId;	
				});
				if (skillGemstoneInBag != null) {
					skillGemstoneInBag.itemCount += item.itemCount;
				} else {
					skillGemstoneInBag = item as SkillGemstone;
					allSkillGemstonesInBag.Add (skillGemstoneInBag);
					if (index == -1) {
						allItemsInBag.Add (skillGemstoneInBag);
					} else {
						allItemsInBag.Insert (index, skillGemstoneInBag);
					}
				}
				break;
			case ItemType.Task:
				TaskItem taskItemInBag = allTaskItemsInBag.Find (delegate(TaskItem obj) {
					return obj.itemId == item.itemId;	
				});
				if (taskItemInBag != null) {
					taskItemInBag.itemCount += item.itemCount;
				} else {
					taskItemInBag = item as TaskItem;
					allTaskItemsInBag.Add (taskItemInBag);
				}
				break;
//			case ItemType.CraftingRecipes:
//				allItemsInBag.Add (item);
//				allCraftingRecipesInBag.Add (item as CraftingRecipe);
//				break;
			}
				
		}


		public bool RemoveItem(Item item,int removeCount){

			bool totallyRemoveFromBag = false;

			switch(item.itemType){
			case ItemType.Equipment:
				
				Equipment equipment = allEquipmentsInBag.Find (delegate(Equipment obj) {
					return obj == item;
				});
	
				if (equipment == null) {
					Debug.Log ("未找到物品");
					return false;
				}

				if (equipment.equiped) {
					for (int i = 0; i < allEquipedEquipments.Length; i++) {
						if (allEquipedEquipments [i] == equipment) {
							allEquipedEquipments [i] = new Equipment ();
						}
					}
				}

				allEquipmentsInBag.Remove (equipment);

				allItemsInBag.Remove (equipment);

				totallyRemoveFromBag = true;

				break;
				// 如果是消耗品，且背包中已经存在该消耗品，则只合并数量
			case ItemType.Consumables:
				Consumables consumablesInBag = allConsumablesInBag.Find (delegate(Consumables obj) {
					return obj.itemId == item.itemId;	
				});

				if (consumablesInBag == null) {
					Debug.Log ("未找到物品");
					return false;
				}

				consumablesInBag.itemCount -= removeCount;

				if (consumablesInBag.itemCount <= 0) {
					allConsumablesInBag.Remove (consumablesInBag);
					allItemsInBag.Remove (consumablesInBag);
					totallyRemoveFromBag = true;
				}
				break;
			case ItemType.Gemstone:
				SkillGemstone skillGemstoneInBag = allSkillGemstonesInBag.Find (delegate(SkillGemstone obj) {
					return obj.itemId == item.itemId;	
				});

				if (skillGemstoneInBag == null) {
					Debug.Log ("未找到物品");
					return false;
				}

				skillGemstoneInBag.itemCount -= removeCount;

				if (skillGemstoneInBag.itemCount <= 0) {
					allSkillGemstonesInBag.Remove (skillGemstoneInBag);
					allItemsInBag.Remove (skillGemstoneInBag);
					totallyRemoveFromBag = true;
				}
				break;
			case ItemType.Task:
				TaskItem taskItemInBag = allTaskItemsInBag.Find (delegate(TaskItem obj) {
					return obj.itemId == item.itemId;	
				});

				if (taskItemInBag == null) {
					Debug.Log ("未找到物品");
					return false;
				}

				taskItemInBag.itemCount -= removeCount;

				if (taskItemInBag.itemCount <= 0) {
					allTaskItemsInBag.Remove (taskItemInBag);
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

		public int[] charactersCount = new int[26];//剩余的字母碎片信息

		public List<Equipment> allEquipmentsInBag;//背包中所有装备信息
		public Equipment[] allEquipedEquipments;//已装备的所有装备信息
		public List<Consumables> allConsumablesInBag;//背包中所有消耗品信息
		public List<SkillGemstone> allSkillGemstonesInBag;//背包中所有的技能宝石
//		public List<Item> allItemsInBag;
//		public List<UnlockScroll> allUnlockScrollsInBag;
//		public List<CraftingRecipe> allCraftRecipesInBag;

//		public int maxUnlockLevelIndex;//最大解锁关卡序号
		public int currentLevelIndex;//当前所在关卡序号
        public List<int> mapIndexRecord = new List<int>();

		public List<HLHTask> inProgressTasks;


		public int experience;//人物经验值
		public int totalGold;//人物金币数量
		public int robTime;//玩家抢劫次数

		public bool isNewPlayer;


		public PlayerData(Player player){

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
			this.allEquipedEquipments = player.allEquipedEquipments;
			this.allConsumablesInBag = player.allConsumablesInBag;
			this.allSkillGemstonesInBag = player.allSkillGemstonesInBag;
//			this.allUnlockScrollsInBag = player.allUnlockScrollsInBag;
//			this.allCraftRecipesInBag = player.allCraftingRecipesInBag;

//			this.maxUnlockLevelIndex = player.maxUnlockLevelIndex;
			this.currentLevelIndex = player.currentLevelIndex;
            this.mapIndexRecord = player.mapIndexRecord;

			this.inProgressTasks = player.inProgressTasks;



			this.totalGold = player.totalGold;
			this.experience = player.experience;
			this.robTime = player.robTime;

			this.isNewPlayer = player.isNewPlayer;

		}
			
	}


}
