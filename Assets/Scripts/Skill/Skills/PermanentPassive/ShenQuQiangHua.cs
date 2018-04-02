using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ShenQuQiangHua : PermanentPassiveSkill {

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int maxHealthGain = (int)(skillSourceValue * self.agent.agentLevel);

			int oriMaxHealth = self.agent.maxHealth;

			self.agent.maxHealth += maxHealthGain;
			self.agent.health = (int)(self.agent.health * self.agent.maxHealth / oriMaxHealth);
		}
	}
}
