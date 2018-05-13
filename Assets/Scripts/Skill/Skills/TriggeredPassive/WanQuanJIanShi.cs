using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class WanQuanJIanShi : TriggeredPassiveSkill {

		public float dodgeDecreaseBase;

		protected override void BeforeFightTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			int dodgeChange = -(int)(self.agent.agentLevel * dodgeDecreaseBase * enemy.agent.armor);

            enemy.agent.dodgeChangeFromSkill += dodgeChange;

            enemy.agent.dodge += dodgeChange;
		}

	}
}