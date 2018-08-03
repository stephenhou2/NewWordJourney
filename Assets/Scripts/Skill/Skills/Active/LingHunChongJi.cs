using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 对敌人造成<color=orange>技能等级×30+10</color>的真实伤害,并降低敌人5%的攻速，可叠加
	public class LingHunChongJi : ActiveSkill {

		public int fixHurt;

		public int hurtBase;

		public float attackSpeedDecreaseScalerBase;

		public override string GetDisplayDescription()
		{
			int hurt = skillLevel * hurtBase + fixHurt;
			return string.Format("造成<color=white>(技能等级×30+10)</color><color=red>{0}</color>的真实伤害,并降低敌人5%的攻速，可叠加", hurt);
		}

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int hurt = fixHurt + hurtBase * skillLevel;

			enemy.AddHurtAndShow(hurt, HurtType.Physical,self.towards);


			enemy.PlayShakeAnim();

			enemy.SetEffectAnim(enemyEffectAnimName);


			float roleAnimScaler = enemy.GetRoleAnimTimeScaler();

			roleAnimScaler *= (1 - attackSpeedDecreaseScalerBase);

            if (roleAnimScaler < 0.6f)
            {
                return;
            }
            else
            {
                // 敌方行动速度减慢
                enemy.SetRoleAnimTimeScale(roleAnimScaler);
				enemy.SetEffectAnim(CommonData.paralyzedEffectName, null, 0, 0);

            }
		
		}

	}
}
