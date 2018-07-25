using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Block : MapEvent {

		public List<Sprite> allDocorationSprites;


		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		public override void InitializeWithAttachedInfo(int mapIndex,MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;
			bc2d.enabled = true;
			int randomDocorationIndex = Random.Range (0, allDocorationSprites.Count);
			Sprite docorationSprite = allDocorationSprites [randomDocorationIndex];
			mapItemRenderer.sprite = docorationSprite;
			SetSortingOrder (-(int)transform.position.y);
		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			bp.isInEvent = false;
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			bp.isInEvent = false;
		}
	}
}
