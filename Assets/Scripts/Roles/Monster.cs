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

			return new PropertyChange (maxHealthChange, maxManaChange, attackChange, magicAttackChange, armorChange, magicResistChange);

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
