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

		public string continuousEffectAnimName;

        private IEnumerator healthDurativeLoseCoroutine;

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			int armorCal = enemy.agent.armor - self.agent.armorDecrease/2;

			if (armorCal < -50)
            {
                armorCal = -50;
            }

			int actualHurt = Mathf.RoundToInt(hurt / (armorCal / 100f + 1));

			enemy.AddHurtAndShow(actualHurt, HurtType.Physical, self.towards);

            enemy.PlayShakeAnim();

			if(enemyEffectAnimName!=string.Empty){
				enemy.SetEffectAnim(enemyEffectAnimName);
			}
         
			enemy.SetEffectAnim(continuousEffectAnimName, null, 0, healthLoseDuration);
            
            if(healthDurativeLoseCoroutine != null){
                StopCoroutine(healthDurativeLoseCoroutine);
            }

			healthDurativeLoseCoroutine = DurativeHurt(self, enemy);

            StartCoroutine(healthDurativeLoseCoroutine);


		}

		private IEnumerator DurativeHurt(BattleAgentController self, BattleAgentController enemy){

            int count = 0;

            while(count < healthLoseDuration){

				if(enemy != null && !enemy.isDead){
					
					enemy.AddHurtAndShow(healthLoseBase, HurtType.Physical, self.towards);
					
					bool fightEnd = enemy.CheckFightEnd();

                    enemy.UpdateStatusPlane();

                    if (fightEnd)
                    {
                        yield break;
                    }
					
					yield return new WaitForSeconds(1.0f);
                }


                count++;
            }


        }


	}

}

