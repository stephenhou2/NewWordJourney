﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
    public class SkillMoFaHuWei : ActiveSkill
    {

        public float healthRecoverScaler;

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			int healthGain = (int)(self.agent.maxHealth * healthRecoverScaler);

            //self.agent.health += healthGain;

            self.AddHealthGainAndShow(healthGain);

			if (selfEffectAnimName != string.Empty)
            {
                self.SetEffectAnim(selfEffectAnimName);
            }

            if (enemyEffectAnimName != string.Empty)
            {
                enemy.SetEffectAnim(enemyEffectAnimName);
            }

            //self.UpdateStatusPlane();
		}


	}

}
