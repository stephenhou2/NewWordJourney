using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class YeXingBiaoJi : TriggeredPassiveSkill {

		protected override void BeforeFightTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			enemy.agent.armor -= (int)(self.agent.agentLevel * skillSourceValue);
		}

	}
}