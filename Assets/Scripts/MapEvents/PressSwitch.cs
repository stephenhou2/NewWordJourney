using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class PressSwitch : TriggeredGear {

//		public Sprite switchOffSprite;
//
//		public Sprite switchOnSprite;


		public Vector2 pairEventPos;

		// 地图高度（用于转换坐标系）
		private int mapHeight;

		// 是否已经触发过
		private bool hasTriggered;

		//是否是弹性开关
		private bool isElastic;

		public Sprite pressSwitchOn;
		public Sprite pressSwitchOff;

		public Sprite elasticSwitchOn;
		public Sprite elasticSwitchOff;


		public override bool IsPlayerNeedToStopWhenEntered ()
		{
			if (isWordTriggered && !hasTriggered) {
				return true;
			}

			return false;
		}

		public override void InitMapItem ()
		{
			bc2d.enabled = true;
//			mapItemRenderer.sprite = switchOnSprite;
			SetSortingOrder (1);
		}

		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		public void ResetPressSwitch(Door door){
			if (!isElastic) {
				mapItemRenderer.sprite = pressSwitchOff;
			} else {
				mapItemRenderer.sprite = elasticSwitchOff;
			}

		}

		void OnTriggerEnter2D(Collider2D other){

			if (other.GetComponent<BattleAgentController> () == null && other.GetComponent<MovableBox>() == null) {
				return;
			}

			PressOnSwitch ();

		}

		private void PressOnSwitch(){
//			mapItemRenderer.sprite = switchOffSprite;

		}

		private void PressOffSwitch(){
//			mapItemRenderer.sprite = switchOnSprite;

		}

		void OnTriggerExit2D(Collider2D other){

			if (other.GetComponent<BattleAgentController> () == null && other.GetComponent<MovableBox>() == null) {
				return;
			}

			PressOffSwitch();

		}

		public void SetPosTransferSeed(int mapHeight){
			this.mapHeight = mapHeight;
		}
			

		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			hasTriggered = false;

			string pairDoorPosString = KVPair.GetPropertyStringWithKey ("pairEventPos", attachedInfo.properties);

			string[] posXY = pairDoorPosString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);

			int posX = int.Parse (posXY [0]);
			int posY = mapHeight - int.Parse (posXY [1]) - 1;

			pairEventPos = new Vector2 (posX, posY);

			isElastic = attachedInfo.type.Equals ("elasticSwitch");

			if (!isElastic) {
				mapItemRenderer.sprite = pressSwitchOn;
			} else {
				mapItemRenderer.sprite = elasticSwitchOn;
			}

			bc2d.enabled = true;
			SetSortingOrder (-Mathf.RoundToInt(attachedInfo.position.y));

			CheckIsWordTriggeredAndShow ();
		}


		public override void EnterMapEvent(BattlePlayerController bp)
		{
			if (isElastic) {
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
			if (hasTriggered) {
				return;
			}

			if (isSuccess) {
				
				ExploreManager.Instance.newMapGenerator.ChangeMapEventStatusAtPosition (pairEventPos);

				mapItemRenderer.sprite = isElastic ? elasticSwitchOff : pressSwitchOff;

				SetSortingOrder (mapItemRenderer.sortingOrder - 1);

				hasTriggered = true;

				bc2d.enabled = false;

				tmPro.enabled = false;

				if (!isElastic) {
					ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y)] = 1;
				}
			}
		}

		public override void ChangeStatus ()
		{
			
		}

	}
}
