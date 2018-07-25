using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 降低敌人<color=orange>技能等级×2</color>的物理攻击,同时提升自己<color=orange>技能等级×3</color>的物理攻击至战斗结束,可叠加
	public class ChongFengNaHan : ActiveSkill
    {      
		public int enemyAttackDecreaseBase;

		public int selfAttackIncreaseBase;

		public override string GetDisplayDescription()
		{
			int attackDecrease = skillLevel * enemyAttackDecreaseBase;

			int attackGain = skillLevel * selfAttackIncreaseBase;

			return string.Format("降低敌人<color=white>(技能等级×2)</color><color=red>{0}</color>的物理攻击," +
								 "同时提升自己<color=white>(技能等级×3)</color><color=red>{1}</color>的物理攻击至战斗结束,可叠加", attackDecrease, attackGain);
		}

		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{
			int enemyAttackChange = enemyAttackDecreaseBase * skillLevel;
			int selfAttackChange = selfAttackIncreaseBase * skillLevel;

			self.agent.attack += selfAttackChange;
			self.agent.attackChangeFromSkill += selfAttackChange;

			enemy.agent.attack -= enemyAttackChange;
			enemy.agent.attackChangeFromSkill -= enemyAttackChange;

			enemy.AddTintTextToQueue("攻击\n降低");
			self.AddTintTextToQueue("攻击\n提升");

			self.SetEffectAnim(selfEffectAnimName);
			enemy.SetEffectAnim(enemyEffectAnimName);
         
		}
	}
}

