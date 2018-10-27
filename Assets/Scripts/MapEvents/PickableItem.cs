using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DG.Tweening;

    /// <summary>
    /// 可拾取物品类
    /// </summary>
	public class PickableItem : MapEvent {

        // 实际对应的物品
		public Item item;
        // 图片渲染器
		public SpriteRenderer itemIcon;

        // 浮动间隔
		public float floatingInterval = 2f;
        // 浮动距离
		public float floatingDistance = 0.1f;
        // 飞向玩家角色的事件
		public float flyToPlayerDuration = 0.3f;
        
		private float oriIconPosY;

		private float oriPosY;
		private float oriPosX;

		public Transform itemTrans;

		private Sequence floatingSequence;

		private int mapIndex;

        /// <summary>
        /// 加入缓存池
        /// </summary>
        /// <param name="pool">Pool.</param>
		public override void AddToPool (InstancePool pool)
		{
			StopFloating ();
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}
        
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mapIndex">Map index.</param>
        /// <param name="attachedInfo">Attached info.</param>
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

        /// <summary>
        /// 开始漂浮动画
        /// </summary>
		private void BeginFloating(){
			
			floatingSequence = DOTween.Sequence ();


			float floatingTop = oriIconPosY + floatingDistance;

			floatingSequence.Append (itemTrans.DOLocalMoveY (floatingTop, floatingInterval))
				.Append (itemTrans.DOLocalMoveY (oriIconPosY, floatingInterval));

			floatingSequence.SetLoops (-1);
			floatingSequence.Play ();
		}


        /// <summary>
        /// 停止漂浮动画
        /// </summary>
		private void StopFloating(){

			floatingSequence.Kill (false);

			itemTrans.localPosition = new Vector3 (itemTrans.localPosition.x, oriIconPosY, itemTrans.localPosition.z);

		}
			

        /// <summary>
        /// 从物品id范围字符串中随机出一个物品id
        /// </summary>
        /// <returns>The random item identifier from range string.</returns>
        /// <param name="itemIdRangeString">Item identifier range string.</param>
		private int GetRandomItemIdFromRangeString(string itemIdRangeString){

			string[] idStringArray = itemIdRangeString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);

			int[] idRangeArray = new int[idStringArray.Length];

			for (int i = 0; i < idStringArray.Length; i++) {
				idRangeArray [i] = int.Parse (idStringArray [i]);
			}

			int randomIndex = Random.Range (0, idRangeArray.Length);

			return idRangeArray [randomIndex];

		}

        /// <summary>
        /// 进入地图事件
        /// </summary>
        /// <param name="bp">Bp.</param>
		public override void EnterMapEvent(BattlePlayerController bp)
		{
			if (isWordTriggered) {
				ExploreManager.Instance.ShowWordsChoosePlane (wordsArray);
			} else {
				MapEventTriggered (true, bp);
			}
		}

        /// <summary>
        /// 地图事件触发
        /// </summary>
        /// <param name="isSuccess">If set to <c>true</c> is success.</param>
        /// <param name="bp">Bp.</param>
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
