using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	/// <summary>
	/// 居合斩
	/// </summary>
	public class Juhezhan : ActiveSkill {

		//public float hurtScaler;
		public float critScalerGain;
		public float baseHurtScaler;

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			float crit = self.agent.crit + critScalerGain;

			//int hurt = (int)((self.agent.attack  + self.agent.armorDecrease / 4) * (1 + hurtScaler));

			int hurt = self.agent.attack;

			if (isEffective (crit)) {
				hurt = (int)(self.agent.attack * self.agent.critHurtScaler);
				enemy.AddTintTextToQueue ("暴击");
			}

			hurt = Mathf.RoundToInt(self.agent.attack * baseHurtScaler / ((enemy.agent.armor - self.agent.armorDecrease) / 100f + 1));

			//hurt -= enemy.agent.armor / 4;

			enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);

			enemy.PlayShakeAnim ();

		}


	}
}
