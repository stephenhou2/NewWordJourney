using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 主动型技能的状态
	public enum ActiveSkillStatus{
		None,//无
		Waiting,//等待上个技能动作结束
        Cooling//冷却状态
	}

    /// <summary>
    /// 主动型技能
    /// </summary>
	public abstract class ActiveSkill : Skill {

        // 自己的技能动画名称
		public string selfRoleAnimName;

		//技能冷却时间
		public float skillCoolenTime;

		// 技能已冷却百分比(转换为x100的整数)
		[HideInInspector] public int coolenPercentage;

		// 技能状态
		[HideInInspector] public ActiveSkillStatus skillStatus;

        // 魔法消耗
		public int manaConsume;


		void Awake(){
			this.skillType = SkillType.Active;
			this.skillStatus = ActiveSkillStatus.None;
			this.coolenPercentage = 0;
		}
        
        /// <summary>
        /// 技能逻辑
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="enemy">Enemy.</param>
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

        /// <summary>
        /// 播放技能特效动画
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="enemy">Enemy.</param>
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
