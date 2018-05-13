using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SkillSiShen : ActiveSkill
    {

		public int hurt;
		public int armorDecrease;

		private bool hasTriggered = false;


		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{
			int actualHurt = Mathf.RoundToInt(hurt / ((enemy.agent.magicResist - self.agent.magicResistDecrease) / 100f + 1));

			enemy.AddHurtAndShow(hurt, HurtType.Magical, self.towards);

			enemy.PlayShakeAnim();

			if(!hasTriggered){
				enemy.agent.armor += -armorDecrease;
				enemy.agent.armorChangeFromSkill += -armorDecrease;
				hasTriggered = true;
			}


			enemy.UpdateStatusPlane();

		}


	}

}
