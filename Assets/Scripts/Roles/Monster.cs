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

//		public int monsterId;

		public int rewardExperience;//奖励的经验值
		public int rewardGold;//奖励的金钱

		public float mAttackInterval;
		public override float attackInterval{
			get { return mAttackInterval; }
			set{
				mAttackInterval = value;
			}
		}




		public override void Awake ()
		{
			ResetBattleAgentProperties (true);

		}

		public void InitializeWithMonsterData(MonsterData monsterData){

			this.agentName = monsterData.monsterName;
//			this.monsterId = monsterData.monsterId;

			int index = Player.mainPlayer.currentLevelIndex / 5;
			MonsterPropertyGain mpg = monsterData.monsterPropertyGainList [index];

			this.originalMaxHealth = monsterData.originalMaxHealth+ mpg.maxHealthGain;//基础最大生命值
			this.originalMaxMana = monsterData.originalMaxMana + mpg.maxManaGain;//基础最大魔法值
			this.originalAttack = monsterData.originalAttack + mpg.attackGain;//基础物理伤害
			this.originalMagicAttack = monsterData.originalMagicAttack + mpg.magicAttackGain;//基础魔法伤害
			this.originalArmor = monsterData.originalArmor + mpg.armorGain;//基础护甲
			this.originalMagicResist = monsterData.originalMagicResist + mpg.magicResistGain;//基础抗性
			this.originalArmorDecrease = monsterData.originalArmorDecrease + mpg.armorDecreaseGain;//基础护甲穿刺
			this.originalMagicResistDecrease = monsterData.originalMagicResistDecrease + mpg.magicResistDecreaseGain;//基础抗性穿刺
			this.originalMoveSpeed = monsterData.originalMoveSpeed + mpg.moveSpeedGain;//基础地图行走速度
			this.originalCrit = monsterData.originalCrit + mpg.critGain;//基础暴击率
			this.originalDodge = monsterData.originalDodge + mpg.dodgeGain;//基础闪避率
			this.originalExtraGold = monsterData.originalExtraGold + mpg.extraGoldGain;//基础额外金币
			this.originalExtraExperience = monsterData.originalExtraExperience + mpg.extraExperienceGain;//基础额外经验
			this.originalPhysicalHurtScaler = 1.5f;//基础物理伤害系数
			this.originalMagicalHurtScaler = 1.5f;//基础魔法伤害系数
			this.originalCritHurtScaler = monsterData.originalCritHurtScaler + mpg.critHurtScalerGain;//基础暴击系数
			this.originalHealthRecovery = monsterData.originalHealthRecovery + mpg.healthRecoveryGain;//基础生命回复
			this.originalMagicRecovery = monsterData.originalMagicRecovery + mpg.magicRecoveryGain;//基础魔法回复
			this.attackInterval = monsterData.attackInterval + mpg.attackIntervalGain;//攻击间隔

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
			this.extraGold = originalExtraGold;
			this.extraExperience = originalExtraExperience;
			this.physicalHurtScaler = 1.5f;//基础物理伤害系数
			this.magicalHurtScaler = 1.5f;//基础魔法伤害系数
			this.critHurtScaler = originalCritHurtScaler;
			this.healthRecovery = originalHealthRecovery;
			this.magicRecovery = originalMagicRecovery;

			this.shenLuTuTengScaler = 0;
			this.poisonHurtScaler = 1f;
			this.isDead = false;
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

			maxHealth = originalMaxHealth;
			maxMana = originalMaxMana;

			attack = originalAttack;
			magicAttack = originalMagicAttack;

			armor = originalArmor;
			magicResist = originalMagicResist;

			armorDecrease = originalArmorDecrease;
			magicResistDecrease = originalMagicResistDecrease;

			moveSpeed = originalMoveSpeed;

			crit = originalCrit;
			dodge = originalDodge;

			critHurtScaler = originalCritHurtScaler;
			physicalHurtScaler = originalPhysicalHurtScaler;
			magicalHurtScaler = originalMagicalHurtScaler;

			extraGold = originalExtraGold;
			extraExperience = originalExtraExperience;

			healthRecovery = originalHealthRecovery;
			magicRecovery = originalMagicRecovery;

			shenLuTuTengScaler = 0;
			poisonHurtScaler = 1f;

			if (toOriginalState) {
				health = maxHealth;
				mana = maxMana;
				isDead = false;
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

			int healthRecoveryChange = healthRecovery - healthRecoveryRecord;
			int magicRecoveryChange = magicRecovery - magicRecoveryRecord;

			int extraGoldChange = extraGold - extraGoldRecord;
			int extraExperienceChange = extraExperience - extraExperienceRecord;

			return new PropertyChange (maxHealthChange, maxManaChange, attackChange, magicAttackChange,
				armorChange, magicResistChange,armorDecreaseChange,magicResistDecreaseChange,
				dodgeChange,critChange,healthRecoveryChange,magicRecoveryChange,extraGoldChange,extraExperienceChange);

		}




	}
}
