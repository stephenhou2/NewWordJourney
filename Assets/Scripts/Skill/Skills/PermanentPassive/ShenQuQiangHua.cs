using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ShenQuQiangHua : PermanentPassiveSkill {

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int maxHealthGain = Mathf.RoundToInt(skillSourceValue * skillLevel);

			int oriMaxHealth = self.agent.maxHealth;

			self.agent.maxHealth += maxHealthGain;

			self.agent.health = (int)(self.agent.health * self.agent.maxHealth / oriMaxHealth);
		}
	}
}
