using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DG.Tweening;

	public class SecretStairs : TriggeredGear {


		private int mapHeight;

		private Vector2 pairStairsPos;

		private bool isOpen;

		public Sprite downstairs;
		public Sprite[] upstairsArray;


		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}
			
		public void EnterHole(MapGenerator mapGenerator,BattlePlayerController bp){

			bp.ActiveBattlePlayer (false, false, false);

			List<SecretStairs> allHolesInMap = mapGenerator.GetAllHolesInMap ();

			SecretStairs randomOtherHole = GetRandomOtherHole (allHolesInMap);

//			Debug.LogFormat ("hole found,time:{0}", Time.realtimeSinceStartup);

			ExploreManager.Instance.DisableExploreInteractivity ();

			Vector3 otherHolePosition = new Vector3(randomOtherHole.transform.position.x,randomOtherHole.transform.position.y,0);

			Vector3 moveVector = otherHolePosition - bp.transform.position;

//			Transform background = Camera.main.transform.Find ("Background");
//
//			Vector3 backgroundImageTargetPos = background.localPosition + new Vector3 (moveVector.x * 0.2f, moveVector.y * 0.2f, 0);


			Vector3 walkablePositionAround = mapGenerator.GetARandomWalkablePositionAround (otherHolePosition);

			bp.SetEffectAnim ("HoleFog");

//			mapGenerator.PlayMapOtherAnim ("HoleFog", this.transform.position);


			if(randomOtherHole.transform.position.z != 0){
				mapGenerator.DirectlyShowSleepingTilesAtPosition(otherHolePosition);
			}

			mapGenerator.ItemsAroundAutoIntoLifeWithBasePoint(otherHolePosition);

			bp.transform.DOMove(otherHolePosition,1).OnComplete(delegate{

				bp.singleMoveEndPos = otherHolePosition;

				IEnumerator WalkOutOfHoleCoroutine = WalkOutOfHole(mapGenerator,walkablePositionAround,bp,randomOtherHole);

				StartCoroutine(WalkOutOfHoleCoroutine);

			});

//			background.transform.DOMove (backgroundImageTargetPos, 1);

		}

		private IEnumerator WalkOutOfHole(MapGenerator mapGenerator,Vector3 walkablePositionAround,BattlePlayerController bp,SecretStairs targetHole){

			targetHole.bc2d.enabled = false;

			int[,] mapWalkableInfo = mapGenerator.mapWalkableInfoArray;

			yield return new WaitUntil(()=> mapWalkableInfo[(int)walkablePositionAround.x,(int)walkablePositionAround.y] > 0);

			bp.SetEffectAnim ("HoleFog");

//			mapGenerator.PlayMapOtherAnim("HoleFog",targetHole.transform.position);

//			Debug.LogFormat ("around items come to life,time:{0}", Time.realtimeSinceStartup);

			bp.MoveToPosition(walkablePositionAround,mapGenerator.mapWalkableInfoArray);

			yield return new WaitUntil (() => bp.isIdle);

//			Debug.LogFormat ("player move End:{0}", Time.realtimeSinceStartup);

			targetHole.bc2d.enabled = true;

			ExploreManager.Instance.EnableExploreInteractivity ();

		}

		private SecretStairs GetRandomOtherHole(List<SecretStairs> allHolesInMap){

			int randomHoleIndex = Random.Range (0, allHolesInMap.Count);

			if (allHolesInMap [randomHoleIndex].transform.position == this.transform.position) {
				return GetRandomOtherHole (allHolesInMap);
			}

			return allHolesInMap[randomHoleIndex];
		}


		public void SetPosTransferSeed(int mapHeight){
			this.mapHeight = mapHeight;
		}



		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{

			transform.position = attachedInfo.position;

			if (attachedInfo.type.Equals ("downStair")) {
				mapItemRenderer.sprite = downstairs;

				isOpen = bool.Parse (KVPair.GetPropertyStringWithKey ("isOpen", attachedInfo.properties));
				if (isOpen) {
					mapItemRenderer.sprite = downstairs;
					bc2d.enabled = true;
				} else {
//					tmPro.enabled = false;
					mapItemRenderer.enabled = false;
					bc2d.enabled = false;
				}

				tmPro.enabled = false;

			} else if(attachedInfo.type.Equals ("upStair")){
				isOpen = true;
				int direction = int.Parse (KVPair.GetPropertyStringWithKey ("direction", attachedInfo.properties));
				mapItemRenderer.sprite = upstairsArray[direction];
				tmPro.enabled = false;
			}

			string pairStairsPosString = KVPair.GetPropertyStringWithKey ("pairEventPos", attachedInfo.properties);

			string[] posXY = pairStairsPosString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);

			int posX = int.Parse (posXY [0]);
			int posY = mapHeight - int.Parse (posXY [1]) -1;

			pairStairsPos = new Vector2 (posX, posY);

			SetSortingOrder (-Mathf.RoundToInt (transform.position.y - 1));
		}

//		protected override void CheckIsWordTriggeredAndShow ()
//		{
//			if (isWordTriggered && isOpen) {
//
//				LearnWord targetWord = wordsArray [0];
//
//				tmPro.text = targetWord.spell;
//
//				tmPro.enabled = true;
//
//			} else {
//
//				tmPro.enabled = false;
//
//			}
//		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			if (tmPro.enabled) {
				ExploreManager.Instance.ShowWordsChoosePlane (wordsArray);
			} else {
				MapEventTriggered (true, bp);
			}
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			bp.isInEvent = false;

			if (isSuccess) {

				isOpen = true;
				tmPro.enabled = false;

				bp.transform.position = pairStairsPos;
				bp.singleMoveEndPos = pairStairsPos;

			}
		}

		public override void ChangeStatus ()
		{
			isOpen = !isOpen;

			mapItemRenderer.enabled = isOpen;
			bc2d.enabled = isOpen;
		}

	}
}
