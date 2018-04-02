using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class JiXing : PermanentPassiveSkill  {

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.moveSpeed = (int)(self.agent.moveSpeed * (1 + skillSourceValue));
		}

	}
}
