using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	public abstract class MapWalkableEvent : MapEvent {


		public bool canMove;

		public bool isInAutoWalk;

		public bool isTriggered;

		public bool canEnterFight;

		public bool isInMoving;

		protected IEnumerator moveCoroutine;

		private IEnumerator autoWalkCoroutine;

		protected IEnumerator delayMoveCoroutine;

		public Vector3 moveOrigin;
		public Vector3 moveDestination;
        
		public int movableDistance = 10;

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
			
			Transform bpTrans = ExploreManager.Instance.battlePlayerCtr.transform;
            
			float distance = (bpTrans.position - this.transform.position).magnitude;

			if(distance > movableDistance){
				baCtr.KillRoleAnim();            
				isInMoving = false;
				isInAutoWalk = false;
				return;
			}

			if (!canMove && !baCtr.isIdle && !baCtr.IsInAnimPlaying()) {
				baCtr.PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
				return;
			}

			if(!canMove){
				return;
			}

			if (isInAutoWalk) {
				return;
			}

			if (isTriggered) {
				return;
			}

			if(autoWalkCoroutine != null){
				StopCoroutine(autoWalkCoroutine);
			}

			autoWalkCoroutine = AutoWalk();	
			StartCoroutine (autoWalkCoroutine);

		}



		public void StopMoveImmidiately(){
			if (moveCoroutine != null) {
				StopCoroutine (moveCoroutine);
			}
			if(autoWalkCoroutine != null){
				StopCoroutine(autoWalkCoroutine);
			}

			if (delayMoveCoroutine != null)
            {
                StopCoroutine(delayMoveCoroutine);
            }
			isInAutoWalk = false;
            isInMoving = false;
			if (!baCtr.isIdle) {
				baCtr.PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);
			}
		}

		public void StopMoveAtEndOfCurrentMove(){

			if(autoWalkCoroutine != null){
				StopCoroutine(autoWalkCoroutine);
			}
         
			if(delayMoveCoroutine != null){
				StopCoroutine(delayMoveCoroutine);
			}

			isInAutoWalk = false;

			Transform bpTrans = ExploreManager.Instance.battlePlayerCtr.transform;

            float distance = (bpTrans.position - this.transform.position).magnitude;

            if (distance > movableDistance)
            {
                baCtr.KillRoleAnim();
				isInAutoWalk = false;
            }
		}


		protected IEnumerator AutoWalk(){

			if(!baCtr.isIdle){
				baCtr.PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
			}

			isInAutoWalk = true;
            
			while (true)
            {           

				float standDuration = Random.Range(2.0f, 4.0f);
            
                float timer = 0;

				while(isInMoving){
					yield return null;
				}

                while (timer < standDuration)
                {

                    yield return null;

                    timer += Time.deltaTime;   
                                   
                }  


                            
                if (CheckCanWalk())
                {
					isInMoving = true;

					Walk(delegate{

						Transform bpTrans = ExploreManager.Instance.battlePlayerCtr.transform;
                        float distance = (bpTrans.position - this.transform.position).magnitude;

						if (distance > movableDistance)
                        {
                            baCtr.KillRoleAnim();
                            isInAutoWalk = false;
							if(autoWalkCoroutine != null){
								StopCoroutine(autoWalkCoroutine);
							}
                        }
					});
                }
            }





		}
        
		protected void Walk(CallBack callBack){

			int[,] mapWalkableInfo = ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray;
			int[,] mapWalkableEventInfoArray = ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray;

			Vector3 randomPositionAround = transform.position;

			bool validPositionAround = false;

			int count = 0;

			while (!validPositionAround) {

				randomPositionAround = GetRandomPositionAround (transform.position);

				if ((randomPositionAround - transform.position).magnitude < 0.9f) {
					validPositionAround = true;
				} else {

					int posX = Mathf.RoundToInt (randomPositionAround.x);
					int posY = Mathf.RoundToInt (randomPositionAround.y);

					validPositionAround = mapWalkableInfo [posX, posY] == 1
					&& mapWalkableEventInfoArray [posX, posY] == 0;

					count++;

					if (count >= 100) {
						randomPositionAround = transform.position;
						break;
					}
				}
			}

			WalkToPosition (randomPositionAround,callBack);

		}

		public abstract void ResetWhenDie ();

        public abstract void WalkToPosition(Vector3 position, CallBack cb, bool showAlertArea = true);
        protected abstract void RunToPosition(Vector3 position, CallBack cb, int layerOrder);

		public abstract void QuitFightAndDelayMove (int delay);

		protected IEnumerator MoveTo(Vector3 position,float timeScale,CallBack cb){

			isInMoving = true;
//			int targetPosX = Mathf.RoundToInt (position.x);
//			int targetPosY = Mathf.RoundToInt (position.y);
//
//			float distance = Mathf.Sqrt ((targetPosX - transform.position.x) * (targetPosX - transform.position.x) + 
//				(targetPosY - transform.position.y) * (targetPosY - transform.position.y));
//
//			float moveDuration = baCtr.moveDuration * distance * timeScale;
//
//			Vector3 moveVector = new Vector3((targetPosX - transform.position.x) / moveDuration,(targetPosY - transform.position.y) / moveDuration,0);
//
//			float timer = 0;
//
//			while (timer < moveDuration) {
//
//				transform.position += moveVector * Time.deltaTime;
//
//				timer += Time.deltaTime;
//
//				yield return null;
//
//			}
//
//			transform.position = position;
//
//			if (cb != null) {
//				cb ();
//			}

//			int targetPosX = Mathf.RoundToInt (position.x);
//			int targetPosY = Mathf.RoundToInt (position.y);

			float distance = Mathf.Sqrt ((position.x - transform.position.x) * (position.x - transform.position.x) + 
				(position.y - transform.position.y) * (position.y - transform.position.y));

			float moveDuration = baCtr.moveDuration * distance * timeScale;

			Vector3 moveVector = new Vector3((position.x - transform.position.x) / moveDuration,(position.y - transform.position.y) / moveDuration,0);

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

			isInMoving = false;

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

			if (posX >= ExploreManager.Instance.newMapGenerator.columns || posX <= 0
			   || posY >= ExploreManager.Instance.newMapGenerator.rows || posY <= 0) {
				return false;
			}

			return mapWalkableInfo [posX, posY] == 1 && mapWalkableEventsLayoutInfo [posX, posY] == 0;
		}

		protected Vector3 GetRandomPositionAround(Vector3 position){

			int posX = Mathf.RoundToInt (position.x);
			int posY = Mathf.RoundToInt (position.y);

			Vector3 oriIntergerPos = new Vector3 (posX, posY, 0);

			if(!MyTool.ApproximatelySamePosition2D(position,oriIntergerPos)){
				return oriIntergerPos;
			}

			int directionSeed = Random.Range (0, 4);

			Vector3 randomPosition = position;

			switch (directionSeed) {
			case 0:
				if (posY + 1 < ExploreManager.Instance.newMapGenerator.rows) {
					randomPosition = new Vector3 (posX, posY + 1, position.z);
				}
				break;
			case 1:
				if (posY - 1 > 0) {
					randomPosition = new Vector3 (posX, posY - 1, position.z);
				}
				break;
			case 2:
				if (posX - 1 > 0) {
					randomPosition = new Vector3 (posX - 1, posY, position.z);
				}
				break;
			case 3:
				if (posX + 1 < ExploreManager.Instance.newMapGenerator.columns) {
					randomPosition = new Vector3 (posX + 1, posY, position.z);
				}
				break;

			}

			return randomPosition;

		}

		public void RefreshWalkableInfoWhenStartMove(){

			int walkableEventOriPosX = Mathf.RoundToInt(moveOrigin.x);
			int walkableEventOriPosY = Mathf.RoundToInt(moveOrigin.y);

			int walkableEventDestPosX = Mathf.RoundToInt (moveDestination.x);
			int walkableEventDestPosY = Mathf.RoundToInt (moveDestination.y);         

			ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [walkableEventOriPosX, walkableEventOriPosY] = 1;
			ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [walkableEventDestPosX, walkableEventDestPosY] = 5;

			ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray [walkableEventOriPosX, walkableEventOriPosY] = 0;
			ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray [walkableEventDestPosX, walkableEventDestPosY] = 1;

            //Debug.LogFormat("开始移动时 更新行走信息[{0},{1}]:1,[{2},{3}]:5,更新行走事件信息[{4},{5}]:0,[{6},{7}]:1", walkableEventOriPosX, walkableEventOriPosY, walkableEventDestPosX, walkableEventDestPosY,
                            //walkableEventOriPosX, walkableEventOriPosY, walkableEventDestPosX, walkableEventDestPosY);

		}

		/// <summary>
		/// 如果移动中被触发了，则需要将移动终点的可行走信息和可移动事件信息更新
		/// </summary>
		public void RefreshWalkableInfoWhenTriggeredInMoving(){

			int walkableEventDestPosX = Mathf.RoundToInt (moveDestination.x);
			int walkableEventDestPosY = Mathf.RoundToInt (moveDestination.y);

			ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [walkableEventDestPosX, walkableEventDestPosY] = 1;
			ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray [walkableEventDestPosX, walkableEventDestPosY] = 0;

            //Debug.LogFormat("行走中碰到 更新行走信息[{0},{1}]:1,更新行走事件信息[{2},{3}]:0",  walkableEventDestPosX, walkableEventDestPosY,
                          //walkableEventDestPosX, walkableEventDestPosY);

		}
			

		/// <summary>
		/// 当和地图上的可行走事件（怪物／npc）交互（战斗，交谈等）结束后，重置行走信息和可行走事件位置信息
		/// </summary>
		public void RefreshWalkableInfoWhenQuit(bool walkableEventDie){
         
			int walkableEventOriPosX = Mathf.RoundToInt (moveOrigin.x);
			int walkableEventOriPosY = Mathf.RoundToInt (moveOrigin.y);

			int walkableEventDestPosX = Mathf.RoundToInt (moveDestination.x);
			int walkableEventDestPosY = Mathf.RoundToInt (moveDestination.y);

			if(walkableEventDie || (walkableEventOriPosX != walkableEventDestPosX && walkableEventOriPosY != walkableEventDestPosY)){
                ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[walkableEventOriPosX, walkableEventOriPosY] = 1;
                ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray[walkableEventOriPosX, walkableEventOriPosY] = 0;
                //Debug.LogFormat("事件结束时 更新行走信息[{0},{1}]:1,更新行走事件信息[{2},{3}]:0", walkableEventOriPosX, walkableEventOriPosY,
                           //walkableEventOriPosX, walkableEventOriPosY);
            }
			

			if (walkableEventDie) {

				ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[walkableEventOriPosX, walkableEventOriPosY] = 1;
                ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray[walkableEventOriPosX, walkableEventOriPosY] = 0;

				ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [walkableEventDestPosX, walkableEventDestPosY] = 1;
				ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray [walkableEventDestPosX, walkableEventDestPosY] = 0;

                //Debug.LogFormat("死亡后 更新行走信息[{0},{1}]:1,更新行走事件信息[{2},{3}]:0", walkableEventDestPosX, walkableEventDestPosY,
                         //walkableEventDestPosX, walkableEventDestPosY);
			}

           
	

		}



	}
}
