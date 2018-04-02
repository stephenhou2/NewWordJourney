using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Mohua : PermanentPassiveSkill {


		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.critHurtScaler += skillSourceValue;
		}

	}
}
