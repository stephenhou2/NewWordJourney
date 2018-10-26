using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using TMPro;

    /// <summary>
    /// 地图事件类
    /// </summary>
	public abstract class MapEvent : MonoBehaviour {

        // 地图事件对应的音效名称
		public string audioClipName;
        // 图片精灵
		protected SpriteRenderer mapItemRenderer;
        // 3d文本
		public TextMeshPro tmPro;
        // 包围盒
		protected BoxCollider2D bc2d;

		// 单词数组，第0项为显示的目标单词
		public HLHWord[] wordsArray;

        // 地图事件在小地图上对应的元素对象
		public Transform miniMapInstance;

        // 是否通过单词才能触发
		protected bool isWordTriggered{
			get{
				return wordsArray != null && wordsArray.Length > 0;
			}
		}



		protected virtual void Awake(){
            // 绑定组件         
			mapItemRenderer = GetComponent<SpriteRenderer> ();

			bc2d = GetComponent<BoxCollider2D> ();

			//Transform wordTrans = transform.Find ("Word");
			//if (wordTrans != null) {
			//	tmPro = wordTrans.GetComponent<TextMeshPro> ();
			//}
		}

        /// <summary>
        /// 加入缓存池
        /// </summary>
        /// <param name="pool">Pool.</param>
		public abstract void AddToPool (InstancePool pool);
        
        /// <summary>
        /// 设置显示层级
        /// </summary>
        /// <param name="order">Order.</param>
		public virtual void SetSortingOrder(int order){
			mapItemRenderer.sortingOrder = order;
		}

        /// <summary>
        /// 根据地图附加信息初始化地图事件
        /// </summary>
        /// <param name="mapIndex">Map index.</param>
        /// <param name="attachedInfo">Attached info.</param>
		public abstract void InitializeWithAttachedInfo (int mapIndex, MapAttachedInfoTile attachedInfo);


		/// <summary>
		/// 显示事件单词
		/// </summary>
		protected virtual void CheckIsWordTriggeredAndShow(){

			if (isWordTriggered && tmPro != null) {

				HLHWord targetWord = wordsArray [0];

				tmPro.text = targetWord.spell;

				tmPro.enabled = true;

			} else {
				
				tmPro.enabled = false;

			}

		}

        /// <summary>
        /// 判断玩家是否需要在触发当前地图事件时停止运动
        /// </summary>
        /// <returns><c>true</c>, if player need to stop when entered was ised, <c>false</c> otherwise.</returns>
		public virtual bool IsPlayerNeedToStopWhenEntered (){
			return true;
		}

        /// <summary>
        /// 判断单词在选择错误的时候是否需要显示单词详细信息
        /// </summary>
        /// <returns><c>true</c>, if full word need to show when choose wrong was ised, <c>false</c> otherwise.</returns>
		public virtual bool IsFullWordNeedToShowWhenChooseWrong(){
			return true;
		}

		/// <summary>
		/// 遇到地图事件
		/// </summary>
		/// <param name="bp">Bp.</param>
		public abstract void EnterMapEvent(BattlePlayerController bp);


		/// <summary>
		/// 地图事件触发
		/// </summary>
		/// <param name="isSuccess">If set to <c>true</c> is success.</param>
		/// <param name="bp">Bp.</param>
		public abstract void MapEventTriggered (bool isSuccess,BattlePlayerController bp);

	}


}
