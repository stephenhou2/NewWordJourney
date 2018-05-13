using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
    public class JiangDiGongSu : TriggeredPassiveSkill
    {

        public float triggeredProbability;

		public float fixedAttackSpeedDecrease;

		public float fixedAttackSpeedDecreaseScaler;

		protected override void HitTriggerCallBack(BattleAgentController self, BattleAgentController enemy)
		{

            if(isEffective(triggeredProbability)){

				enemy.SetRoleAnimTimeScale(1 - fixedAttackSpeedDecreaseScaler * Player.mainPlayer.agentLevel - fixedAttackSpeedDecrease);

				if (selfEffectAnimName != string.Empty)
                {
                    self.SetEffectAnim(selfEffectAnimName);
                }

                if (enemyEffectAnimName != string.Empty)
                {
                    enemy.SetEffectAnim(enemyEffectAnimName);
                }

            }

		}


	}
}

