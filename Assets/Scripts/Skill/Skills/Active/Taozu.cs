using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Taozu : ActiveSkill {

		protected override void ExcuteActiveSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			self.QuitFight ();
			enemy.QuitFight ();

			ExploreManager.Instance.EnableInteractivity ();

			ExploreManager.Instance.currentEnteredMapEvent = null;

			(self as BattlePlayerController).FixPosition ();

			Vector3 monsterFixedPosition = new Vector3 (Mathf.RoundToInt (enemy.transform.position.x),
				                               Mathf.RoundToInt (enemy.transform.position.y),
				                               Mathf.RoundToInt (enemy.transform.position.z));


			ExploreManager.Instance.AllWalkableEventsStartMove ();

			MapMonster mm = enemy.GetComponent<MapMonster> ();

			mm.StopMoveImmidiately ();

			mm.WalkToPosition (monsterFixedPosition,delegate{
				mm.QuitFightAndDelayMove(10);
			},false);
				
			self.PlayRoleAnim ("wait", 0, null);

			ExploreManager.Instance.expUICtr.QuitFight ();

		}
	}
}
