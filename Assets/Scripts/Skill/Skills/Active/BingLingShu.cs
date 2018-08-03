using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 造成<color=orange>魔法攻击+技能等级×25+20</color>的魔法伤害,同时降低敌人5%的攻速，可叠加
	public class BingLingShu : ActiveSkill {

        // 固定伤害
		public int fixHurt;
        // 随技能提升的伤害
		public int hurtBase;
        // 行动速度减慢幅度
		public float attackSpeedDecreaseScalerBase;

		public override string GetDisplayDescription()
		{
			int magicHurt = Player.mainPlayer.magicAttack + skillLevel * hurtBase + fixHurt;
			return string.Format("造成<color=white>(魔法攻击+技能等级×25+20)</color><color=red>{0}</color>的魔法伤害,同时降低敌人5%的攻速，可叠加", magicHurt);
		}

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{         

			int hurt = self.agent.magicAttack + fixHurt + hurtBase * skillLevel;

			int magicResistCal = enemy.agent.magicResist - self.agent.magicResistDecrease/2;

			if (magicResistCal < -50)
            {
                magicResistCal = -50;
            }

			hurt = Mathf.RoundToInt(hurt / (magicResistCal / 100f + 1));

			if(hurt < 0){
				hurt = 0;
			}

			// 伤害生效并显示
            enemy.AddHurtAndShow(hurt, HurtType.Magical, self.towards);

            enemy.SetEffectAnim(enemyEffectAnimName);

			// 敌方被击中后退
            enemy.PlayShakeAnim();

			float roleAnimScaler = enemy.GetRoleAnimTimeScaler();

			roleAnimScaler *= (1 - attackSpeedDecreaseScalerBase);

			if(roleAnimScaler < 0.6f){
				return;
			}else{
				// 敌方行动速度减慢
                enemy.SetRoleAnimTimeScale(roleAnimScaler);
				enemy.SetEffectAnim(CommonData.frozenEffectName, null, 0, 0);
			}

		}
	}
}
