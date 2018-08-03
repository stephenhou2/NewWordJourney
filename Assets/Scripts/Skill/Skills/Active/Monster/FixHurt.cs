using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class FixHurt : ActiveSkill
    {

		public HurtType hurtType;

		public int fixHurt;

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			int actualHurt = 0;

			switch(hurtType){
				case HurtType.Physical:
					int armorCal = enemy.agent.armor - self.agent.armorDecrease/2;

                    if (armorCal < -50)
                    {
                        armorCal = -50;
                    }

					actualHurt = (int)(fixHurt / (armorCal / 100f + 1));               

					break;
				case HurtType.Magical:

					int magicResistCal = enemy.agent.magicResist - self.agent.magicResistDecrease/2;

                    if (magicResistCal < -50)
                    {
                        magicResistCal = -50;
                    }

					actualHurt = (int)(self.agent.magicAttack / (magicResistCal / 100f + 1));
               
					break;
			}

			enemy.AddHurtAndShow(actualHurt, hurtType, self.towards);

            enemy.PlayShakeAnim();

            enemy.SetEffectAnim(enemyEffectAnimName);

		}


	}
}

