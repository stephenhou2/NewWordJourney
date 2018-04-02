using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	
	public abstract class PermanentPassiveSkill : Skill {


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
