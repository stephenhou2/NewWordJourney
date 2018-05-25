using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class XieEZhiHun : TriggeredPassiveSkill {
    
		public int manaRecoveryBase;

		public int manaRecoveryExtra;

		public float triggeredProbability;

		protected override void AttackTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			if (isEffective (triggeredProbability)) {

				int manaGain = skillLevel * manaRecoveryBase + manaRecoveryExtra + self.agent.magicRecovery;

				self.agent.mana += manaGain;

				self.UpdateStatusPlane ();

				SetEffectAnims(self, enemy);

                if(self.isInFight && self is BattlePlayerController){
                    ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons();
                }

			}
		}
	}
}