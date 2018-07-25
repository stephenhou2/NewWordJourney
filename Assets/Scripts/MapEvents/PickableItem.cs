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
			StopCoroutine("FlyToPlayer");
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		//public void InitializeWithItemAndPosition(Vector3 position,Item item){
         
		//	transform.position = position;
		//	transform.localScale = Vector3.one;
		//	transform.localRotation = Quaternion.identity;

		//	this.item = item;

		//	Sprite s = GameManager.Instance.gameDataCenter.GetGameItemSprite (item);

		//	itemIcon.sprite = s;

			//oriIconPosY = itemTrans.localPosition.y;

			//oriPosX = transform.position.x;
			//oriPosY = transform.position.y;

		//	BeginFloating ();

		//	CheckIsWordTriggeredAndShow ();

		//}


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
                
				//StartCoroutine("FlyToPlayer");
			}




		}

		//private void ShowItemDetail(){
		//	ExploreManager.Instance.expUICtr.SetUpSimpleItemDetail(item);
		//}

		//private IEnumerator FlyToPlayer()
   //     {

			//tmPro.enabled = false;

   //         yield return new WaitUntil(() => Time.timeScale == 1);

   //         float passedTime = 0;

			//float leftTime = flyToPlayerDuration - passedTime;

			//int oriPosX = Mathf.RoundToInt(transform.position.x);
			//int oriPosY = Mathf.RoundToInt(transform.position.y);

			//Transform battlePlayerTrans = ExploreManager.Instance.battlePlayerCtr.transform;

   //         float distance = Mathf.Sqrt(Mathf.Pow((battlePlayerTrans.position.x - transform.position.x), 2.0f)
   //             + Mathf.Pow((battlePlayerTrans.position.y - transform.position.y), 2.0f));

   //         while (distance > 0.5f)
   //         {

   //             if (leftTime <= 0)
   //             {
   //                 break;
   //             }

   //             Vector3 rewardVelocity = new Vector3((battlePlayerTrans.position.x - transform.position.x) / leftTime,
   //                 (battlePlayerTrans.position.y - transform.position.y) / leftTime, 0);

   //             Vector3 newRewardPos = new Vector3(transform.position.x + rewardVelocity.x * Time.deltaTime,
   //                 transform.position.y + rewardVelocity.y * Time.deltaTime);

   //             transform.position = newRewardPos;

   //             passedTime += Time.deltaTime;

			//	leftTime = flyToPlayerDuration - passedTime;

   //             distance = Mathf.Sqrt(Mathf.Pow((battlePlayerTrans.position.x - transform.position.x), 2.0f)
   //                 + Mathf.Pow((battlePlayerTrans.position.y - transform.position.y), 2.0f));

   //             yield return null;

   //         }


			////if (Player.mainPlayer.CheckBagFull(item))
    // //       {
    // //           GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.bagCanvasBundleName, "BagCanvas", () => {
				//	//TransformManager.FindTransform("BagCanvas").GetComponent<BagViewController>().AddBagItemWhenBagFull(item);
    //        //    }, false, true);
    //        //}
    //        //else
    //        //{            
				//ExploreManager.Instance.ObtainReward(item);

				//ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[oriPosX, oriPosY] = 1;

    //            AddToPool(ExploreManager.Instance.newMapGenerator.mapEventsPool);

				////ExploreManager.Instance.newMapGenerator.MiniMapInstanceToPool(miniMapInstance);

        //    //}

        //}



	}
}
