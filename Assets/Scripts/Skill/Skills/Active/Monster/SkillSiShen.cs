using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SkillSiShen : ActiveSkill
    {

		public int hurt;
		public int armorAndMagicReistDecrease;

		//private bool hasTriggered = false;


		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{
			int actualHurt = Mathf.RoundToInt(hurt / ((enemy.agent.magicResist - self.agent.magicResistDecrease) / 100f + 1));

			enemy.AddHurtAndShow(hurt, HurtType.Magical, self.towards);

			enemy.PlayShakeAnim();

			int armorAndMagicResistChange = -armorAndMagicReistDecrease;
	
			enemy.agent.armor += armorAndMagicResistChange;
			enemy.agent.armorChangeFromSkill += armorAndMagicResistChange;


			enemy.agent.magicResist += armorAndMagicResistChange;
			enemy.agent.magicResistChangeFromSkill += armorAndMagicResistChange;
            
         
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
