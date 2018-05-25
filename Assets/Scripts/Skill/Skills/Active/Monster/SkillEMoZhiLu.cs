using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SkillEMoZhiLu : ActiveSkill
    {

		public int hurt;

		public int armorChange;

		public int magicResistChange;

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			int actualHurt = Mathf.RoundToInt(hurt / ((enemy.agent.armor - self.agent.armorDecrease) / 100f + 1));

			enemy.AddHurtAndShow(actualHurt, HurtType.Physical, self.towards);

			enemy.PlayShakeAnim();

			enemy.agent.armor += armorChange;
			enemy.agent.armorChangeFromSkill += armorChange;

			enemy.agent.magicResist += magicResistChange;
			enemy.agent.magicResistChangeFromSkill += magicResistChange;

			if (selfEffectAnimName != string.Empty)
            {
                self.SetEffectAnim(selfEffectAnimName);
            }

            if (enemyEffectAnimName != string.Empty)
            {
                enemy.SetEffectAnim(enemyEffectAnimName);
            }

			enemy.UpdateStatusPlane();


                     
		}

	}   

}
