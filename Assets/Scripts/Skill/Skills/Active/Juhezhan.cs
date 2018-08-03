using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 本次攻击提升30%暴击率,并造成<color=orange>物理攻击+技能等级×20+10</color>点物理伤害，该技能可触发暴击
	public class Juhezhan : ActiveSkill {
    
        // 暴击概率提升值
		public float critScalerGain;
        // 技能的固定额外伤害
		public int fixHurt;
        // 技能虽等级造成的额外伤害基数
		public int hurtBase;
        
		public override string GetDisplayDescription(){
			int hurt = Player.mainPlayer.attack + skillLevel * hurtBase + fixHurt;
			return string.Format("本次攻击提升30%暴击率,并造成<color=white>(物理攻击+技能等级×20+10)</color><color=red>{0}</color>点物理伤害，该技能可触发暴击", hurt);
		}

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
            // 提升暴击概率
			float crit = self.agent.crit + critScalerGain;

            // 普通攻击的伤害
			int hurt = self.agent.attack;

            // 判断是否暴击，暴击后伤害提升为暴击伤害
			if (isEffective(crit))
            {
                hurt = (int)(hurt * self.agent.critHurtScaler);
                enemy.AddTintTextToQueue("暴击");
            }

            // 加上技能产生的额外伤害
			hurt += hurtBase * skillLevel + fixHurt;
         
			int armorCal = enemy.agent.armor - self.agent.armorDecrease/2;

            if (armorCal < -50)
            {
                armorCal = -50;
            }

            // 与对方护甲进行结算后得出实际伤害
			hurt = Mathf.RoundToInt(hurt / (armorCal / 100f + 1));

            // 伤害生效并显示
			enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);

            // 敌方做被击中的后退动作
			enemy.PlayShakeAnim ();

			enemy.SetEffectAnim(enemyEffectAnimName);
		}      
	}
}
