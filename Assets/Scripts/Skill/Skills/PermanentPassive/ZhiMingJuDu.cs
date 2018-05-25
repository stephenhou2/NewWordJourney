using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ZhiMingJuDu : PermanentPassiveSkill {

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.extraPoisonHurt = Mathf.RoundToInt(skillSourceValue * skillLevel);
		}

	}
}
