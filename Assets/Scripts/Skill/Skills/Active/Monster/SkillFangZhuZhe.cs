using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
    
    public class SkillFangZhuZhe : ActiveSkill
    {
        public int hurt;

        public int healthLoseBase;

        public int healthLoseDuration;

        private IEnumerator healthDurativeLoseCoroutine;

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			int actualHurt = Mathf.RoundToInt(hurt / ((enemy.agent.armor - self.agent.armorDecrease) / 100f + 1));

			enemy.AddHurtAndShow(actualHurt, HurtType.Physical, self.towards);

            enemy.PlayShakeAnim();

            enemy.UpdateStatusPlane();

            if(healthDurativeLoseCoroutine != null){
                StopCoroutine(healthDurativeLoseCoroutine);
            }

			healthDurativeLoseCoroutine = DurativeHurt(self, enemy);

            StartCoroutine(healthDurativeLoseCoroutine);


		}

		private IEnumerator DurativeHurt(BattleAgentController self, BattleAgentController enemy){

            int count = 0;

            while(count < healthLoseDuration){

				enemy.AddHurtAndShow(healthLoseBase, HurtType.Physical, self.towards);

                enemy.UpdateStatusPlane();

                yield return new WaitForSeconds(1.0f);

                count++;
            }


        }


	}

}

