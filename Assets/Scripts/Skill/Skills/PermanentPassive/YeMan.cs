using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class YeMan : PermanentPassiveSkill {


		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.maxMana -= (int)(self.agent.agentLevel * skillSourceValue * self.agent.maxMana);
			self.agent.mana = (int)(self.agent.agentLevel * skillSourceValue * self.agent.mana);
			self.agent.maxHealth += (int)(self.agent.agentLevel * skillSourceValue * self.agent.maxHealth);
			self.agent.health = (int)(self.agent.agentLevel / skillSourceValue * self.agent.health);
		}

	}
}
