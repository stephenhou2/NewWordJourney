using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 永久提升<color=white>(技能等级×0.5%)</color>的暴击率
	public class Shuangjianzitai : PermanentPassiveSkill {


		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			float critGain = skillSourceValue * skillLevel;
			self.agent.crit += critGain;
		}

		public override string GetDisplayDescription()
		{
			string critGainString = (skillLevel * skillSourceValue * 100).ToString("0.0");
			return string.Format("永久提升<color=white>(技能等级×0.5%)</color><color=red>{0}%</color>的暴击率", critGainString);
		}
	}
}
