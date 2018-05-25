using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class QiJiShengZhang : PermanentPassiveSkill {

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			int extraExperienceGain = Mathf.RoundToInt(skillSourceValue * skillLevel);

			(self.agent as Player).extraExperience += extraExperienceGain;
		}
	}
}