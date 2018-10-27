using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	/// <summary>
    /// 重铸水晶
    /// </summary>
	public class RebuildStone : MapEvent
    {
      
        // 是否已经使用过了
		private bool isExausted;

        // 重铸水晶的正常图片精灵和exausted图片精灵
        public Sprite normalSprite;
        public Sprite exaustedSprite;

        // 水晶上的闪烁动画
        public Transform blinkAnim;

        // 添加到缓存池中
        public override void AddToPool(InstancePool pool)
        {
            bc2d.enabled = false;
            pool.AddInstanceToPool(this.gameObject);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mapIndex">Map index.</param>
        /// <param name="attachedInfo">Attached info.</param>
        public override void InitializeWithAttachedInfo(int mapIndex, MapAttachedInfoTile attachedInfo)
        {
            transform.position = attachedInfo.position;
            bc2d.enabled = true;
            isExausted = false;
            mapItemRenderer.sprite = normalSprite;
            blinkAnim.gameObject.SetActive(true);
            SetSortingOrder(-(int)(attachedInfo.position.y));
        }

        /// <summary>
        /// 进入地图事件
        /// </summary>
        /// <param name="bp">Bp.</param>
        public override void EnterMapEvent(BattlePlayerController bp)
        {
            if (isExausted)
            {
                ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
                return;
            }
            MapEventTriggered(true, bp);
        }
        
        /// <summary>
        /// 地图事件触发
        /// </summary>
        /// <param name="isSuccess">If set to <c>true</c> is success.</param>
        /// <param name="bp">Bp.</param>
        public override void MapEventTriggered(bool isSuccess, BattlePlayerController bp)
        {
			Transform finalChapterCanvas = TransformManager.FindTransform("CanvasContainer/FinalChapterCanvas");

			if(finalChapterCanvas == null){
				ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
				return;
			}

			FinalChapterViewControlller finalChapter = finalChapterCanvas.GetComponent<FinalChapterViewControlller>();

			finalChapter.ShowQueryRebuildHUD(TriggerCallBack);
        }
        

        /// <summary>
        /// 触发后的回调
        /// </summary>
        private void TriggerCallBack()
        {
            isExausted = true;
            mapItemRenderer.sprite = exaustedSprite;
            blinkAnim.gameObject.SetActive(false);
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
        }

        /// <summary>
        /// 水晶枯竭【使用过一次后就枯竭】
        /// </summary>
		public void Exaust(){
			isExausted = true;
			blinkAnim.gameObject.SetActive(false);
			mapItemRenderer.sprite = exaustedSprite;
		}

		public override bool IsFullWordNeedToShowWhenChooseWrong()
		{
			return false;
		}

	}
}

