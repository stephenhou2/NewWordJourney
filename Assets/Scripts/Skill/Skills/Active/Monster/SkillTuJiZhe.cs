using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{


    public class SkillTuJiZhe : ActiveSkill
    {

        public int hurt;
        public int attackDecrease;

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			int armorCal = enemy.agent.armor - self.agent.armorDecrease/2;

			if (armorCal < -50)
            {
                armorCal = -50;
            }


			int actualHurt = Mathf.RoundToInt(hurt / (armorCal / 100f + 1));

			enemy.AddHurtAndShow(actualHurt, HurtType.Physical, self.towards);

			if(!hasTriggered){
				
				enemy.agent.attack += -attackDecrease;

				enemy.agent.attackChangeFromSkill += -attackDecrease;

				hasTriggered = true;
            
                enemy.SetEffectAnim(enemyEffectAnimName);

				enemy.AddTintTextToQueue("攻击降低");

			}

            enemy.PlayShakeAnim();

		}

	}

}
