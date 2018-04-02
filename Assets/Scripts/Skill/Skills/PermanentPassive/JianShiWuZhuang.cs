using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class JianShiWuZhuang : PermanentPassiveSkill {

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int armorGain = (int)(self.agent.agentLevel * skillSourceValue);

			self.agent.armor += armorGain;
		}
	}
}
