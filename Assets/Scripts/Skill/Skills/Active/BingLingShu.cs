using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	public class BingLingShu : ActiveSkill {

		public float baseHurtScaler;
		public float attackSpeedDecreaseScaler;

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			//int hurt = (int)((self.agent.magicAttack + self.agent.magicResistDecrease / 4) * skillSourceValue) - enemy.agent.magicResist / 4;

			int hurt = Mathf.RoundToInt(self.agent.magicAttack * baseHurtScaler / ((enemy.agent.magicResist - self.agent.magicResistDecrease) / 100f + 1));

			if(hurt < 0){
				hurt = 0;
			}

			enemy.AddHurtAndShow (hurt, HurtType.Magical,self.towards);

			enemy.SetRoleAnimTimeScale (1 - attackSpeedDecreaseScaler);

			self.UpdateStatusPlane ();
			enemy.UpdateStatusPlane ();
		}
	}
}
