using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



namespace WordJourney
{
	[System.Serializable]
	public struct SkillWithProbability{
		public Skill skill;
		public float probability;
	}

	public class Monster : Agent{

		public int monsterId;

		public int rewardExperience;//奖励的经验值
		public int rewardGold;//奖励的金钱

		public float mAttackInterval;
		public override float attackInterval{
			get { return mAttackInterval; }
			set{
				mAttackInterval = value;
			}
		}

        // 奖励的物品id范围，如果数组为null或者数组长度为0，代表随机生成奖励，否则在id数组内随机生成奖励物品
        //public int[] rewardItemIds;

        // 标记怪物是否是boss
        public bool isBoss;

		public string[] monsterSays = new string[3];

		//public PropertySet puzzleRightDecrease;

		//public PropertySet puzzleWrongIncrease;

		public override void Awake ()
		{
			ResetBattleAgentProperties (true);

		}

		public void InitializeWithMonsterData(MonsterData monsterData){

			this.agentName = monsterData.monsterName;
			this.agentLevel = 1;
			this.monsterId = monsterData.monsterId;

			//int index = Player.mainPlayer.currentLevelIndex / 5;
			//MonsterPropertyGain mpg = monsterData.monsterPropertyGainList [index];

			this.originalMaxHealth = monsterData.originalMaxHealth;//基础最大生命值
			this.originalAttack = monsterData.originalAttack;//基础物理伤害
			this.originalMagicAttack = monsterData.originalMagicAttack;//基础魔法伤害
			this.originalArmor = monsterData.originalArmor;//基础护甲
			this.originalMagicResist = monsterData.originalMagicResist;//基础抗性
			this.originalArmorDecrease = monsterData.originalArmorDecrease;//基础护甲穿刺
			this.originalMagicResistDecrease = monsterData.originalMagicResistDecrease;//基础抗性穿刺
			this.originalMoveSpeed = monsterData.originalMoveSpeed;//基础地图行走速度
			this.originalCrit = monsterData.originalCrit;//基础暴击率
			this.originalDodge = monsterData.originalDodge;//基础闪避率
			//this.originalExtraGold = monsterData.originalExtraGold;//基础额外金币
			//this.originalExtraExperience = monsterData.originalExtraExperience;//基础额外经验
			this.originalPhysicalHurtScaler = 1f;//基础物理伤害系数
			this.originalMagicalHurtScaler = 1f;//基础魔法伤害系数
			this.originalCritHurtScaler = monsterData.originalCritHurtScaler;//基础暴击系数
			this.originalHealthRecovery = monsterData.originalHealthRecovery;//基础生命回复
			this.originalMagicRecovery = monsterData.originalMagicRecovery;//基础魔法回复 
			this.attackInterval = monsterData.attackInterval;//攻击间隔

			this.maxHealth = originalMaxHealth;
			this.health = maxHealth;
			this.maxMana = originalMaxMana;
			this.mana = maxMana;
			this.attack = originalAttack;
			this.magicAttack = originalMagicAttack;
			this.armor = originalArmor;
			this.magicResist = originalMagicResist;
			this.armorDecrease = originalArmorDecrease;
			this.magicResistDecrease = originalMagicResistDecrease;
			this.moveSpeed = originalMoveSpeed;
			this.crit = originalCrit;
			this.dodge = originalDodge;

			this.physicalHurtScaler = 1;//基础物理伤害系数
			this.magicalHurtScaler = 1f;//基础魔法伤害系数
			this.critHurtScaler = originalCritHurtScaler;

            
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
			//int healthRecoveryRecord = healthRecovery;
			//int magicRecoveryRecord = magicRecovery;
			//int extraGoldRecord = extraGold;
			//int extraExperienceRecord = extraExperience;

            maxHealth = originalMaxHealth + maxHealthChangeFromSkill;
            maxMana = originalMaxMana + maxManaChangeFromSkill;

            attack = originalAttack + attackChangeFromSkill;
            magicAttack = originalMagicAttack + magicAttackChangeFromSkill;

            armor = originalArmor + armorChangeFromSkill;
            magicResist = originalMagicResist + magicResistChangeFromSkill;

            armorDecrease = originalArmorDecrease + armorDecreaseChangeFromSkill;
            magicResistDecrease = originalMagicResistDecrease + magicResistChangeFromSkill;

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
			//extraPoisonHurt = 0;

			if (toOriginalState) {
				health = maxHealth;
				mana = maxMana;
				//isDead = false;
			} else {
				health = (int)(healthRecord * (float)maxHealth / maxHealthRecord);
				mana = (int)(manaRecord * (float)maxMana / maxManaRecord);
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

			//int healthRecoveryChange = healthRecovery - healthRecoveryRecord;
			//int magicRecoveryChange = magicRecovery - magicRecoveryRecord;

			//int extraGoldChange = extraGold - extraGoldRecord;
			//int extraExperienceChange = extraExperience - extraExperienceRecord;

			return new PropertyChange (maxHealthChange, maxManaChange, attackChange, magicAttackChange,
				armorChange, magicResistChange,armorDecreaseChange,magicResistDecreaseChange,
				dodgeChange,critChange,0,0,0,0);

		}







	}
}
