using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class NPCAlertArea : MonoBehaviour {


		public MapNPC mapNPC;

		private BoxCollider2D bc2D;


		public void InitializeAlertArea(){
			bc2D = GetComponent<BoxCollider2D> ();
		}

		public void EnableAlerAreaDetect(){
			bc2D.enabled = true;
		}
			

		public void DisableAlertDetect(){
			bc2D.enabled = false;
		}


		public void OnTriggerEnter2D (Collider2D col)
		{

			BattleAgentController ba = col.GetComponent<BattleAgentController> ();

			if (!(ba is BattlePlayerController)) {
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
