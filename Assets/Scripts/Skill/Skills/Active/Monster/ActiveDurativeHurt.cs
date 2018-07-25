using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
   
	public class ActiveDurativeHurt : ActiveSkill
    {

		public int hurtBase;

		public int hurtDuration;

		public string continuousEffectAnimName;

		private IEnumerator poisonCoroutine;
        

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			enemy.SetEffectAnim(enemyEffectAnimName);

			enemy.SetEffectAnim(continuousEffectAnimName, null, 0, hurtDuration);

            if (poisonCoroutine != null)
            {
                StopCoroutine(poisonCoroutine);
            }

			poisonCoroutine = DurativeHurt(self, enemy);

            StartCoroutine(poisonCoroutine);
		}

		private IEnumerator DurativeHurt(BattleAgentController self, BattleAgentController enemy)
        {

			int count = 0;

			while (count < hurtDuration)
            {
                SetEffectAnims(self, enemy);

				enemy.AddHurtAndShow(hurtBase, HurtType.Physical, self.towards);

                enemy.CheckFightEnd();

                enemy.UpdateStatusPlane();

                yield return new WaitForSeconds(1f);

                count++;
            }


        }
        
    }
}

