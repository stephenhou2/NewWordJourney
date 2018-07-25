using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 每次受到攻击时，有25%的几率牺牲<color=orange>技能等级×1</color>的生命，提高<color=orange>技能等级×3</color>的物理攻击，可叠加
	public class ManNiuTuTeng : TriggeredPassiveSkill
    {
		public float triggerProbability;

		public int healthDecreaseBase;

		public int attackIncreaseBase;

		//public int magicResistIncreaseBase;

		public override string GetDisplayDescription()
		{
			int healthDecrease = skillLevel * healthDecreaseBase;
			int attackIncrease = skillLevel * attackIncreaseBase;
			return string.Format("每次受到攻击时，有25%的几率牺牲<color=white>(技能等级×1)</color><color=red>{0}</color>的生命，" +
			                     "提高<color=white>(技能等级×3)</color><color=red>{1}</color>的物理攻击，可叠加", healthDecrease, attackIncrease);
		}

		protected override void BeAttackedTriggerCallBack(BattleAgentController self, BattleAgentController enemy)
		{

			if(!isEffective(triggerProbability)){
				return;
			}

			float scaler = 1f;

			int healthDecrease = skillLevel * healthDecreaseBase;

			if(self.agent.health <= healthDecrease){
				scaler = (self.agent.health - 1) / healthDecrease;
				healthDecrease = self.agent.health - 1;
			}

			int armorIncrease = Mathf.RoundToInt(scaler * skillLevel * attackIncreaseBase);
			int magicResistIncrease = Mathf.RoundToInt(scaler * skillLevel * attackIncreaseBase);

			self.agent.health -= healthDecrease;

			self.agent.attack += armorIncrease;
			self.agent.attackChangeFromSkill += armorIncrease;
         
			string tint = "攻击\n提升";
			self.AddTintTextToQueue(tint);
         
			SetEffectAnims(self, enemy);

		}
	}

}

