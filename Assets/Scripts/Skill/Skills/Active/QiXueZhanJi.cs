using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class QiXueZhanJi : ActiveSkill {

		public int fixHurt;
		public int hurtBase;
		public float absorbScaler;

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			// 计算原始伤害值
			int hurt = self.agent.attack + fixHurt + hurtBase * skillLevel;
            // 结算护甲和护甲穿透后的伤害
			hurt = Mathf.RoundToInt(hurt / ((enemy.agent.armor - self.agent.armorDecrease) / 100f + 1));
            // 吸血值
			int healthGain = (int)(hurt * absorbScaler + self.agent.healthRecovery);

            // 伤害生效
			enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);
            // 生命回复生效 
			self.AddHealthGainAndShow (healthGain);

		}
	}
}