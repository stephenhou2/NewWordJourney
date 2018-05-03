using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DragonBones;

	public class MonsterAlertArea : MonoBehaviour {

		public MapEvent mapMonster;

		private UnityArmatureComponent alertAreaTint;
		private MeshRenderer mr;

		private EdgeCollider2D ec2D;


		public void InitializeAlertArea(){
			alertAreaTint = GetComponent<UnityArmatureComponent> ();
            mr = transform.Find("detect").GetComponent<MeshRenderer> ();
			ec2D = GetComponent<EdgeCollider2D> ();
			alertAreaTint.animation.timeScale = 0.2f;
			ec2D.enabled = false;
		}

		public void ShowAlerAreaTint(){
			mr.enabled = true;
			alertAreaTint.enabled = true;
			alertAreaTint.animation.Play ("default", 0);
			ec2D.enabled = true;
		}

		public void HideAlertAreaTint(){
			alertAreaTint.enabled = false;
			mr.enabled = false;
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

			if (bp.isInEvent) {
				return;
			}

			if (bp.isInFight) {
				return;
			}
				

			MapMonster mm = mapMonster as MapMonster;
			mm.isReadyToFight = true;

			mm.DetectPlayer (bp);
		}


	}
}
