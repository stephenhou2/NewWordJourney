using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{

	using DG.Tweening;

	public class MapNPC : MapWalkableEvent {

		private int npcId;

		[HideInInspector]public HLHNPC npc;
//		private bool hasNpcDataLoaded;

		public NPCAlertArea[] alertAreas;

//		public bool canShowNpcPlane;

		[HideInInspector]public HLHNPCReward fightReward;

		public bool isInPosFix;


		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			DisableAllDetect ();
			StopCoroutine ("DelayedMovement");
			pool.AddInstanceToPool (this.gameObject);
			StopMoveImmidiately ();
			gameObject.SetActive (false);
			pool.AddInstanceToPool (this.gameObject);
			ExploreManager.Instance.newMapGenerator.allWalkableEventsInMap.Remove (this);
		}

		public void SetNpcId(int npcId){
			this.npcId = npcId;
		}

		public void QuitFightAndDelayMove(int delay){

			StopCoroutine ("DelayedMovement");

			StartCoroutine ("DelayedMovement",delay);

		}


		private IEnumerator DelayedMovement(int delay){

			DisableAllDetect ();

			yield return new WaitForSeconds (delay);

			StartMove ();

		}


		public void EnableTalk(){
			bc2d.enabled = true;
		}

		public void DisableAllDetect(){

			DisableAllAlertAreaDetect ();

			bc2d.enabled = false;

		}
			
		private void InitAllAlertAreaDetect(){
			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].InitializeAlertArea ();
			}
		}

		private void DisableAllAlertAreaDetect(){
			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].DisableAlertDetect ();
			}
		}


		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			baCtr.SetSortingOrder (-(int)transform.position.y);

//			npc = GameManager.Instance.gameDataCenter.LoadNpc (npcId);

			npc = GameManager.Instance.gameDataCenter.LoadNpc (4);

//			hasNpcDataLoaded = true;

//			npc.RefreshNPC ();

			GetComponent<Monster> ().InitializeWithMonsterData (npc.monsterData);

			InitAllAlertAreaDetect ();

			gameObject.SetActive (true);

			baCtr.PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);

			bc2d.enabled = true;

			isTriggered = false;

			isInAutoWalk = false;

			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].InitializeAlertArea ();
			}

			StartMove ();
		}
			

		public void OnTriggerEnter2D(Collider2D col){

			BattlePlayerController bp = col.GetComponent<BattlePlayerController> ();

			if (bp == null) {
				return;
			}

			if (bp.isInPosFix) {
				return;
			}

			if (isInPosFix) {
				return;
			}


			MapEventTriggered (false, bp);
		}

		public void OnTriggerExit2D(Collider2D col){

			BattleAgentController ba = col.GetComponent<BattleAgentController> ();

			if (!(ba is BattlePlayerController)) {
				return;
			}
		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			MapEventTriggered (false, bp);
		}

		public void DetectPlayer(BattlePlayerController bp){
			MapEventTriggered (true, bp);
		}

		public override void MapEventTriggered (bool isFromDetect, BattlePlayerController bp)
		{
			if (isTriggered) {
				return;
			}
				
			bp.isInEvent = true;

			DisableAllAlertAreaDetect ();

			ExploreManager.Instance.DisableInteractivity ();

			ExploreManager.Instance.AllWalkableEventsStopMove ();

			StopMoveImmidiately ();

			ExploreManager.Instance.currentEnteredMapEvent = this;

			if (isFromDetect) {
				
				bp.StopMoveAtEndOfCurrentStep ();

				StartCoroutine ("AdjustPositionAndTowards", bp);

			} else {
				bp.StopMoveAndWait ();

				AdjustTowards (bp);
			}

			isTriggered = true;

		}

		public override void ResetWhenDie ()
		{
			StopAllCoroutines ();
			DisableAllDetect ();
		}

		public void EnterFight(BattlePlayerController bp){

			StartCoroutine ("AdjustPositionAndTowardsAndFight", bp);
		}

		private void AdjustTowards(BattlePlayerController battlePlayerCtr){

			switch (battlePlayerCtr.towards) {
			case MyTowards.Up:
				if (battlePlayerCtr.transform.position.x < transform.position.x) {
					baCtr.TowardsLeft ();
					if (battlePlayerCtr.transform.position.x <= transform.position.x - 0.5f) {
						battlePlayerCtr.TowardsRight ();
					} else if (battlePlayerCtr.transform.position.x >= transform.position.x + 0.5f) {
						battlePlayerCtr.TowardsLeft ();
					}
				} else {
					baCtr.TowardsRight ();
					if (battlePlayerCtr.transform.position.x <= transform.position.x - 0.5f) {
						battlePlayerCtr.TowardsRight ();
					} else if (battlePlayerCtr.transform.position.x >= transform.position.x + 0.5f) {
						battlePlayerCtr.TowardsLeft ();
					}
				}
				break;
			case MyTowards.Down:
				if (battlePlayerCtr.transform.position.x < transform.position.x) {
					baCtr.TowardsLeft ();
					if (battlePlayerCtr.transform.position.x <= transform.position.x - 0.5f) {
						battlePlayerCtr.TowardsRight ();
					} else if (battlePlayerCtr.transform.position.x >= transform.position.x + 0.5f) {
						battlePlayerCtr.TowardsLeft ();
					}
				} else {
					baCtr.TowardsRight ();
					if (battlePlayerCtr.transform.position.x <= transform.position.x - 0.5f) {
						battlePlayerCtr.TowardsRight ();
					} else if (battlePlayerCtr.transform.position.x >= transform.position.x + 0.5f) {
						battlePlayerCtr.TowardsLeft ();
					}
				}
				break;
			case MyTowards.Left:
				baCtr.TowardsRight ();
				break;
			case MyTowards.Right:
				baCtr.TowardsLeft ();
				break;
			}

			ExploreManager.Instance.ShowNPCPlane (this);

		}

		private IEnumerator AdjustPositionAndTowardsAndFight(BattlePlayerController battlePlayerCtr){

			yield return new WaitUntil (()=>battlePlayerCtr.isIdle);

				Vector3 playerOriPos = battlePlayerCtr.transform.position;
				Vector3 npcOriPos = transform.position;

				int playerPosX = Mathf.RoundToInt (playerOriPos.x);
				int playerPosY = Mathf.RoundToInt (playerOriPos.y);
				int npcPosX = Mathf.RoundToInt (npcOriPos.x);
				int npcPosY = Mathf.RoundToInt (npcOriPos.y);

				int posOffsetX = playerPosX - npcPosX; 

				Vector3 npcFightPos = Vector3.zero;

				if (posOffsetX > 0) {

					battlePlayerCtr.TowardsLeft (false);
					baCtr.TowardsRight ();

					if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX - 1, playerPosY] == 1) {

						npcFightPos = new Vector3 (playerOriPos.x - 1, playerPosY, 0);

					} else if (playerPosX - 1 == npcPosX && playerPosY == npcPosY) {
						npcFightPos = new Vector3 (playerOriPos.x - 1, playerPosY, 0);
					} else {
						npcFightPos = new Vector3 (playerOriPos.x - 0.4f, playerPosY, 0);
						battlePlayerCtr.transform.position = new Vector3 (playerOriPos.x + 0.3f, playerPosY, 0);
					}

				} else if (posOffsetX == 0) {

					if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {

						baCtr.TowardsLeft ();

						battlePlayerCtr.TowardsRight (false);

						npcFightPos = new Vector3 (playerOriPos.x + 1, playerPosY, 0);

					} else if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX - 1, playerPosY] == 1) {

						baCtr.TowardsRight ();

						battlePlayerCtr.TowardsLeft (false);

						npcFightPos = new Vector3 (playerOriPos.x - 1, playerPosY, 0);

					} else {

						baCtr.TowardsLeft ();

						battlePlayerCtr.TowardsRight (false);

						npcFightPos = new Vector3 (playerOriPos.x + 0.4f, playerPosY, 0);
						battlePlayerCtr.transform.position = new Vector3 (playerOriPos.x - 0.3f, playerPosY, 0);

					}

				} else if (posOffsetX < 0) {

					battlePlayerCtr.TowardsRight(false);
					baCtr.TowardsLeft ();

					if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {

						npcFightPos = new Vector3 (playerOriPos.x + 1, playerPosY, 0);

					} else if (playerPosX + 1 == npcPosX && playerPosY == npcPosY) {

						npcFightPos = new Vector3 (playerOriPos.x + 1, playerPosY, 0);

					} else {
						npcFightPos = new Vector3 (playerOriPos.x + 0.4f, playerPosY, 0);
						battlePlayerCtr.transform.position = new Vector3 (playerOriPos.x - 0.3f, playerPosY, 0);

					}

				}

//				canShowNpcPlane = false;

				RunToPosition (npcFightPos, delegate {
					if(transform.position.x <= ExploreManager.Instance.battlePlayerCtr.transform.position.x){
						baCtr.TowardsRight();
					}else{
						baCtr.TowardsLeft();
					}
					ExploreManager.Instance.EnterFight(this.transform);
					ExploreManager.Instance.PlayerAndMonsterStartFight();
//					DisableAllAlertAreaDetect ();
				});

				
		}
			

		private IEnumerator AdjustPositionAndTowards(BattlePlayerController battlePlayerCtr){

			yield return new WaitUntil (()=>battlePlayerCtr.isIdle);

//			if (!canMove) {
//				
//
//			} else {

				Vector3 playerOriPos = battlePlayerCtr.transform.position;
				Vector3 npcOriPos = transform.position;

				int playerPosX = Mathf.RoundToInt (playerOriPos.x);
				int playerPosY = Mathf.RoundToInt (playerOriPos.y);
				int npcPosX = Mathf.RoundToInt (npcOriPos.x);
				int npcPosY = Mathf.RoundToInt (npcOriPos.y);

				int posOffsetX = playerPosX - npcPosX; 

				Vector3 npcFightPos = Vector3.zero;

				if (posOffsetX > 0) {

					battlePlayerCtr.TowardsLeft (true);
//					baCtr.TowardsRight ();

					if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX - 1, playerPosY] == 1) {

						npcFightPos = new Vector3 (playerPosX - 1, playerOriPos.y, 0);

					} else if (playerPosX - 1 == npcPosX && playerPosY == npcPosY) {
						npcFightPos = new Vector3 (playerPosX - 1, playerOriPos.y, 0);
					} else {
						npcFightPos = new Vector3 (playerPosX - 0.45f, playerOriPos.y, 0);

						battlePlayerCtr.transform.position = new Vector3 (playerPosX + 0.4f, playerOriPos.y, 0);
//						battlePlayerCtr.SetNavigationOrigin (playerOriPos + new Vector3 (1, 0, 0));
					}

				} else if (posOffsetX == 0) {

					if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {

//						baCtr.TowardsLeft ();

						battlePlayerCtr.TowardsRight (true);

						npcFightPos = new Vector3 (playerPosX + 1, playerOriPos.y, 0);

					} else if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX - 1, playerPosY] == 1) {

//						baCtr.TowardsRight ();

						battlePlayerCtr.TowardsLeft (true);

						npcFightPos = new Vector3 (playerPosX - 1, playerOriPos.y, 0);

					} else {

//						baCtr.TowardsLeft ();

						battlePlayerCtr.TowardsRight (true);

						npcFightPos = new Vector3 (playerPosX + 0.45f, playerOriPos.y, 0);
						battlePlayerCtr.transform.position = new Vector3 (playerPosX - 0.4f, playerPosY, 0);
//						battlePlayerCtr.SetNavigationOrigin (playerOriPos + new Vector3 (-1, 0, 0));

					}

				} else if (posOffsetX < 0) {

					battlePlayerCtr.TowardsRight (true);
//					baCtr.TowardsLeft ();

					if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {

						npcFightPos = new Vector3 (playerPosX + 1, playerOriPos.y, 0);

					} else if (playerPosX + 1 == npcPosX && playerPosY == npcPosY) {

						npcFightPos = new Vector3 (playerPosX + 1, playerOriPos.y, 0);

					} else {
						npcFightPos = new Vector3 (playerPosX + 0.45f, playerOriPos.y, 0);
						battlePlayerCtr.transform.position = new Vector3 (playerPosX - 0.4f, playerOriPos.y, 0);
//						battlePlayerCtr.SetNavigationOrigin (playerOriPos + new Vector3 (-1, 0, 0));
					}

				}

				RunToPosition (npcFightPos, delegate {
					if(transform.position.x <= ExploreManager.Instance.battlePlayerCtr.transform.position.x){
						baCtr.TowardsRight();
					}else{
						baCtr.TowardsLeft();
					}
					ExploreManager.Instance.ShowNPCPlane (this);
//					DisableAllAlertAreaDetect();
				});


//			}
		}

		public override void SetSortingOrder (int order)
		{
			baCtr.SetSortingOrder (order);
		}



		public override void WalkToPosition(Vector3 position,CallBack cb,bool showAlertArea = true){

			if (MyTool.ApproximatelySamePosition2D (position, ExploreManager.Instance.battlePlayerCtr.transform.position)) {
				return;
			}

			baCtr.PlayRoleAnim (CommonData.roleWalkAnimName, 0, null);

			int oriPosX = Mathf.RoundToInt (transform.position.x);
			int oriPosY = Mathf.RoundToInt (transform.position.y);

			int targetPosX = Mathf.RoundToInt (position.x);
			int targetPosY = Mathf.RoundToInt (position.y);

			ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray [oriPosX, oriPosY] = 0;
			ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray [targetPosX, targetPosY] = 1;

			ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [oriPosX, oriPosY] = 1;
			ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [targetPosX, targetPosY] = 5;

//			if (targetPosY == oriPosY) {
//				if (targetPosX >= oriPosX) {
//					baCtr.TowardsRight ();
//				} else {
//					baCtr.TowardsLeft ();
//				}
//			} else if(targetPosX == oriPosX){
//
//				if (targetPosY >= oriPosY) {
//					baCtr.TowardsUp ();
//				} else {
//					baCtr.TowardsDown ();
//				}
//			}

			if (moveCoroutine != null) {
				StopCoroutine (moveCoroutine);
			}

			float distance = (position - transform.position).sqrMagnitude;

			if (distance < 0.9f) {
				isInPosFix = true;
			} else {
				isInPosFix = false;
			}

			float timeScale = 3f;

			if (position.x >= transform.position.x) {
				baCtr.TowardsRight ();
			} else {
				baCtr.TowardsLeft ();
			}

			moveCoroutine = MoveTo (position,timeScale,delegate{

				this.transform.position = position;

				baCtr.PlayRoleAnim(CommonData.roleIdleAnimName,0,null);

				if(cb != null){
					cb();
				}
				SetSortingOrder (-Mathf.RoundToInt (position.y));
			});

			StartCoroutine (moveCoroutine);

		}


		protected override void RunToPosition(Vector3 position,CallBack cb){

			if (position.Equals(transform.position)) {

				if(cb != null){
					cb();
				}

				return;
			}
				
			baCtr.PlayRoleAnim (CommonData.roleRunAnimName, 0, null);

			int oriPosX = Mathf.RoundToInt (transform.position.x);
			int oriPosY = Mathf.RoundToInt (transform.position.y);

			int targetPosX = Mathf.RoundToInt (position.x);
			int targetPosY = Mathf.RoundToInt (position.y);

			ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray [oriPosX, oriPosY] = 0;
			ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray [targetPosX, targetPosY] = 1;

			ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [oriPosX, oriPosY] = 1;
			ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [targetPosX, targetPosY] = 5;

			moveOrigin = new Vector3 (oriPosX, oriPosY, 0);
			moveDestination = new Vector3 (targetPosX, targetPosY, 0);

			float timeScale = 1f;

			if (position.x >= transform.position.x) {
				baCtr.TowardsRight ();
			} else {
				baCtr.TowardsLeft ();
			}

			moveCoroutine = MoveTo (position,timeScale,delegate{

				baCtr.PlayRoleAnim(CommonData.roleIdleAnimName,0,null);

				if(cb != null){
					cb();
				}

				SetSortingOrder (-Mathf.RoundToInt (position.y));
			});

			StartCoroutine (moveCoroutine);

		}

	}
}
