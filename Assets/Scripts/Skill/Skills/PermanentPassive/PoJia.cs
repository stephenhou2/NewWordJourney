using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 永久提升<color=orange>技能等级×6</color>的护甲穿透
	public class PoJia : PermanentPassiveSkill {


		public override string GetDisplayDescription()
		{
			int armorDecreaseGain = Mathf.RoundToInt(skillLevel * skillSourceValue);
			return string.Format("永久提升<color=white>(技能等级×6)</color><color=red>{0}</color>的护甲穿透", armorDecreaseGain);
		}

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int armorDecreaseGain = Mathf.RoundToInt(skillSourceValue * skillLevel);

			self.agent.armorDecrease += armorDecreaseGain;

		}
	}
}
