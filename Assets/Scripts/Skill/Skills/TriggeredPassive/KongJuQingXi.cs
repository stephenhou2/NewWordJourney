using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class KongJuQingXi : TriggeredPassiveSkill {

		protected override void BeforeFightTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			enemy.agent.attack -= (int)(self.agent.agentLevel * skillSourceValue);
			enemy.agent.magicAttack -= (int)(self.agent.agentLevel * skillSourceValue);
		}

	
	}
}
