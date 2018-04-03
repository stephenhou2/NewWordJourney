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
		public override float attackInterval{get { return mAttackInterval; }}




		public override void Awake ()
		{
			ResetBattleAgentProperties (true);

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
			magicRecovery = originalMagicRecoverty;

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



		void OnDestroy(){

//			mBattleAgentController = null;
//			mTriggeredSkillsContainer = null;
//			mConsumbalesSkillsContainer = null;
//			allEquipmentsInBag = null;
//			allEquipedEquipments = null;
//			for (int i = 0; i < attachedTriggeredSkills.Count; i++) {
//				Destroy (attachedTriggeredSkills [i].gameObject);
//			}
//			for (int i = 0; i < attachedConsumablesSkills.Count; i++) {
//				Destroy (attachedConsumablesSkills [i].gameObject);
//			}
//			allStatus = null;
//			mCharactersCount = null;

		}

	}
}
