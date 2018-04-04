using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public abstract class MapWalkableEvent : MapEvent {


		protected bool canMove;

		protected bool isInAutoWalk;

		protected IEnumerator moveCoroutine;

		protected BattleMonsterController mBaCtr;
		protected BattleMonsterController baCtr{
			get{
				if (mBaCtr == null) {
					mBaCtr = GetComponent<BattleMonsterController> ();
				}

				return mBaCtr;
			}
		}


		public void StartMove(){
			if (!canMove) {
				return;
			}

			if (isInAutoWalk) {
				return;
			}
			StartCoroutine ("AutoWalk");
			isInAutoWalk = true;
		}

		public void StopMoveImmidiately(){
			if (moveCoroutine != null) {
				StopCoroutine (moveCoroutine);
			}
			StopCoroutine("AutoWalk");
			isInAutoWalk = false;
		}

		public void StopMoveAtEndOfCurrentMove(){
			StopCoroutine("AutoWalk");
			isInAutoWalk = false;
		}


		private IEnumerator AutoWalk(){

			while (true) {

				float standDuration = Random.Range (2.0f, 4.0f);

				float timer = 0;

				while (timer < standDuration) {

					yield return null;

					timer += Time.deltaTime;

				}

				if (CheckCanWalk ()) {
					Walk ();
				}
			}

		}

		protected void Walk(){

			int[,] mapWalkableInfo = ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray;
			int[,] mapWalkableEventInfoArray = ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray;

			Vector3 randomPositionAround = transform.position;

			bool validPositionAround = false;

			while (!validPositionAround) {

				randomPositionAround = GetRandomPositionAround (transform.position);
				int posX = Mathf.RoundToInt (randomPositionAround.x);
				int posY = Mathf.RoundToInt (randomPositionAround.y);
				validPositionAround = mapWalkableInfo [posX, posY] == 1 
					&& mapWalkableEventInfoArray[posX,posY] == 0;
			}

			WalkToPosition (randomPositionAround,null);

		}

		public abstract void WalkToPosition (Vector3 position, CallBack cb, bool showAlertArea = true);

		protected abstract void RunToPosition (Vector3 position, CallBack cb);

		protected IEnumerator MoveTo(Vector3 position,float timeScale,CallBack cb){

			int oriPosX = Mathf.RoundToInt (transform.position.x);
			int oriPosY = Mathf.RoundToInt (transform.position.y);

			int targetPosX = Mathf.RoundToInt (position.x);
			int targetPosY = Mathf.RoundToInt (position.y);

			float distance = Mathf.Sqrt ((targetPosX - oriPosX) * (targetPosX - oriPosX) + (targetPosY - oriPosY) * (targetPosY - oriPosY));

			float moveDuration = baCtr.moveDuration * distance * timeScale;

			Vector3 moveVector = new Vector3((targetPosX - oriPosX) / moveDuration,(targetPosY - oriPosY) / moveDuration,0);

			float timer = 0;

			while (timer < moveDuration) {

				transform.position += moveVector * Time.deltaTime;

				timer += Time.deltaTime;

				yield return null;

			}

			transform.position = position;

			if (cb != null) {
				cb ();
			}

		}



		protected bool CheckCanWalk(){
			int posX = Mathf.RoundToInt (transform.position.x);
			int posY = Mathf.RoundToInt (transform.position.y);
			return IsPositionCanWalk (posX, posY + 1) || IsPositionCanWalk (posX, posY - 1)
				|| IsPositionCanWalk (posX - 1, posY) || IsPositionCanWalk (posX + 1, posY);
		}

		protected bool IsPositionCanWalk(int posX,int posY){

			int[,] mapWalkableInfo = ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray;
			int[,] mapWalkableEventsLayoutInfo = ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray;
			return mapWalkableInfo [posX, posY] == 1 && mapWalkableEventsLayoutInfo [posX, posY] == 0;
		}

		protected Vector3 GetRandomPositionAround(Vector3 position){

			int posX = Mathf.RoundToInt (position.x);
			int posY = Mathf.RoundToInt (position.y);

			int directionSeed = Random.Range (0, 4);

			Vector3 randomPosition = position;

			switch (directionSeed) {
			case 0:
				randomPosition = new Vector3 (posX, posY + 1, position.z);
				break;
			case 1:
				randomPosition = new Vector3 (posX, posY - 1, position.z);
				break;
			case 2:
				randomPosition = new Vector3 (posX - 1, posY, position.z);
				break;
			case 3:
				randomPosition = new Vector3 (posX + 1, posY, position.z);
				break;

			}

			return randomPosition;

		}


	}
}
