using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class CanYing : PermanentPassiveSkill {


		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			float dodgeGain = skillSourceValue * skillLevel;
			self.agent.dodge += dodgeGain;
		}

	}
}
