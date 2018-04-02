﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Shuangjianzitai : PermanentPassiveSkill {


		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			float critGain = skillSourceValue * self.agent.agentLevel;
			self.agent.crit += critGain;
		}


	}
}
