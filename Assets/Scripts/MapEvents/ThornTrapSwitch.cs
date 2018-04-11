using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ThornTrapSwitch : TriggeredGear {

		// 标记开关是否控制全图陷阱
		public bool isControlAllTrap;

		// 开关控制的陷阱位置
		public Vector2 pairTrapPos;

		public Sprite switchOffSprite;

		public Sprite switchOnSprite;

		private int switchStatusChangeCount;

		private int mapHeight;

		private bool hasFinishTest;



		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			hasFinishTest = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		/// <summary>
		/// 关闭陷阱
		/// </summary>
		public void ChangeSwitchStatus(){

			switchStatusChangeCount++;

			if (switchStatusChangeCount % 2 == 0) {
				mapItemRenderer.sprite = switchOffSprite;
			} else {
				mapItemRenderer.sprite = switchOnSprite;
			}
				
		}

		public void SetPosTransferSeed(int mapHeight){
			this.mapHeight = mapHeight;
		}


		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			string pairDoorPosString = KVPair.GetPropertyStringWithKey ("pairEventPos", attachedInfo.properties);



			if (pairDoorPosString.Equals ("-1")) {
				isControlAllTrap = true;

			} else {

				isControlAllTrap = false;
				string[] posXY = pairDoorPosString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);

				int posX = int.Parse (posXY [0]);
				int posY = mapHeight - int.Parse (posXY [1]) - 1;

				pairTrapPos = new Vector2 (posX, posY);
			}

			CheckIsWordTriggeredAndShow ();

			bc2d.enabled = true;
			mapItemRenderer.sprite = switchOffSprite;
			switchStatusChangeCount = 0;
			SetSortingOrder (-(int)attachedInfo.position.y);

			hasFinishTest = false;

		}


		public override void EnterMapEvent(BattlePlayerController bp)
		{
			if (hasFinishTest) {
				MapEventTriggered (true, bp);
				return;
			}

			if (isWordTriggered) {
				ExploreManager.Instance.ShowWordsChoosePlane (wordsArray);
			} else {
				MapEventTriggered (true, bp);
			}
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			ChangeStatus ();
			if (!hasFinishTest) {
				hasFinishTest = true;
				tmPro.enabled = false;
			}
		}

		public override void ChangeStatus ()
		{
			switchStatusChangeCount++;

			if (switchStatusChangeCount % 2 == 0) {
				mapItemRenderer.sprite = switchOffSprite;
			} else {
				mapItemRenderer.sprite = switchOnSprite;
			}

			if (isControlAllTrap) {
				ExploreManager.Instance.newMapGenerator.ChangeAllThornTrapsInMap ();
			} else {
				ExploreManager.Instance.newMapGenerator.ChangeMapEventStatusAtPosition (pairTrapPos);
			}

		}

	}
}
