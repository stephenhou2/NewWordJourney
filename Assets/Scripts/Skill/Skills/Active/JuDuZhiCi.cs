using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class JuDuZhiCi : ActiveSkill {

		public float hurtScaler;

		public int poisonBase;

		public int poisonDuration;

		private IEnumerator poisonCoroutine;

		protected override void ExcuteActiveSkillLogic(BattleAgentController self,BattleAgentController enemy){

			//int hurt = (int)((self.agent.attack + self.agent.armorDecrease / 4) * hurtScaler - enemy.agent.armor / 4);

			int hurt = Mathf.RoundToInt(self.agent.attack /((enemy.agent.armor - self.agent.armorDecrease)/100f + 1));

			enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);

			enemy.UpdateStatusPlane ();

			if (poisonCoroutine != null) {
				StopCoroutine (poisonCoroutine);
			}

			poisonCoroutine = Poison (self, enemy);

			StartCoroutine (poisonCoroutine);

		}

		private IEnumerator Poison(BattleAgentController self,BattleAgentController enemy){

			int count = 0;

			while (count < poisonDuration) {

				int hurt = Mathf.RoundToInt(poisonBase * self.agent.agentLevel * self.agent.poisonHurtScaler) ;

				enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);

				enemy.UpdateStatusPlane ();

				yield return new WaitForSeconds (1.0f);

				count++;
			}

		}

	}
}
