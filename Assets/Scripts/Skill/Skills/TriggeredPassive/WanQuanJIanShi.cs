using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class WanQuanJIanShi : TriggeredPassiveSkill {


		protected override void BeforeFightTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			enemy.agent.dodge -= self.agent.agentLevel * skillSourceValue;
		}

	}
}