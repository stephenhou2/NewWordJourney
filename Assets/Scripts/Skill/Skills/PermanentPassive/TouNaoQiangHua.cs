using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 永久提升<color=orange>技能等级×30</color>的最大魔法
	public class TouNaoQiangHua : PermanentPassiveSkill {

		public override string GetDisplayDescription()
		{
			int maxManaGain = Mathf.RoundToInt(skillLevel * skillSourceValue);
			return string.Format("永久提升<color=whiet>(技能等级×30)</color><color=red>{0}</color>的最大魔法", maxManaGain);
		}


        protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
        {
			
			int maxManaGain = Mathf.RoundToInt(skillSourceValue * skillLevel);

			int oriMaxMana = self.agent.maxMana;

			self.agent.maxMana += maxManaGain;

			self.agent.mana = (int)(self.agent.mana * self.agent.maxMana / oriMaxMana);
		}
	}
}
