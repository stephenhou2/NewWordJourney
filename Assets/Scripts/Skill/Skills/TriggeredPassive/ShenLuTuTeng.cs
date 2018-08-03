using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 每次受到攻击有<color=orange>技能等级×2%+30%</color>的概率消耗<color=orange>技能等级x2</color>的魔法，回复<color=orange>技能等级x5</color>的生命
	public class ShenLuTuTeng : TriggeredPassiveSkill {

		public float fixTriggerProbability;

		public float triggerProbabilityBase;
        
		public int manaDecreaseBase;

		public int healthIncreaseBase;

		public override string GetDisplayDescription()
		{
			int triggerProbability = Mathf.RoundToInt((fixTriggerProbability + skillLevel * triggerProbabilityBase) * 100);
			int manaLose = skillLevel * manaDecreaseBase;
			int healthGain = skillLevel * healthIncreaseBase;
			return string.Format("每次受到攻击有<color=white>(技能等级×2%+30%)</color><color=red>{0}%</color>的概率" +
			                     "消耗<color=white>(技能等级x2)</color><color=red>{1}</color>的魔法，" +
			                     "回复<color=white>(技能等级x5)</color><color=red>{2}</color>的生命" ,
			                     triggerProbability,manaLose, healthGain);
		}

		protected override void BeAttackedTriggerCallBack(BattleAgentController self, BattleAgentController enemy)
		{
			int manaDecrease = skillLevel * manaDecreaseBase;
         
			if(self.agent.mana < manaDecrease){
				return;
			}

			float triggerProbability = fixTriggerProbability + skillLevel * triggerProbabilityBase;

			if(!isEffective(triggerProbability)){
				return;            
			}

			int healthIncrease = skillLevel * healthIncreaseBase + self.agent.healthRecovery;

			self.agent.mana -= manaDecrease;
                     
			self.AddHealthGainAndShow(healthIncrease);

			self.SetEffectAnim(selfEffectAnimName);

			//self.UpdateStatusPlane();

		
		}

        


	}
}
