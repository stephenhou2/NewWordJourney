using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 每次攻击有<color=orange>技能等级×1%+10%</color>的概率进行一次连击
	public class ShanDianLianJi : TriggeredPassiveSkill
    {
		public float fixTriggerProbability;

		public float triggerProbabilityBase;

		public override string GetDisplayDescription()
		{
			int probability = Mathf.RoundToInt((skillLevel * triggerProbabilityBase + fixTriggerProbability) * 100);
			return string.Format("每次攻击有<color=white>(技能等级×1%+10%)</color><color=red>{0}%</color>的概率进行一次连击", probability);
		}

		protected override void AttackTriggerCallBack(BattleAgentController self, BattleAgentController enemy)
		{
			if(self.towards == MyTowards.Up || self.towards == MyTowards.Down){
				return;
			}

			float triggerProbability = fixTriggerProbability + skillLevel * triggerProbabilityBase;

			if(!isEffective(triggerProbability)){
				return;
			}
			self.agent.attackInterval = 0;
			if(self is BattlePlayerController){
				(self.agent as Player).attackSpeed = AttackSpeed.NoInterval;
			}
                     
		}
	}
}

