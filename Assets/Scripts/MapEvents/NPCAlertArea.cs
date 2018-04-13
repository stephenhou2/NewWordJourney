using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class NPCAlertArea : MonoBehaviour {


		public MapNPC mapNPC;

		private EdgeCollider2D ec2D;


		public void InitializeAlertArea(){
			ec2D = GetComponent<EdgeCollider2D> ();
		}

		public void EnableAlerAreaDetect(){
			ec2D.enabled = true;
		}
			

		public void DisableAlertDetect(){
			ec2D.enabled = false;
		}


		public void OnTriggerEnter2D (Collider2D col)
		{

			BattleAgentController ba = col.GetComponent<BattleAgentController> ();

			if (!(ba is BattlePlayerController)) {
				return;
			}

			if (mapNPC.npc.isExcutor && Player.mainPlayer.robTime <= 0) {
				return;
			}

			BattlePlayerController bp = ba as BattlePlayerController;

			if (bp.isInFight) {
				return;
			}

			mapNPC.MapEventTriggered (true, bp);


		}

		public void OnTriggerExit2D(Collider2D col){

			BattleAgentController ba = col.GetComponent<BattleAgentController> ();

			if (!(ba is BattlePlayerController)) {
				return;
			}

			mapNPC.canShowNpcPlane = true;
		}


	}
}
