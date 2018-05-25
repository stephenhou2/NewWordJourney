using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class WanQuanJIanShi : TriggeredPassiveSkill {

		public float dodgeDecreaseBase;
		public float triggerProbability;

		protected override void HitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			if(!isEffective(triggerProbability)){
				return;
			}

			float dodgeChange = skillLevel * dodgeDecreaseBase;

            enemy.agent.dodgeChangeFromSkill -= dodgeChange;

            enemy.agent.dodge -= dodgeChange;
         
			enemy.SetEffectAnim(enemyEffectAnimName);
		}

	}
}