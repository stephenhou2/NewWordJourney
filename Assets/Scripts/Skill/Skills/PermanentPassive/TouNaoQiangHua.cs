using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class TouNaoQiangHua : PermanentPassiveSkill {
		
		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int maxManaGain = Mathf.RoundToInt(skillSourceValue * skillLevel);

			int oriMaxMana = self.agent.maxMana;

			self.agent.maxMana += maxManaGain;

			self.agent.mana = (int)(self.agent.mana * self.agent.maxMana / oriMaxMana);
		}
	}
}
