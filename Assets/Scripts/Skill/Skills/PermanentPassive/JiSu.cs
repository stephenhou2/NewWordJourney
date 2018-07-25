using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	// 永久提升<color=orange>技能等级×2%</color>的行走速度，永久提升一个等级的攻击速度(攻速最多可提升至极快)
	public class JiSu : PermanentPassiveSkill  {
      
		protected override void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			int moveSpeedChange = Mathf.RoundToInt(self.agent.moveSpeed * skillSourceValue * skillLevel);

			self.agent.moveSpeed += moveSpeedChange;

			int speed = (int)(self.agent as Player).attackSpeed;

            speed++;

            if (speed > 3)
            {
                speed = 3;
            }

            (self.agent as Player).attackSpeed = (AttackSpeed)speed;         
		}

		public override string GetDisplayDescription()
		{
			int moveSpeedGainScaler = Mathf.RoundToInt(skillLevel * skillSourceValue * 100);
			return string.Format("永久提升<color=white>(技能等级×2%)</color><color=red>{0}%</color>的行走速度，永久提升一个等级的攻击速度(攻速最多可提升至极快)", moveSpeedGainScaler);
		}

	}
}
