using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class XieEZhiHun : TriggeredPassiveSkill {

//		public float triggerProbability;
//
		public int manaRecoveryBase;

		public float triggeredProbability;

		protected override void AttackTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			if (isEffective (triggeredProbability)) {

				self.agent.mana += self.agent.agentLevel * manaRecoveryBase + self.agent.magicRecovery;

				self.UpdateStatusPlane ();

                if(self.isInFight && self is BattlePlayerController){
                    ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons();
                }

			}
		}
	}
}