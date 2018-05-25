using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Taozu : ActiveSkill {

		public float escapeTimeBase;
		public float escapeTimeDecreaseBase;

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			float escapeTime = escapeTimeBase - escapeTimeDecreaseBase * skillLevel;

			self.QuitFight ();

			(self as BattlePlayerController).isInEscaping = true;

			ExploreManager.Instance.expUICtr.ShowEscapeBar (escapeTime, delegate {
				EscapeCallBack(self,enemy);
			});
		}

		private void EscapeCallBack(BattleAgentController self, BattleAgentController enemy){

			enemy.QuitFight ();

			ExploreManager.Instance.EnableExploreInteractivity ();

			ExploreManager.Instance.currentEnteredMapEvent = null;

			BattlePlayerController bpCtr = self as BattlePlayerController;

			bpCtr.PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);

			bpCtr.escapeFromFight = true;

			bpCtr.isInEscaping = false;

			bpCtr.FixPositionToStandard ();

			MapWalkableEvent mwe = enemy.GetComponent<MapWalkableEvent> ();

			ExploreManager.Instance.AllWalkableEventsStartMove ();

			mwe.RefreshWalkableInfoWhenQuit (enemy.isDead);

			if (!mwe.isInMoving) {
				mwe.QuitFightAndDelayMove (5);
			}
				
			ExploreManager.Instance.expUICtr.QuitFight ();

		}

	}
}
