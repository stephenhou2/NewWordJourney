using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DG.Tweening;

	public class GoldPack : MapEvent {

		public int goldAmount;

		public float floatingInterval = 2f;
		public float floatingDistance = 0.1f;

		public Transform goldpackTrans;

		private Sequence floatingSequence;

		private float oriPosY;

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

		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			goldAmount = int.Parse (KVPair.GetPropertyStringWithKey ("gainAmount", attachedInfo.properties));

			if (goldAmount == -1) {
				goldAmount = 100;
			}

			oriPosY = goldpackTrans.localPosition.y;
		
			BeginFloating ();

			CheckIsWordTriggeredAndShow ();
		}
			
		private void BeginFloating(){

			floatingSequence = DOTween.Sequence ();

			float floatingTop = oriPosY + floatingDistance;

			floatingSequence.Append (goldpackTrans.DOLocalMoveY (floatingTop, floatingInterval))
				.Append (goldpackTrans.DOLocalMoveY (oriPosY, floatingInterval));

			floatingSequence.SetLoops (-1);
			floatingSequence.Play ();
		}

		private void StopFloating(){

			floatingSequence.Kill (false);

			goldpackTrans.localPosition = new Vector3 (goldpackTrans.localPosition.x, oriPosY, goldpackTrans.localPosition.z);

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
				(bp.agent as Player).totalGold += goldAmount;
				ExploreManager.Instance.UpdatePlayerStatusPlane ();
				string tintText = string.Format ("+{0}", goldAmount);
				ExploreManager.Instance.ShowTint (tintText, null);
			}

			AddToPool (ExploreManager.Instance.newMapGenerator.mapEventsPool);
		}

	}
}
