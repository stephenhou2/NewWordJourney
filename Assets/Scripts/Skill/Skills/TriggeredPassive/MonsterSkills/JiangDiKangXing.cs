using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
    public class JiangDiKangXing : TriggeredPassiveSkill
    {
        
        private bool isTriggered;
        public float triggeredProbability;
		public int fixedMagicResistDecrease;

		public float magicResistDecreaseScaler;

        protected override void HitTriggerCallBack(BattleAgentController self, BattleAgentController enemy)
        {
            if (!isTriggered && isEffective(triggeredProbability))
            {
                isTriggered = true;
				int magicResistDecrease = -(int)(magicResistDecreaseScaler * Player.mainPlayer.agentLevel + fixedMagicResistDecrease);
                enemy.agent.magicResistChangeFromSkill += magicResistDecrease;
                enemy.agent.magicResist += magicResistDecrease;
                enemy.UpdateStatusPlane();


				if(selfEffectAnimName != string.Empty){
					self.SetEffectAnim(selfEffectAnimName);
				}

				if(enemyEffectAnimName != string.Empty){
					enemy.SetEffectAnim(enemyEffectAnimName);
				}

            }
        }




    }

}
