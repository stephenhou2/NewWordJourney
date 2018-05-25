using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class YongWangZhiQian : PermanentPassiveSkill {

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int armorGain= Mathf.RoundToInt(skillLevel * skillSourceValue);

			self.agent.armor += armorGain;

			int speed = (int)(self.agent as Player).attackSpeed;

			speed++;

			if (speed > 4) {
				speed = 4;
			}

			(self.agent as Player).attackSpeed = (AttackSpeed)speed;

		}
	}
}
