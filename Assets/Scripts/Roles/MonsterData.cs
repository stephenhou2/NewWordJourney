using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	[System.Serializable]
	public class MonsterData {

		public string monsterName;
		public int monsterId;
		public int agentLevel = 1;

		//*****人物基础信息(无装备，无状态加成时的人物属性)********//
		public int originalMaxHealth;//基础最大生命值
		public int originalMaxMana;//基础最大魔法值
		public int originalAttack;//基础物理伤害
		public int originalMagicAttack;//基础魔法伤害
		public int originalArmor;//基础护甲
		public int originalMagicResist;//基础抗性
		public int originalArmorDecrease;//基础护甲穿刺
		public int originalMagicResistDecrease;//基础抗性穿刺
		public int originalMoveSpeed;//基础地图行走速度
		public float originalCrit;//基础暴击率
		public float originalDodge;//基础闪避率
		public int originalExtraGold;//基础额外金币
		public int originalExtraExperience;//基础额外经验
		public float originalPhysicalHurtScaler = 1.5f;//基础物理伤害系数
		public float originalMagicalHurtScaler = 1.5f;//基础魔法伤害系数
		public float originalCritHurtScaler;//基础暴击系数
		public int originalHealthRecovery;//基础生命回复
		public int originalMagicRecovery;//基础魔法回复
		public float attackInterval;//攻击间隔
		//*****人物基础信息(无装备，无状态加成时的人物属性)********//

		public int rewardExperience;//奖励的经验值
		public int rewardGold;//奖励的金钱

		public int shenLuTuTengScaler = 0;
		public float poisonHurtScaler = 1;

		public List<MonsterPropertyGain> monsterPropertyGainList = new List<MonsterPropertyGain> ();



	}

	[System.Serializable]
	public class MonsterPropertyGain{

		public int maxHealthGain;//基础最大生命值
		public int maxManaGain;//基础最大魔法值
		public int attackGain;//基础物理伤害
		public int magicAttackGain;//基础魔法伤害
		public int armorGain;//基础护甲
		public int magicResistGain;//基础抗性
		public int armorDecreaseGain;//基础护甲穿刺
		public int magicResistDecreaseGain;//基础抗性穿刺
		public int moveSpeedGain;//基础地图行走速度
		public float critGain;//基础暴击率
		public float dodgeGain;//基础闪避率
		public int extraGoldGain;//基础额外金币
		public int extraExperienceGain;//基础额外经验
		public float critHurtScalerGain;//基础暴击系数
		public int healthRecoveryGain;//基础生命回复
		public int magicRecoveryGain;//基础魔法回复
		public float attackIntervalGain;//攻击间隔
		//*****人物基础信息(无装备，无状态加成时的人物属性)********//

		public int rewardExperienceGain;//奖励的经验值
		public int rewardGoldGain;//奖励的金钱

	}
}
