using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



namespace WordJourney
{


    /// <summary>
    /// 怪物数据类
    /// </summary>
	public class Monster : Agent{

        // 怪物id
		public int monsterId;

		public int rewardExperience;//奖励的经验值
		public int rewardGold;//奖励的金钱

        // 攻击间隔
		public float mAttackInterval;
		public override float attackInterval{
			get { return mAttackInterval; }
			set{
				mAttackInterval = value;
			}
		}

      
        // 标记怪物是否是boss
        public bool isBoss;

        // 怪物说的话
		public string[] monsterSays = new string[3];

		public override void Awake ()
		{
			ResetBattleAgentProperties (true);

		}

        /// <summary>
        /// 使用怪物数据初始化
        /// </summary>
        /// <param name="monsterData">Monster data.</param>
		public void InitializeWithMonsterData(MonsterData monsterData){

			this.agentName = monsterData.monsterName;
			this.agentLevel = 1;
			this.monsterId = monsterData.monsterId;
            

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


        /// <summary>
        /// 重算角色属性
        /// </summary>
        /// <returns>The battle agent properties.</returns>
        /// <param name="toOriginalState">If set to <c>true</c> to original state.</param>
		public override PropertyChange ResetBattleAgentProperties (bool toOriginalState = false)
		{

            // 记录重算前的属性
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

            // 使用原始属性计算叠加技能影响后的属性值
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
            
            // 如果血量魔法恢复到原始状态
			if (toOriginalState) {
				// 满血满蓝
				health = maxHealth;
				mana = maxMana;            
			} 
            // 否则按照最大生命和最大魔法值的变化来更新血量和魔法
			else {
				
				health = (int)(healthRecord * (float)maxHealth / maxHealthRecord);
				mana = (int)(manaRecord * (float)maxMana / maxManaRecord);
			}

            // 计算属性变化
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
            

			return new PropertyChange (maxHealthChange, maxManaChange, attackChange, magicAttackChange,
				armorChange, magicResistChange,armorDecreaseChange,magicResistDecreaseChange,
				dodgeChange,critChange,0,0,0,0);

		}







	}
}
