using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ZhiLiaoShu : ActiveSkill {

		public int fixHealthGain;
		public int healthGainBase;

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			// 计算生命回复值
			int healthGain = self.agent.agentLevel * healthGainBase + fixHealthGain + self.agent.healthRecovery;
            // 生命回复生效
			self.AddHealthGainAndShow (healthGain);
		}

	}
}
