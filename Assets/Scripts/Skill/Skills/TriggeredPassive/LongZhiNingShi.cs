using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class LongZhiNingShi : TriggeredPassiveSkill {

		public int magicReistDecreaseBase;

		protected override void BeforeFightTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			int magicResistChange = -self.agent.agentLevel * magicReistDecreaseBase;
            enemy.agent.magicResistChangeFromSkill += magicResistChange;
            enemy.agent.magicResist += magicResistChange;
		}

	}
}
