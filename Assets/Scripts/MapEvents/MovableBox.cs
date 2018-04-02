using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	using DG.Tweening;

	public class MovableBox : MapEvent {


		private NewMapGenerator mapGenerator;

		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			SetSortingOrder (-(int)transform.position.y);
		}

		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}



		public void OnAgentPushBox(BattlePlayerController bp,NewMapGenerator mapGenerator){

			this.mapGenerator = mapGenerator;

			ExploreManager em = mapGenerator.GetComponent<ExploreManager> ();
			em.DisableInteractivity ();

			int[,] mapWalkableInfo = mapGenerator.mapWalkableInfoArray;

			int playerPosX = Mathf.RoundToInt (bp.transform.position.x);
			int playerPosY = Mathf.RoundToInt (bp.transform.position.y);

			int boxPosX = Mathf.RoundToInt (this.transform.position.x);
			int boxPosY = Mathf.RoundToInt (this.transform.position.y);


			if (playerPosX < boxPosX && CanMoveTo(boxPosX + 1, boxPosY)) {
				this.transform.DOMove (new Vector3 (boxPosX + 1, boxPosY),1.0f).OnComplete (delegate {
					mapWalkableInfo[boxPosX,boxPosY] = 1;
					mapWalkableInfo[boxPosX + 1,boxPosY] = 0;
					em.EnableInteractivity();
				});

			} else if (playerPosX > boxPosX && CanMoveTo(boxPosX - 1, boxPosY)) {
				this.transform.DOMove (new Vector3 (boxPosX - 1, boxPosY),1.0f).OnComplete (delegate {
					mapWalkableInfo[boxPosX,boxPosY] = 1;
					mapWalkableInfo[boxPosX - 1,boxPosY] = 0;
					em.EnableInteractivity();
				});

			} else if (playerPosY < boxPosY && CanMoveTo(boxPosX, boxPosY + 1)) {
				this.transform.DOMove (new Vector3 (boxPosX, boxPosY + 1),1.0f).OnComplete (delegate {
					mapWalkableInfo[boxPosX,boxPosY] = 1;
					mapWalkableInfo[boxPosX,boxPosY + 1] = 0;
					SetSortingOrder(-boxPosY-1);
					em.EnableInteractivity();
				});

			} else if (playerPosY > boxPosY && CanMoveTo(boxPosX, boxPosY - 1)) {

				this.transform.DOMove (new Vector3 (boxPosX, boxPosY - 1),1.0f).OnComplete (delegate {
					mapWalkableInfo[boxPosX,boxPosY] = 1;
					mapWalkableInfo[boxPosX,boxPosY - 1] = 0;
					SetSortingOrder(-boxPosY+1);
					em.EnableInteractivity();
				});
			}

			em.EnableInteractivity ();

		}

		private bool CanMoveTo(int posX,int posY){

			if (mapGenerator.mapWalkableInfoArray [posX, posY] == 1) {
				return true;
			}

//			Transform mapItem = mapGenerator.GetAliveOtherItemAt (new Vector3 (posX, posY, 0));
//
//
//			if (mapItem != null && mapItem.GetComponent<PressSwitch> () != null) {
//				return true;
//			}

			return false;

		}

		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;
		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			throw new System.NotImplementedException ();
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			throw new System.NotImplementedException ();
		}
	}
}
