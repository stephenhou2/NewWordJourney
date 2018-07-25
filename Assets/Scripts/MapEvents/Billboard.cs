using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Billboard : MapEvent {

        public HLHSentenceAndPoem sap;

		public override bool IsPlayerNeedToStopWhenEntered ()
		{
			return true;
		}
			


		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		public override void InitializeWithAttachedInfo(int mapIndex,MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;
			bc2d.enabled = true;
			SetSortingOrder (-(int)transform.position.y);
            sap = GameManager.Instance.gameDataCenter.GetARandomProverb();

		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			MapEventTriggered (true, bp);
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			ExploreManager.Instance.ShowBillboard (this);
		}


	}
}
