using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class KongJuQinXi : TriggeredPassiveSkill {

		public float triggeredProbability;
		public float attackDecreaseBase;

		protected override void HitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			if(!isEffective(triggeredProbability)){
				return;
			}

			int attackDecrease = Mathf.RoundToInt(skillLevel * attackDecreaseBase);

			enemy.agent.attack -= attackDecrease;
			enemy.agent.attackChangeFromSkill += attackDecrease;

		}

	
	}
}
