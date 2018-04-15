using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DG.Tweening;

	public class GoldPack : MapEvent {

		public int goldAmount;

		public Animator mapItemAnimator;


		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			Count goldAmountRange = GameManager.Instance.gameDataCenter.gameLevelDatas [Player.mainPlayer.currentLevelIndex].goldAmountRange;
				
			goldAmount = goldAmountRange.GetAValueWithinRange ();
				
			bc2d.enabled = true;
			mapItemAnimator.gameObject.SetActive (false);
			mapItemRenderer.enabled = true;

			SetSortingOrder (-(int)transform.position.y);

			SetAnimationSortingOrder (-(int)transform.position.y);

			CheckIsWordTriggeredAndShow ();
		}
			

		private void SetAnimationSortingOrder(int order){
			mapItemAnimator.GetComponent<SpriteRenderer> ().sortingOrder = order;
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
			bc2d.enabled = false;

			mapItemRenderer.enabled = false;

			mapItemAnimator.gameObject.SetActive (true);
			// 播放对应动画
			mapItemAnimator.SetTrigger ("Play");

			IEnumerator openGoldPackCoroutine = LatelyOpenGoldPack (isSuccess, bp);

			StartCoroutine (openGoldPackCoroutine);
		}

		private IEnumerator LatelyOpenGoldPack(bool isSuccess,BattlePlayerController bp){

			yield return null;

			float animTime = mapItemAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime;

			while (animTime < 1) {

				yield return null;

				animTime = mapItemAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime;

			}

			if (isSuccess) {
				(bp.agent as Player).totalGold += goldAmount + bp.agent.extraGold;
				ExploreManager.Instance.UpdatePlayerStatusPlane ();
				string tintText = string.Format ("+{0}", goldAmount);
				ExploreManager.Instance.ShowTint (tintText, null);
			}
			int posX = Mathf.RoundToInt (transform.position.x);
			int posY = Mathf.RoundToInt (transform.position.y);
			ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [posX, posY] = 1;

			AddToPool (ExploreManager.Instance.newMapGenerator.mapEventsPool);
		}



	}
}
