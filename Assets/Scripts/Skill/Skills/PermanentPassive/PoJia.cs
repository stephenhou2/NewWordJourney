using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class PoJia : PermanentPassiveSkill {

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.attack += (int)(self.agent.agentLevel * skillSourceValue);
		}
	}
}
