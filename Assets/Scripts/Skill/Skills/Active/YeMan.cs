using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 消耗<color=orange>技能等级x10</color>的生命，回复<color=orange>技能等级x物理攻击x3%+5</color>的魔法
	public class YeMan : ActiveSkill {


		public int healthDecreaseBase;

		public float manaIncreaseScaler;

		public int fixManaIncrease;

		public override string GetDisplayDescription()
		{
			int healthDecrease = skillLevel * healthDecreaseBase;
			int manaGain = Mathf.RoundToInt(skillLevel * Player.mainPlayer.attack * manaIncreaseScaler + fixManaIncrease);
			return string.Format("消耗<color=white>(技能等级x10)</color><color=red>{0}</color>的生命，回复<color=white>(技能等级x物理攻击x3%+5)</color><color=red>{1}</color>的魔法", healthDecrease, manaGain);
		}

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			int healthDecrease = skillLevel * healthDecreaseBase;

			float scaler = 1f;

			if(self.agent.health <= healthDecrease){

				scaler = (self.agent.health - 1) / healthDecrease;

				healthDecrease = self.agent.health - 1;

			}

			self.agent.health -= healthDecrease;

			int manaIncrease = Mathf.RoundToInt(scaler * skillLevel * manaIncreaseScaler * self.agent.attack + fixManaIncrease) + self.agent.magicRecovery;

			self.AddManaGainAndShow(manaIncrease);

			self.SetEffectAnim(selfEffectAnimName);

			//self.UpdateStatusPlane();

			if (self.isInFight && self is BattlePlayerController)
			{
				ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons();
			}

		}
	}
}
