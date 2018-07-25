using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 回复<color=orange>技能等级×护甲×3%+50</color>的生命

	public class ShenShengZhuFu : ActiveSkill
    {
		public int fixHealthGain;

		public float healthGainScaler;

		public override string GetDisplayDescription()
		{
			int healthGain = Mathf.RoundToInt(skillLevel * healthGainScaler * Player.mainPlayer.armor + fixHealthGain);
			return string.Format("回复<color=white>(技能等级×护甲×3%+50)</color><color=red>{0}</color>的生命", healthGain);
		}

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{
			int healthGain = (int)(skillLevel * self.agent.armor * healthGainScaler + fixHealthGain + self.agent.healthRecovery);

			self.AddHealthGainAndShow(healthGain);

			self.SetEffectAnim(selfEffectAnimName);

		}

	}
}

