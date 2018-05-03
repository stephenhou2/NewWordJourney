﻿using System.Collections;
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
			DisableAllDetect ();
			isReadyToFight = false;
			HideAllAlertAreas ();
			bc2d.enabled = false;
			gameObject.SetActive (false);
			pool.AddInstanceToPool (this.gameObject);
			ExploreManager.Instance.newMapGenerator.allWalkableEventsInMap.Remove (this);
		}


		public override void ResetWhenDie(){

			StopAllCoroutines ();
			HideAllAlertAreas ();
			alertToFightIcon.enabled = false;

		}


		public override void QuitFightAndDelayMove(int delay){

			StopMoveImmidiately ();

			isInAutoWalk = false;
			isTriggered = false;
			isReadyToFight = false;

			HideAllAlertAreas ();

			bc2d.enabled = true;

			StopCoroutine ("DelayedMovement");

			StartCoroutine ("DelayedMovement",delay);

		}


		private IEnumerator DelayedMovement(int delay){

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

			if (baCtr.isDead) {
				return;
			}

			if (bp.isInPosFixAfterFight) {
				return;
			}


				
			isReadyToFight = true;

			EnterMapEvent (bp);
		}

		public override void EnterMapEvent (BattlePlayerController bp)
		{

			if (isInMoving) {
				RefreshWalkableInfoWhenTriggeredInMoving ();
			}

			bp.isInEvent = true;

			ExploreManager.Instance.AllWalkableEventsStopMove ();

			StopMoveImmidiately ();

			bp.StopMoveAndWait ();

			ExploreManager.Instance.EnterFight (this.transform);

			MapEventTriggered (false, bp);
		}

		public void DetectPlayer(BattlePlayerController bp){

			if (bp.escapeFromFight) {
				return;
			}

			bp.isInEvent = true;

			ExploreManager.Instance.DisableExploreInteractivity ();

			if (isInMoving) {
				RefreshWalkableInfoWhenTriggeredInMoving ();
			}

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

            RandomTowards();

			if (canMove) {
				ShowAlertAreaTint ();
			}

			transform.position = attachedInfo.position;

			gameObject.SetActive (true);

			GetComponent<Monster> ().ResetBattleAgentProperties (true);

			baCtr.SetAlive();

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

			bp.escapeFromFight = false;
			bp.isInEscaping = false;

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
			DisableAllDetect ();

            Vector3 playerOriPos = battlePlayerCtr.transform.position;
            Vector3 monsterOriPos = transform.position;

            int playerPosX = Mathf.RoundToInt(playerOriPos.x);
            int playerPosY = Mathf.RoundToInt(playerOriPos.y);
            int monsterPosX = Mathf.RoundToInt(monsterOriPos.x);
            int monsterPosY = Mathf.RoundToInt(monsterOriPos.y);

			
            int monsterLayerOrder = -monsterPosY;

		
			int posOffsetX = playerPosX - monsterPosX; 
            int posOffsetY = playerPosY - monsterPosY;


			Vector3 monsterFightPos = Vector3.zero;
            Vector3 playerFightPos = new Vector3(playerPosX, playerPosY, 0);

            int minX = 0;
            int maxX = ExploreManager.Instance.newMapGenerator.columns - 1;

			HLHRoleAnimInfo playerCurrentAnimInfo = battlePlayerCtr.GetCurrentRoleAnimInfo ();

			if (posOffsetX > 0) {
				
                battlePlayerCtr.TowardsLeft();

                if (playerPosX - 1 >= minX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX - 1, playerPosY] == 1) {
					monsterFightPos = new Vector3 (playerPosX - 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
                } else if (playerPosX - 1 >= minX && playerPosX - 1 == monsterPosX && playerPosY == monsterPosY) {
					monsterFightPos = new Vector3 (playerPosX - 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
                } else if (playerPosX - 1 >= minX && playerPosX - 1 == Mathf.RoundToInt (moveDestination.x) && playerPosY == Mathf.RoundToInt (moveDestination.y)) {
                    monsterFightPos = new Vector3(playerPosX - 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
				} else {
                    if (posOffsetY > 0)
                    {

                        monsterFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);
                        monsterLayerOrder = -playerPosY + 1;

                        playerFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY + 1);

                    }
                    else
                    {

                        monsterFightPos = new Vector3(playerPosX - 0.25f, playerPosY + 0.15f, 0);
                        monsterLayerOrder = -playerPosY - 1;

                        playerFightPos = new Vector3(playerPosX + 0.25f, playerPosY - 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY - 1);
                    }
				}

			} else if (posOffsetX == 0) {
                
                if (playerPosX + 1 <= maxX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {

					battlePlayerCtr.TowardsRight ();

					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);

                    monsterLayerOrder = -playerPosY;

                } else if (playerPosX + 1 <= maxX && playerPosX + 1 == Mathf.RoundToInt (moveDestination.x) && playerPosY == Mathf.RoundToInt (moveDestination.y)) {
                    
                    battlePlayerCtr.TowardsRight();

					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);

                    monsterLayerOrder = -playerPosY;
                } else if (playerPosX - 1 >= minX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX - 1, playerPosY] == 1) {

					battlePlayerCtr.TowardsLeft ();

					monsterFightPos = new Vector3 (playerPosX - 1, playerPosY, 0);

                    monsterLayerOrder = -playerPosY;

                } else if (playerPosX - 1 >= minX && playerPosX - 1 == Mathf.RoundToInt (moveDestination.x) && playerPosY == Mathf.RoundToInt (moveDestination.y)) {
                    
					battlePlayerCtr.TowardsLeft ();

					monsterFightPos = new Vector3 (playerPosX - 1, playerPosY, 0);

                    monsterLayerOrder = -playerPosY;
				} else {

                    if(posOffsetY > 0){

                        monsterFightPos = new Vector3(playerPosX + 0.25f, playerPosY - 0.15f, 0);

                        monsterLayerOrder = -playerPosY + 1;

                        playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY + 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY + 1);

                        battlePlayerCtr.TowardsRight();

                    }else{

                        monsterFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);

                        monsterLayerOrder = -playerPosY - 1;

                        playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY - 1);

                        battlePlayerCtr.TowardsRight();


                    }
					
				}

			} else if (posOffsetX < 0) {

				battlePlayerCtr.TowardsRight ();

                if (playerPosX + 1 <= maxX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {
					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
                } else if (playerPosX + 1 <= maxX && playerPosX + 1 == monsterPosX && playerPosY == monsterPosY) {
					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
                } else if (playerPosX + 1 <= maxX && playerPosX + 1 == Mathf.RoundToInt (moveDestination.x) && playerPosY == Mathf.RoundToInt (moveDestination.y)) {
					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
				} else {
                    if (posOffsetY > 0)
                    {

                        monsterFightPos = new Vector3(playerPosX + 0.25f, playerPosY - 0.15f, 0);

                        monsterLayerOrder = -playerPosY + 1;

                        playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY + 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY + 1);

                    }
                    else
                    {

                        monsterFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);

                        monsterLayerOrder = -playerPosY - 1;

                        playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY - 1);
                    }
				}
			}

			if (battlePlayerCtr.isInFight) {
				battlePlayerCtr.PlayRoleAnimByTime (playerCurrentAnimInfo.roleAnimName, playerCurrentAnimInfo.roleAnimTime,
					playerCurrentAnimInfo.playTimes, playerCurrentAnimInfo.animEndCallback);
			}

            battlePlayerCtr.FixPosTo(playerFightPos, 0.1f, null);
				
			RunToPosition (monsterFightPos, delegate {

				if(!battlePlayerCtr.escapeFromFight){
					
					this.transform.position = monsterFightPos;

					if(transform.position.x <= ExploreManager.Instance.battlePlayerCtr.transform.position.x){
						baCtr.TowardsRight();
					}else{
						baCtr.TowardsLeft();
					}

					if (!battlePlayerCtr.isInEscaping && !battlePlayerCtr.isInFight) {
						ExploreManager.Instance.PlayerAndMonsterStartFight ();
						battlePlayerCtr.isInFight = true;
					} else {
						ExploreManager.Instance.MonsterStartFight ();
					}
				}else{
					bool monsterDie = baCtr.agent.health <= 0;
					RefreshWalkableInfoWhenQuit(monsterDie);
					QuitFightAndDelayMove(5);
					battlePlayerCtr.escapeFromFight = false;
				}
            },monsterLayerOrder);

		}


		public override void WalkToPosition(Vector3 position,CallBack cb,bool showAlertArea = true){

			baCtr.PlayRoleAnim (CommonData.roleWalkAnimName, 0, null);

			int oriPosX = Mathf.RoundToInt (transform.position.x);
			int oriPosY = Mathf.RoundToInt (transform.position.y);

			int targetPosX = Mathf.RoundToInt (position.x);
			int targetPosY = Mathf.RoundToInt (position.y);


			moveOrigin = new Vector3 (oriPosX, oriPosY, 0);
            moveDestination = new Vector3(targetPosX, targetPosY, 0);

			RefreshWalkableInfoWhenStartMove ();

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



		protected override void RunToPosition(Vector3 position,CallBack cb,int layerOrder){

			baCtr.PlayRoleAnim (CommonData.roleRunAnimName, 0, null);

			int oriPosX = Mathf.RoundToInt (transform.position.x);
			int oriPosY = Mathf.RoundToInt (transform.position.y);

			int targetPosX = Mathf.RoundToInt (position.x);
			int targetPosY = Mathf.RoundToInt (position.y);


			moveOrigin = new Vector3 (oriPosX, oriPosY, 0);
			moveDestination = new Vector3 (targetPosX, targetPosY, 0);

//			Debug.LogFormat ("MOVE ORIGIN:{0}++++++MOVE DESTINATION:{1}", moveOrigin, moveDestination);

			RefreshWalkableInfoWhenStartMove ();

			float timeScale = 1f;

			if (position.x >= transform.position.x + 0.2f) {
				baCtr.TowardsRight ();
			} else if(position.x <= transform.position.x - 0.2f){
				baCtr.TowardsLeft ();
			}


			moveCoroutine = MoveTo (position,timeScale,delegate{

				this.transform.position = position;

				baCtr.PlayRoleAnim(CommonData.roleIdleAnimName,0,null);

				if(cb != null){
					cb();
				}

                SetSortingOrder (layerOrder);
			});

			StartCoroutine (moveCoroutine);

		}


		public override void SetSortingOrder (int order)
		{
			baCtr.SetSortingOrder (order);
		}
			

		
	}
}
