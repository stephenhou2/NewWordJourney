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

		private bool effectAnimTriggered = false;

		protected override void HitTriggerCallBack(BattleAgentController self, BattleAgentController enemy)
		{

            if(isEffective(triggeredProbability)){

				enemy.SetRoleAnimTimeScale(1 - fixedAttackSpeedDecreaseScaler * Player.mainPlayer.agentLevel - fixedAttackSpeedDecrease);
                            
				enemy.AddTintTextToQueue("攻速\n降低");

				if(!effectAnimTriggered){
					enemy.SetEffectAnim(enemyEffectAnimName, null, 0, 0);
					effectAnimTriggered = true;
				}            
            }

		}


	}
}

