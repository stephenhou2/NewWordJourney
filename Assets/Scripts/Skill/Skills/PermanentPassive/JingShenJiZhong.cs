using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class JingShenJiZhong : PermanentPassiveSkill {

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int magicResistGain = Mathf.RoundToInt(skillSourceValue * skillLevel);

			self.agent.magicResist += magicResistGain;
		}

	}
}