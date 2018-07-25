using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 回复<color=orange>技能等级×20+100</color>的生命
	public class ZhiLiaoShu : ActiveSkill {

		public int fixHealthGain;
		public int healthGainBase;

		public override string GetDisplayDescription()
		{
			int healthGain = skillLevel * healthGainBase + fixHealthGain;
			return string.Format("回复<color=white>(技能等级×20+100)</color><color=red>{0}</color>的生命", healthGain);

		}

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			// 计算生命回复值
			int healthGain = skillLevel * healthGainBase + fixHealthGain + self.agent.healthRecovery;
            // 生命回复生效
			self.AddHealthGainAndShow (healthGain);

			self.SetEffectAnim(selfEffectAnimName);


		}

	}
}
