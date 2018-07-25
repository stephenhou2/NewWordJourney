using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 永久提升<color=orange>技能等级×5%</color>的暴击伤害
	public class JingZhunDaJi : PermanentPassiveSkill {


		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			float critHurtScalerGain = skillSourceValue * skillLevel;
			self.agent.critHurtScaler += critHurtScalerGain;
		}

		public override string GetDisplayDescription()
		{
			int hurtScalerGain = Mathf.RoundToInt(skillLevel * skillSourceValue * 100);

			return string.Format("永久提升<color=white>(技能等级×5%)</color><color=red>{0}%</color>的暴击伤害", hurtScalerGain);
		}

	}
}
