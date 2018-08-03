using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	// 造成<color=orange>物理攻击+技能等级×30+30</color>点物理伤害，并提升自身<color=orange>技能等级×3</color>的魔法攻击，可叠加
	public class ZuZhouDuCi : ActiveSkill {

		public int fixHurt;

		public int hurtBase;

		public int magicAttackGainBase;

		//      // 中毒后每秒损失的生命值
		//private int poisonBase;

		//public int fixPoisonHurt;

		//      // 中毒伤害系数
		//public float poisonScaler;

		//      // 中毒持续时间
		//public int poisonDuration;

		//private IEnumerator poisonCoroutine;

		//public string poisonEffectName;

		public override string GetDisplayDescription()
		{
			int hurt = Player.mainPlayer.attack + skillLevel * hurtBase + fixHurt;
			int magicalAttackGain = skillLevel * magicAttackGainBase;
			return string.Format("造成<color=white>(物理攻击+技能等级x30+30)</color><color=red>{0}</color>点物理伤害，并提升自身<color=white>(技能等级x3)</color><color=red>{1}</color>的魔法攻击，可叠加", hurt, magicalAttackGain);
		}

		//本次攻击造成额外<color=orange> 技能等级×20+30</color>点魔法伤害,
		//并使敌人中毒,每秒损失<color=orange> 技能等级×1%×物理攻击</color> 的生命, 持续4s
        
		protected override void ExcuteActiveSkillLogic(BattleAgentController self,BattleAgentController enemy){
                 
			int physicalHurt = fixHurt + hurtBase * skillLevel + self.agent.attack;

			int armorCal = enemy.agent.armor - self.agent.armorDecrease/2;

			if (armorCal < -50)
            {
				armorCal = -50;
            }

			physicalHurt = Mathf.RoundToInt(physicalHurt / (armorCal / 100f + 1));

			enemy.AddHurtAndShow(physicalHurt, HurtType.Physical, self.towards);
         
			enemy.PlayShakeAnim();         

			enemy.SetEffectAnim(enemyEffectAnimName);

			int magicAttackGain = magicAttackGainBase * skillLevel;

			self.agent.magicAttack += magicAttackGain;

			self.agent.magicAttackChangeFromSkill += magicAttackGain;

			self.AddTintTextToQueue("魔攻\n提升");

			//enemy.SetEffectAnim(enemyEffectAnimName, null, 0, poisonDuration);
            
			//poisonBase = (int)(self.agent.attack * poisonScaler * skillLevel) + fixPoisonHurt;

   //         // 中毒后的持续伤害
			//if (poisonCoroutine != null) {
			//	StopCoroutine (poisonCoroutine);
			//}

			//poisonCoroutine = Poison (self, enemy);

			//StartCoroutine (poisonCoroutine);

		}

		//private IEnumerator Poison(BattleAgentController self,BattleAgentController enemy){

		//	int count = 0;

		//	//int hurt = poisonBase * skillLevel + self.agent.extraPoisonHurt;

		//	while (count < poisonDuration) {
            
		//		enemy.AddHurtAndShow (poisonBase, HurtType.Physical,self.towards);
                
		//		enemy.CheckFightEnd();
            
		//		enemy.UpdateStatusPlane ();

		//		yield return new WaitForSeconds (1.0f);

		//		count++;
		//	}

		//}

	}
}
