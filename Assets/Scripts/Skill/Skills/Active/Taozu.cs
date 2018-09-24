using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 使用后在1秒后可以脱离战斗，并在<color=orange>技能等级×1+5</color>步内处于隐身状态
	public class Taozu : ActiveSkill {

		public float escapeTime;

		public int fixFadeStep;

		public int fadeStepGainBase;

		public override string GetDisplayDescription()
		{
			int fadeStep = skillLevel * fadeStepGainBase + fixFadeStep;
			return string.Format("使用后在1秒后可以脱离战斗，并在<color=white>(技能等级×1+5)</color><color=red>{0}</color>步内处于隐身状态", fadeStep);
		}

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{          

			self.QuitFight ();

			BattlePlayerController battlePlayer = self as BattlePlayerController;

			battlePlayer.isInEscaping = true;

			ExploreManager.Instance.expUICtr.ShowEscapeBar (escapeTime, delegate {
				EscapeCallBack(self,enemy);
			});
         
		}

		private void EscapeCallBack(BattleAgentController self, BattleAgentController enemy)
		{            
			int fadeStep = fixFadeStep + fadeStepGainBase * skillLevel;

			BattlePlayerController battlePlayer = self as BattlePlayerController;

			if(battlePlayer.agent.health <= 0){
				return;
			}

			int oriFadeStepsLeft = battlePlayer.fadeStepsLeft;

			if (oriFadeStepsLeft == 0){
				self.SetEffectAnim(selfEffectAnimName, null, 0, 0);
			}

			battlePlayer.fadeStepsLeft = Mathf.Max(oriFadeStepsLeft, fadeStep);
			enemy.QuitFight ();

			ExploreManager.Instance.EnableExploreInteractivity ();

			ExploreManager.Instance.currentEnteredMapEvent = null;

			BattlePlayerController bpCtr = self as BattlePlayerController;

			bpCtr.PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);

			bpCtr.escapeFromFight = true;

			bpCtr.isInEscaping = false;

			bpCtr.FixPositionToStandard ();

			MapWalkableEvent mwe = enemy.GetComponent<MapWalkableEvent> ();

			ExploreManager.Instance.MapWalkableEventsStartAction ();

			mwe.RefreshWalkableInfoWhenQuit (enemy.isDead);

			if (!mwe.isInMoving) {
				mwe.QuitFightAndDelayMove (5);
			}
				
			ExploreManager.Instance.expUICtr.QuitFight ();

		}

	}
}
