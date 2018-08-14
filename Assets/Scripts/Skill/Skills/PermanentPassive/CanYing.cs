using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 永久提升<color=orange>技能等级×1%</color>的闪避率
	public class CanYing : PermanentPassiveSkill {


		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			float dodgeGain = skillSourceValue * skillLevel;
			self.agent.dodge += dodgeGain;
		}

		public override string GetDisplayDescription()
		{
			string dodgeGainStrig = (skillLevel * skillSourceValue * 100).ToString("0");
			return string.Format("永久提升<color=white>(技能等级×1%)</color><color=red>{0}%</color>的闪避率", dodgeGainStrig);
		}

	}
}
