using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 提升<color=orange>魔法上限×技能等级×5%+5</color>的护甲和抗性至战斗结束,可叠加
	public class XianJi : ActiveSkill {

        // 护甲抗性提升系数基数
		public float increaseScaler;
        // 固定护甲抗性提升
		public int fixIncrease;
        
		public override string GetDisplayDescription()
		{
			int increase = Mathf.RoundToInt(Player.mainPlayer.maxMana * skillLevel * increaseScaler + fixIncrease);
			return string.Format("提升<color=white>(魔法上限×技能等级×5%+5)</color><color=red>{0}</color>的护甲和抗性至战斗结束,可叠加", increase);
		}

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{


			int gain = Mathf.RoundToInt(self.agent.maxMana * skillLevel * increaseScaler) + fixIncrease;

			self.agent.armor += gain;
			self.agent.armorChangeFromSkill += gain;

			self.agent.magicResist += gain;
			self.agent.magicResistChangeFromSkill += gain;

			self.AddTintTextToQueue("护甲提升");
			self.AddTintTextToQueue("抗性提升");
         
			self.SetEffectAnim(selfEffectAnimName);
         
		}

	}
}
