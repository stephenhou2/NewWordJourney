using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ChongFengNaHan : ActiveSkill
    {

		// 降低敌人（技能等级×2）的物理攻击+同时提升自己（技能等级×2）的物理攻击+到战斗结束+不叠加

		public int attackChangeBase;

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{
			int attackChange = attackChangeBase * skillLevel;

			self.agent.attack += attackChange;
			self.agent.attackChangeFromSkill += attackChange;

			enemy.agent.attack -= attackChange;
			self.agent.attackChangeFromSkill -= attackChange;
         
		}
	}
}

