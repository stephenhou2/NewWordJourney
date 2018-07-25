using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 每次受到攻击有20%的概率降低敌人<color=orange>技能等级×1</color>的物理攻击,可叠加
	public class KongJuQinXi : TriggeredPassiveSkill {

		public float triggeredProbability;
		public float attackDecreaseBase;

		public override string GetDisplayDescription()
		{
			int attackDecrease = Mathf.RoundToInt(skillLevel * attackDecreaseBase);
			return string.Format("每次受到攻击有20%的概率降低敌人<color=white>(技能等级×1)</color><color=red>{0}</color>的物理攻击,可叠加", attackDecrease);
		}

		protected override void BeAttackedTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			if(!isEffective(triggeredProbability)){
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
