using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 永久提高<color=orange>技能等级×4</color>的抗性穿透
	public class AoShuChuanTou : PermanentPassiveSkill
    {

		public override string GetDisplayDescription()
		{
			int magicResistDecreaseGain = Mathf.RoundToInt(skillLevel * skillSourceValue);
			return string.Format("永久提高<color=white>(技能等级×4)</color><color=red>{0}</color>的抗性穿透", magicResistDecreaseGain);
		}

		protected override void ExcutePermanentPassiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{
			int magicResistDecreaseGain = Mathf.RoundToInt(skillLevel * skillSourceValue);

			self.agent.magicResistDecrease += magicResistDecreaseGain;
		}
	}
	
}

