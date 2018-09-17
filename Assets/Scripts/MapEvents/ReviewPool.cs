using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ReviewPool : MapEvent
    {
		private bool isExausted;

		public Sprite normalSprite;
		public Sprite exaustedSprite;

		public Transform blinkAnim;

		private int mapIndex;

		public override void AddToPool(InstancePool pool)
		{
			bc2d.enabled = false;
            pool.AddInstanceToPool(this.gameObject);
		}

		public override void InitializeWithAttachedInfo(int mapIndex, MapAttachedInfoTile attachedInfo)
		{
			this.mapIndex = mapIndex;         
			transform.position = attachedInfo.position;
			bc2d.enabled = true;

			SetSortingOrder(-(int)(attachedInfo.position.y));
			if(GameManager.Instance.gameDataCenter.currentMapEventsRecord.IsMapEventTriggered(mapIndex,attachedInfo.position)){
				isExausted = true;
				mapItemRenderer.sprite = exaustedSprite;
				blinkAnim.gameObject.SetActive(false);
			}else{
				isExausted = false;
                mapItemRenderer.sprite = normalSprite;
                blinkAnim.gameObject.SetActive(true);
			}
		}
        
		public override void EnterMapEvent(BattlePlayerController bp)
		{
			if(isExausted){
				ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
				return;
			}
			MapEventTriggered(true, bp);
		}

		public override void MapEventTriggered(bool isSuccess, BattlePlayerController bp)
		{
			ExploreManager.Instance.expUICtr.QueryEnterWordRecordQuizView(TriggerCallBack);
		}

		private void TriggerCallBack(){
			isExausted = true;
			mapItemRenderer.sprite = exaustedSprite;
            blinkAnim.gameObject.SetActive(false);
			GameManager.Instance.gameDataCenter.currentMapEventsRecord.AddEventTriggeredRecord(mapIndex, transform.position);
		}
        

	}
}

