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
                enemy.UpdateStatusPlane();


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

