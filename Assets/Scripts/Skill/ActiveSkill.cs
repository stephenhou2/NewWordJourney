using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public enum ActiveSkillStatus{
		None,
		Waiting,
        Cooling
	}

	public abstract class ActiveSkill : Skill {

		public string selfRoleAnimName;

		//技能冷却时间
		public float skillCoolenTime;

		// 技能已冷却百分比(转换为x100的整数)
		[HideInInspector] public int coolenPercentage;

		// 技能状态
		[HideInInspector] public ActiveSkillStatus skillStatus;

		public int manaConsume;

		//public bool hasTriggered;
        

		void Awake(){
			this.skillType = SkillType.Active;
			this.skillStatus = ActiveSkillStatus.None;
			this.coolenPercentage = 0;
		}


		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteActiveSkillLogic (self, enemy);
			//coolenPercentage = 0;
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
