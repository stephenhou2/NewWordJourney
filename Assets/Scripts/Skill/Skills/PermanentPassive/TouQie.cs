using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 永久提升<color=orange> 技能等级×2</color>的额外金币
	public class TouQie : PermanentPassiveSkill {

		public override string GetDisplayDescription()
		{
			int extraGoldGain = Mathf.RoundToInt(skillLevel * skillSourceValue);
			return string.Format("永久提升<color=white>(技能等级×2)</color><color=red>{0}</color>的额外金币", extraGoldGain);
		}

		protected override void ExcutePermanentPassiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			int extraGoldGain = Mathf.RoundToInt(skillLevel * skillSourceValue);

			self.agent.extraGold += extraGoldGain;

		}

	}
}
