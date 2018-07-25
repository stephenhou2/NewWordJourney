using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SkillSiShen : ActiveSkill
    {

		public int hurt;
		public int attackDecrease;
		public int armorDecrease;

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			int magicResistCal = enemy.agent.magicResist - self.agent.magicResistDecrease;

			if (magicResistCal < -50)
            {
                magicResistCal = -50;
            }
                  
			int actualHurt = Mathf.RoundToInt(hurt / (magicResistCal / 100f + 1));

			enemy.AddHurtAndShow(actualHurt, HurtType.Magical, self.towards);

			enemy.PlayShakeAnim();

			if (!hasTriggered)
            {
				hasTriggered = true;

				int armorChange = -armorDecrease;
				int attackChange = -armorDecrease;

				if(armorDecrease > 0){
					enemy.agent.armor += armorChange;
                    enemy.agent.armorChangeFromSkill += armorChange;               
					enemy.AddTintTextToQueue("护甲降低");
				}

				if(attackDecrease > 0){
					enemy.agent.attack += attackChange;
                    enemy.agent.attackChangeFromSkill += attackChange;
					enemy.AddTintTextToQueue("攻击降低");

				}
                            

            }

			enemy.SetEffectAnim(enemyEffectAnimName);

		}


	}

}
