using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 造成<color=orange>魔法攻击+技能等级×40+30</color>点魔法伤害
	public class HuoQiuShu : ActiveSkill {

		public int fixHurt;

		public int hurtBase;

		public override string GetDisplayDescription()
		{
			int magicHurt = Player.mainPlayer.magicAttack + skillLevel * hurtBase + fixHurt;
			return string.Format("造成<color=white>(魔法攻击+技能等级×40+30)</color><color=red>{0}</color>点魔法伤害", magicHurt);
		}

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
            // 原始魔法伤害
			int hurt = self.agent.magicAttack + hurtBase * skillLevel + fixHurt;

			int magicResistCal = enemy.agent.magicResist - self.agent.magicResistDecrease/2;

			if (magicResistCal < -50)
            {
                magicResistCal = -50;
            }

			// 结算抗性和抗性穿透之后的实际伤害
			hurt = Mathf.RoundToInt(hurt / (magicResistCal / 100f + 1));

			if (hurt < 0) {
				hurt = 0;
			}

            // 伤害生效并显示
			enemy.AddHurtAndShow (hurt, HurtType.Magical,self.towards);

            // 敌方被击中后退
			enemy.PlayShakeAnim ();

			// 播放技能特效
			enemy.SetEffectAnim(enemyEffectAnimName);

		}
	}
}
