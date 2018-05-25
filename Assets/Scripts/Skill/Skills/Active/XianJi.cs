using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class XianJi : ActiveSkill {
    

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
                 
            // 生命回复值
			int healthGain = (int)(self.agent.maxMana * skillLevel + self.agent.healthRecovery);

            // 生命回复生效并显示
			self.AddHealthGainAndShow (healthGain);
         
		}

	}
}
