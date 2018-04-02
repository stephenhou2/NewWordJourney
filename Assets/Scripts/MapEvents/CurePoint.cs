using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class CurePoint : MapEvent {

		public float curePercentage = 0.1f;

		public Sprite curePointFull;
		public Sprite curePointExausted;

		public bool isExausted;

		private int cureAmount;

		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;
			bc2d.enabled = true;

			isExausted = false;

			cureAmount = int.Parse (KVPair.GetPropertyStringWithKey ("gainAmount", attachedInfo.properties));

			mapItemRenderer.sprite = curePointFull;

			SetSortingOrder (-Mathf.RoundToInt (attachedInfo.position.y));

			CheckIsWordTriggeredAndShow ();

		}

		public override void InitMapItem ()
		{
			throw new System.NotImplementedException ();
		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			if (isExausted) {
				return;
			}



			if (isWordTriggered) {
				ExploreManager.Instance.ShowWordsChoosePlane (wordsArray);
			} else {
				MapEventTriggered (true, bp);
			}
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			bp.agent.health += cureAmount > 0 ? cureAmount : (int)(curePercentage * bp.agent.maxHealth);

			bp.SetEffectAnim ("Healing", null);

			ExploreManager.Instance.UpdatePlayerStatusPlane ();

			tmPro.enabled = false;

			isExausted = true;

			mapItemRenderer.sprite = curePointExausted;
		}

	}
}
