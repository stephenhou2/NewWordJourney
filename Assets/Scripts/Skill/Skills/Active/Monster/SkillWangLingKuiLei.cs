using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

    public class SkillWangLingKuiLei : ActiveSkill {

		public int hurt;
              
		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			int magicResistCal = enemy.agent.magicResist - self.agent.magicResistDecrease/2;

			if (magicResistCal < -50)
            {
                magicResistCal = -50;
            }

			int acutalHurt = Mathf.RoundToInt(hurt / (magicResistCal / 100f + 1));

			enemy.AddHurtAndShow(acutalHurt, HurtType.Magical, self.towards);

			//enemy.UpdateStatusPlane();

			if (selfEffectAnimName != string.Empty)
            {
                self.SetEffectAnim(selfEffectAnimName);
            }

            if (enemyEffectAnimName != string.Empty)
            {
                enemy.SetEffectAnim(enemyEffectAnimName);
            }

			enemy.PlayShakeAnim();
		}

	}
}
