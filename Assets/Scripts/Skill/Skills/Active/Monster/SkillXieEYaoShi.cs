using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
    public class SkillXieEYaoShi : ActiveSkill
    {

        public int hurt;

        //public HurtType hurtType;


		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{
			int actualHurt = Mathf.RoundToInt(hurt / ((enemy.agent.armor - self.agent.armorDecrease) / 100f + 1));

			enemy.AddHurtAndShow(actualHurt,HurtType.Physical,self.towards);

            enemy.UpdateStatusPlane();

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

