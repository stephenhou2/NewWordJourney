using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class HuoQiuShu : ActiveSkill {

		public float skillSourceValue;

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			int hurt = (int)((self.agent.magicAttack + self.agent.magicResistDecrease / 4) * skillSourceValue) - enemy.agent.magicResist / 4;

			if (hurt < 0) {
				hurt = 0;
			}

			enemy.AddHurtAndShow (hurt, HurtType.Magical,self.towards);

			enemy.PlayShakeAnim ();

			self.UpdateStatusPlane ();
			enemy.UpdateStatusPlane ();

		}
	}
}
