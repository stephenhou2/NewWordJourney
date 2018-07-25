using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 每次攻击附加<color=orange>技能等级×4</color>的真实伤害
	public class CuiRuoDaJi : TriggeredPassiveSkill {

		public int hurtBase;

		public override string GetDisplayDescription()
		{
			int hurtGain = skillLevel * hurtBase;
			return string.Format("每次攻击附加<color=white>(技能等级×4)</color><color=red>{0}</color>的真实伤害", hurtGain);
		}

		protected override void HitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			int hurt = Mathf.RoundToInt(skillLevel * hurtBase);

			enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);

			//enemy.UpdateStatusPlane ();

		}

	}
}