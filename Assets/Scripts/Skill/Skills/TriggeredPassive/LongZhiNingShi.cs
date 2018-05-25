using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class LongZhiNingShi : TriggeredPassiveSkill {

		public int magicReistDecreaseBase;
		public float triggerProbability;

		protected override void HitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{

			if(!isEffective(triggerProbability)){
				return;
			}

			int magicResistDecrease= skillLevel * magicReistDecreaseBase;

			enemy.agent.magicResist -= magicResistDecrease;

			enemy.agent.magicResistChangeFromSkill -= magicResistDecrease;         
         
			enemy.SetEffectAnim(enemyEffectAnimName);
		}

	}
}
