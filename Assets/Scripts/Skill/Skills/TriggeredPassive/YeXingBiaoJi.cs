using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 每次攻击有20%的概率降低敌人<color=orange>技能等级×2</color>的护甲,可叠加
	public class YeXingBiaoJi : TriggeredPassiveSkill {

		public int armorDecreaseBase;
		public float triggerProbability;

		public override string GetDisplayDescription()
		{
			int armorDecrease = skillLevel * armorDecreaseBase;
			return string.Format("每次攻击有20%的概率降低敌人<color=white>(技能等级×2)</color><color=red>{0}</color>的护甲,可叠加", armorDecrease);
		}

		protected override void HitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			if(!isEffective(triggerProbability)){
				return;
			}

			int armorChange = skillLevel * armorDecreaseBase;

            enemy.agent.armorChangeFromSkill -= armorChange;

            enemy.agent.armor -= armorChange;

			enemy.AddTintTextToQueue("护甲降低");

			SetEffectAnims(self, enemy);
		}

	}
}