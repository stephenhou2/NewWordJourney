using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	public enum ExitType{
		NextLevel,
        LastLevel
	}

	public class Exit : TriggeredGear {

		//private int direction;

		// 关闭的门图片数组（0:上 1:下 2:左 3:右）
		public Sprite[] exitCloseSprites;

		public bool isOpen;

		public ExitType exitType;


		public override void AddToPool(InstancePool pool){
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		public void SetUpExitType(ExitType exitType){
			this.exitType = exitType;
		}


		public override void InitializeWithAttachedInfo(int mapIndex,MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			//if(exitType == ExitType.NextLevel){
			//	direction = int.Parse(KVPair.GetPropertyStringWithKey("direction",attachedInfo.properties));      
			//}
            isOpen = true;

			//if (!isOpen) {
			//	mapItemRenderer.enabled = true;
			//	mapItemRenderer.sprite = exitCloseSprites [direction];
			//} else {
			//	mapItemRenderer.sprite = null;
			//	mapItemRenderer.enabled = false;
			//}

			bc2d.enabled = true;

			SetSortingOrder (-(int)transform.position.y);


		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			if (!isOpen) {
				ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD ("这里被一股奇怪的魔力封印了");
				bp.isInEvent = false;
			} else {
				MapEventTriggered (true, bp);
			}

		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			if (Player.mainPlayer.currentLevelIndex == 0 && exitType == ExitType.LastLevel)
            {
				ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD("这里好像没办法出去");
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.exitAudioName);
				ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
                return;
            }
			ExploreManager.Instance.expUICtr.ShowEnterExitQueryHUD(exitType);
		}

		public override void ChangeStatus ()
		{
			isOpen = true;

			mapItemRenderer.sprite = null;
		}

	}
}
