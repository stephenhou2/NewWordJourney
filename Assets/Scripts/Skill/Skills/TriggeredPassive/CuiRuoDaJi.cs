using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class CuiRuoDaJi : TriggeredPassiveSkill {

		protected override void HitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			int hurt = (int)(self.agent.agentLevel * skillSourceValue);

			enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);

			enemy.UpdateStatusPlane ();

		}

	}
}