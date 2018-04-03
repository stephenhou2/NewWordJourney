using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	public class MapMonster : MapWalkableEvent {

//		private BattleMonsterController baCtr;

		public MonsterAlertArea[] alertAreas;

		public SpriteRenderer alertToFightIcon;

//		private IEnumerator moveCoroutine;

		public LayerMask collisionLayer;

		private float alertIconOffsetX;
		private float alertIconOffsetY;

		public bool isReadyToFight;

//		private bool canMove;

//		private bool isInAutoWalk;

		// 掉落物品的ID
		public int dropItemID;

		// 掉落物品的概率
		public float dropItemProbability;

		protected override void Awake ()
		{
			base.Awake ();
//			baCtr = GetComponent<BattleMonsterController> ();
			alertIconOffsetX = alertToFightIcon.transform.localPosition.x;
			alertIconOffsetY = alertToFightIcon.transform.localPosition.y;
		}

		public override void AddToPool (InstancePool pool)
		{
			StopMoveImmidiately ();
			StopCoroutine ("DelayedMovement");
			bc2d.enabled = false;
			isReadyToFight = false;
			HideAllAlertAreas ();
			gameObject.SetActive (false);
			pool.AddInstanceToPool (this.gameObject);
			ExploreManager.Instance.newMapGenerator.allWalkableEventsInMap.Remove (this);
		}

		public void QuitFightAndDelayMove(int delay){

			StopCoroutine ("DelayedMovement");

			StartCoroutine ("DelayedMovement",delay);

		}

		public void ResetWhenDie(){

			StopAllCoroutines ();
			HideAllAlertAreas ();
			alertToFightIcon.enabled = false;

		}

		private IEnumerator DelayedMovement(int delay){

			HideAllAlertAreas ();

			yield return new WaitForSeconds (delay);

			StartMove ();

		}
			

		public void OnTriggerEnter2D (Collider2D col){

			BattlePlayerController bp = col.GetComponent<BattlePlayerController> ();

			if (bp == null) {
				return;
			}

			isReadyToFight = true;

			EnterMapEvent (bp);
		}

		public override void EnterMapEvent (BattlePlayerController bp)
		{
			ExploreManager.Instance.AllWalkableEventsStopMove ();

			StopMoveImmidiately ();

			DisableAllDetect ();

			bp.StopMoveAndWait ();

			ExploreManager.Instance.EnterFight (this.transform);

			MapEventTriggered (false, bp);
		}

		public void DetectPlayer(BattlePlayerController bp){

			Vector3 lineCastEnd = Vector3.zero;
			Vector3 lineCastStart = Vector3.zero;
			switch (baCtr.towards) {
			case MyTowards.Up:
				lineCastStart = transform.position + new Vector3 (0, 0.5f, 0);
				lineCastEnd = transform.position + new Vector3 (0, 2.5f, 0);
				break;
			case MyTowards.Down:
				lineCastStart = transform.position + new Vector3 (0, -0.5f, 0);
				lineCastEnd = transform.position + new Vector3 (0, -2.5f, 0);
				break;
			case MyTowards.Left:
				lineCastStart = transform.position + new Vector3 (-0.5f, 0, 0);
				lineCastEnd = transform.position + new Vector3 (-2.5f, 0, 0);
				break;
			case MyTowards.Right:
				lineCastStart = transform.position + new Vector3 (0.5f, 0, 0);
				lineCastEnd = transform.position + new Vector3 (2.5f, 0, 0);
				break;
			}

			RaycastHit2D[] r2ds = Physics2D.LinecastAll (lineCastStart, lineCastEnd ,collisionLayer);

			for (int i = 0; i < r2ds.Length; i++) {

				RaycastHit2D r2d = r2ds [i];

				if (r2d.transform != null) {

					bool isBlocked = CheckIsBlocked (r2d.transform.position,bp.transform.position);

					if (isBlocked) {
						return;
					}
				}
			}
				
			ExploreManager.Instance.DisableInteractivity ();
//			Debug.Log ("禁止点击");

			DisableAllDetect ();

			ExploreManager.Instance.AllWalkableEventsStopMove ();
			StopMoveImmidiately ();

			bp.StopMoveAtEndOfCurrentStep ();

			ExploreManager.Instance.EnterFight (this.transform);

			MapEventTriggered (false, bp);

		}

		private bool CheckIsBlocked(Vector3 collisionPos,Vector3 playerPos){

			bool isBlocked = true;

			int posX = Mathf.RoundToInt (collisionPos.x);
			int posY = Mathf.RoundToInt (collisionPos.y);

			isBlocked = ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [posX, posY] != 1;

			if (posX == Mathf.RoundToInt (transform.position.x)) {
				isBlocked = (transform.position.y - posY) * (playerPos.y - posY) < 0;
			} else if (posY == Mathf.RoundToInt (transform.position.y)) {
				isBlocked = (transform.position.x - posX) * (playerPos.x - posX) < 0;
			}
				
			return isBlocked;

		}

		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			BattleMonsterController baCtr = transform.GetComponent<BattleMonsterController> ();

//			bool canTalk = bool.Parse(KVPair.GetPropertyStringWithKey ("canTalk", attachedInfo.properties));
//			int monsterId = int.Parse (KVPair.GetPropertyStringWithKey ("monsterID", attachedInfo.properties));
			int dropItemId = int.Parse (KVPair.GetPropertyStringWithKey ("dropItemID", attachedInfo.properties));

			canMove = bool.Parse(KVPair.GetPropertyStringWithKey("canMove",attachedInfo.properties));

			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].InitializeAlertArea ();
			}

			HideAllAlertAreas ();

			if (canMove) {
				ShowAlertAreaTint (baCtr.towards);
			}

			transform.position = attachedInfo.position;

			dropItemID = dropItemId;
			gameObject.SetActive (true);


			baCtr.SetAlive();

			RandomTowards ();

			StartMove ();

			bc2d.enabled = true;
			isReadyToFight = false;

		}

		private void RandomTowards(){

			int towardsIndex = Random.Range (0, 4);

			switch (towardsIndex) {
			case 0:
				baCtr.TowardsRight ();
				break;
			case 1:
				baCtr.TowardsLeft ();
				alertToFightIcon.transform.localPosition = new Vector3 (-alertIconOffsetX, alertIconOffsetY, 0);
				break;
			case 2:
				baCtr.TowardsUp ();
				break;
			case 3:
				baCtr.TowardsDown ();
				break;
			}

		}


		/// <summary>
		/// 怪物头上红色感叹号闪烁动画
		/// </summary>
		/// <returns>The to fight icon shining.</returns>
		private IEnumerator AlertToFightIconShining(){

			yield return new WaitUntil (() => isReadyToFight);

			for (int i = 0; i < 3; i++) {

				alertToFightIcon.enabled = true;

				yield return new WaitForSeconds (0.1f);

				alertToFightIcon.enabled = false;

				yield return new WaitForSeconds (0.1f);

			}
		}

		/// <summary>
		/// 根据怪物朝向显示警示区域
		/// </summary>
		/// <param name="towards">Towards.</param>
		private void ShowAlertAreaTint(MyTowards towards){
			for (int i = 0; i < alertAreas.Length; i++) {
				if (i == (int)towards) {
					alertAreas [i].ShowAlerAreaTint ();
				} else {
					alertAreas [i].HideAlertAreaTint ();
				}
			}
		}

		private void DisableAllDetect(){

			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].DisableAlertDetect ();
			}

			bc2d.enabled = false;

		}

		/// <summary>
		/// 隐藏所有的警示区域
		/// </summary>
		private void HideAllAlertAreas(){
			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].HideAlertAreaTint ();
			}
		}

		public override void InitMapItem ()
		{
			throw new System.NotImplementedException ();
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			
			bc2d.enabled = false;

			if (!isReadyToFight) {

				if (bp.towards == MyTowards.Up || bp.towards == MyTowards.Down) {
					bp.TowardsLeft ();
				}

				ExploreManager.Instance.PlayerStartFight ();
			}

			StartCoroutine ("AlertToFightIconShining");
			StartCoroutine ("ResetPositionAndStartFight", bp);

		}
			

		private IEnumerator ResetPositionAndStartFight(BattlePlayerController battlePlayerCtr){

			yield return new WaitUntil (() => isReadyToFight);

			baCtr.PlayRoleAnim ("wait", 0, null);

			yield return new WaitForSeconds (1f);

			HideAllAlertAreas ();

			Vector3 playerOriPos = battlePlayerCtr.transform.position;
			Vector3 monsterOriPos = transform.position;

			int playerPosX = Mathf.RoundToInt (playerOriPos.x);
			int playerPosY = Mathf.RoundToInt (playerOriPos.y);
			int monsterPosX = Mathf.RoundToInt (monsterOriPos.x);
			int monsterPosY = Mathf.RoundToInt (monsterOriPos.y);

			int posOffsetX = playerPosX - monsterPosX; 

			Vector3 monsterFightPos = Vector3.zero;

			if (posOffsetX > 0) {

				battlePlayerCtr.TowardsLeft (false);
				baCtr.TowardsRight ();

				if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX - 1, playerPosY] == 1) {

					monsterFightPos = new Vector3 (playerOriPos.x - 1, playerOriPos.y, 0);

				} else if (playerPosX - 1 == monsterPosX && playerPosY == monsterPosY) {
					monsterFightPos = new Vector3 (playerOriPos.x - 1, playerOriPos.y, 0);
				} else {
					monsterFightPos = new Vector3 (playerOriPos.x - 0.5f, playerOriPos.y, 0);
					battlePlayerCtr.transform.position = new Vector3 (playerOriPos.x + 0.3f, playerOriPos.y, 0);
				}

			} else if (posOffsetX == 0) {

				if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {

					baCtr.TowardsLeft ();

					battlePlayerCtr.TowardsRight (false);

					monsterFightPos = new Vector3 (playerOriPos.x+ 1, playerOriPos.y, 0);

				} else if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX - 1, playerPosY] == 1) {

					baCtr.TowardsRight ();

					battlePlayerCtr.TowardsLeft (false);

					monsterFightPos = new Vector3 (playerOriPos.x - 1, playerOriPos.y, 0);

				} else {

					baCtr.TowardsLeft ();

					battlePlayerCtr.TowardsRight (false);
						
					monsterFightPos = new Vector3 (playerOriPos.x + 0.5f, playerOriPos.y, 0);
					battlePlayerCtr.transform.position = new Vector3 (playerOriPos.x - 0.3f, playerPosY, 0);

				}

			} else if (posOffsetX < 0) {

				battlePlayerCtr.TowardsRight (false);
				baCtr.TowardsLeft ();

				if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {

					monsterFightPos = new Vector3 (playerOriPos.x + 1, playerOriPos.y, 0);

				} else if (playerPosX + 1 == monsterPosX && playerPosY == monsterPosY) {
					
					monsterFightPos = new Vector3 (playerOriPos.x + 1, playerOriPos.y, 0);

				} else {
					monsterFightPos = new Vector3 (playerOriPos.x + 0.5f, playerOriPos.y, 0);
					battlePlayerCtr.transform.position = new Vector3 (playerOriPos.x - 0.3f, playerOriPos.y, 0);

				}

			}

			RunToPosition (monsterFightPos, delegate {
				if (!battlePlayerCtr.isInFight) {
					ExploreManager.Instance.PlayerAndMonsterStartFight ();
					battlePlayerCtr.isInFight = true;
				} else {
					ExploreManager.Instance.MonsterStartFight ();
				}
			});

		}


		public override void WalkToPosition(Vector3 position,CallBack cb,bool showAlertArea = true){

			baCtr.PlayRoleAnim ("walk", 0, null);

			int oriPosX = Mathf.RoundToInt (transform.position.x);
			int oriPosY = Mathf.RoundToInt (transform.position.y);

			int targetPosX = Mathf.RoundToInt (position.x);
			int targetPosY = Mathf.RoundToInt (position.y);

			ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray [oriPosX, oriPosY] = 0;
			ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray [targetPosX, targetPosY] = 1;

			if (targetPosY == oriPosY) {
				if (targetPosX >= oriPosX) {
					baCtr.TowardsRight ();
					alertToFightIcon.transform.localPosition = new Vector3 (alertIconOffsetX, alertIconOffsetY, 0);
				} else {
					baCtr.TowardsLeft ();
					alertToFightIcon.transform.localPosition = new Vector3 (-alertIconOffsetX, alertIconOffsetY, 0);
				}
			} else if(targetPosX == oriPosX){

				if (targetPosY >= oriPosY) {
					baCtr.TowardsUp ();
				} else {
					baCtr.TowardsDown ();
				}
			}

			if (showAlertArea) {
				ShowAlertAreaTint (baCtr.towards);
			}

			SetSortingOrder (-Mathf.RoundToInt (position.y));

			if (moveCoroutine != null) {
				StopCoroutine (moveCoroutine);
			}

			moveCoroutine = MoveTo (position,3f,delegate{
				baCtr.PlayRoleAnim("wait",0,null);
				ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [oriPosX, oriPosY] = 1;
				ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [targetPosX, targetPosY] = 2;
				if(cb != null){
					cb();
				}
			});

			StartCoroutine (moveCoroutine);

		}



		protected override void RunToPosition(Vector3 position,CallBack cb){

			baCtr.PlayRoleAnim ("run", 0, null);

			int oriPosX = Mathf.RoundToInt (transform.position.x);
			int oriPosY = Mathf.RoundToInt (transform.position.y);

			int targetPosX = Mathf.RoundToInt (position.x);
			int targetPosY = Mathf.RoundToInt (position.y);

			ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray [oriPosX, oriPosY] = 0;
			ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray [targetPosX, targetPosY] = 1;

			SetSortingOrder (-Mathf.RoundToInt (position.y));

			moveCoroutine = MoveTo (position,1f,delegate{
				baCtr.PlayRoleAnim("wait",0,null);
				if(cb != null){
					cb();
				}
			});

			StartCoroutine (moveCoroutine);

		}


		public override void SetSortingOrder (int order)
		{
			baCtr.SetSortingOrder (order);
		}
			

	}
}
