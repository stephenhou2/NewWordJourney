using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DG.Tweening;

	public class PickableItem : MapEvent {

		public Item item;

		public SpriteRenderer itemIcon;

		public float floatingInterval = 2f;
		public float floatingDistance = 0.1f;

		public float flyToPlayerDuration = 0.3f;

		private float oriIconPosY;

		private float oriPosY;
		private float oriPosX;

		public Transform itemTrans;

		private Sequence floatingSequence;

		private int mapIndex;


		public override void AddToPool (InstancePool pool)
		{
			StopFloating ();
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}
        


		public override void InitializeWithAttachedInfo(int mapIndex,MapAttachedInfoTile attachedInfo)
		{
			this.mapIndex = mapIndex;

			transform.position = attachedInfo.position;

			bc2d.enabled = true;

			tmPro.enabled = true;

			string dropItemIds = KVPair.GetPropertyStringWithKey("dropID", attachedInfo.properties);
			string[] dropItemIdArray = dropItemIds.Split(new char[]{'_'});
			int randomIndex = Random.Range(0, dropItemIdArray.Length);
			int itemId = int.Parse(dropItemIdArray[randomIndex]);
          
			item = Item.NewItemWith(itemId, 1);

			Sprite s = GameManager.Instance.gameDataCenter.GetGameItemSprite (item);

			itemTrans.GetComponent<SpriteRenderer>().sprite = s;

			oriIconPosY = itemTrans.localPosition.y;

            oriPosX = transform.position.x;
            oriPosY = transform.position.y;

			BeginFloating ();

			CheckIsWordTriggeredAndShow ();

		}

		private void BeginFloating(){
			
			floatingSequence = DOTween.Sequence ();


			float floatingTop = oriIconPosY + floatingDistance;

			floatingSequence.Append (itemTrans.DOLocalMoveY (floatingTop, floatingInterval))
				.Append (itemTrans.DOLocalMoveY (oriIconPosY, floatingInterval));

			floatingSequence.SetLoops (-1);
			floatingSequence.Play ();
		}

		private void StopFloating(){

			floatingSequence.Kill (false);

			itemTrans.localPosition = new Vector3 (itemTrans.localPosition.x, oriIconPosY, itemTrans.localPosition.z);

		}
			

		private int GetRandomItemIdFromRangeString(string itemIdRangeString){

			string[] idStringArray = itemIdRangeString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);

			int[] idRangeArray = new int[idStringArray.Length];

			for (int i = 0; i < idStringArray.Length; i++) {
				idRangeArray [i] = int.Parse (idStringArray [i]);
			}

			int randomIndex = Random.Range (0, idRangeArray.Length);

			return idRangeArray [randomIndex];

		}


		public override void EnterMapEvent(BattlePlayerController bp)
		{
			if (isWordTriggered) {
				ExploreManager.Instance.ShowWordsChoosePlane (wordsArray);
			} else {
				MapEventTriggered (true, bp);
			}
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{

			bp.isInEvent = false;
            
			if (isSuccess) {

				int posX = Mathf.RoundToInt(oriPosX);
				int posY = Mathf.RoundToInt(oriPosY);

				ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[posX, posY] = 1;

				ExploreManager.Instance.newMapGenerator.SetUpRewardInMap(item, transform.position);

                MapEventsRecord.AddEventTriggeredRecord(mapIndex, new Vector2(posX, posY));
            
				AddToPool(ExploreManager.Instance.newMapGenerator.mapEventsPool);

			}
         

		}


	}
}
