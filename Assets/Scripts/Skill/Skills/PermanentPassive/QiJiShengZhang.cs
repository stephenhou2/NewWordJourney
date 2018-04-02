using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class QiJiShengZhang : PermanentPassiveSkill {

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			(self.agent as Player).extraExperience += (int)(self.agent.agentLevel * skillSourceValue);
		}
	}
}