using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 降低自身<color=orange>技能等级×2</color>的护甲，提高<color=orange>技能等级×0.4%</color>的暴击率至战斗结束，可叠加

	public class ZhanDouXuanYan : ActiveSkill
    {

		public int armorDecreaseBase;

		public float critIncreaseScaler;

		public override string GetDisplayDescription()
		{
			int armorDecrease = skillLevel * armorDecreaseBase;
			string critGainString = (skillLevel * critIncreaseScaler * 100).ToString("0.0");
			return string.Format("降低自身<color=white>(技能等级×2)</color><color=red>{0}</color>的护甲，" +
			                     "提高<color=white>(技能等级×0.4%)</color><color=red>{1}%</color>的暴击率至战斗结束，可叠加", armorDecrease, critGainString);
		}

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{
			int armorDecrease = armorDecreaseBase * skillLevel;

			if(self.agent.armor < armorDecrease){
				armorDecrease = self.agent.armor;
			}


			self.agent.armor -= armorDecrease;
			self.agent.armorChangeFromSkill -= armorDecrease;

			float critIncrease = critIncreaseScaler * skillLevel;

			self.agent.crit += critIncrease;
			self.agent.critChangeFromSkill += critIncrease;

			self.SetEffectAnim(selfEffectAnimName);

			self.AddTintTextToQueue("护甲\n降低");
			self.AddTintTextToQueue("暴击\n提升");

		}


	}
}

