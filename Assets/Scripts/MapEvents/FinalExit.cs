using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class FinalExit : MapEvent
    {

		public override void AddToPool(InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool(this.gameObject);
		}

		public override void InitializeWithAttachedInfo(int mapIndex, MapAttachedInfoTile attachedInfo)
		{
			bc2d.enabled = true;
			transform.position = attachedInfo.position;
		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			MapEventTriggered(true, bp);
		}

		public override void MapEventTriggered(bool isSuccess, BattlePlayerController bp)
		{
			ExploreManager.Instance.expUICtr.ShowFinalQuitQueryHUD();
		}
	}
}

