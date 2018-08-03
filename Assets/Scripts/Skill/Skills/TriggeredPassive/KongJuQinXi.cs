using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 每次受到攻击有<color=orange>技能等级×2%+30%</color>的概率降低敌人<color=orange>技能等级×2</color>的物理攻击,可叠加
	public class KongJuQinXi : TriggeredPassiveSkill {

		public float fixTriggeredProbability;
		public float triggeredProbabilityBase;
		public float attackDecreaseBase;

		public override string GetDisplayDescription()
		{
			int attackDecrease = Mathf.RoundToInt(skillLevel * attackDecreaseBase);
			int triggerProbabilityX100 = Mathf.RoundToInt((fixTriggeredProbability + skillLevel * triggeredProbabilityBase) * 100);
			return string.Format("每次受到攻击有<color=white>(技能等级×2%+30%)</color><color=red>{0}%</color>的概率降低敌人<color=white>(技能等级×2)</color><color=red>{1}</color>的物理攻击,可叠加", triggerProbabilityX100,attackDecrease);
		}

		protected override void BeAttackedTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			float triggerProbability = fixTriggeredProbability + skillLevel * triggeredProbabilityBase;

			if(!isEffective(triggerProbability)){
				return;
			}

			int attackDecrease = Mathf.RoundToInt(skillLevel * attackDecreaseBase);

			enemy.agent.attack -= attackDecrease;
			enemy.agent.attackChangeFromSkill += attackDecrease;
            
			enemy.AddTintTextToQueue("攻击降低");

			SetEffectAnims(self, enemy);
		}

	
	}
}
