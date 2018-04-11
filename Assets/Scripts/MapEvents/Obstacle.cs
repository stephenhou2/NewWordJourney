using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Obstacle : MapEvent {

		public string destroyToolName;

		public Animator mapItemAnimator;

		// 掉落概率（百分制）
		private int dropRate;

		private int dropItemId;


		private bool isItemDropped{
			get{
				return Random.Range (0, 100) < dropRate;
			}
		}



		private void SetAnimationSortingOrder(int order){
			mapItemAnimator.GetComponent<SpriteRenderer> ().sortingOrder = order;
		}
			
		public override void AddToPool(InstancePool pool){
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		/// <summary>
		/// 地图物品被破坏或开启
		/// </summary>
		/// <param name="cb">Cb.</param>
		public void DestroyObstacle(CallBack cb){

			animEndCallBack = cb;

			bc2d.enabled = false;

			mapItemRenderer.enabled = false;

			mapItemAnimator.gameObject.SetActive (true);
			// 播放对应动画
			mapItemAnimator.SetTrigger ("Play");

			StartCoroutine ("ResetMapItemOnAnimFinished");
		}

		/// <summary>
		/// 动画结束后重置地图物品
		/// </summary>
		/// <returns>The map item on animation finished.</returns>
		protected IEnumerator ResetMapItemOnAnimFinished(){

			yield return null;

			float animTime = mapItemAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime;

			while (animTime < 1) {

				yield return null;

				animTime = mapItemAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime;

			}

			// 障碍物清除之后在图层内层级下调一级低层级（防止人物在上面走的时候遮挡住人物）
			int sortingOrder = mapItemRenderer.sortingOrder - 1;
			SetSortingOrder (sortingOrder);
			SetAnimationSortingOrder (sortingOrder);
			AnimEnd ();

		}


		protected void AnimEnd (){
			if (animEndCallBack != null) {
				animEndCallBack ();
			}
		}

		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			dropItemId = int.Parse (KVPair.GetPropertyStringWithKey ("dropItemID", attachedInfo.properties));

			if (dropItemId == -1) {
				dropRate = 0;
			} else {
				dropRate = int.Parse (KVPair.GetPropertyStringWithKey ("dropRate", attachedInfo.properties));
			}


			bc2d.enabled = true;
			mapItemAnimator.gameObject.SetActive (false);
			mapItemRenderer.enabled = true;
			int sortingOrder = -(int)transform.position.y;
			SetSortingOrder (sortingOrder);
			SetAnimationSortingOrder (sortingOrder);

			CheckIsWordTriggeredAndShow ();

		}


		public override void EnterMapEvent(BattlePlayerController bp)
		{
			if (isWordTriggered) {
				ExploreManager.Instance.ShowWordsChoosePlane (wordsArray);
			} else {
				Debug.Log ("非单词触发的障碍物");
			}
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			DestroyObstacle (delegate() {
				if(isSuccess && isItemDropped){
					Item itemDropped = Item.NewItemWith(dropItemId,1);
					(bp.agent as Player).AddItem(itemDropped);
				}
				ExploreManager.Instance.ResetMapWalkableInfo(transform.position,1);
				AddToPool (ExploreManager.Instance.newMapGenerator.mapEventsPool);
			});



		}

	}
}
