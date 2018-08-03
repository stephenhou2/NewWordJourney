using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 提高自身<color=orange>技能等级×2%</color>的命中率
	public class WanQuanJIanShi : TriggeredPassiveSkill {

		public float dodgeDecreaseBase;

		public override string GetDisplayDescription()
		{
			int hitIncrease = Mathf.RoundToInt(skillLevel * dodgeDecreaseBase * 100);
			return string.Format("提高自身<color=white>(技能等级×2%)</color><color=red>{0}%</color>的命中率", hitIncrease);
		}

		protected override void BeforeFightTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{         
			float dodgeDecrease = skillLevel * dodgeDecreaseBase;

			if(dodgeDecrease > enemy.agent.dodge){
				dodgeDecrease = enemy.agent.dodge;
			}

			enemy.agent.dodge -= dodgeDecrease;

			enemy.agent.dodgeChangeFromSkill -= dodgeDecrease;


		}

	}
}