using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	public class BingLingShu : ActiveSkill {

        // 固定伤害
		public int fixHurt;
        // 随技能提升的伤害
		public int hurtBase;
        // 行动速度减慢幅度
		public float attackSpeedDecreaseScaler;

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			int hurt = fixHurt + hurtBase * skillLevel;

			hurt = Mathf.RoundToInt(hurt / ((enemy.agent.magicResist - self.agent.magicResistDecrease) / 100f + 1));

			if(hurt < 0){
				hurt = 0;
			}
            
            // 敌方行动速度减慢
			enemy.SetRoleAnimTimeScale (1 - attackSpeedDecreaseScaler);

			// 伤害生效并显示
            enemy.AddHurtAndShow(hurt, HurtType.Magical, self.towards);

            // 敌方被击中后退
            enemy.PlayShakeAnim();

		}
	}
}
