using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Taozu : ActiveSkill {

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			self.QuitFight ();

			(self as BattlePlayerController).isInEscaping = true;

			ExploreManager.Instance.expUICtr.ShowEscapeBar (skillCoolenTime, delegate {
				EscapeCallBack(self,enemy);
			});
		}

		private void EscapeCallBack(BattleAgentController self, BattleAgentController enemy){

			enemy.QuitFight ();

			ExploreManager.Instance.EnableInteractivity ();

			ExploreManager.Instance.currentEnteredMapEvent = null;

			BattlePlayerController bpCtr = self as BattlePlayerController;

			bpCtr.PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);

			bpCtr.escapeFromFight = true;

			bpCtr.isInEscaping = false;

			bpCtr.FixPosition ();

			MapWalkableEvent mwe = enemy.GetComponent<MapWalkableEvent> ();

			ExploreManager.Instance.AllWalkableEventsStartMove ();

			mwe.RefreshWalkableInfoWhenQuit (enemy.agent.isDead);

			if (!mwe.isInMoving) {
				mwe.QuitFightAndDelayMove (5);
			}
				
			ExploreManager.Instance.expUICtr.QuitFight ();

		}

	}
}
