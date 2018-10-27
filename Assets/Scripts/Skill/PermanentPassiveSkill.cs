using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	/// <summary>
    /// 永久型技能
    /// </summary>
	public abstract class PermanentPassiveSkill : Skill {

        // 元数据【通用的数据，用于作为技能效果的计算base number】
		public float skillSourceValue;

		void Awake(){
			this.skillType = SkillType.PermanentPassive;
		}

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			ExcutePermanentPassiveSkillLogic (self, enemy);
		}

		protected abstract void ExcutePermanentPassiveSkillLogic (BattleAgentController self, BattleAgentController enemy);

	}
}
