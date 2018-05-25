using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ShenLuTuTeng : TriggeredPassiveSkill {

		public float triggerProbability;

		public float armorAndMagicResistGainScaler;


		protected override void BeHitTriggerCallBack(BattleAgentController self, BattleAgentController enemy)
		{


			if(!isEffective(triggerProbability)){
				return;            
			}

			self.agent.mana -= skillLevel;

			int armorAndMagicResistGain = Mathf.RoundToInt(skillLevel * armorAndMagicResistGainScaler);

			self.agent.armor += armorAndMagicResistGain;

			self.agent.armorChangeFromSkill += armorAndMagicResistGain;

			self.agent.magicResist += armorAndMagicResistGain;

			self.agent.magicResistChangeFromSkill += armorAndMagicResistGain;

			self.UpdateStatusPlane();

		
		}

        


	}
}
