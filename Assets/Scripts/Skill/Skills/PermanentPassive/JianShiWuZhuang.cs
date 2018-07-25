using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 永久提升<color=orange>技能等级×8</color>的护甲
	public class JianShiWuZhuang : PermanentPassiveSkill {

		public override string GetDisplayDescription()
		{
			int armorGain = Mathf.RoundToInt(skillLevel * skillSourceValue);
			return string.Format("永久提升<color=white>(技能等级×8)</color><color=red>{0}</color>的护甲", armorGain);
		}

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int armorGain = Mathf.RoundToInt(skillLevel * skillSourceValue);

			self.agent.armor += armorGain;
		}
	}
}
