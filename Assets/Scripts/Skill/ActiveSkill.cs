using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public abstract class ActiveSkill : Skill {

		public string selfRoleAnimName;

		//技能冷却时间
		public float skillCoolenTime;

		public int manaConsume;



		void Awake(){
			this.skillType = SkillType.Active;
		}


		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.mana -= manaConsume;
			ExcuteActiveSkillLogic (self, enemy);
		}

		/// <summary>
		/// 主动型技能的逻辑写在这个方法里
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="enemy">Enemy.</param>
		protected abstract void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy);

		public void SetEffectAnims(BattleAgentController self,BattleAgentController enemy){
			if (selfEffectAnimName != string.Empty) {
				self.SetEffectAnim (selfEffectAnimName, null);
			}
			if (enemyEffectAnimName != string.Empty) {
				enemy.SetEffectAnim (enemyEffectAnimName, null);
			}
		}

	}


}
