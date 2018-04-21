using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DragonBones;

	public class MapMonster : MapWalkableEvent {

		public MonsterAlertArea[] alertAreas;

		public SpriteRenderer alertToFightIcon;

		public LayerMask collisionLayer;

		private float alertIconOffsetX;
		private float alertIconOffsetY;

		public bool isReadyToFight;

		public bool isBoss;

		// 触发机关的位置【如果没有触发的机关，则设置为（-1，-1，-1）】
		public Vector3 pairEventPos;

		private int mapHeight;


		public void SetPosTransferSeed(int mapHeight){
			this.mapHeight = mapHeight;
		}


		protected override void Awake ()
		{
			base.Awake ();
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


		public override void ResetWhenDie(){

			StopAllCoroutines ();
			HideAllAlertAreas ();
			alertToFightIcon.enabled = false;

		}


		public void QuitFightAndDelayMove(int delay){

			StopCoroutine ("DelayedMovement");

			StartCoroutine ("DelayedMovement",delay);

		}


		private IEnumerator DelayedMovement(int delay){

			HideAllAlertAreas ();

			yield return new WaitForSeconds (delay);

			StartMove ();

		}


		public void ShowAlertAreaTint(){
			MyTowards towards = baCtr.towards;
			for (int i = 0; i < alertAreas.Length; i++) {
				if (i == (int)towards) {
					alertAreas [i].ShowAlerAreaTint ();
				} else {
					alertAreas [i].HideAlertAreaTint ();
				}
			}
		}

		public void DisableAllDetect(){

			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].DisableAlertDetect ();
			}

			bc2d.enabled = false;

		}

		/// <summary>
		/// 隐藏所有的警示区域
		/// </summary>
		public void HideAllAlertAreas(){
			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].HideAlertAreaTint ();
			}
		}




		public void OnTriggerEnter2D (Collider2D col){

			BattlePlayerController bp = col.GetComponent<BattlePlayerController> ();

			if (bp == null) {
				return;
			}

			if (bp.isInEvent) {
				return;
			}

			if (bp.isInFight) {
				return;
			}

			isReadyToFight = true;

			EnterMapEvent (bp);
		}

		public override void EnterMapEvent (BattlePlayerController bp)
		{
			ExploreManager.Instance.AllWalkableEventsStopMove ();

			StopMoveImmidiately ();

//			DisableAllDetect ();

			bp.StopMoveAndWait ();

			ExploreManager.Instance.EnterFight (this.transform);

			MapEventTriggered (false, bp);
		}

		public void DetectPlayer(BattlePlayerController bp){

			ExploreManager.Instance.DisableInteractivity ();

//			DisableAllDetect ();

			ExploreManager.Instance.AllWalkableEventsStopMove ();

			StopMoveImmidiately ();

			bp.StopMoveAtEndOfCurrentStep ();

			ExploreManager.Instance.EnterFight (this.transform);

			MapEventTriggered (false, bp);

		}

		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			BattleMonsterController baCtr = transform.GetComponent<BattleMonsterController> ();

			canMove = bool.Parse(KVPair.GetPropertyStringWithKey("canMove",attachedInfo.properties));

			string pairEventPosString = KVPair.GetPropertyStringWithKey ("pairEventPos", attachedInfo.properties);

			if (pairEventPosString != string.Empty && pairEventPosString != "-1") {

				string[] posXY = pairEventPosString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);

				int posX = int.Parse (posXY [0]);
				int posY = mapHeight - int.Parse (posXY [1]) - 1;

				pairEventPos = new Vector3 (posX, posY, transform.position.z);
			} else {
				pairEventPos = -Vector3.one;
			}

			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].InitializeAlertArea ();
			}

			HideAllAlertAreas ();

			if (canMove) {
				ShowAlertAreaTint ();
			}

			transform.position = attachedInfo.position;

			gameObject.SetActive (true);

			GetComponent<Monster> ().ResetBattleAgentProperties (true);

			baCtr.SetAlive();

			RandomTowards ();

			StartMove ();

			bc2d.enabled = true;
			isReadyToFight = false;

			isTriggered = false;

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
			

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{

			if (isTriggered) {
				return;
			}

			bp.isInEvent = true;

			bc2d.enabled = false;

			if (!isReadyToFight) {

				ExploreManager.Instance.PlayerStartFight ();
			}

			StartCoroutine ("AlertToFightIconShining");
			StartCoroutine ("ResetPositionAndStartFight", bp);

			isTriggered = true;

		}
			

		private IEnumerator ResetPositionAndStartFight(BattlePlayerController battlePlayerCtr){

			yield return new WaitUntil (() => isReadyToFight);

			baCtr.PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);

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

			HLHRoleAnimInfo playerCurrentAnimInfo = battlePlayerCtr.GetCurrentRoleAnimInfo ();

			if (posOffsetX > 0) {
				
				battlePlayerCtr.TowardsLeft ();
//				baCtr.TowardsRight ();

				if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX - 1, playerPosY] == 1) {
					monsterFightPos = new Vector3 (playerPosX - 1, playerPosY, 0);
				} else if (playerPosX - 1 == monsterPosX && playerPosY == monsterPosY) {
					monsterFightPos = new Vector3 (playerPosX - 1, playerPosY, 0);
				} else {
					monsterFightPos = new Vector3 (playerPosX - 0.45f,playerPosY, 0);
					battlePlayerCtr.transform.position = new Vector3 (playerPosX + 0.3f, playerPosY, 0);
				}

			} else if (posOffsetX == 0) {

				if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {

//					baCtr.TowardsLeft ();

					battlePlayerCtr.TowardsRight ();

					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);

				} else if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX - 1, playerPosY] == 1) {

//					baCtr.TowardsRight ();

					battlePlayerCtr.TowardsLeft ();

					monsterFightPos = new Vector3 (playerPosX - 1, playerPosY, 0);

				} else {

//					baCtr.TowardsLeft ();

					battlePlayerCtr.TowardsRight ();

					monsterFightPos = new Vector3 (playerPosX + 0.45f, playerPosY, 0);
					battlePlayerCtr.transform.position = new Vector3 (playerPosX - 0.4f, playerPosY, 0);

				}

			} else if (posOffsetX < 0) {

				battlePlayerCtr.TowardsRight ();
//				baCtr.TowardsLeft ();

				if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {

					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);

				} else if (playerPosX + 1 == monsterPosX && playerPosY == monsterPosY) {

					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);

				} else {
					monsterFightPos = new Vector3 (playerPosX + 0.45f, playerPosY, 0);
					battlePlayerCtr.transform.position = new Vector3 (playerPosX - 0.4f, playerPosY, 0);

				}

			}

			battlePlayerCtr.PlayRoleAnimByTime (playerCurrentAnimInfo.roleAnimName,playerCurrentAnimInfo.roleAnimTime,
				playerCurrentAnimInfo.playTimes, playerCurrentAnimInfo.animEndCallback);
				
			RunToPosition (monsterFightPos, delegate {
				
				this.transform.position = monsterFightPos;

				if(transform.position.x <= ExploreManager.Instance.battlePlayerCtr.transform.position.x){
					baCtr.TowardsRight();
				}else{
					baCtr.TowardsLeft();
				}

				if (!battlePlayerCtr.isInFight) {
					ExploreManager.Instance.PlayerAndMonsterStartFight ();
					battlePlayerCtr.isInFight = true;
				} else {
					ExploreManager.Instance.MonsterStartFight ();
				}
			});

		}


		public override void WalkToPosition(Vector3 position,CallBack cb,bool showAlertArea = true){

			baCtr.PlayRoleAnim (CommonData.roleWalkAnimName, 0, null);

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

			if (targetPosY == oriPosY) {
				if (targetPosX >= oriPosX) {
					baCtr.TowardsRight ();
					alertToFightIcon.transform.localPosition = new Vector3 (alertIconOffsetX, alertIconOffsetY, 0);
				} else {
					baCtr.TowardsLeft ();
					alertToFightIcon.transform.localPosition = new Vector3 (-alertIconOffsetX, alertIconOffsetY, 0);
				}
			} 
			else if(targetPosX == oriPosX){

				if (targetPosY >= oriPosY) {
					baCtr.TowardsUp ();
				} else {
					baCtr.TowardsDown ();
				}
			}

			if (moveCoroutine != null) {
				StopCoroutine (moveCoroutine);
			}

			float timeScale = 3f;

//			if (position.x >= transform.position.x) {
//				baCtr.TowardsRight ();
//			} else {
//				baCtr.TowardsLeft ();
//			}

			if (showAlertArea) {
				ShowAlertAreaTint ();
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



		protected override void RunToPosition(Vector3 position,CallBack cb){

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

				this.transform.position = position;

				baCtr.PlayRoleAnim(CommonData.roleIdleAnimName,0,null);

				if(cb != null){
					cb();
				}

				SetSortingOrder (-Mathf.RoundToInt (position.y));
			});

			StartCoroutine (moveCoroutine);

		}


		public override void SetSortingOrder (int order)
		{
			baCtr.SetSortingOrder (order);
		}
			

		public Item GenerateRandomRewardItem(){

			Item rewardItem = null;

			int randomSeed = Random.Range (0, 100);

			if (randomSeed >= 0 && randomSeed < 90) {
				rewardItem = null;
			} else {

				randomSeed = Random.Range (0, 2);

				if (randomSeed == 0) {
					int index = 0;
					if (!isBoss) {
						index = Player.mainPlayer.currentLevelIndex / 5 + 1;
					} else {
						index = (Player.mainPlayer.currentLevelIndex / 5 + 1) * 10;
					}
					List<EquipmentModel> ems = GameManager.Instance.gameDataCenter.allEquipmentModels.FindAll (delegate(EquipmentModel obj) {
						return obj.equipmentGrade == index;
					});
					randomSeed = Random.Range (0, ems.Count);
					rewardItem = new Equipment (ems [randomSeed], 1);
				} else {
					randomSeed = Random.Range (300, 316);
					rewardItem = Item.NewItemWith (randomSeed, 1);
				}

			}

			return rewardItem;
		}

	}
}
