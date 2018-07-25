using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class PressSwitch : TriggeredGear {


		public Vector2 pairEventPos;

		// 地图高度（用于转换坐标系）
		private int mapHeight;

		// 是否已经触发过
		private bool hasTriggered;

		public Sprite pressSwitchOn;
		public Sprite pressSwitchOff;

		private int mapIndex;

		public override void ChangeStatus()
		{
			
		}


		public override bool IsPlayerNeedToStopWhenEntered ()
		{
			if (isWordTriggered) {
				return true;
			}

			return false;
		}



		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}
        


		public void SetPosTransferSeed(int mapHeight){
			this.mapHeight = mapHeight;
		}
			

		public override void InitializeWithAttachedInfo(int mapIndex,MapAttachedInfoTile attachedInfo)
		{
			this.mapIndex = mapIndex;
         
			transform.position = attachedInfo.position;

			hasTriggered = MapEventsRecord.IsMapEventTriggered(mapIndex, attachedInfo.position);

			if(hasTriggered){

				mapItemRenderer.sprite = pressSwitchOff;

				SetSortingOrder(-Mathf.RoundToInt(attachedInfo.position.y) - 1);

                hasTriggered = true;

				bc2d.enabled = true;

                tmPro.enabled = false;

                ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)] = 0;

				return;

			}else{
				mapItemRenderer.sprite = pressSwitchOn;
			}
                     
			hasTriggered = false;

			string pairDoorPosString = KVPair.GetPropertyStringWithKey ("pairEventPos", attachedInfo.properties);

			string[] posXY = pairDoorPosString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);

			int posX = int.Parse (posXY [0]);
			int posY = mapHeight - int.Parse (posXY [1]) - 1;

			pairEventPos = new Vector2 (posX, posY);
         

			bc2d.enabled = true;
			SetSortingOrder (-Mathf.RoundToInt(attachedInfo.position.y));

			CheckIsWordTriggeredAndShow ();
		}


		public override void EnterMapEvent(BattlePlayerController bp)
		{        
			if(hasTriggered){
				bp.isInEvent = false;
				return;
			}

			if (isWordTriggered) {
				ExploreManager.Instance.ShowWordsChoosePlane (wordsArray);
			} else {
				MapEventTriggered (true, bp);
			}
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			bp.isInEvent = false;

			if (hasTriggered) {
				return;
			}

			if (isSuccess) {

				mapItemRenderer.sprite = pressSwitchOff;
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.switchAudioName);
				
				ExploreManager.Instance.newMapGenerator.ChangeMapEventStatusAtPosition (pairEventPos);

				SetSortingOrder (mapItemRenderer.sortingOrder - 1);

				hasTriggered = true;

				bc2d.enabled = true;

				tmPro.enabled = false;            

				ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y)] = 1;

				int posX = Mathf.RoundToInt(this.transform.position.x);
                int posY = Mathf.RoundToInt(this.transform.position.y);

                MapEventsRecord.AddEventTriggeredRecord(mapIndex, new Vector2(posX, posY));

			}
		}


	}
}
