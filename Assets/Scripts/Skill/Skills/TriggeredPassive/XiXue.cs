using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class XiXue : TriggeredPassiveSkill {

		public float triggerProbability;

		public float healthAbsorbScaler;

		public int fixedHealthAbsorb;

		protected override void HitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			if (isEffective (triggerProbability)) {


				int healthGain = (int)(fixedHealthAbsorb + self.agent.hurtToEnemyFromNormalAttack * healthAbsorbScaler);
                            
                //if(skillSourceValue >= 1){
                //    healthGain = (int)(skillSourceValue);
                //}else{
                //    healthGain = (int)(self.agent.hurtToEnemyFromNormalAttack * skillSourceValue + self.agent.healthRecovery);
                //}
				
				self.AddHealthGainAndShow (healthGain);
				self.UpdateStatusPlane ();
			}
		}

	}
}
