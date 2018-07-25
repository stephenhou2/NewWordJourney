using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 造成<color=orange>技能等级×15+10</color>的真实伤害,并使敌人进入麻痹状态
	public class LingHunChongJi : ActiveSkill {

		public int fixHurt;

		public int hurtBase;

		public float attackSpeedDecreaseScaler;

		public override string GetDisplayDescription()
		{
			int hurt = skillLevel * hurtBase + fixHurt;
			return string.Format("造成<color=white>(技能等级×15+10)</color><color=red>{0}</color>的真实伤害,并使敌人进入麻痹状态", hurt);
		}

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int hurt = fixHurt + hurtBase * skillLevel;

			enemy.AddHurtAndShow(hurt, HurtType.Physical,self.towards);

			enemy.SetRoleAnimTimeScale (1 - attackSpeedDecreaseScaler);

			enemy.PlayShakeAnim();

			enemy.SetEffectAnim(enemyEffectAnimName);

			enemy.SetEffectAnim(CommonData.paralyzedEffectName, null, 0, 0);

			if (sfxName != string.Empty)
            {
				GameManager.Instance.soundManager.PlayAudioClip(sfxName);
            }
		}

	}
}
