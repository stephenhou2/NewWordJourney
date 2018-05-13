﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

    public class SkillWangLingKuiLei : ActiveSkill {

		public int hurt;
              
		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{
			int acutalHurt = Mathf.RoundToInt(hurt / ((enemy.agent.magicResist - self.agent.magicResistDecrease) / 100f + 1));

			enemy.AddHurtAndShow(acutalHurt, HurtType.Magical, self.towards);

			enemy.UpdateStatusPlane();

			enemy.PlayShakeAnim();
		}

	}
}
