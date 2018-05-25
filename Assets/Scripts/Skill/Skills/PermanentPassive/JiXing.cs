using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class JiXing : PermanentPassiveSkill  {

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int moveSpeedChange = Mathf.RoundToInt(self.agent.moveSpeed * (1 + skillSourceValue * skillLevel));

			self.agent.moveSpeed += moveSpeedChange;
		}

	}
}
