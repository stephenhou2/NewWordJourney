using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class QiXueZhanJi : ActiveSkill {

		public float hurtScaler;
		public float absorbScaler;

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int hurt = (int)((self.agent.attack + self.agent.armorDecrease / 4) * hurtScaler - enemy.agent.armor / 4);

			int healthGain = (int)(hurt * absorbScaler + self.agent.healthRecovery);

			enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);

			self.AddHealthGainAndShow (healthGain);

			self.UpdateStatusPlane ();
			enemy.UpdateStatusPlane ();

		}
	}
}