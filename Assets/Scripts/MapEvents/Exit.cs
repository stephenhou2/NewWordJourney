using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Exit : MapEvent {

		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			SetSortingOrder (-(int)transform.position.y);
		}

		public override void AddToPool(InstancePool pool){
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			MapEventTriggered (true, bp);
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			ExploreManager.Instance.QuitExploreScene (true);
		}

	}
}
