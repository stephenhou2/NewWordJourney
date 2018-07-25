using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 永久提升<color=orange>技能等级×5</color>的额外经验
	public class QiJiShengZhang : PermanentPassiveSkill {


		public override string GetDisplayDescription()
		{
			int extraExpGain = Mathf.RoundToInt(skillLevel * skillSourceValue);
			return string.Format("永久提升<color=white>(技能等级×5)</color><color=red>{0}</color>的额外经验", extraExpGain);
		}

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			int extraExperienceGain = Mathf.RoundToInt(skillSourceValue * skillLevel);

			(self.agent as Player).extraExperience += extraExperienceGain;
		}
	}
}