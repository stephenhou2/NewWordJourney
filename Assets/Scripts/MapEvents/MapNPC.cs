using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
   
    public class MapNPC : MapWalkableEvent
    {

        private int npcId;

        [HideInInspector] public HLHNPC npc;

        public NPCAlertArea[] alertAreas;

        [HideInInspector] public HLHNPCReward fightReward;

        public bool needPosFix;


        public override void AddToPool(InstancePool pool)
        {
            bc2d.enabled = false;
            DisableAllDetect();
            StopCoroutine("DelayedMovement");
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

            StopCoroutine("DelayedMovement");

            StartCoroutine("DelayedMovement", delay);

        }


        private IEnumerator DelayedMovement(int delay)
        {

            DisableAllDetect();

            yield return new WaitForSeconds(delay);

            EnableAllDetect();

            StartMove();

        }
              
        private void DisableAllDetect()
        {

            DisableAllAlertAreaDetect();

            bc2d.enabled = false;

        }

        private void EnableAllDetect()
        {
            EnableAllAlertAreaDetect();
            bc2d.enabled = true;
        }

        private void InitAllAlertAreaDetect()
        {
            for (int i = 0; i < alertAreas.Length; i++)
            {
                alertAreas[i].InitializeAlertArea();
            }
        }

        private void EnableAllAlertAreaDetect()
        {
            for (int i = 0; i < alertAreas.Length; i++)
            {
                alertAreas[i].EnableAlertDetect();
            }
        }

        private void DisableAllAlertAreaDetect()
        {
            for (int i = 0; i < alertAreas.Length; i++)
            {
                alertAreas[i].DisableAlertDetect();
            }
        }


        public override void InitializeWithAttachedInfo(int mapIndex, MapAttachedInfoTile attachedInfo)
        {
            transform.position = attachedInfo.position;

			this.moveOrigin = attachedInfo.position;

            this.moveDestination = attachedInfo.position;


            baCtr.SetSortingOrder(-(int)transform.position.y);

            npc = GameManager.Instance.gameDataCenter.LoadNpc(npcId);

            InitAllAlertAreaDetect();

            gameObject.SetActive(true);

            baCtr.PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);

            bc2d.enabled = true;

            isTriggered = false;

            isInAutoWalk = false;

            needPosFix = false;

            for (int i = 0; i < alertAreas.Length; i++)
            {
                alertAreas[i].InitializeAlertArea();
            }

            StartMove();
        }


        public void OnTriggerEnter2D(Collider2D col)
        {

            BattlePlayerController bp = col.GetComponent<BattlePlayerController>();

            if (bp == null)
            {
                return;
            }

            if (bp.isInEvent)
            {
                return;
            }

            if (bp.needPosFix)
            {
                return;
            }

            if (needPosFix)
            {
                return;
            }

            if (baCtr.isDead)
            {
                return;
            }

            if (bp.isInPosFixAfterFight)
            {
                return;
            }

            EnterMapEvent(bp);
        }

        //      public void OnTriggerExit2D(Collider2D col){
        //
        //          BattleAgentController ba = col.GetComponent<BattleAgentController> ();
        //
        //          if (!(ba is BattlePlayerController)) {
        //              return;
        //          }
        //      }

        public override void EnterMapEvent(BattlePlayerController bp)
        {
            if (isInMoving)
            {
                RefreshWalkableInfoWhenTriggeredInMoving();
            }

            bp.isInEvent = true;

            bp.StopMoveAndWait();

            bp.FixPositionToStandard();

            MapEventTriggered(false, bp);
        }

        public void DetectPlayer(BattlePlayerController bp)
        {
            if (isInMoving)
            {
                RefreshWalkableInfoWhenTriggeredInMoving();
            }
            bp.StopMoveAtEndOfCurrentStep();
            bp.isInEvent = true;
            MapEventTriggered(true, bp);
        }

        public override void MapEventTriggered(bool isFromDetect, BattlePlayerController bp)
        {
            bp.boxCollider.enabled = false;

            if (isTriggered)
            {
                return;
            }

            //if (!npc.isExcutor) {
            //  DisableAllAlertAreaDetect ();
            //}

            ExploreManager.Instance.DisableExploreInteractivity();

            ExploreManager.Instance.MapWalkableEventsStopAction();

            StopMoveImmidiately();

            ExploreManager.Instance.currentEnteredMapEvent = this;

            //bp.StopMoveAtEndOfCurrentStep ();

            if (canMove)
            {
                StartCoroutine("AdjustPositionAndTowards", bp);
            }
            else
            {
                StartCoroutine("AdjustTowards", bp);
            }

            isTriggered = true;

        }

        public override void ResetWhenDie()
        {
            StopAllCoroutines();
            DisableAllDetect();
        }

        public void EnterFight(BattlePlayerController bp)
        {
            bp.escapeFromFight = false;
            bp.isInEscaping = false;
            StartCoroutine("AdjustPositionAndTowardsAndFight", bp);
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

        private IEnumerator AdjustPositionAndTowardsAndFight(BattlePlayerController battlePlayerCtr)
        {

            yield return new WaitUntil(() => battlePlayerCtr.isIdle);

            Vector3 playerOriPos = battlePlayerCtr.transform.position;
            Vector3 npcOriPos = transform.position;

            int playerPosX = Mathf.RoundToInt(playerOriPos.x);
            int playerPosY = Mathf.RoundToInt(playerOriPos.y);
            int npcPosX = Mathf.RoundToInt(npcOriPos.x);
            int npcPosY = Mathf.RoundToInt(npcOriPos.y);

            int npcLayerOrder = -npcPosY;

            int posOffsetX = playerPosX - npcPosX;
            int posOffsetY = playerPosY - npcPosY;

            Vector3 npcFightPos = Vector3.zero;
            Vector3 playerFightPos = new Vector3(playerPosX, playerPosY, 0);

            int minX = 0;
            int maxX = ExploreManager.Instance.newMapGenerator.columns - 1;

            if (posOffsetX > 0)
            {

                battlePlayerCtr.TowardsLeft(false);
                baCtr.TowardsRight();

                if (playerPosX - 1 >= minX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[playerPosX - 1, playerPosY] == 1)
                {
                    npcFightPos = new Vector3(playerOriPos.x - 1, playerPosY, 0);
                    npcLayerOrder = -playerPosY;
                    needPosFix = false;
                    battlePlayerCtr.needPosFix = false;
                }
                else if (playerPosX - 1 >= minX && playerPosX - 1 == npcPosX && playerPosY == npcPosY)
                {
                    npcFightPos = new Vector3(playerOriPos.x - 1, playerPosY, 0);
                    npcLayerOrder = -playerPosY;
                    needPosFix = false;
                    battlePlayerCtr.needPosFix = false;
                }
                else if (playerPosX - 1 >= minX && playerPosX - 1 == Mathf.RoundToInt(moveDestination.x) && playerPosY == Mathf.RoundToInt(moveDestination.y))
                {
                    npcFightPos = new Vector3(playerOriPos.x - 1, playerPosY, 0);
                    npcLayerOrder = -playerPosY;
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

                    baCtr.TowardsLeft();

                    battlePlayerCtr.TowardsRight(false);

                    npcFightPos = new Vector3(playerOriPos.x + 1, playerPosY, 0);

                    needPosFix = false;
                    battlePlayerCtr.needPosFix = false;

                }
                else if (playerPosX + 1 <= maxX && playerPosX + 1 == Mathf.RoundToInt(moveDestination.x) && playerPosY == Mathf.RoundToInt(moveDestination.y))
                {
                    baCtr.TowardsLeft();

                    battlePlayerCtr.TowardsRight(false);

                    npcFightPos = new Vector3(playerOriPos.x + 1, playerPosY, 0);

                    needPosFix = false;
                    battlePlayerCtr.needPosFix = false;
                }
                else if (playerPosX - 1 >= minX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[playerPosX - 1, playerPosY] == 1)
                {

                    baCtr.TowardsRight();

                    battlePlayerCtr.TowardsLeft(false);

                    npcFightPos = new Vector3(playerOriPos.x - 1, playerPosY, 0);

                    needPosFix = false;
                    battlePlayerCtr.needPosFix = false;

                }
                else if (playerPosX - 1 >= minX && playerPosX - 1 == Mathf.RoundToInt(moveDestination.x) && playerPosY == Mathf.RoundToInt(moveDestination.y))
                {

                    baCtr.TowardsRight();

                    battlePlayerCtr.TowardsLeft(false);

                    npcFightPos = new Vector3(playerOriPos.x - 1, playerPosY, 0);

                    needPosFix = false;
                    battlePlayerCtr.needPosFix = false;

                }
                else
                {

                    baCtr.TowardsLeft();

                    battlePlayerCtr.TowardsRight(false);

                    if (posOffsetY > 0)
                    {

                        npcFightPos = new Vector3(playerPosX + 0.25f, playerPosY - 0.15f, 0);
                        npcLayerOrder = -playerPosY + 1;

                        playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY + 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY + 1);

                        //battlePlayerCtr.FixPosTo(playerFightPos, 0.1f, null);

                    }
                    else
                    {

                        npcFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);
                        npcLayerOrder = -playerPosY - 1;

                        playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY - 1);

                        //battlePlayerCtr.FixPosTo(playerFightPos, 0.1f, null);
                    }

                    needPosFix = true;
                    battlePlayerCtr.needPosFix = true;

                }

            }
            else if (posOffsetX < 0)
            {

                battlePlayerCtr.TowardsRight(false);
                baCtr.TowardsLeft();

                if (playerPosX + 1 <= maxX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[playerPosX + 1, playerPosY] == 1)
                {

                    npcFightPos = new Vector3(playerOriPos.x + 1, playerPosY, 0);

                    needPosFix = false;
                    battlePlayerCtr.needPosFix = false;

                }
                else if (playerPosX + 1 <= maxX && playerPosX + 1 == npcPosX && playerPosY == npcPosY)
                {

                    npcFightPos = new Vector3(playerOriPos.x + 1, playerPosY, 0);

                    needPosFix = false;
                    battlePlayerCtr.needPosFix = false;

                }
                else if (playerPosX + 1 <= maxX && playerPosX + 1 == Mathf.RoundToInt(moveDestination.x) && playerPosY == Mathf.RoundToInt(moveDestination.y))
                {
                    npcFightPos = new Vector3(playerOriPos.x + 1, playerPosY, 0);

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

                        //battlePlayerCtr.FixPosTo(playerFightPos, 0.1f, null);

                    }
                    else
                    {

                        npcFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);
                        npcLayerOrder = -playerPosY - 1;

                        playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY - 1);

                        //battlePlayerCtr.FixPosTo(playerFightPos, 0.1f, null);
                    }

                    needPosFix = true;
                    battlePlayerCtr.needPosFix = true;

                }

            }

            battlePlayerCtr.FixPosTo(playerFightPos, null);

            RunToPosition(npcFightPos, delegate {

                if (!battlePlayerCtr.escapeFromFight)
                {
                    if (transform.position.x <= ExploreManager.Instance.battlePlayerCtr.transform.position.x)
                    {
                        baCtr.TowardsRight();
                    }
                    else
                    {
                        baCtr.TowardsLeft();
                    }
                    if (!battlePlayerCtr.isInEscaping)
                    {
                        ExploreManager.Instance.EnterFight(this.transform);
                        ExploreManager.Instance.PlayerAndMonsterStartFight();
                    }
                    else
                    {
                        ExploreManager.Instance.EnterFight(this.transform);
                        ExploreManager.Instance.MonsterStartFight();
                    }
                }
                else
                {
                    bool monsterDie = baCtr.agent.health <= 0;
                    RefreshWalkableInfoWhenQuit(monsterDie);
                    QuitFightAndDelayMove(5);
                    battlePlayerCtr.escapeFromFight = false;
                }
            }, npcLayerOrder);
        }


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

						//battlePlayerCtr.FixPosTo(playerFightPos, 0.1f, null);

					}
					else
					{

						npcFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);
						npcLayerOrder = -playerPosY - 1;

						playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);

						baCtr.SetSortingOrder(-playerPosY - 1);

						//battlePlayerCtr.FixPosTo(playerFightPos, 0.1f, null);
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

						//battlePlayerCtr.FixPosTo(playerFightPos, 0.1f, null);

					}
					else
					{

						npcFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);
						npcLayerOrder = -playerPosY - 1;

						playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);

						baCtr.SetSortingOrder(-playerPosY - 1);

						//battlePlayerCtr.FixPosTo(playerFightPos, 0.1f, null);
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

						//battlePlayerCtr.FixPosTo(playerFightPos, 0.1f, null);

					}
					else
					{

						npcFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);
						npcLayerOrder = -playerPosY - 1;

						playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);

						baCtr.SetSortingOrder(-playerPosY - 1);

						//battlePlayerCtr.FixPosTo(playerFightPos, 0.1f, null);
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
