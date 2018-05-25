using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class LingHunChongJi : ActiveSkill {

		public int fixHurt;

		public int hurtBase;

		public float attackSpeedDecreaseScaler;

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int hurt = fixHurt + hurtBase * skillLevel;

			enemy.AddHurtAndShow(hurt, HurtType.Physical,self.towards);

			enemy.SetRoleAnimTimeScale (1 - attackSpeedDecreaseScaler);

			enemy.PlayShakeAnim();
		}

	}
}
