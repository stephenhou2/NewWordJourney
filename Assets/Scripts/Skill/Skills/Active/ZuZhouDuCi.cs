using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ZuZhouDuCi : ActiveSkill {

		public int fixHurt;

		public int hurtBase;

        // 中毒后每秒损失的生命值
		public int poisonBase;

        // 中毒持续时间
		public int poisonDuration;

		private IEnumerator poisonCoroutine;

		public string poisonEffectName;

		protected override void ExcuteActiveSkillLogic(BattleAgentController self,BattleAgentController enemy){

            // 原始伤害值
			int hurt = self.agent.attack + fixHurt + hurtBase * skillLevel;

            // 计算护甲和护甲穿透后的伤害值
			hurt = Mathf.RoundToInt(hurt /((enemy.agent.armor - self.agent.armorDecrease)/100f + 1));

			enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);

			enemy.PlayShakeAnim();

            // 中毒后的持续伤害
			if (poisonCoroutine != null) {
				StopCoroutine (poisonCoroutine);
			}

			poisonCoroutine = Poison (self, enemy);

			StartCoroutine (poisonCoroutine);

		}

		private IEnumerator Poison(BattleAgentController self,BattleAgentController enemy){

			int count = 0;

			int hurt = poisonBase * skillLevel + self.agent.extraPoisonHurt;

			while (count < poisonDuration) {

				SetEffectAnims(self, enemy);
            
				enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);

				enemy.SetEffectAnim(poisonEffectName);

				enemy.UpdateStatusPlane ();

				yield return new WaitForSeconds (1.0f);

				count++;
			}

		}

	}
}
