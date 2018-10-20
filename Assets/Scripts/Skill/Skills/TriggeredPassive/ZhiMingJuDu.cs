using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 普通攻击有15%几率使敌人中毒，每秒损失<color=orange>技能等级×5</color>的生命，持续3s
	public class ZhiMingJuDu : TriggeredPassiveSkill {

		public float triggerdProbability;

		public int healthDecreaseBase;

		public int poisonDuration;

		private IEnumerator poisonCoroutine;

		private int poisonHurt;

		public override string GetDisplayDescription()
		{
			int hurt = skillLevel * healthDecreaseBase;
			return string.Format("普通攻击有15%几率使敌人中毒，每秒损失<color=white>(技能等级×5)</color><color=red>{0}</color>的生命，持续3s", hurt);
		}

		protected override void HitTriggerCallBack(BattleAgentController self, BattleAgentController enemy)
		{
			if(!isEffective(triggerdProbability)){            
				return;
			}

			poisonHurt = skillLevel * healthDecreaseBase;

			if(poisonCoroutine != null){
				StopCoroutine(poisonCoroutine);
			}

			poisonCoroutine = Poison(self, enemy);

			StartCoroutine(poisonCoroutine);

			enemy.SetEffectAnim(enemyEffectAnimName, null, 0, poisonDuration);

		}

		private IEnumerator Poison(BattleAgentController self, BattleAgentController enemy)
        {

            int count = 0;

            //int hurt = poisonBase * skillLevel + self.agent.extraPoisonHurt;

            while (count < poisonDuration)
            {
				if (enemy != null && !enemy.isDead)
				{

					enemy.AddHurtAndShow(poisonHurt, HurtType.Physical, self.towards);

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
