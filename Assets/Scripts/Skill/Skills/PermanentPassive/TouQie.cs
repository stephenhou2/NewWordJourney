using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class TouQie : TriggeredPassiveSkill {

		public float triggerProbability;

		protected override void AttackTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			if (isEffective (triggerProbability)) {
				if (self is BattlePlayerController) {
					int goldChange = self.agent.agentLevel * (int)skillSourceValue;
					(self.agent as Player).totalGold += goldChange;
					string tint = string.Format ("金币+{0}", goldChange);
					self.AddTintTextToQueue (tint);
					self.UpdateStatusPlane ();
				} else if(self is BattleMonsterController){
					int goldChange = self.agent.agentLevel * (int)skillSourceValue;
					(enemy.agent as Player).totalGold -= goldChange;
					string tint = string.Format ("金币-{0}", goldChange);
					self.AddTintTextToQueue (tint);
					enemy.UpdateStatusPlane ();
				}

			}
		}

	}
}
