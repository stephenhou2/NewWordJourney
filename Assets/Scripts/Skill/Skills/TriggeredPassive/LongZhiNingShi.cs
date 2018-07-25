using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 每次攻击有25%的概率降低敌人<color=orange>技能等级×2</color>的抗性,可叠加
	public class LongZhiNingShi : TriggeredPassiveSkill {

		public int magicReistDecreaseBase;
		public float triggerProbability;

		public override string GetDisplayDescription()
		{
			int magicResistDecrease = skillLevel * magicReistDecreaseBase;
			return string.Format("每次攻击有25%的概率降低敌人<color=white>(技能等级×2)</color><color=red>{0}</color>的抗性,可叠加", magicResistDecrease);
		}

		protected override void HitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{

			if(!isEffective(triggerProbability)){
				return;
			}

			int magicResistDecrease = skillLevel * magicReistDecreaseBase;

			enemy.agent.magicResist -= magicResistDecrease;

			enemy.agent.magicResistChangeFromSkill -= magicResistDecrease;

			enemy.AddTintTextToQueue("抗性降低");

			SetEffectAnims(self, enemy);
		}

	}
}
