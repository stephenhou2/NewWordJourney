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
			ExploreManager.Instance.ShowWordsChoosePlane (wordsArray);
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			bp.isInEvent = false;

			bc2d.enabled = false;

			mapItemRenderer.enabled = false;

			mapItemAnimator.gameObject.SetActive (true);
			// 播放对应动画
			mapItemAnimator.SetTrigger ("Play");

			IEnumerator openGoldPackCoroutine = LatelyOpenGoldPack (isSuccess, bp);

			StartCoroutine (openGoldPackCoroutine);

			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
		}

		private IEnumerator LatelyOpenGoldPack(bool isSuccess,BattlePlayerController bp){

			yield return null;

			float animTime = mapItemAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime;

			while (animTime < 1) {

				yield return null;

				animTime = mapItemAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime;

			}

			if (isSuccess) {
				int goldGain = goldAmount + bp.agent.extraGold;
				(bp.agent as Player).totalGold += goldGain;
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.goldAudioName);
				ExploreManager.Instance.UpdatePlayerStatusPlane ();
				ExploreManager.Instance.expUICtr.SetUpGoldGainTintHUD(goldGain);
			}
			int posX = Mathf.RoundToInt (transform.position.x);
			int posY = Mathf.RoundToInt (transform.position.y);
			ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [posX, posY] = 1;

			AddToPool (ExploreManager.Instance.newMapGenerator.mapEventsPool);
		}



	}
}
