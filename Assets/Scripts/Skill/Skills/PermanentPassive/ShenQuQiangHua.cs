using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 永久提升<color=orange>技能等级×100</color>的生命上限
	public class ShenQuQiangHua : PermanentPassiveSkill {

		public override string GetDisplayDescription()
		{
			int maxHealthGain = Mathf.RoundToInt(skillLevel * skillSourceValue);
			return string.Format("永久提升<color=white>(技能等级×100)</color><color=red>{0}</color>的生命上限", maxHealthGain);
		}

		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int maxHealthGain = Mathf.RoundToInt(skillSourceValue * skillLevel);

			int oriMaxHealth = self.agent.maxHealth;

			self.agent.maxHealth += maxHealthGain;

			self.agent.health = (int)(self.agent.health * self.agent.maxHealth / oriMaxHealth);
		}
	}
}
