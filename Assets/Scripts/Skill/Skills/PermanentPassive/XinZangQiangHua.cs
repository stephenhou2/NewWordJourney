using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 永久提高<color=orange>技能等级×3+5</color>的生命回复
	public class XinZangQiangHua : PermanentPassiveSkill
    {
		public int fixHealthRecoveryGain;

		public int healthRecoveryGainBase;

		public override string GetDisplayDescription()
		{
			int healthRecoveryGain = skillLevel * healthRecoveryGainBase + fixHealthRecoveryGain;
			return string.Format("永久提高<color=white>(技能等级×3+5)</color><color=red>{0}</color>的生命回复", healthRecoveryGain);
		}

		protected override void ExcutePermanentPassiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
        {
			int healthRecoveryGain = (int)(skillLevel * healthRecoveryGainBase + fixHealthRecoveryGain);

			self.agent.healthRecovery += healthRecoveryGain;

			//self.agent.healthRecoveryChangeFromSkill += healthRecoveryGain;

			//self.SetEffectAnim(selfEffectAnimName);
		}


	}

}
