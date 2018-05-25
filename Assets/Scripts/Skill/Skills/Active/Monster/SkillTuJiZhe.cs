using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{


    public class SkillTuJiZhe : ActiveSkill
    {

        public int hurt;
        public int attackDecrease;

		private bool hasTriggered = false;

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			int actualHurt = Mathf.RoundToInt(hurt / ((enemy.agent.armor - self.agent.armorDecrease) / 100f + 1));

			enemy.AddHurtAndShow(actualHurt, HurtType.Physical, self.towards);

			if(!hasTriggered){
				
				enemy.agent.attack += -attackDecrease;

				enemy.agent.attackChangeFromSkill += -attackDecrease;

				hasTriggered = true;



			}
           
			if (selfEffectAnimName != string.Empty)
            {
                self.SetEffectAnim(selfEffectAnimName);
            }

            if (enemyEffectAnimName != string.Empty)
            {
                enemy.SetEffectAnim(enemyEffectAnimName);
            }

            enemy.UpdateStatusPlane();

            enemy.PlayShakeAnim();

		}

	}

}
