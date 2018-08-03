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

			int armorCal = enemy.agent.armor - self.agent.armorDecrease/2;

			if (armorCal < -50)
            {
                armorCal = -50;
            }

			int acutalHurt = Mathf.RoundToInt(hurt / (armorCal / 100f + 1));

			enemy.AddHurtAndShow(acutalHurt, HurtType.Physical, self.towards);

			enemy.PlayShakeAnim();
         
			enemy.SetEffectAnim(enemyEffectAnimName,null,0,0);
     
			enemy.SetRoleAnimTimeScale(1 - actionSpeedDecrease);

		}

	}

}
