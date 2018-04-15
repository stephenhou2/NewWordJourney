using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	/// <summary>
	/// 居合斩
	/// </summary>
	public class Juhezhan : ActiveSkill {

		public float hurtScaler;
		public float critScalerGain;

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			float crit = self.agent.crit + critScalerGain;

			int hurt = (int)((self.agent.attack  + self.agent.armorDecrease / 4) * (1 + hurtScaler));

			if (isEffective (crit)) {
				hurt = (int)(hurt * self.agent.critHurtScaler);
				enemy.AddTintTextToQueue ("暴击");
			}

			hurt -= enemy.agent.armor / 4;

			enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);

			enemy.PlayShakeAnim ();

		}


	}
}
