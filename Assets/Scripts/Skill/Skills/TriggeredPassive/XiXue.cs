using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class XiXue : TriggeredPassiveSkill {

		public float triggerProbability;

		protected override void HitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			if (isEffective (triggerProbability)) {
				int healthGain = (int)(self.agent.hurtToEnemyFromNormalAttack * skillSourceValue + self.agent.healthRecovery);
				self.AddHealthGainAndShow (healthGain);
				self.UpdateStatusPlane ();
			}
		}

	}
}
