using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 造成<color=orange>技能等级×8%×护甲+10</color>的真实伤害，同时提高自身<color=orange>技能等级×4</color>的护甲至战斗结束，不叠加
	public class WuWeiFanJi : ActiveSkill
    {

		public float healthDecreaseScaler;

		public int fixHealthDecrease;

		public int armorGainBase;

		public override string GetDisplayDescription()
		{
			int hurt = Mathf.RoundToInt(skillLevel * healthDecreaseScaler * Player.mainPlayer.armor + fixHealthDecrease);
			int armorGain = skillLevel * armorGainBase;
			return string.Format("造成<color=white>(技能等级×8%×护甲+10)</color><color=red>{0}</color>的真实伤害，" +
								 "同时提高自身<color=white>(技能等级×4)</color><color=red>{1}</color>的护甲至战斗结束，可叠加", hurt, armorGain);
		}

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{
			int healthDecrease = (int)(skillLevel * healthDecreaseScaler * self.agent.armor + fixHealthDecrease);
             
			enemy.AddHurtAndShow(healthDecrease, HurtType.Physical, self.towards);

			enemy.PlayShakeAnim();

			enemy.SetEffectAnim(enemyEffectAnimName);  


			hasTriggered = true;

			int armorIncrease = skillLevel * armorGainBase;

            self.agent.armor += armorIncrease;
            self.agent.armorChangeFromSkill += armorIncrease;

            self.AddTintTextToQueue("护甲\n提升");

			self.SetEffectAnim(selfEffectAnimName);


		}        
    }

}
