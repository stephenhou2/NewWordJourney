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

			isOpen = false;

			#warning 更新地图数据后打开
//			direction = int.Parse(KVPair.GetPropertyStringWithKey("direction",attachedInfo.properties));

			direction = 1;

			mapItemRenderer.enabled = true;
			mapItemRenderer.sprite = exitCloseSprites [direction];


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
