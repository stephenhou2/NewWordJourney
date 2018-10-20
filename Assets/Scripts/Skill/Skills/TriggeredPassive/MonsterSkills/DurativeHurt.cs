using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class DurativeHurt : TriggeredPassiveSkill
    {

		public int duration;

        public float triggerProbability;

		public int durativeHurtBase;

		public int extraHurtBase;

        private IEnumerator poisonCoroutine;


        protected override void HitTriggerCallBack(BattleAgentController self, BattleAgentController enemy)
        {
            if (!isEffective(triggerProbability))
            {
                return;
            }

			enemy.SetEffectAnim(enemyEffectAnimName, null, 0, 3f);

            if (poisonCoroutine != null)
            {
                StopCoroutine(poisonCoroutine);
            }

            poisonCoroutine = DurativePoison(self, enemy);

            StartCoroutine(poisonCoroutine);
        
        }


        private IEnumerator DurativePoison(BattleAgentController self, BattleAgentController enemy)
        {

            int count = 0;

			int hurt = durativeHurtBase * skillLevel + extraHurtBase;

            while (count < duration)
            {
				if(enemy != null && !enemy.isDead){
					
					SetEffectAnims(self, enemy);
					
					enemy.AddHurtAndShow(hurt, HurtType.Physical, self.towards);
					
					bool fightEnd = enemy.CheckFightEnd();
					
					enemy.UpdateStatusPlane();

					if(fightEnd){
						yield break;
					}
					
					yield return new WaitForSeconds(1f);
                }
         
                count++;
            }

        }


        protected override void FightEndTriggerCallBack(BattleAgentController self, BattleAgentController enemy)
        {
            if (poisonCoroutine != null)
            {
                StopCoroutine(poisonCoroutine);
            }
        }
    }

}
