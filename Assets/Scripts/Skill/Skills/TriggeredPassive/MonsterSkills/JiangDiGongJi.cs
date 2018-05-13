﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
    public class JiangDiGongJi : TriggeredPassiveSkill
    {

        private bool hasTriggered;
        public float triggeredProbability;
		public int fixedAttackDecrease;
		public float attackDecreaseScaler;

		protected override void HitTriggerCallBack(BattleAgentController self, BattleAgentController enemy)
		{
			if(!hasTriggered && isEffective(triggeredProbability)){
				hasTriggered = true;
				int attackChange = -(int)((attackDecreaseScaler * Player.mainPlayer.agentLevel) + fixedAttackDecrease);
                enemy.agent.attackChangeFromSkill += attackChange;
                enemy.agent.attack += attackChange;
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
