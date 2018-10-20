using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SkillMoLong : ActiveSkill
	{

		public int physicalHurt;
		public int magicalHurt;

		public int durativeHurtBase;
		public int durativeHurtDuration;

		private IEnumerator durativeHurtCoroutine;


		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			int armorCal = enemy.agent.armor - self.agent.armorDecrease/2;

			if (armorCal < -50)
            {
                armorCal = -50;
            }
			int actualPhysicalHurt = Mathf.RoundToInt(physicalHurt / (armorCal/ 100f + 1));

			int magicResistCal = enemy.agent.magicResist - self.agent.magicResistDecrease/2;

			if (magicResistCal < -50)
            {
                magicResistCal = -50;
            }

			int actualMagicalHurt = Mathf.RoundToInt(magicalHurt / (magicResistCal / 100f + 1));

			enemy.AddHurtAndShow(actualPhysicalHurt, HurtType.Physical, self.towards);

			enemy.AddHurtAndShow(actualMagicalHurt, HurtType.Magical, self.towards);

			//enemy.UpdateStatusPlane();

			enemy.PlayShakeAnim();

			enemy.SetEffectAnim(enemyEffectAnimName,null,0,durativeHurtDuration);

			if(durativeHurtCoroutine != null){
				StopCoroutine(durativeHurtCoroutine);
			}

			durativeHurtCoroutine = DurativeHurt(self, enemy);

			StartCoroutine(durativeHurtCoroutine);

           
		}


		private IEnumerator DurativeHurt(BattleAgentController self, BattleAgentController enemy){

			int count = 0;

			while(count < durativeHurtDuration){

				if(enemy != null && !enemy.isDead){
					
					enemy.AddHurtAndShow(durativeHurtBase, HurtType.Physical, self.towards);
					
					bool fightEnd = enemy.CheckFightEnd();

                    enemy.UpdateStatusPlane();

                    if (fightEnd)
                    {
                        yield break;
                    }

					yield return new WaitForSeconds(1.0f);
                }

				count++;

			}


		}

	}
}
