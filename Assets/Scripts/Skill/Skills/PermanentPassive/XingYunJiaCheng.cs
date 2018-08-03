using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 永久提高<color=orange>技能等级×2%</color>普通宝箱开出金色装备的概率
	public class XingYunJiaCheng : PermanentPassiveSkill
    {

		public override string GetDisplayDescription()
		{
			int treasureLuckGain = Mathf.RoundToInt(skillLevel * skillSourceValue);
			return string.Format("永久提高<color=white>(技能等级×2%)</color><color=red>{0}%</color>普通宝箱开出金色装备的概率", treasureLuckGain);
		}

		protected override void ExcutePermanentPassiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{
			if(!(self is BattlePlayerController)){
				return;
			}

			Player player = self.agent as Player;

			player.extraLuckInOpenTreasure = Mathf.RoundToInt(skillLevel * skillSourceValue);

		}

	}
}

