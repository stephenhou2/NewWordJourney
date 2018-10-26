using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
   
    public class MapNPC : MapWalkableEvent
    {
        // npc 的 id
        private int npcId;
        // npc
        [HideInInspector] public HLHNPC npc;
        // 战胜npc的奖励      
        [HideInInspector] public HLHNPCReward fightReward;
        // 是否需要进行位置修正
        public bool needPosFix;
      
        public override void AddToPool(InstancePool pool)
        {
            bc2d.enabled = false;
            //DisableAllDetect();
			if(delayMoveCoroutine != null){
				StopCoroutine(delayMoveCoroutine);
			}
            //StopCoroutine("DelayedMovement");
            pool.AddInstanceToPool(this.gameObject);
            StopMoveImmidiately();
            gameObject.SetActive(false);
            pool.AddInstanceToPool(this.gameObject);
        }

        public void SetNpcId(int npcId)
        {
            this.npcId = npcId;
        }

        public override void QuitFightAndDelayMove(int delay)
        {

            StopMoveImmidiately();

            isInAutoWalk = false;
            isTriggered = false;

			if (delayMoveCoroutine != null)
            {
                StopCoroutine(delayMoveCoroutine);
            }

			delayMoveCoroutine = DelayedMovement(delay);
			StartCoroutine(delayMoveCoroutine);

        }

        /// <summary>
        /// 延迟移动【暂时没有用，npc不动】
        /// </summary>
        /// <returns>The movement.</returns>
        /// <param name="delay">Delay.</param>
        private IEnumerator DelayedMovement(int delay)
        {         
            yield return new WaitForSeconds(delay);
         
            StartMove();

        }
              
        
        /// <summary>
        /// 初始化地图npc
        /// </summary>
        /// <param name="mapIndex">Map index.</param>
        /// <param name="attachedInfo">Attached info.</param>
        public override void InitializeWithAttachedInfo(int mapIndex, MapAttachedInfoTile attachedInfo)
        {
            transform.position = attachedInfo.position;

			this.moveOrigin = attachedInfo.position;

            this.moveDestination = attachedInfo.position;


            baCtr.SetSortingOrder(-(int)transform.position.y);

			npc = null;

			for (int i = 0; i < GameManager.Instance.gameDataCenter.currentMapEventsRecord.npcPosArray.Length;i++){

				Vector2 pos = GameManager.Instance.gameDataCenter.currentMapEventsRecord.npcPosArray[i];
                            
				if(MyTool.ApproximatelySameIntPosition2D(pos,attachedInfo.position)){
					npc = GameManager.Instance.gameDataCenter.currentMapEventsRecord.npcArray[i];
					break;
				}            
			}

			if(npc == null){
				npc = GameManager.Instance.gameDataCenter.LoadNpc(npcId);
				for (int i = 0; i < GameManager.Instance.gameDataCenter.currentMapEventsRecord.npcPosArray.Length; i++)
                {

                    Vector2 pos = GameManager.Instance.gameDataCenter.currentMapEventsRecord.npcPosArray[i];

                    if (pos == -Vector2.one)
                    {
						GameManager.Instance.gameDataCenter.currentMapEventsRecord.npcPosArray[i] = transform.position;
						GameManager.Instance.gameDataCenter.currentMapEventsRecord.npcArray[i] = npc;
                        break;
                    }

                    
                }
			}
         
			bc2d.enabled = true;

            gameObject.SetActive(true);

            baCtr.PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);

            bc2d.enabled = true;

            isTriggered = false;

            isInAutoWalk = false;

            needPosFix = false;

			Sprite miniMapNpcSprite = GameManager.Instance.gameDataCenter.allMiniMapSprites.Find(delegate (Sprite obj)
            {

				return obj.name == string.Format("MiniMapNPC_{0}",npc.npcId);
            });

            miniMapInstance.GetComponent<SpriteRenderer>().sprite = miniMapNpcSprite;

        }

        
        /// <summary>
        /// 进入地图npc事件中
        /// </summary>
        /// <param name="bp">Bp.</param>
        public override void EnterMapEvent(BattlePlayerController bp)
        {
			if(ExploreManager.Instance.expUICtr.rejectNewUI){
				bp.isInEvent = false;
				return;
			}

			ExploreManager.Instance.expUICtr.rejectNewUI = true;

            if (isInMoving)
            {
                RefreshWalkableInfoWhenTriggeredInMoving();
            }

            bp.isInEvent = true;

            bp.FixPositionToStandard();

            MapEventTriggered(false, bp);
        }

        /// <summary>
        /// 地图事件触发后的逻辑
        /// </summary>
        /// <param name="isFromDetect">If set to <c>true</c> is from detect.</param>
        /// <param name="bp">Bp.</param>
        public override void MapEventTriggered(bool isFromDetect, BattlePlayerController bp)
        {
            bp.boxCollider.enabled = false;

            if (isTriggered)
            {
                return;
            }

            ExploreManager.Instance.DisableExploreInteractivity();

            ExploreManager.Instance.MapWalkableEventsStopAction();

            StopMoveImmidiately();

            ExploreManager.Instance.currentEnteredMapEvent = this;
         
            if (canMove)
            {
				IEnumerator adjustAndFightCoroutine = AdjustPositionAndTowards(bp);
				StartCoroutine(adjustAndFightCoroutine);
            }
            else
            {
				IEnumerator adjustCoroutine = AdjustTowards(bp);
				StartCoroutine(adjustCoroutine);
            }

            isTriggered = true;

        }

        public override void ResetWhenDie()
        {
            StopAllCoroutines();
			bc2d.enabled = false;
        }

      
        /// <summary>
        /// 与npc相遇时只调整方向，不调整位置
        /// </summary>
        /// <param name="battlePlayerCtr">Battle player ctr.</param>
        private IEnumerator AdjustTowards(BattlePlayerController battlePlayerCtr)
        {

            yield return new WaitUntil(() => battlePlayerCtr.isIdle);

            float posOffsetX = battlePlayerCtr.transform.position.x - this.transform.position.x;
            float posOffsetY = battlePlayerCtr.transform.position.y - this.transform.position.y;

            if (posOffsetX > 0.1f)
            {
                battlePlayerCtr.TowardsLeft();
                baCtr.TowardsRight(false);
            }
            else if (posOffsetX < -0.1f)
            {
                battlePlayerCtr.TowardsRight();
                baCtr.TowardsLeft(false);
            }
            else if (posOffsetY > 0.1f)
            {
                battlePlayerCtr.TowardsDown();
                baCtr.TowardsUp(false);
            }
            else
            {
                battlePlayerCtr.TowardsUp();
                baCtr.TowardsDown(false);
            }

            switch (battlePlayerCtr.towards)
            {
                case MyTowards.Up:
                    if (battlePlayerCtr.transform.position.x < transform.position.x)
                    {
                        baCtr.TowardsLeft();
                        if (battlePlayerCtr.transform.position.x <= transform.position.x - 0.5f)
                        {
                            battlePlayerCtr.TowardsRight();
                        }
                        else if (battlePlayerCtr.transform.position.x >= transform.position.x + 0.5f)
                        {
                            battlePlayerCtr.TowardsLeft();
                        }
                    }
                    else
                    {
                        baCtr.TowardsRight();
                        if (battlePlayerCtr.transform.position.x <= transform.position.x - 0.5f)
                        {
                            battlePlayerCtr.TowardsRight();
                        }
                        else if (battlePlayerCtr.transform.position.x >= transform.position.x + 0.5f)
                        {
                            battlePlayerCtr.TowardsLeft();
                        }
                    }
                    break;
                case MyTowards.Down:
                    if (battlePlayerCtr.transform.position.x < transform.position.x)
                    {
                        baCtr.TowardsLeft();
                        if (battlePlayerCtr.transform.position.x <= transform.position.x - 0.5f)
                        {
                            battlePlayerCtr.TowardsRight();
                        }
                        else if (battlePlayerCtr.transform.position.x >= transform.position.x + 0.5f)
                        {
                            battlePlayerCtr.TowardsLeft();
                        }
                    }
                    else
                    {
                        baCtr.TowardsRight();
                        if (battlePlayerCtr.transform.position.x <= transform.position.x - 0.5f)
                        {
                            battlePlayerCtr.TowardsRight();
                        }
                        else if (battlePlayerCtr.transform.position.x >= transform.position.x + 0.5f)
                        {
                            battlePlayerCtr.TowardsLeft();
                        }
                    }
                    break;
                case MyTowards.Left:
                    baCtr.TowardsRight();
                    break;
                case MyTowards.Right:
                    baCtr.TowardsLeft();
                    break;
            }

            ExploreManager.Instance.ShowNPCPlane(this);

        }

      
        /// <summary>
        /// 调整npc的位置和朝向
        /// </summary>
        /// <returns>The position and towards.</returns>
        /// <param name="battlePlayerCtr">Battle player ctr.</param>
		private IEnumerator AdjustPositionAndTowards(BattlePlayerController battlePlayerCtr)
		{

			yield return new WaitUntil(() => battlePlayerCtr.isIdle);

			Vector3 playerOriPos = battlePlayerCtr.transform.position;
			Vector3 npcOriPos = transform.position;

			int playerPosX = Mathf.RoundToInt(playerOriPos.x);
			int playerPosY = Mathf.RoundToInt(playerOriPos.y);
			int npcPosX = Mathf.RoundToInt(npcOriPos.x);
			int npcPosY = Mathf.RoundToInt(npcOriPos.y);

			int posOffsetX = playerPosX - npcPosX;
			int posOffsetY = playerPosY - npcPosY;

			int npcLayerOrder = -npcPosY;

			Vector3 npcFightPos = Vector3.zero;
			Vector3 playerFightPos = new Vector3(playerPosX, playerPosY, 0);


			int minX = 0;
			int maxX = ExploreManager.Instance.newMapGenerator.rows - 1;

			if (posOffsetX > 0)
			{
				battlePlayerCtr.TowardsLeft(true);
				if (playerPosX - 1 >= minX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[playerPosX - 1, playerPosY] == 1)
				{
					npcFightPos = new Vector3(playerPosX - 1, playerOriPos.y, 0);
					needPosFix = false;
					battlePlayerCtr.needPosFix = false;
				}
				else if (playerPosX - 1 >= minX && playerPosX - 1 == npcPosX && playerPosY == npcPosY)
				{
					npcFightPos = new Vector3(playerPosX - 1, playerOriPos.y, 0);
					needPosFix = false;
					battlePlayerCtr.needPosFix = false;
				}
				else if (playerPosX - 1 >= minX && playerPosX - 1 == Mathf.RoundToInt(moveDestination.x) && playerPosY == Mathf.RoundToInt(moveDestination.y))
				{
					npcFightPos = new Vector3(playerPosX - 1, playerOriPos.y, 0);
					needPosFix = false;
					battlePlayerCtr.needPosFix = false;
				}
				else
				{
					if (posOffsetY > 0)
					{

						npcFightPos = new Vector3(playerPosX + 0.25f, playerPosY - 0.15f, 0);
						npcLayerOrder = -playerPosY + 1;

						playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY + 0.15f, 0);

						baCtr.SetSortingOrder(-playerPosY + 1);

					}
					else
					{

						npcFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);
						npcLayerOrder = -playerPosY - 1;

						playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);

						baCtr.SetSortingOrder(-playerPosY - 1);
					}

					needPosFix = true;
					battlePlayerCtr.needPosFix = true;
				}
			}
			else if (posOffsetX == 0)
			{
				if (playerPosX + 1 <= maxX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[playerPosX + 1, playerPosY] == 1)
				{
					battlePlayerCtr.TowardsRight(true);
					npcFightPos = new Vector3(playerPosX + 1, playerOriPos.y, 0);
					needPosFix = false;
					battlePlayerCtr.needPosFix = false;
				}
				else if (playerPosX - 1 >= minX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[playerPosX - 1, playerPosY] == 1)
				{
					battlePlayerCtr.TowardsLeft(true);
					npcFightPos = new Vector3(playerPosX - 1, playerOriPos.y, 0);
					needPosFix = false;
					battlePlayerCtr.needPosFix = false;
				}
				else
				{
					battlePlayerCtr.TowardsRight(true);
					if (posOffsetY > 0)
					{

						npcFightPos = new Vector3(playerPosX + 0.25f, playerPosY - 0.15f, 0);
						npcLayerOrder = -playerPosY + 1;

						playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY + 0.15f, 0);

						baCtr.SetSortingOrder(-playerPosY + 1);
                  
					}
					else
					{

						npcFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);
						npcLayerOrder = -playerPosY - 1;

						playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);

						baCtr.SetSortingOrder(-playerPosY - 1);
					}

					needPosFix = true;
					battlePlayerCtr.needPosFix = true;
				}
			}
			else if (posOffsetX < 0)
			{
				battlePlayerCtr.TowardsRight(true);
				if (playerPosX + 1 <= maxX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[playerPosX + 1, playerPosY] == 1)
				{
					npcFightPos = new Vector3(playerPosX + 1, playerOriPos.y, 0);
					needPosFix = false;
					battlePlayerCtr.needPosFix = false;
				}
				else if (playerPosX + 1 <= maxX && playerPosX + 1 == npcPosX && playerPosY == npcPosY)
				{
					npcFightPos = new Vector3(playerPosX + 1, playerOriPos.y, 0);
					needPosFix = false;
					battlePlayerCtr.needPosFix = false;
				}
				else if (playerPosX + 1 <= maxX && playerPosX + 1 == Mathf.RoundToInt(moveDestination.x) && playerPosY == Mathf.RoundToInt(moveDestination.y))
				{
					npcFightPos = new Vector3(playerPosX + 1, playerOriPos.y, 0);
					needPosFix = false;
					battlePlayerCtr.needPosFix = false;
				}
				else
				{
					if (posOffsetY > 0)
					{

						npcFightPos = new Vector3(playerPosX + 0.25f, playerPosY - 0.15f, 0);
						npcLayerOrder = -playerPosY + 1;

						playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY + 0.15f, 0);

						baCtr.SetSortingOrder(-playerPosY + 1);

					}
					else
					{

						npcFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);
						npcLayerOrder = -playerPosY - 1;

						playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);

						baCtr.SetSortingOrder(-playerPosY - 1);

					}

					needPosFix = true;
					battlePlayerCtr.needPosFix = true;
				}


				battlePlayerCtr.FixPosTo(playerFightPos, null);

				RunToPosition(npcFightPos, delegate
				{
					if (transform.position.x <= ExploreManager.Instance.battlePlayerCtr.transform.position.x)
					{
						baCtr.TowardsRight();
					}
					else
					{
						baCtr.TowardsLeft();
					}
					ExploreManager.Instance.ShowNPCPlane(this);
				}, npcLayerOrder);

			}

		}
        public override void SetSortingOrder(int order)
        {
            baCtr.SetSortingOrder(order);
        }


        /// <summary>
        /// 行走到指定位置【暂时没有用，npc不会动】
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="cb">Cb.</param>
        /// <param name="showAlertArea">If set to <c>true</c> show alert area.</param>
        public override void WalkToPosition(Vector3 position, CallBack cb, bool showAlertArea = true)
        {

            int oriPosX = Mathf.RoundToInt(transform.position.x);
            int oriPosY = Mathf.RoundToInt(transform.position.y);

            int targetPosX = Mathf.RoundToInt(position.x);
            int targetPosY = Mathf.RoundToInt(position.y);


            moveOrigin = new Vector3(oriPosX, oriPosY, 0);
            moveDestination = new Vector3(targetPosX, targetPosY, 0);


            if (MyTool.ApproximatelySamePosition2D(position, ExploreManager.Instance.battlePlayerCtr.transform.position))
            {
                return;
            }

            baCtr.PlayRoleAnim(CommonData.roleWalkAnimName, 0, null);

            //Debug.LogFormat("MOVE ORIGIN:{0}++++++MOVE DESTINATION:{1}", moveOrigin, moveDestination);

            RefreshWalkableInfoWhenStartMove();


            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            float timeScale = 3f;

            if (position.x >= transform.position.x)
            {
                baCtr.TowardsRight();
            }
            else
            {
                baCtr.TowardsLeft();
            }

            moveCoroutine = MoveTo(position, timeScale, delegate {

                this.transform.position = position;

                baCtr.PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);

                if (cb != null)
                {
                    cb();
                }
                SetSortingOrder(-Mathf.RoundToInt(position.y));

                if (needPosFix)
                {
                    needPosFix = false;
                }

            });

            StartCoroutine(moveCoroutine);

        }

        /// <summary>
        /// 跑到指定位置【暂时没有用，npc不会动】
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="cb">Cb.</param>
        /// <param name="layerOrder">Layer order.</param>
        protected override void RunToPosition(Vector3 position, CallBack cb, int layerOrder)
        {

            int oriPosX = Mathf.RoundToInt(transform.position.x);
            int oriPosY = Mathf.RoundToInt(transform.position.y);

            int targetPosX = Mathf.RoundToInt(position.x);
            int targetPosY = Mathf.RoundToInt(position.y);

            moveOrigin = new Vector3(oriPosX, oriPosY, 0);
            moveDestination = new Vector3(targetPosX, targetPosY, 0);

            //Debug.LogFormat ("MOVE ORIGIN:{0}++++++MOVE DESTINATION:{1}", moveOrigin, moveDestination);


            if (position.Equals(transform.position))
            {

                if (cb != null)
                {
                    cb();
                }

                return;
            }

            baCtr.PlayRoleAnim(CommonData.roleRunAnimName, 0, null);


            RefreshWalkableInfoWhenStartMove();

            float timeScale = 1f;

            if (position.x >= transform.position.x + 0.2f)
            {
                baCtr.TowardsRight();
            }
            else if (position.x <= transform.position.x - 0.2f)
            {
                baCtr.TowardsLeft();
            }

            moveCoroutine = MoveTo(position, timeScale, delegate {

                baCtr.PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);

                if (cb != null)
                {
                    cb();
                }

                SetSortingOrder(layerOrder);
            });

            StartCoroutine(moveCoroutine);

        }

    }
}
