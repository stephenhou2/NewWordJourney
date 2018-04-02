using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class LingHunChongJi : ActiveSkill {

		public float attackSpeedDecreaseScaler;

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			enemy.SetRoleAnimTimeScale (1 - attackSpeedDecreaseScaler);
		}

	}
}
