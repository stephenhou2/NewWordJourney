using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 主动回复<color=orange>魔法攻击×技能等级×5%+5</color>的魔法值
	public class XieEZhiHun : ActiveSkill {
    
		public int fixManaRecovery;

		public float manaRecoveryScaler;

		public override string GetDisplayDescription()
		{
			int manaGain = Mathf.RoundToInt(Player.mainPlayer.magicAttack * skillLevel * manaRecoveryScaler + fixManaRecovery);
			return string.Format("主动回复<color=white>(魔法攻击×技能等级×5%+5)</color><color=red>{0}</color>的魔法值", manaGain);
		}


		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{
			int manaGain = (int)(skillLevel * manaRecoveryScaler * self.agent.magicAttack + fixManaRecovery + self.agent.magicRecovery);

			self.AddManaGainAndShow(manaGain);

			self.SetEffectAnim(selfEffectAnimName);
   
		}

	}
}