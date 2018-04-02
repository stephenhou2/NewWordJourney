using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class LongZhiNingShi : TriggeredPassiveSkill {

		protected override void BeforeFightTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			enemy.agent.magicResist -= (int)(self.agent.agentLevel * skillSourceValue);
		}

	}
}
