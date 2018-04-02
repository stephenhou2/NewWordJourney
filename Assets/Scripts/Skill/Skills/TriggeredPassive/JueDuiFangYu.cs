using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class JueDuiFangYu : TriggeredPassiveSkill {

		protected override void BeHitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			float triggeredPropability = skillSourceValue * self.agent.agentLevel;

			if (isEffective (triggeredPropability)) {

				int hurt = self.agent.armor;

				enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);

				enemy.PlayShakeAnim ();
			}

		}
	}
}
