using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 永久提高<color=orange>技能等级×1%</color>怪物掉落物品的概率
	public class TanLanLueDuo : PermanentPassiveSkill
    {
        
		public override string GetDisplayDescription()
		{
			int monsterLuckGain = Mathf.RoundToInt(skillLevel * skillSourceValue);
			return string.Format("永久提高<color=white>(技能等级×1%)</color><color=red>{0}%</color>怪物掉落物品的概率", monsterLuckGain);
		}

		protected override void ExcutePermanentPassiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{
			if(!(self is BattlePlayerController)){
				return;
			}

			int extraLuckInMonster = Mathf.RoundToInt(skillLevel * skillSourceValue);

			(self.agent as Player).extraLuckInMonsterTreasure = extraLuckInMonster;

        }

    }
}

