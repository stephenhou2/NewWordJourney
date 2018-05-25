using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class XiXue : TriggeredPassiveSkill {

		public float fixTriggerProbability;

		public float triggerProbabilityBase;

		public float healthAbsorbScaler;

		protected override void HitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			float triggerProbability = fixTriggerProbability + triggerProbabilityBase * skillLevel;

			if (isEffective (triggerProbability)) {
				
				int healthGain = Mathf.RoundToInt(self.agent.hurtToEnemy * healthAbsorbScaler + self.agent.healthRecovery);
				
				self.AddHealthGainAndShow (healthGain);

				SetEffectAnims(self, enemy);

				self.UpdateStatusPlane ();
			}
		}

	}
}
