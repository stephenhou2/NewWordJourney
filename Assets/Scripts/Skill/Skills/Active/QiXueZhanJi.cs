using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 造成<color=orange>物理攻击+技能等级×25+15</color>点物理伤害,并将所造成伤害的40%转化为自身生命
	public class QiXueZhanJi : ActiveSkill {

		public int fixHurt;
		public int hurtBase;
		public float absorbScaler;

		public override string GetDisplayDescription()
		{
			int hurt = Player.mainPlayer.attack + skillLevel * hurtBase + fixHurt;
			return string.Format("造成<color=white>(物理攻击+技能等级×25+15)</color><color=red>{0}</color>点物理伤害,并将所造成伤害的40%转化为自身生命", hurt);
		}

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			// 计算原始伤害值
			int hurt = self.agent.attack + fixHurt + hurtBase * skillLevel;

			int armorCal = enemy.agent.armor - self.agent.armorDecrease;

			if (armorCal < -50)
            {
                armorCal = -50;
            }
            // 结算护甲和护甲穿透后的伤害
			hurt = Mathf.RoundToInt(hurt / (armorCal / 100f + 1));
            // 吸血值
			int healthGain = (int)(hurt * absorbScaler + self.agent.healthRecovery);

            // 伤害生效
			enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);
            // 生命回复生效 
			self.AddHealthGainAndShow (healthGain);

			enemy.SetEffectAnim(enemyEffectAnimName);


		}
	}
}