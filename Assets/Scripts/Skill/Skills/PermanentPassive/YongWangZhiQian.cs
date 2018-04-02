using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class YongWangZhiQian : PermanentPassiveSkill {

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.armor += (int)(self.agent.agentLevel * skillSourceValue);

			int speed = (int)(self.agent as Player).attackSpeed;

			speed++;

			if (speed > 4) {
				speed = 4;
			}

			(self.agent as Player).attackSpeed = (AttackSpeed)speed;

		}
	}
}
