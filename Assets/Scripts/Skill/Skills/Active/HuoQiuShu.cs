using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class HuoQiuShu : ActiveSkill {

		public int fixHurt;

		public int hurtBase;

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
            // 原始魔法伤害
			int hurt = fixHurt + hurtBase * skillLevel;

			// 结算抗性和抗性穿透之后的实际伤害
			hurt = Mathf.RoundToInt(hurt / ((enemy.agent.magicResist - self.agent.magicResistDecrease) / 100f + 1));

			if (hurt < 0) {
				hurt = 0;
			}

            // 伤害生效并显示
			enemy.AddHurtAndShow (hurt, HurtType.Magical,self.towards);

            // 敌方被击中后退
			enemy.PlayShakeAnim ();

			// 播放技能特效
			SetEffectAnims(self, enemy);
         
		}
	}
}
