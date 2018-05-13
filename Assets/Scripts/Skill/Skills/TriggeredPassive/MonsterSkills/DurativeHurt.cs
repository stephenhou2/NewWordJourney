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

            if (poisonCoroutine != null)
            {
                StopCoroutine(poisonCoroutine);
            }

            poisonCoroutine = DurativePoison(self, enemy);

            StartCoroutine(poisonCoroutine);

            if (selfEffectAnimName != string.Empty)
            {
                self.SetEffectAnim(selfEffectAnimName);
            }

            if (enemyEffectAnimName != string.Empty)
            {
                enemy.SetEffectAnim(enemyEffectAnimName);
            }

        }


        private IEnumerator DurativePoison(BattleAgentController self, BattleAgentController enemy)
        {

            int count = 0;

			int hurt = Mathf.RoundToInt((enemy.agent.agentLevel * durativeHurtBase + extraHurtBase) * self.agent.poisonHurtScaler);

            while (count < duration)
            {

                enemy.AddHurtAndShow(hurt, HurtType.Physical, self.towards);

                yield return new WaitForSeconds(1f);

                enemy.UpdateStatusPlane();

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
