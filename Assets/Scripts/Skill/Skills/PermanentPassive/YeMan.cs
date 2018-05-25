using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class YeMan : PermanentPassiveSkill {

        

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int maxHealthChange = Mathf.RoundToInt(skillLevel * skillSourceValue * self.agent.maxHealth);
			int maxManaChange = Mathf.RoundToInt(skillLevel * skillSourceValue * 2 * self.agent.maxMana);

			int maxHealthRecord = self.agent.maxHealth;
			int maxManaRecord = self.agent.maxMana;

			self.agent.maxMana -= maxManaChange;
			self.agent.mana = Mathf.RoundToInt(self.agent.mana * (float)self.agent.maxMana / maxManaRecord);
				
			self.agent.maxHealth += maxHealthChange;
			self.agent.health = Mathf.RoundToInt(self.agent.health * (float)self.agent.maxHealth / maxHealthRecord);
		}

	}
}
