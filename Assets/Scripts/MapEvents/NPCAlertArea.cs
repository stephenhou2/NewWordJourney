using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DragonBones;

	public class NPCAlertArea : MonoBehaviour {


		public MapNPC mapNPC;

		private UnityArmatureComponent alertAreaTint;
		private MeshRenderer mr;
		private EdgeCollider2D ec2D;


		public void InitializeAlertArea(){
			alertAreaTint = GetComponent<UnityArmatureComponent> ();
			mr = GetComponent<MeshRenderer> ();
			ec2D = GetComponent<EdgeCollider2D> ();

			mr.enabled = false;
			alertAreaTint.enabled = false;

			alertAreaTint.animation.timeScale = 0.2f;
			ec2D.enabled = true;
		}
			

		public void EnableAlertDetect(){
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

			if (bp.isInEvent) {
				return;
			}

			if (bp.isInFight) {
				return;
			}

			if (mapNPC.isInPosFix) {
				return;
			}

			if (bp.isInPosFix) {
				return;
			}

				
			mapNPC.DetectPlayer (bp);


		}

		public void OnTriggerExit2D(Collider2D col){

			BattleAgentController ba = col.GetComponent<BattleAgentController> ();

			if (!(ba is BattlePlayerController)) {
				return;
			}

//			mapNPC.canShowNpcPlane = true;
		}


	}
}
