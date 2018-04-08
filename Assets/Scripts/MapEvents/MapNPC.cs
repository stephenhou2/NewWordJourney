using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{

	using DG.Tweening;

	public class MapNPC : MapWalkableEvent {

		[HideInInspector]public HLHNPC npc;
		private bool hasNpcDataLoaded;

		public NPCAlertArea[] alertAreas;

		public bool canShowNpcPlane;

		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			SetSortingOrder (-(int)transform.position.y);
		}

		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			DisableAllAlertAreas ();
			pool.AddInstanceToPool (this.gameObject);
			StopMoveImmidiately ();
			gameObject.SetActive (false);
			pool.AddInstanceToPool (this.gameObject);
			ExploreManager.Instance.newMapGenerator.allWalkableEventsInMap.Remove (this);
		}



		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			baCtr.SetSortingOrder (-(int)transform.position.y);


//				int npcId = int.Parse (KVPair.GetPropertyStringWithKey ("npcID", attachedInfo.properties));

//				int npcId = Random.Range (0, 13);
//				#warning 这里暂时使用id为0的npc作为测试数据

			if (!hasNpcDataLoaded) {
				
				int npcId = 2;

				npc = GameManager.Instance.gameDataCenter.LoadNpc (npcId);

				hasNpcDataLoaded = true;
			}

			if (npc.npcId == 1) {
				InitNpcGoodsFromLevelData ();
			}

			baCtr.PlayRoleAnim ("wait", 0, null);

			bc2d.enabled = true;

			canShowNpcPlane = true;

			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].InitializeAlertArea ();
			}

			StartMove ();
		}


//		private void InitFirstThreeGoodsGroup(string goodsDataString,List<Item> goodsList){
//
//			string[] goodsGroupStrings = goodsDataString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);
//
//			for (int i = 0; i < 3; i++) {
//
//				string[] goodsStrings = goodsGroupStrings [i].Split (new char[]{ '/' }, System.StringSplitOptions.RemoveEmptyEntries);
//
//				int randomSeed = Random.Range (0, goodsStrings.Length);
//
//				int randomGoodsId = int.Parse (goodsStrings [randomSeed]);
//
//				Item itemAsGoods = Item.NewItemWith (randomGoodsId, 1);
//
//				goodsList.Add (itemAsGoods);
//
//			}
//
//		}
//
//		private void InitLastTwoGoodsGroup(string goodsDataString,List<Item> goodsList){
//
//			string[] goodsGroupStrings = goodsDataString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);
//
//			for (int i = 0; i < 2; i++) {
//
//				int randomSeed = Random.Range (0, goodsGroupStrings.Length);
//
//				int randomGoodsId = int.Parse (goodsGroupStrings [randomSeed]);
//
//				Item itemAsGoods = Item.NewItemWith (randomGoodsId, 1);
//
//				goodsList.Add (itemAsGoods);
//
//			}
//
//		}

		private void DisableAllAlertAreas(){
			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].DisableAlertDetect ();
			}
		}

		public void EnableAllAlertAreas(){
			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].EnableAlerAreaDetect ();
			}
		}


		public override void EnterMapEvent(BattlePlayerController bp)
		{
			canShowNpcPlane = true;

			MapEventTriggered (true, bp);

		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			if (!canShowNpcPlane) {
				return;
			}

			ExploreManager.Instance.DisableInteractivity ();

			ExploreManager.Instance.AllWalkableEventsStopMove ();

			StopMoveImmidiately ();

			ExploreManager.Instance.currentEnteredMapEvent = this;

			bp.StopMoveAtEndOfCurrentStep ();

			DisableAllAlertAreas ();

			StartCoroutine ("AdjustPositionAndTowards",bp);

		}

		public override void ResetWhenDie ()
		{
			StopAllCoroutines ();
			DisableAllAlertAreas ();
		}

		public void EnterFight(BattlePlayerController bp){

			StartCoroutine ("AdjustPositionAndTowardsAndFight", bp);
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

						npcFightPos = new Vector3 (playerOriPos.x - 1, playerOriPos.y, 0);

					} else if (playerPosX - 1 == npcPosX && playerPosY == npcPosY) {
						npcFightPos = new Vector3 (playerOriPos.x - 1, playerOriPos.y, 0);
					} else {
						npcFightPos = new Vector3 (playerOriPos.x - 0.5f, playerOriPos.y, 0);
						battlePlayerCtr.transform.position = new Vector3 (playerOriPos.x + 0.3f, playerOriPos.y, 0);
					}

				} else if (posOffsetX == 0) {

					if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {

						baCtr.TowardsLeft ();

						battlePlayerCtr.TowardsRight (false);

						npcFightPos = new Vector3 (playerOriPos.x + 1, playerOriPos.y, 0);

					} else if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX - 1, playerPosY] == 1) {

						baCtr.TowardsRight ();

						battlePlayerCtr.TowardsLeft (false);

						npcFightPos = new Vector3 (playerOriPos.x - 1, playerOriPos.y, 0);

					} else {

						baCtr.TowardsLeft ();

						battlePlayerCtr.TowardsRight (false);

						npcFightPos = new Vector3 (playerOriPos.x + 0.5f, playerOriPos.y, 0);
						battlePlayerCtr.transform.position = new Vector3 (playerOriPos.x - 0.3f, playerPosY, 0);

					}

				} else if (posOffsetX < 0) {

					battlePlayerCtr.TowardsRight (false);
					baCtr.TowardsLeft ();

					if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {

						npcFightPos = new Vector3 (playerOriPos.x + 1, playerOriPos.y, 0);

					} else if (playerPosX + 1 == npcPosX && playerPosY == npcPosY) {

						npcFightPos = new Vector3 (playerOriPos.x + 1, playerOriPos.y, 0);

					} else {
						npcFightPos = new Vector3 (playerOriPos.x + 0.5f, playerOriPos.y, 0);
						battlePlayerCtr.transform.position = new Vector3 (playerOriPos.x - 0.3f, playerOriPos.y, 0);

					}

				}

				RunToPosition (npcFightPos, delegate {
					ExploreManager.Instance.EnterFight(this.transform);
					ExploreManager.Instance.PlayerAndMonsterStartFight();
				});

				canShowNpcPlane = false;

		}


		private IEnumerator AdjustPositionAndTowards(BattlePlayerController battlePlayerCtr){

			yield return new WaitUntil (()=>battlePlayerCtr.isIdle);

			if (!canMove) {
				switch (battlePlayerCtr.towards) {
				case MyTowards.Up:
				case MyTowards.Down:
					break;
				case MyTowards.Left:
					baCtr.TowardsRight ();
					break;
				case MyTowards.Right:
					baCtr.TowardsLeft ();
					break;
				}

				ExploreManager.Instance.ShowNPCPlane (this);
				canShowNpcPlane = false;
			} else {

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

						npcFightPos = new Vector3 (playerOriPos.x - 1, playerOriPos.y, 0);

					} else if (playerPosX - 1 == npcPosX && playerPosY == npcPosY) {
						npcFightPos = new Vector3 (playerOriPos.x - 1, playerOriPos.y, 0);
					} else {
						npcFightPos = new Vector3 (playerOriPos.x - 0.5f, playerOriPos.y, 0);
						battlePlayerCtr.transform.position = new Vector3 (playerOriPos.x + 0.3f, playerOriPos.y, 0);
					}

				} else if (posOffsetX == 0) {

					if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {

						baCtr.TowardsLeft ();

						battlePlayerCtr.TowardsRight (false);

						npcFightPos = new Vector3 (playerOriPos.x + 1, playerOriPos.y, 0);

					} else if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX - 1, playerPosY] == 1) {

						baCtr.TowardsRight ();

						battlePlayerCtr.TowardsLeft (false);

						npcFightPos = new Vector3 (playerOriPos.x - 1, playerOriPos.y, 0);

					} else {

						baCtr.TowardsLeft ();

						battlePlayerCtr.TowardsRight (false);

						npcFightPos = new Vector3 (playerOriPos.x + 0.5f, playerOriPos.y, 0);
						battlePlayerCtr.transform.position = new Vector3 (playerOriPos.x - 0.3f, playerPosY, 0);

					}

				} else if (posOffsetX < 0) {

					battlePlayerCtr.TowardsRight (false);
					baCtr.TowardsLeft ();

					if (ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {

						npcFightPos = new Vector3 (playerOriPos.x + 1, playerOriPos.y, 0);

					} else if (playerPosX + 1 == npcPosX && playerPosY == npcPosY) {

						npcFightPos = new Vector3 (playerOriPos.x + 1, playerOriPos.y, 0);

					} else {
						npcFightPos = new Vector3 (playerOriPos.x + 0.5f, playerOriPos.y, 0);
						battlePlayerCtr.transform.position = new Vector3 (playerOriPos.x - 0.3f, playerOriPos.y, 0);

					}

				}

				RunToPosition (npcFightPos, delegate {
					ExploreManager.Instance.ShowNPCPlane (this);
				});

				canShowNpcPlane = false;
			}
		}

		public override void SetSortingOrder (int order)
		{
			baCtr.SetSortingOrder (order);
		}



		public override void WalkToPosition(Vector3 position,CallBack cb,bool showAlertArea = true){

			if (MyTool.ApproximatelySamePosition2D (position, ExploreManager.Instance.battlePlayerCtr.transform.position)) {
				return;
			}

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
				} else {
					baCtr.TowardsLeft ();
				}
			} else if(targetPosX == oriPosX){

				if (targetPosY >= oriPosY) {
					baCtr.TowardsUp ();
				} else {
					baCtr.TowardsDown ();
				}
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

			if (MyTool.ApproximatelySamePosition2D (position, transform.position)) {

				if(cb != null){
					cb();
				}

				return;
			}


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


		private void InitNpcGoodsFromLevelData(){

			int currentLevel = Player.mainPlayer.currentLevelIndex;

			HLHGameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas [currentLevel];

			GenerateRandomGoods (levelData.goodsIdsListArray_0);
			GenerateRandomGoods (levelData.goodsIdsListArray_1);
			GenerateRandomGoods (levelData.goodsIdsListArray_2);
			GenerateRandomGoods (levelData.goodsIdsListArray_3);
			GenerateRandomGoods (levelData.goodsIdsListArray_4);


		}

		private void GenerateRandomGoods(List<int> goodsIds){
			
			int randomSeed = Random.Range (0, goodsIds.Count);

			HLHNPCGoods goods = new HLHNPCGoods (goodsIds [randomSeed]);

			npc.npcGoodsList.Add (goods);
		}

	}
}
