using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
 
	public abstract class TriggeredPassiveSkill : Skill {


		//public bool canOverlay;// 技能效果是否可以叠加


		public bool beforeFightTrigger;
		public bool attackTrigger;
		public bool hitTrigger;
		public bool beAttackTrigger;
		public bool beHitTrigger;
		public bool fightEndTrigger;

		public string statusName;//状态名称

//		public float triggeredProbability;//触发概率

		//public float skillSourceValue;//技能结算数据源

//		public HurtType hurtType;

		//技能持续事件或状态持续时间
		//单次型技能代表技能对应状态的持续时间（如 ： 减少10点护甲，【持续】90秒）
		//连续型技能代表技能持续时间(如 ：每秒损失10点生命，【持续】3秒）
//		public float duration;


		void Awake(){
			this.skillType = SkillType.TriggeredPassive;
		}
			
			

		/// <summary>
		/// 技能作用效果
		/// 【！子类重写AffectAgents方法时，必须调用base.AffectAgents，否则必须重写技能触发回调逻辑！】
		/// </summary>
		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{

			//BattleAgentController triggerSource = null;

			if (beforeFightTrigger) {
				TriggeredSkillExcutor beforeFightSkillExcutor = new TriggeredSkillExcutor (this, BeforeFightTriggerCallBack);
				self.beforeFightTriggerExcutors.Add (beforeFightSkillExcutor);
			}

			if (attackTrigger) {
				TriggeredSkillExcutor attackSkillExcutor = new TriggeredSkillExcutor (this, AttackTriggerCallBack);
				self.attackTriggerExcutors.Add (attackSkillExcutor);
			}

			if (hitTrigger) {
				TriggeredSkillExcutor hitSkillExcutor = new TriggeredSkillExcutor (this, HitTriggerCallBack);
				self.hitTriggerExcutors.Add (hitSkillExcutor);
			}	

			if (beAttackTrigger) {
				TriggeredSkillExcutor beAttackSkillExcutor = new TriggeredSkillExcutor (this, BeAttackedTriggerCallBack);
				self.beAttackedTriggerExcutors.Add (beAttackSkillExcutor);
			}	

			if (beHitTrigger) {
				TriggeredSkillExcutor beHitSkillExcutor = new TriggeredSkillExcutor (this, BeHitTriggerCallBack);
				self.beHitTriggerExcutors.Add (beHitSkillExcutor);
			}

			if (fightEndTrigger) {
				TriggeredSkillExcutor fightEndSkillExcutor = new TriggeredSkillExcutor (this, FightEndTriggerCallBack);
				self.fightEndTriggerExcutors.Add (fightEndSkillExcutor);
			}	

		}



		protected virtual void BeforeFightTriggerCallBack(BattleAgentController self,BattleAgentController enemy){

		}

		protected virtual void AttackTriggerCallBack(BattleAgentController self,BattleAgentController enemy){

		}

		protected virtual void HitTriggerCallBack(BattleAgentController self,BattleAgentController enemy){

		}

		protected virtual void BeAttackedTriggerCallBack(BattleAgentController self,BattleAgentController enemy){

		}

		protected virtual void BeHitTriggerCallBack(BattleAgentController self,BattleAgentController enemy){

		}

		/// <summary>
		/// 战斗结束后的逻辑回调
		/// 用于重置角色状态，该方法一定会加入战斗结束的回调队列中
		/// 如果不需要重置，重写一个空的方法即可
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="enemy">Enemy.</param>
		protected virtual void FightEndTriggerCallBack (BattleAgentController self, BattleAgentController enemy){
		
		}

		/// <summary>
		/// 技能效果触发后执行的逻辑函数
		/// 该方法必须在战斗的各种状态（进入战斗／攻击动作／攻击命中／被攻击／被攻击命中）的回调中调用才能执行
		/// 也可以不用这个函数，在战斗的各种状态回调中单独描述回调逻辑
		/// </summary>
		/// <param name="triggerInfo">状态触发信息.</param>
		/// <param name="self">攻击方.</param>
		/// <param name="enemy">被攻击方.</param>
		protected virtual void ExcuteTriggeredSkillLogic(BattleAgentController self,BattleAgentController enemy){

		}

		/// <summary>
		/// 强制取消技能效果
		/// </summary>
		/// <returns><c>true</c> if this instance cancel skill effect; otherwise, <c>false</c>.</returns>
		public virtual void CancelSkillEffect (bool removeSkill){

		}



		protected void SetEffectAnims(BattleAgentController self,BattleAgentController enemy){

			if (selfEffectAnimName != string.Empty) {
				self.SetEffectAnim (selfEffectAnimName);
			}
			if (enemyEffectAnimName != string.Empty) {
				enemy.SetEffectAnim (enemyEffectAnimName);
			}


		}
	}
}
