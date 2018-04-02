using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
//	using DG.Tweening;

	public class MapMonster : MapEvent {

		private BattleMonsterController bmCtr;

		public MonsterAlertArea[] alertAreas;

		public SpriteRenderer alertToFightIcon;

//		private Tween moveTweener;

		private IEnumerator moveCoroutine;

		public LayerMask collisionLayer;

		private float alertIconOffsetX;
		private float alertIconOffsetY;

		public bool isReadyToFight;

		private bool canMove;

		private bool isInAutoWalk;

		// 掉落物品的ID
		public int dropItemID;

		// 掉落物品的概率
		public float dropItemProbability;

		protected override void Awake ()
		{
			base.Awake ();
			bmCtr = GetComponent<BattleMonsterController> ();
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
			ExploreManager.Instance.newMapGenerator.allMonstersInMap.Remove (this);
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

		public void StartMove(){
			if (isInAutoWalk) {
				return;
			}
			StartCoroutine ("AutoWalk");
			isInAutoWalk = true;
		}

		public void StopMoveImmidiately(){
//			if (moveTweener != null) {
//				moveTweener.Kill (false);
//			}
			if (moveCoroutine != null) {
				StopCoroutine (moveCoroutine);
			}
			StopCoroutine("AutoWalk");
			isInAutoWalk = false;
		}

		public void StopMoveAtEndOfCurrentMove(){
//			if (moveTweener != null) {
//				moveTweener.Kill (true);
//			}
			StopCoroutine("AutoWalk");
			isInAutoWalk = false;
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
			ExploreManager.Instance.AllMonstersStopMove ();

			StopMoveImmidiately ();

			DisableAllDetect ();

			bp.StopMoveAndWait ();

			ExploreManager.Instance.EnterFight (this.transform);

			MapEventTriggered (false, bp);
		}

		public void DetectPlayer(BattlePlayerController bp){

			Vector3 lineCastEnd = Vector3.zero;
			Vector3 lineCastStart = Vector3.zero;
			switch (bmCtr.towards) {
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

			ExploreManager.Instance.AllMonstersStopMove ();
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
			BattleMonsterController bmCtr = transform.GetComponent<BattleMonsterController> ();

//			bool canTalk = bool.Parse(KVPair.GetPropertyStringWithKey ("canTalk", attachedInfo.properties));
//			int monsterId = int.Parse (KVPair.GetPropertyStringWithKey ("monsterID", attachedInfo.properties));
			int dropItemId = int.Parse (KVPair.GetPropertyStringWithKey ("dropItemID", attachedInfo.properties));

			canMove = bool.Parse(KVPair.GetPropertyStringWithKey("canMove",attachedInfo.properties));

			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].InitializeAlertArea ();
			}

			HideAllAlertAreas ();

			if (canMove) {
				ShowAlertAreaTint (bmCtr.towards);
			}

			transform.position = attachedInfo.position;

			dropItemID = dropItemId;
			gameObject.SetActive (true);


			bmCtr.SetAlive();

			RandomTowards ();

			StartMove ();


			bc2d.enabled = true;
			isReadyToFight = false;

		}

		private void RandomTowards(){

			int towardsIndex = Random.Range (0, 4);

			switch (towardsIndex) {
			case 0:
				bmCtr.TowardsRight ();
				break;
			case 1:
				bmCtr.TowardsLeft ();
				alertToFightIcon.transform.localPosition = new Vector3 (-alertIconOffsetX, alertIconOffsetY, 0);
				break;
			case 2:
				bmCtr.TowardsUp ();
				break;
			case 3:
				bmCtr.TowardsDown ();
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

			bmCtr.PlayRoleAnim ("wait", 0, null);

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
				bmCtr.TowardsRight ();

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

					bmCtr.TowardsLeft ();

					battlePlayerCtr.TowardsRight (false);

					monsterFightPos = new Vector3 (playerOriPos.x+ 1, playerOriPos.y, 0);

				} else if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX - 1, playerPosY] == 1) {

					bmCtr.TowardsRight ();

					battlePlayerCtr.TowardsLeft (false);

					monsterFightPos = new Vector3 (playerOriPos.x - 1, playerOriPos.y, 0);

				} else {

					bmCtr.TowardsLeft ();

					battlePlayerCtr.TowardsRight (false);
						
					monsterFightPos = new Vector3 (playerOriPos.x + 0.5f, playerOriPos.y, 0);
					battlePlayerCtr.transform.position = new Vector3 (playerOriPos.x - 0.3f, playerPosY, 0);

				}

			} else if (posOffsetX < 0) {

				battlePlayerCtr.TowardsRight (false);
				bmCtr.TowardsLeft ();

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


		private IEnumerator AutoWalk(){

			while (true) {

				float standDuration = Random.Range (4.0f, 5.0f);

				float timer = 0;

				while (timer < standDuration) {

					yield return null;

					timer += Time.deltaTime;

				}

				if (CheckCanWalk ()) {
					Walk ();
				}
			}

		}

		private void Walk(){

			int[,] mapWalkableInfo = ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray;
			int[,] mapMonsterInfoArray = ExploreManager.Instance.newMapGenerator.mapMonsterInfoArray;

			Vector3 randomPositionAround = transform.position;

			bool validPositionAround = false;

			while (!validPositionAround) {
				
				randomPositionAround = GetRandomPositionAround (transform.position);
				int posX = Mathf.RoundToInt (randomPositionAround.x);
				int posY = Mathf.RoundToInt (randomPositionAround.y);
				validPositionAround = mapWalkableInfo [posX, posY] == 1 
					&& mapMonsterInfoArray[posX,posY] == 0;
			}

			WalkToPosition (randomPositionAround,null);

		}

		public void WalkToPosition(Vector3 position,CallBack cb,bool showAlertArea = true){

			bmCtr.PlayRoleAnim ("walk", 0, null);

			int oriPosX = Mathf.RoundToInt (transform.position.x);
			int oriPosY = Mathf.RoundToInt (transform.position.y);

			int targetPosX = Mathf.RoundToInt (position.x);
			int targetPosY = Mathf.RoundToInt (position.y);

			ExploreManager.Instance.newMapGenerator.mapMonsterInfoArray [oriPosX, oriPosY] = 0;
			ExploreManager.Instance.newMapGenerator.mapMonsterInfoArray [targetPosX, targetPosY] = 1;

			if (targetPosY == oriPosY) {
				if (targetPosX >= oriPosX) {
					bmCtr.TowardsRight ();
					alertToFightIcon.transform.localPosition = new Vector3 (alertIconOffsetX, alertIconOffsetY, 0);
				} else {
					bmCtr.TowardsLeft ();
					alertToFightIcon.transform.localPosition = new Vector3 (-alertIconOffsetX, alertIconOffsetY, 0);
				}
			} else if(targetPosX == oriPosX){

				if (targetPosY >= oriPosY) {
					bmCtr.TowardsUp ();
				} else {
					bmCtr.TowardsDown ();
				}
			}

			if (showAlertArea) {
				ShowAlertAreaTint (bmCtr.towards);
			}

			SetSortingOrder (-Mathf.RoundToInt (position.y));
		
//			float distance = Mathf.Sqrt ((targetPosX - oriPosX) * (targetPosX - oriPosX) + (targetPosY - oriPosY) * (targetPosY - oriPosY));

			if (moveCoroutine != null) {
				StopCoroutine (moveCoroutine);
			}

			moveCoroutine = MoveTo (position,3f,delegate{
				bmCtr.PlayRoleAnim("wait",0,null);
				ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [oriPosX, oriPosY] = 1;
				ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [targetPosX, targetPosY] = 2;
				if(cb != null){
					cb();
				}
			});

			StartCoroutine (moveCoroutine);

		}



		private void RunToPosition(Vector3 position,CallBack cb){

			bmCtr.PlayRoleAnim ("run", 0, null);

			int oriPosX = Mathf.RoundToInt (transform.position.x);
			int oriPosY = Mathf.RoundToInt (transform.position.y);

			int targetPosX = Mathf.RoundToInt (position.x);
			int targetPosY = Mathf.RoundToInt (position.y);

			ExploreManager.Instance.newMapGenerator.mapMonsterInfoArray [oriPosX, oriPosY] = 0;
			ExploreManager.Instance.newMapGenerator.mapMonsterInfoArray [targetPosX, targetPosY] = 1;

			SetSortingOrder (-Mathf.RoundToInt (position.y));

//			float distance = Mathf.Sqrt ((targetPosX - oriPosX) * (targetPosX - oriPosX) + (targetPosY - oriPosY) * (targetPosY - oriPosY));

			moveCoroutine = MoveTo (position,1f,delegate{
				bmCtr.PlayRoleAnim("wait",0,null);
				if(cb != null){
					cb();
				}
			});

			StartCoroutine (moveCoroutine);

		}

		private IEnumerator MoveTo(Vector3 position,float timeScale,CallBack cb){

			int oriPosX = Mathf.RoundToInt (transform.position.x);
			int oriPosY = Mathf.RoundToInt (transform.position.y);

			int targetPosX = Mathf.RoundToInt (position.x);
			int targetPosY = Mathf.RoundToInt (position.y);

			float distance = Mathf.Sqrt ((targetPosX - oriPosX) * (targetPosX - oriPosX) + (targetPosY - oriPosY) * (targetPosY - oriPosY));

			float moveDuration = bmCtr.moveDuration * distance * timeScale;

//			float moveSpeedX = ;
//
//			float moveSpeedY = ;

			Vector3 moveVector = new Vector3((targetPosX - oriPosX) / moveDuration,(targetPosY - oriPosY) / moveDuration,0);

			float timer = 0;

			while (timer < moveDuration) {

				transform.position += moveVector * Time.deltaTime;

				timer += Time.deltaTime;

				yield return null;

			}

			transform.position = position;

			if (cb != null) {
				cb ();
			}

		}

		public override void SetSortingOrder (int order)
		{
			bmCtr.SetSortingOrder (order);
		}

		private bool CheckCanWalk(){
			int posX = Mathf.RoundToInt (transform.position.x);
			int posY = Mathf.RoundToInt (transform.position.y);
			return IsPositionCanWalk (posX, posY + 1) || IsPositionCanWalk (posX, posY - 1)
			|| IsPositionCanWalk (posX - 1, posY) || IsPositionCanWalk (posX + 1, posY);
		}

		private bool IsPositionCanWalk(int posX,int posY){
			
			int[,] mapWalkableInfo = ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray;
			int[,] mapMonsterLayoutInfo = ExploreManager.Instance.newMapGenerator.mapMonsterInfoArray;
			return mapWalkableInfo [posX, posY] == 1 && mapMonsterLayoutInfo [posX, posY] == 0;
		}

		private Vector3 GetRandomPositionAround(Vector3 position){

			int posX = Mathf.RoundToInt (position.x);
			int posY = Mathf.RoundToInt (position.y);

			int directionSeed = Random.Range (0, 4);

			Vector3 randomPosition = position;

			switch (directionSeed) {
			case 0:
				randomPosition = new Vector3 (posX, posY + 1, position.z);
				break;
			case 1:
				randomPosition = new Vector3 (posX, posY - 1, position.z);
				break;
			case 2:
				randomPosition = new Vector3 (posX - 1, posY, position.z);
				break;
			case 3:
				randomPosition = new Vector3 (posX + 1, posY, position.z);
				break;

			}

			return randomPosition;

		}

	}
}
