using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 永久提升<color=orange> 技能等级×5</color>的物理攻击
	public class YongWangZhiQian : PermanentPassiveSkill {

		public override string GetDisplayDescription()
		{
			int attackGain = Mathf.RoundToInt(skillLevel * skillSourceValue);
			return string.Format("永久提升<color=white>(技能等级×5)</color><color=red>{0}</color>的物理攻击", attackGain);
		}

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			int attackIncrease = Mathf.RoundToInt(skillLevel * skillSourceValue);

			self.agent.attack += attackIncrease;

		}
	}
}
