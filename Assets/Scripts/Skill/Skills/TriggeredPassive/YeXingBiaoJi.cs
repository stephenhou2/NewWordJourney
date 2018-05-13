using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class YeXingBiaoJi : TriggeredPassiveSkill {

		public int armorDecreaseBase;

		protected override void BeforeFightTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			int armorChange = -(int)(self.agent.agentLevel * armorDecreaseBase);

            enemy.agent.armorChangeFromSkill += armorChange;

            enemy.agent.armor += armorChange;
		}

	}
}