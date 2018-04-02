using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ZhiLiaoShu : ActiveSkill {

		public int recoverBase;
		public int recoverSeed;

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int healthGain = self.agent.agentLevel * recoverSeed + recoverBase + self.agent.healthRecovery;
			self.AddHealthGainAndShow (healthGain);
			self.UpdateStatusPlane ();
		}

	}
}
