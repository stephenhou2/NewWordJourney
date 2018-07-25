using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class JiangDiHuJia : TriggeredPassiveSkill
    {
		private bool isTriggered;
        public float triggeredProbability;
        public int fixedArmorDecrease;
		public float armorDecreaseScaler;

        protected override void HitTriggerCallBack(BattleAgentController self, BattleAgentController enemy)
        {
            if (!isTriggered && isEffective(triggeredProbability))
            {
                isTriggered = true;
				int armorDecrease = -(int)(armorDecreaseScaler * Player.mainPlayer.agentLevel + fixedArmorDecrease);
				enemy.agent.armorChangeFromSkill += armorDecrease;
				enemy.agent.armor += armorDecrease;
                //enemy.UpdateStatusPlane();

				enemy.AddTintTextToQueue("护甲降低");

				SetEffectAnims(self, enemy);

            }
        }

        
    }
}

