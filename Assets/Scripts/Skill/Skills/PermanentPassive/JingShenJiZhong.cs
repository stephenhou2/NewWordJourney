using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 永久提升<color=orange>技能等级×10</color>的抗性
	public class JingShenJiZhong : PermanentPassiveSkill {

		public override string GetDisplayDescription()
		{
			int magicResistGain = Mathf.RoundToInt(skillLevel * skillSourceValue);
			return string.Format("永久提升<color=white>(技能等级×10)</color><color=red>{0}</color>的抗性", magicResistGain);
		}

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int magicResistGain = Mathf.RoundToInt(skillSourceValue * skillLevel);

			self.agent.magicResist += magicResistGain;
		}

	}
}