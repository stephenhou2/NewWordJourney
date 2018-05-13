using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class TouQie : TriggeredPassiveSkill {

		public float triggerProbability;

		public int goldBase;

		protected override void AttackTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			if (isEffective (triggerProbability)) {
				if (self is BattlePlayerController) {
					int goldChange = self.agent.agentLevel * goldBase;
                    int goldGain = goldChange + self.agent.extraGold;
                    (self.agent as Player).totalGold += goldGain;
					string tint = string.Format ("金币+{0}", goldChange);
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.goldAudioName);
					self.AddTintTextToQueue (tint);
					self.UpdateStatusPlane ();
				} else if(self is BattleMonsterController){
					int goldChange = self.agent.agentLevel * goldBase;
					(enemy.agent as Player).totalGold -= goldChange;
					string tint = string.Format ("金币-{0}", goldChange);
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.goldAudioName);
					self.AddTintTextToQueue (tint);
					enemy.UpdateStatusPlane ();
				}

			}
		}

	}
}
