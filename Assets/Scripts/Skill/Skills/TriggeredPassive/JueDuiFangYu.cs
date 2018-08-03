using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 每次受到攻击时，有<color=orange>技能等级×2%+30%</color>的概率对敌人造成30%自身护甲值的真实伤害
	public class JueDuiFangYu : TriggeredPassiveSkill {

		public float fixTriggerProbability;

		public float triggeredProbabilityBase;

		public float refrectScaler;

		public override string GetDisplayDescription()
		{
			int probability = Mathf.RoundToInt((skillLevel * triggeredProbabilityBase + fixTriggerProbability) * 100);
			return string.Format("每次受到攻击时有<color=white>(技能等级×2%+30%)</color><color=red>{0}%</color>的概率对敌人造成30%自身护甲值的真实伤害", probability);
		}

		protected override void BeHitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			float triggeredProbability = fixTriggerProbability + triggeredProbabilityBase * skillLevel;

			if (isEffective (triggeredProbability)) {

				int hurt = Mathf.RoundToInt(self.agent.armor * refrectScaler);

				enemy.AddHurtAndShow (hurt, HurtType.Physical,self.towards);

                //enemy.UpdateStatusPlane();

				SetEffectAnims(self, enemy);

				if (sfxName != string.Empty)
                {
					GameManager.Instance.soundManager.PlayAudioClip(sfxName);
                }
			}

		}
	}
}
