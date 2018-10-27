using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	/// <summary>
    /// 怪物数据模型
    /// </summary>
	[System.Serializable]
	public class MonsterData {

		public string monsterName;
		public int monsterId;
		public int agentLevel = 1;

		//*****人物基础信息(无装备，无状态加成时的人物属性)********//
		public int originalMaxHealth;//基础最大生命值
		public int originalAttack;//基础物理伤害
		public int originalMagicAttack;//基础魔法伤害
		public int originalArmor;//基础护甲
		public int originalMagicResist;//基础抗性
		public int originalArmorDecrease;//基础护甲穿刺
		public int originalMagicResistDecrease;//基础抗性穿刺
		public int originalMoveSpeed;//基础地图行走速度
		public float originalCrit;//基础暴击率
		public float originalDodge;//基础闪避率
		//public int originalExtraGold;//基础额外金币
		//public int originalExtraExperience;//基础额外经验
		public float originalPhysicalHurtScaler = 1f;//基础物理伤害系数
		public float originalMagicalHurtScaler = 1f;//基础魔法伤害系数
		public float originalCritHurtScaler;//基础暴击系数
		public int originalHealthRecovery;//基础生命回复
		public int originalMagicRecovery;//基础魔法回复
		public float attackInterval;//攻击间隔
		//*****人物基础信息(无装备，无状态加成时的人物属性)********//

		public int rewardExperience;//奖励的经验值
		public int rewardGold;//奖励的金钱


		public int attackSpeedLevel;//怪物攻速等级【0:极快 1:快 2:中等 3:慢 4:极慢】
		public int mosnterEvaluate;//怪物评级【0:普通怪 1:精英怪 2:boss】
		public string monsterStory;//怪物背景介绍



	}


}
