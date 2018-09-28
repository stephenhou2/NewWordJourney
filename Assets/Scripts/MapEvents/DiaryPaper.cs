using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class DiaryPaper : MapEvent
    {
		private int mapIndex;
		private DiaryModel diary;

		public override void AddToPool(InstancePool pool)
		{
			bc2d.enabled = true;
			pool.AddInstanceToPool(this.gameObject);
		}

		public void OnTriggerEnter2D(Collider2D collision)
		{
			BattlePlayerController battlePlayerController = collision.GetComponent<BattlePlayerController>();

			if(battlePlayerController == null){
				return;
			}

			MapEventTriggered(true,battlePlayerController);
		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			bp.isInEvent = false;
			MapEventTriggered(true, bp);
		}

		public override void InitializeWithAttachedInfo(int mapIndex, MapAttachedInfoTile attachedInfo)
		{
			this.mapIndex = mapIndex;
			transform.position = attachedInfo.position;         
			bc2d.enabled = true;
			diary = GameManager.Instance.gameDataCenter.GetDiaryInLevel(Player.mainPlayer.currentLevelIndex);

		}

		public override void MapEventTriggered(bool isSuccess, BattlePlayerController bp)
		{   
			if (bp.isInEvent)
            {
                return;
            }

			ExploreManager.Instance.expUICtr.SetUpDiaryView(diary);

			MapEventsRecord.DiaryFinishAtMapIndex(mapIndex);
                     
			Destroy(this.gameObject, 0.3f);
		}

		public override bool IsPlayerNeedToStopWhenEntered()
		{
			return true;
		}
	}

}
