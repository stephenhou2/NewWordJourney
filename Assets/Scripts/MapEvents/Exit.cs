using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Exit : TriggeredGear {

		private int direction;

		// 关闭的门图片数组（0:上 1:下 2:左 3:右）
		public Sprite[] exitCloseSprites;

		public bool isOpen;


		public override void AddToPool(InstancePool pool){
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			direction = int.Parse(KVPair.GetPropertyStringWithKey("direction",attachedInfo.properties));

			isOpen = bool.Parse(KVPair.GetPropertyStringWithKey("isOpen",attachedInfo.properties));

			if (!isOpen) {
				mapItemRenderer.enabled = true;
				mapItemRenderer.sprite = exitCloseSprites [direction];
			} else {
				mapItemRenderer.sprite = null;
				mapItemRenderer.enabled = false;
			}

			bc2d.enabled = true;

			SetSortingOrder (-(int)transform.position.y);


		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			if (!isOpen) {
				ExploreManager.Instance.ShowTint ("这里被一股奇怪的魔力封印了",null);
			} else {
				MapEventTriggered (true, bp);
			}

		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			ExploreManager.Instance.expUICtr.ShowEnterNextLevelQueryHUD ();
		}

		public override void ChangeStatus ()
		{
			isOpen = true;

			mapItemRenderer.sprite = null;
		}

	}
}
