using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class JianShiWuZhuang : PermanentPassiveSkill {

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int armorGain = Mathf.RoundToInt(skillLevel * skillSourceValue);

			self.agent.armor += armorGain;
		}
	}
}
