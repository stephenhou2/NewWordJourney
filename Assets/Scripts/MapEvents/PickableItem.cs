using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DG.Tweening;

	public class PickableItem : MapEvent {

		public Item item;

//		public SpriteRenderer itemIcon;

		public float floatingInterval = 2f;
		public float floatingDistance = 0.1f;

		private float oriPosY;

		public Transform itemTrans;

		private Sequence floatingSequence;

		


		public override void InitMapItem ()
		{
			bc2d.enabled = true;
		}

		public override void AddToPool (InstancePool pool)
		{
			StopFloating ();
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		public void InitializeWithItemAndPosition(Vector3 position,Item item){

			transform.position = position;
			transform.localScale = Vector3.one;
			transform.localRotation = Quaternion.identity;

			this.item = item;

			Sprite s = GameManager.Instance.gameDataCenter.GetGameItemSprite (item);

			itemTrans.GetComponent<SpriteRenderer>().sprite = s;

			oriPosY = itemTrans.localPosition.y;


			BeginFloating ();

			CheckIsWordTriggeredAndShow ();

		}


		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;
			string itemIdRangeString = KVPair.GetPropertyStringWithKey ("IDRange", attachedInfo.properties);

			// 完全随机物品
			if (itemIdRangeString.Equals ("-1")) {
				item = Item.GetRandomItem ();
			} else {//范围随机物品
				int itemId = GetRandomItemIdFromRangeString (itemIdRangeString);
				item = Item.NewItemWith (itemId, 1);
			}


			Sprite s = GameManager.Instance.gameDataCenter.GetGameItemSprite (item);

			itemTrans.GetComponent<SpriteRenderer>().sprite = s;

			oriPosY = itemTrans.localPosition.y;

			BeginFloating ();

			CheckIsWordTriggeredAndShow ();

		}

		private void BeginFloating(){
			
			floatingSequence = DOTween.Sequence ();


			float floatingTop = oriPosY + floatingDistance;

			floatingSequence.Append (itemTrans.DOLocalMoveY (floatingTop, floatingInterval))
				.Append (itemTrans.DOLocalMoveY (oriPosY, floatingInterval));

			floatingSequence.SetLoops (-1);
			floatingSequence.Play ();
		}

		private void StopFloating(){

			floatingSequence.Kill (false);

			itemTrans.localPosition = new Vector3 (itemTrans.localPosition.x, oriPosY, itemTrans.localPosition.z);

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

			if (isSuccess) {
				ExploreManager.Instance.ObtainReward (item);
			}

			AddToPool (ExploreManager.Instance.newMapGenerator.mapEventsPool);

		}

	}
}
