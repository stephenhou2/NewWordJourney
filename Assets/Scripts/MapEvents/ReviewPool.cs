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

		public override void AddToPool(InstancePool pool)
		{
			bc2d.enabled = false;
            pool.AddInstanceToPool(this.gameObject);
		}

		public override void InitializeWithAttachedInfo(int mapIndex, MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;
			bc2d.enabled = true;
			isExausted = false;
			mapItemRenderer.sprite = normalSprite;
			blinkAnim.gameObject.SetActive(true);
			SetSortingOrder(-(int)(attachedInfo.position.y));
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
		}
        

	}
}

