using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 造成1000点魔法伤害，同时降低敌人的魔抗和护甲50点，可以叠加，最高5次
	public class SkillEMoZhiLu : ActiveSkill
    {

		public int hurt;

		public int armorChange;

		public int magicResistChange;

		private int triggeredCount = 0;

		public int maxOverlayCount;

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			int armorCal = enemy.agent.armor - self.agent.armorDecrease/2;

			if (armorCal < -50)
            {
                armorCal = -50;
            }

			int actualHurt = Mathf.RoundToInt(hurt / (armorCal / 100f + 1));

			enemy.AddHurtAndShow(actualHurt, HurtType.Physical, self.towards);

			enemy.PlayShakeAnim();


			if(triggeredCount < maxOverlayCount){

				triggeredCount++;
				
				enemy.agent.armor += -armorChange;
                enemy.agent.armorChangeFromSkill += -armorChange;

                enemy.agent.magicResist += -magicResistChange;
                enemy.agent.magicResistChangeFromSkill += -magicResistChange;
                
                enemy.SetEffectAnim(enemyEffectAnimName);

				enemy.AddTintTextToQueue("护甲降低");
				enemy.AddTintTextToQueue("抗性降低");

			}

			//enemy.UpdateStatusPlane();
         
		}

	}   

}
