using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 消耗大量魔法,回复<color=orange>魔法上限×0.5×技能等级+50</color>的生命
	public class XianJi : ActiveSkill {

        // 回复系数
		public float scaler;

        // 固定回复
		public int fixHealthRecover;

		public override string GetDisplayDescription()
		{
			int healthGain = Mathf.RoundToInt(Player.mainPlayer.maxMana * scaler + fixHealthRecover);
			return string.Format("消耗大量魔法,回复<color=white>(魔法上限×0.5×技能等级+50)</color><color=red>{0}</color>的生命", healthGain);
		}

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
                 
            // 生命回复值
			int healthGain = (int)(self.agent.maxMana * skillLevel * scaler + fixHealthRecover + self.agent.healthRecovery);

            // 生命回复生效并显示
			self.AddHealthGainAndShow (healthGain);

			self.SetEffectAnim(selfEffectAnimName);


		}

	}
}
