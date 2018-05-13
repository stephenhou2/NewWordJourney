using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class KongJuQingXi : TriggeredPassiveSkill {

		public float decreaseBase;

		protected override void BeforeFightTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			int change = -(int)(self.agent.agentLevel * decreaseBase);

            enemy.agent.attackChangeFromSkill += change;
            enemy.agent.magicAttackChangeFromSkill += change;

            enemy.agent.attack += change;
            enemy.agent.magicAttack += change;
		}

	
	}
}
