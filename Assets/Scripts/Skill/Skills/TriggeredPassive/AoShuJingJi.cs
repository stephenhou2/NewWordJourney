using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 每次攻击有<color=orange>技能等级×1%+5%</color>的概率将所造成魔法伤害的30%转换为自身生命
	public class AoShuJingJi : TriggeredPassiveSkill
    {

		public float fixTriggerProbability;

		public float triggerProbabilityBase;

		public float healthAbsorbScaler;

		public override string GetDisplayDescription()
		{
			int probability = Mathf.RoundToInt((skillLevel * triggerProbabilityBase + fixTriggerProbability) * 100);
			return string.Format("每次攻击有<color=white>(技能等级×1%+5%)</color><color=red>{0}%</color>的概率将所造成魔法伤害的30%转换为自身生命", probability);
		}

		protected override void HitTriggerCallBack(BattleAgentController self, BattleAgentController enemy){

			float triggerProbability = fixTriggerProbability + skillLevel * triggerProbabilityBase;

			if(!isEffective(triggerProbability)){
				return;
			}

			int healthAbsorb = Mathf.RoundToInt(self.agent.magicalHurtToEnemy * healthAbsorbScaler + self.agent.healthRecovery);

			self.AddHealthGainAndShow(healthAbsorb);

			self.UpdateStatusPlane();

			self.SetEffectAnim(selfEffectAnimName);

		}
    }
}

