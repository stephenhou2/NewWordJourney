using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class JueDuiFangYu : TriggeredPassiveSkill {

		public float triggeredProbabilityBase;

		public float refrectScaler;

		protected override void BeHitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			float triggeredProbability = triggeredProbabilityBase * self.agent.agentLevel;

			if (isEffective (triggeredProbability)) {

				int hurt =(int)(self.agent.armor * refrectScaler);

				enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);

                enemy.UpdateStatusPlane();

				enemy.PlayShakeAnim ();
			}

		}
	}
}
