using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class JueDuiFangYu : TriggeredPassiveSkill {

		public float fixTriggerProbability;

		public float triggeredProbabilityBase;

		public float refrectScaler;

		protected override void BeHitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			float triggeredProbability = fixTriggerProbability + triggeredProbabilityBase * skillLevel;

			if (isEffective (triggeredProbability)) {

				int hurt = Mathf.RoundToInt(self.agent.armor * refrectScaler);

				enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);

                enemy.UpdateStatusPlane();

				SetEffectAnims(self, enemy);
			}

		}
	}
}
