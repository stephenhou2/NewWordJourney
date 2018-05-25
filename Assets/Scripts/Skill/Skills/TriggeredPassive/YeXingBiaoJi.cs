using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class YeXingBiaoJi : TriggeredPassiveSkill {

		public int armorDecreaseBase;
		public float triggerProbability;

		protected override void HitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			if(!isEffective(triggerProbability)){
				return;
			}

			int armorChange = skillLevel * armorDecreaseBase;

            enemy.agent.armorChangeFromSkill -= armorChange;

            enemy.agent.armor -= armorChange;

			enemy.SetEffectAnim(enemyEffectAnimName);
		}

	}
}