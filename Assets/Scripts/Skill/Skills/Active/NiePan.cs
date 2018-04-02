using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class NiePan : ActiveSkill {

		public float skillSourceValue;

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			int healthGain = (int)(self.agent.mana * skillSourceValue + self.agent.healthRecovery);

			self.AddHealthGainAndShow (healthGain);

			self.UpdateStatusPlane ();
			enemy.UpdateStatusPlane ();

		}

	}
}
