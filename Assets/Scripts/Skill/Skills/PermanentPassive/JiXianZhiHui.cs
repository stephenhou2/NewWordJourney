using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 永久提高<color=orange>技能等级×3+5</color>的魔法回复
	public class JiXianZhiHui : PermanentPassiveSkill
    {

		public int fixMagicRecoveryIncrease;

		public int magicRecoveryIncreaseBase;

		public override string GetDisplayDescription()
		{
			int magicRecoveryGain = skillLevel * magicRecoveryIncreaseBase + fixMagicRecoveryIncrease;
			return string.Format("永久提高<color=white>(技能等级×3+5)</color><color=red>{0}</color>的魔法回复", magicRecoveryGain);
		}

		protected override void ExcutePermanentPassiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{
			int magicRecoveryIncrease = skillLevel * magicRecoveryIncreaseBase + fixMagicRecoveryIncrease;

			self.agent.magicRecovery += magicRecoveryIncrease;

		}
	}
}

