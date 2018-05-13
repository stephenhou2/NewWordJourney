using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SkillHuangTuZhiLing : ActiveSkill
    {

		public int hurt;

		public float actionSpeedDecrease;


		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			int acutalHurt = Mathf.RoundToInt(hurt / ((enemy.agent.armor - self.agent.armorDecrease) / 100f + 1));

			enemy.AddHurtAndShow(acutalHurt, HurtType.Physical, self.towards);
            
			enemy.UpdateStatusPlane();

			enemy.PlayShakeAnim();
                     
			enemy.SetRoleAnimTimeScale(1 - actionSpeedDecrease);

		}

	}

}
