using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class JingZhunDaJi : PermanentPassiveSkill {


		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			float critHurtScalerGain = skillSourceValue * skillLevel;
			self.agent.critHurtScaler += critHurtScalerGain;
		}

	}
}
