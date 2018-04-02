using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class MonsterAlertArea : MonoBehaviour {

		public MapEvent mapMonster;

		private SpriteRenderer alertAreaTint;

		private EdgeCollider2D ec2D;
//		private BoxCollider2D bc2D;


		public void InitializeAlertArea(){
			alertAreaTint = GetComponent<SpriteRenderer> ();
			ec2D = GetComponent<EdgeCollider2D> ();
			ec2D.enabled = false;
		}

		public void ShowAlerAreaTint(){
			alertAreaTint.enabled = true;
			ec2D.enabled = true;
		}

		public void HideAlertAreaTint(){
			alertAreaTint.enabled = false;
			ec2D.enabled = false;
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

			BattlePlayerController bp = ba as BattlePlayerController;

			if (bp.isInFight) {
				return;
			}

			MapMonster mm = mapMonster as MapMonster;
			mm.isReadyToFight = true;



			mm.DetectPlayer (bp);
		}


	}
}
