using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Door : TriggeredGear {


		private int direction;

		// 关闭的门图片数组（0:上 1:下 2:左 3:右）
		public Sprite[] doorCloseSprites;

		public bool isOpen;

		public bool isWordTrigger;

		public Vector3 pairDoorPos;

		private int mapHeight;

		public void SetPosTransferSeed(int mapHeight){
			this.mapHeight = mapHeight;
		}

		public void OpenTheDoor(){
			mapItemRenderer.enabled = false;
			isOpen = true;
			bc2d.enabled = false;
		}

		public void CloseTheDoor(){
			mapItemRenderer.enabled = true;
			mapItemRenderer.sprite = doorCloseSprites[direction];
			isOpen = false;
			bc2d.enabled = true;
		}

		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			isOpen = false;
			SetSortingOrder (-(int)transform.position.y);
		}

		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);

		}

//		public override bool IsPlayerNeedToStopWhenEntered ()
//		{
//			return false;
//		}

		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;


			isWordTrigger = bool.Parse (KVPair.GetPropertyStringWithKey ("isWordTrigger", attachedInfo.properties));
			isOpen = bool.Parse (KVPair.GetPropertyStringWithKey ("isOpen", attachedInfo.properties));

			direction = int.Parse(KVPair.GetPropertyStringWithKey("direction",attachedInfo.properties));

			if (!isOpen) {
				mapItemRenderer.enabled = true;
				mapItemRenderer.sprite = doorCloseSprites [direction];
			}

			string pairDoorPosString = KVPair.GetPropertyStringWithKey ("pairDoorPos", attachedInfo.properties);

			string[] posXY = pairDoorPosString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);

			int posX = int.Parse (posXY [0]);
			int posY = mapHeight - int.Parse (posXY [1]) - 1;

			pairDoorPos = new Vector3 (posX, posY,transform.position.z);

			bc2d.enabled = true;
			SetSortingOrder (-(int)transform.position.y);

		}



		public override void EnterMapEvent(BattlePlayerController bp)
		{
			if (isOpen) {
				MapEventTriggered (true, bp);
				return;
			}

			if (isWordTrigger) {
				ExploreManager.Instance.ShowCharacterFillPlane (wordsArray[0]);
			}

			if (!isOpen && !isWordTrigger) {
				ExploreManager.Instance.ShowTint ("隐约听到齿轮转动的声音,应该需要通过机关才能打开", null);
			}
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			if (isSuccess) {

				isOpen = true;
				mapItemRenderer.sprite = null;


				bp.transform.position = pairDoorPos;
				bp.singleMoveEndPos = pairDoorPos;
				bp.moveDestination = pairDoorPos;

				Vector3 continueMovePos = Vector3.zero;

				switch (bp.towards) {
				case MyTowards.Up:
					continueMovePos = pairDoorPos + new Vector3 (0, 1, 0);
					break;
				case MyTowards.Down:
					continueMovePos = pairDoorPos + new Vector3 (0, -1, 0);
					break;
				case MyTowards.Left:
					continueMovePos = pairDoorPos + new Vector3 (-1, 0, 0);
					break;
				case MyTowards.Right:
					continueMovePos = pairDoorPos + new Vector3 (1, 0, 0);
					break;
				}

				bp.MoveToPosition (continueMovePos, ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray);


			} else {

				ExploreManager.Instance.ShowTint("口令不正确！",null);

			}
		}

		public override void ChangeStatus ()
		{
			isOpen = !isOpen;
			mapItemRenderer.sprite = isOpen ? null : doorCloseSprites[direction];

		}

	}
}
