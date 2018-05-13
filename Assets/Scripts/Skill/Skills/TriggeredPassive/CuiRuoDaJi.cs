using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class CuiRuoDaJi : TriggeredPassiveSkill {

		public int hurtBase;

		protected override void HitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			int hurt = Mathf.RoundToInt(self.agent.agentLevel * hurtBase);

			enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);

			enemy.UpdateStatusPlane ();

		}

	}
}