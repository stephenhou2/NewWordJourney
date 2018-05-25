using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class PoJia : PermanentPassiveSkill {

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int attackGain = Mathf.RoundToInt(skillSourceValue * skillLevel);

			self.agent.attack += attackGain;

		}
	}
}
