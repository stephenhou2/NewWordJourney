using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace WordJourney
{
	using DG.Tweening;
	using DragonBones;
	using Transform = UnityEngine.Transform;
	using TMPro;

    public class BattlePlayerController : BattleAgentController
    {

        public GameObject playerForward;
        public GameObject playerBackWard;
        public GameObject playerSide;


        // 当前的单步移动动画对象
        private Tweener moveTweener;

        // 标记是否在单步移动中
        private bool inSingleMoving;

		public bool isInMoving{
			get{
				return inSingleMoving;
			}
		}

        // 移动路径点集
        public List<Vector3> pathPosList;


        // 正在前往的节点位置
        public Vector3 singleMoveEndPos;

        // 移动的终点位置
        public Vector3 moveDestination;

        // 玩家UI控制器
        private BattlePlayerUIController bpUICtr;

        // 当前碰到的怪物控制器  
        private NavigationHelper navHelper;


        public bool isInEvent; //是否在一个事件触发过程中

        public bool needPosFix;// 是否需要进行位置修正

        public bool escapeFromFight; //是否从战斗中逃脱

        public bool isInEscaping; //是否正在脱离战斗中

        public bool isInPosFixAfterFight;

		public bool isInSkillAttackAnimBeforeHit;// 是否处在 [非普通攻击] 的技能击中前的攻击动作中

        
		private IEnumerator attackDelayWhenChangeWeapon;// 更换武器时的等待携程

		private IEnumerator newMoveCoroutine;// 新的行走协程

        public TextMeshPro stepCount;// 显示剩余隐身步数的3D文本

        // 实际剩余隐身步数
        private int mFadeStepsLeft;
		public int fadeStepsLeft
        {
            get { return mFadeStepsLeft; }
            set
            {
                mFadeStepsLeft = value;
                // 剩余隐身步数为0时，激活包围盒，显示隐身步数的文本隐藏，将隐身特效动画回收
                if (mFadeStepsLeft == 0)
                {
                    boxCollider.enabled = true;
                    stepCount.enabled = false;
					for (int i = 0; i < effectAnimContainer.childCount;i++){
						EffectAnim effectAnim = effectAnimContainer.GetChild(i).GetComponent<EffectAnim>();
						if(effectAnim.effectName.Equals(CommonData.yinShenEffectName)){
							exploreManager.newMapGenerator.AddEffectAnimToPool(effectAnim);
							break;
						}
					}

                }
                // 剩余隐身步数不为0时，隐身步数文本激活显示，包围盒隐
                else
                {
                    stepCount.enabled = true;
                    stepCount.text = mFadeStepsLeft.ToString();
                    boxCollider.enabled = false;
                    if (mFadeStepsLeft > 5)
                    {
                        stepCount.color = Color.green;
                    }
                    else
                    {
                        stepCount.color = Color.red;
                    }

                }
            }
        }

        // 是否保持当前探索行走完成，拒绝新的路径输入
		private bool refuseNewPath = false;

        // 行走过程结束时的回调
		private CallBack moveEndCallBack;


        
        // 重置角色属性
        protected override void Awake()
        {

            ActiveBattlePlayer(false, false, false);

            agent = GetComponentInParent<Player>();

            navHelper = GetComponent<NavigationHelper>();

            isInFight = false;

            isInEvent = false;

            needPosFix = false;

            escapeFromFight = false;

            isInEscaping = false;

            isInPosFixAfterFight = false;
                    
            base.Awake();

        }
        
        /// <summary>
        /// 初始化角色对象
        /// </summary>
        public void InitBattlePlayer()
        {
            // 绑定UI组件
            Transform canvas = TransformManager.FindTransform("ExploreCanvas");

            agentUICtr = canvas.GetComponent<BattlePlayerUIController>();

            bpUICtr = agentUICtr as BattlePlayerUIController;

            moveDestination = transform.position;
            
        }

        /// <summary>
        /// 设置行走结束时的回调
        /// </summary>
        /// <param name="moveEndCallBack">Move end call back.</param>
		public void SetMoveEndCallBack(CallBack moveEndCallBack){
			this.moveEndCallBack = moveEndCallBack;
		}


        /// <summary>
        /// 设置龙骨动画的层级
        /// </summary>
        /// <param name="order">Order.</param>
        public override void SetSortingOrder(int order)
        {
            playerForward.GetComponent<UnityArmatureComponent>().sortingOrder = order;
            playerBackWard.GetComponent<UnityArmatureComponent>().sortingOrder = order;
            playerSide.GetComponent<UnityArmatureComponent>().sortingOrder = order;
        }

        /// <summary>
        /// 设置寻路起点
        /// </summary>
        /// <param name="position">Position.</param>
        public void SetNavigationOrigin(Vector3 position)
        {
            singleMoveEndPos = position;
            moveDestination = position;
        }

        /// <summary>
        /// 按照指定路径 pathPosList 移动到终点 moveDestination
        /// </summary>
        /// <param name="pathPosList">Path position list.</param>
        /// <param name="moveDestination">End position.</param>
        public bool MoveToPosition(Vector3 moveDestination, int[,] mapWalkableInfoArray)
        {
            //拒绝新路径时，返回false，输入点暂时无法行走
			if(refuseNewPath){
				return false;
			}

            boxCollider.enabled = true;

            // 在战斗过程中时，返回false，输入点暂时无法行走
            if (isInFight)
            {
                return false;
            }

            // 在地图事件中是，返回false，输入点暂时无法行走
            if (isInEvent)
            {
                return false;
            }

            // 计算自动寻路路径
            pathPosList = navHelper.FindPath(singleMoveEndPos, moveDestination, mapWalkableInfoArray);

            // 停止原来的行走协程
			if(newMoveCoroutine != null){
				StopCoroutine(newMoveCoroutine);
			}

            // 设置行走终点
            this.moveDestination = moveDestination;

            // 如果寻路路径内的点集数量为0
            if (pathPosList.Count == 0)
            {

                // 移动路径中没有点时，说明没有有效移动路径，此时终点设置为当前单步移动的终点
                this.moveDestination = singleMoveEndPos;

                // 当前行动结束后执行的操作
                moveTweener.OnComplete(() =>
                {
					// 隐身步数--
                    if (fadeStepsLeft > 0)
                    {
                        fadeStepsLeft--;
                    }
                    // 标记已经停止单步移动
                    inSingleMoving = false;
                    // 播放idle动画
                    PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
                    // 设置龙骨动画的层
					SetSortingOrder(-Mathf.RoundToInt(transform.position.y));
                });

                Debug.Log("无有效路径");

                return false;
            }

            // 如果自动寻路计算出可以行走，则开启新的行走协程
			newMoveCoroutine = MoveWithNewPath();
			StartCoroutine(newMoveCoroutine);

            return pathPosList.Count > 0;

        }

        /// <summary>
        /// 按照新路径移动
        /// </summary>
        /// <returns>The with new path.</returns>
        private IEnumerator MoveWithNewPath()
        {

            // 如果移动动画不为空
            if (moveTweener != null)
            {
                        
                // 等待当前行走结【如果不等待当前行走结束就开始的话会导致行走不是完全按照方格沙盘进行的，点击新的行走点时会沿着斜向运动】
				// 将当步的动画结束回调改为只标记已走
				// 不删除路径点（因为新的路径就是根据当步的结束点计算的，改点不在新路径内）
                moveTweener.OnComplete(() =>
                {
					Vector3 targetPos = transform.position;

					exploreManager.newMapGenerator.miniMapPlayer.localPosition = targetPos;
                    
                    // 单步结束后清除单步移动点在小地图上周围的迷雾
                    exploreManager.newMapGenerator.ClearMiniMapMaskAround(targetPos);

					// 小地图的实现原理：相机渲染到纹理 -> 读取纹理 -> 渲染到屏幕【相机如果在激活状态，则每帧都会渲染到纹理一次】
                    // 1帧以后小地图相机关闭【这样做可以保证只在每次行走结束后更新小地图，减少不必要的消耗】
					exploreManager.newMapGenerator.MiniMapCameraLatelySleep();

                    // 更新小地图
                    exploreManager.expUICtr.UpdateMiniMapDisplay(targetPos);

					SetSortingOrder(-Mathf.RoundToInt(transform.position.y));

                    if (fadeStepsLeft > 0)
                    {
                        fadeStepsLeft--;
                    }
                    inSingleMoving = false;

					if(!isInEvent && !isInFight){
                        exploreManager.MapWalkableEventsStartAction();
                    }   
                });

            }

            yield return new WaitUntil(() => !inSingleMoving);
         
            // 移动到新路径上的下一个节点
            MoveToNextPosition();

        }


        /// <summary>
        /// 匀速移动到指定节点位置
        /// </summary>
        /// <param name="targetPos">Target position.</param>
        private void MoveToPosition(Vector3 targetPos)
        {         
            float distance = (targetPos - transform.position).magnitude;

            // 匀速移动到指定点
            moveTweener = transform.DOMove(targetPos, moveDuration * distance).OnComplete(() =>
            {

				exploreManager.newMapGenerator.miniMapPlayer.localPosition = targetPos;

                exploreManager.newMapGenerator.ClearMiniMapMaskAround(targetPos);

				exploreManager.newMapGenerator.MiniMapCameraLatelySleep();

                exploreManager.expUICtr.UpdateMiniMapDisplay(targetPos);   

                if (needPosFix)
                {
                    needPosFix = false;
                }

                if (fadeStepsLeft > 0)
                {
                    fadeStepsLeft--;
                }

                // 动画结束时已经移动到指定节点位置，标记单步行动结束
                inSingleMoving = false;

				SetSortingOrder(-Mathf.RoundToInt(transform.position.y));

                if (pathPosList.Count > 0)
                {

                    // 将当前节点从路径点中删除
                    pathPosList.RemoveAt(0);

					if(!isInEvent && !isInFight){
                        exploreManager.MapWalkableEventsStartAction();
                    }            

                    // 移动到下一个节点位置
                    MoveToNextPosition();

                }

				if(!isInEvent && !isInFight){
					exploreManager.MapWalkableEventsStartAction();
				}            

            });

            // 设置匀速移动
            moveTweener.SetEase(Ease.Linear);


        }

        /// <summary>
        /// 判断当前是否已经走到了终点位置
        /// </summary>
        /// <returns><c>true</c>, if end point was arrived, <c>false</c> otherwise.</returns>
        private bool ArriveEndPoint()
        {

            // 如果已经移动到了终点，则可以接受新的行走指令，并返回true
            if (MyTool.ApproximatelySamePosition2D(moveDestination, transform.position))
            {
				refuseNewPath = false;
                return true;
            }

            return false;

        }


        /// <summary>
        /// 激活龙骨
        /// </summary>
        /// <param name="forward">正面的龙骨.</param>
        /// <param name="backward">背面的龙骨</param>
        /// <param name="side">侧面的龙骨</param>
        public void ActiveBattlePlayer(bool forward, bool backward, bool side)
        {

            // 判断想要激活的龙骨和角色当前的朝向是否一致
			// 正面龙骨对应角色朝向下
			// 背面龙骨对应角色朝向上
			// 侧面龙骨对应角色朝向左或者朝向右
            // 如果想要激活的龙骨和当前朝向不一致，则停止当前动画的播放
            // 如果想要激活的龙骨和当前朝向一致，则上个动画应该继续播放【否则激活时会重新播放动画，原本动画可能播放到一半了，突然又从开头播放】

            bool lastRoleAnimStop = false;

            if (forward && towards != MyTowards.Down)
            {
                lastRoleAnimStop = true;
            }

            if (backward && towards != MyTowards.Up)
            {
                lastRoleAnimStop = true;
            }

            if (side && towards != MyTowards.Left && towards != MyTowards.Right)
            {
                lastRoleAnimStop = true;
            }

            if (lastRoleAnimStop && modelActive != null)
            {
                armatureCom.animation.Stop();
                isIdle = false;
            }

            playerForward.SetActive(forward);
            playerBackWard.SetActive(backward);
            playerSide.SetActive(side);

            if (forward)
            {
                modelActive = playerForward;
            }
            else if (backward)
            {
                modelActive = playerBackWard;
            }
            else if (side)
            {
                modelActive = playerSide;
            }

        }



        /// <summary>
        /// 移动到下一个节点【自动寻路过程都是按照一个一个点走出来的，走完当前点继续走下个点】
        /// </summary>
        private void MoveToNextPosition()
        {
            Vector3 nextPos = Vector3.zero;

            boxCollider.enabled = fadeStepsLeft == 0;

            bool resetWalkAnim = false;

            if (pathPosList.Count > 0)
            {

                // 获取新的行走点
                nextPos = pathPosList[0];

                // 如果当前已经走到终点了
                if (ArriveEndPoint())
                {
					// 执行行走结束的回调
					if(moveEndCallBack != null){
						moveEndCallBack();
						moveEndCallBack = null;
						refuseNewPath = false;
					}

                    // 播放idle动画
					if(armatureCom.animation.lastAnimationName != CommonData.roleIdleAnimName){
						PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
					}
                   
                    return;
                }

               
                // 如果下个路径点和当前所在位置近似重合
                if (MyTool.ApproximatelySamePosition2D(nextPos, transform.position))
                {
					// 直接把下个路径点删除
                    pathPosList.RemoveAt(0);
                    // 获取新的点继续行走
                    MoveToNextPosition();
                    return;
                }

				// 如果行走点集中点的数量>=2[战斗结束后可能会有位置上的偏移,如下图中0位置，这时寻路算法可能会算出先走到2，再重新走到3，不是直接走到3]
                // -------------------------
				// |  2 0   3  |     |     |  
				// |     |   1 |     |     |             
				// -------------------------
				// |     |     |     |     | 
				// |     |     |     |     | 
				// -------------------------

                // 故需要判断前两个行走点的位置关系
                if (pathPosList.Count >= 2)
                {

                    Vector3 firstFollowingPos = pathPosList[0];
                    Vector3 secondFollowingPos = pathPosList[1];

                    // 如果存在上面的行走路径，则删除第一个点
                    if ((firstFollowingPos.x - transform.position.x) * (secondFollowingPos.x - transform.position.x) < 0
                        && Mathf.RoundToInt(firstFollowingPos.y) == Mathf.RoundToInt(transform.position.y)
                        && Mathf.RoundToInt(firstFollowingPos.y) == Mathf.RoundToInt(secondFollowingPos.y))
                    {
                        pathPosList.RemoveAt(0);
                        MoveToNextPosition();
                        return;
                    }
                }

                // 同上
                if (pathPosList.Count >= 2)
                {

                    Vector3 firstFollowingPos = pathPosList[0];
                    Vector3 secondFollowingPos = pathPosList[1];

                    if ((firstFollowingPos.y - transform.position.y) * (secondFollowingPos.y - transform.position.y) < 0
                        && Mathf.RoundToInt(firstFollowingPos.x) == Mathf.RoundToInt(transform.position.x)
                        && Mathf.RoundToInt(firstFollowingPos.x) == Mathf.RoundToInt(secondFollowingPos.x))
                    {
                        pathPosList.RemoveAt(0);
                        MoveToNextPosition();
                        return;
                    }
                }

                // 根据下个点和当前点的位置关系激活对应龙骨，设置正确的朝向
                if (Mathf.RoundToInt(nextPos.x) == Mathf.RoundToInt(transform.position.x))
                {

                    if (nextPos.y < transform.position.y)
                    {
                        if (modelActive != playerForward)
                        {
                            resetWalkAnim = true;
                        }
                        ActiveBattlePlayer(true, false, false);
                        towards = MyTowards.Down;
                    }
                    else if (nextPos.y > transform.position.y)
                    {
                        if (modelActive != playerBackWard)
                        {
                            resetWalkAnim = true;
                        }
                        ActiveBattlePlayer(false, true, false);
                        towards = MyTowards.Up;
                    }

                }

                if (Mathf.RoundToInt(nextPos.y) == Mathf.RoundToInt(transform.position.y))
                {

                    if (modelActive != playerSide)
                    {
                        resetWalkAnim = true;
                    }
                    else if ((nextPos.x > transform.position.x && armatureCom.armature.flipX == true) ||
                       (nextPos.x < transform.position.x && armatureCom.armature.flipX == false))
                    {
                        resetWalkAnim = true;
                    }

                    ActiveBattlePlayer(false, false, true);

                    bool nextPosLeft = nextPos.x < transform.position.x;
                    armatureCom.armature.flipX = nextPosLeft;
                    towards = nextPosLeft ? MyTowards.Left : MyTowards.Right;

                }

                if (isIdle)
                {
                    resetWalkAnim = true;
                }


            }

            // 到达终点前的单步移动开始前进行碰撞检测
            // 1.如果碰撞体存在，则根据碰撞体类型给exploreManager发送消息执行指定回调
            // 2.如果未检测到碰撞体，则开始本次移动
            if (pathPosList.Count == 1)
            {

                // 射线检测，是否有碰撞体
                Vector3 rayStartPos = (transform.position + pathPosList[0]) / 2;

                RaycastHit2D r2d = Physics2D.Linecast(rayStartPos, pathPosList[0], collosionLayer);

                // 如果射线检测检测到了碰撞体
                if (r2d.transform != null)
                {

                    MapEvent me = r2d.transform.GetComponent<MapEvent>();

					// 标记是否需要停止当前的行走
                    bool needToStopMove = true;

                    // 如果碰到的是地图事件
                    if (me != null)
                    {
                        needToStopMove = me.IsPlayerNeedToStopWhenEntered();
                    }
                    else
                    {
                        needToStopMove = false;
                    }

                    // 如果要停止运动
                    if (needToStopMove)
                    {
                        StopMoveAndWait();
                    }

                    // 如果碰到了地图事件
                    if (me != null)
                    {
						// 绑定当前地图事件
                        exploreManager.currentEnteredMapEvent = me;
                        // 标记玩家在地图事件中
                        isInEvent = true;

                        me.EnterMapEvent(this);
                    }

                    if (needToStopMove)
                    {
						if (moveEndCallBack != null)
                        {
                            moveEndCallBack();
                            moveEndCallBack = null;
							refuseNewPath = false;
                        }

                        return;
                    }

                }
            }
            // 路径中没有节点
            // 按照行动路径已经将所有的节点走完
            if (pathPosList.Count == 0)
            {

                //// 走到了终点
                //if (ArriveEndPoint())
                //{
                    moveTweener.Kill(true);

                    if (!isIdle)
                    {
                        PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
                    }
                //}

				if (moveEndCallBack != null)
                {
                    moveEndCallBack();
                    moveEndCallBack = null;
					refuseNewPath = false;
                }


                return;
            }

            // 如果还没有走到终点
            if (!ArriveEndPoint())
            {

				GameManager.Instance.soundManager.PlayAudioClip(CommonData.footstepAudioName);

                // 记录下一节点位置
                singleMoveEndPos = nextPos;

                if (resetWalkAnim)
                {
                    PlayRoleAnim(CommonData.roleWalkAnimName, 0, null);
                }

                // 向下一节点移动
                MoveToPosition(nextPos);

                // 标记单步移动中
                inSingleMoving = true;

            }
            else
            {
                moveTweener.Kill(true);
               
                if(!isIdle){
                    PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
                }

				if (moveEndCallBack != null)
                {
                    moveEndCallBack();
                    moveEndCallBack = null;
					refuseNewPath = false;
                }

               
            }

        }
        
        /// <summary>
        /// 强制在遇到地图事件时停止运动，并移动到指定位置
        /// </summary>
        /// <param name="pos">Position.</param>
        /// <param name="callBack">Call back.</param>
		public void ForceMoveToAndStopWhenEnconterWithMapEvent(Vector3 pos,CallBack callBack){
         
			if (newMoveCoroutine != null)
            {
                StopCoroutine(newMoveCoroutine);
            }

			MoveToPosition(pos, exploreManager.newMapGenerator.mapWalkableInfoArray);   

            // 此时拒绝新路径输入
			refuseNewPath = true;

			SetMoveEndCallBack(callBack);
         

		}

        // 当步运动接受后停止自动寻路，并播放idle动画
		public void StopMoveAtEndOfCurrentStep(CallBack callBack = null)
        {
         
			if (newMoveCoroutine != null)
            {
                StopCoroutine(newMoveCoroutine);
            }

			pathPosList.Clear();

            this.moveDestination = singleMoveEndPos;

			MoveToPosition(singleMoveEndPos);

			refuseNewPath = true;
         
            if (moveTweener != null)
            {

                moveTweener.OnComplete(() =>
                {
					if(callBack != null){
						callBack();
					}


                    if (fadeStepsLeft > 0)
                    {
                        fadeStepsLeft--;
                    }

                    // 动画结束时已经移动到指定节点位置，标记单步行动结束
                    inSingleMoving = false;

					SetSortingOrder(-Mathf.RoundToInt(transform.position.y));

					PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);

					refuseNewPath = false; 
                });
            }
            else if(!isIdle)
            {
                PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
				refuseNewPath = false;
            }

        }


        /// <summary>
        /// 立刻停止运动并播放idle动画
        /// </summary>
        public void StopMoveAndWait()
        {
			if (newMoveCoroutine != null)
            {
                StopCoroutine(newMoveCoroutine);
            }
            moveTweener.Kill(false);
            inSingleMoving = false;
            if (!isIdle) 
            {
                PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
            }
            Vector3 currentPos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
            SetNavigationOrigin(currentPos);
        }

        /// <summary>
        /// 角色龙骨朝向左方
        /// </summary>
        /// <param name="andWait">If set to <c>true</c> and wait.</param>
        public override void TowardsLeft(bool andWait = true)
        {
            ActiveBattlePlayer(false, false, true);
            if (andWait && !isIdle)
            {
                PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
            }
            armatureCom.armature.flipX = true;
            towards = MyTowards.Left;
        }

        /// <summary>
        /// 角色龙骨朝向右方
        /// </summary>
        /// <param name="andWait">If set to <c>true</c> and wait.</param>
        public override void TowardsRight(bool andWait = true)
        {
            ActiveBattlePlayer(false, false, true);
            if (andWait && !isIdle)
            {
                PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
            }
            armatureCom.armature.flipX = false;
            towards = MyTowards.Right;
        }

        /// <summary>
        /// 角色龙骨朝向上方
        /// </summary>
        /// <param name="andWait">If set to <c>true</c> and wait.</param>
        public override void TowardsUp(bool andWait = true)
        {
            ActiveBattlePlayer(false, true, false);
            if (andWait && !isIdle)
            {
                PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
            }
            towards = MyTowards.Up;
        }

        /// <summary>
        /// 角色龙骨朝向下方
        /// </summary>
        /// <param name="andWait">If set to <c>true</c> and wait.</param>
        public override void TowardsDown(bool andWait = true)
        {
            ActiveBattlePlayer(true, false, false);
            if (andWait && !isIdle)
            {
                PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
            }
            towards = MyTowards.Down;

        }

        /// <summary>
        /// 位置修正到最近整数点
        /// </summary>
		public void FixPositionToStandard()
        {
            Vector3 fixedPosition = new Vector3(Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y),
                transform.position.z);
            //			transform.position = fixedPosition;
            singleMoveEndPos = fixedPosition;
            moveDestination = fixedPosition;
            isInPosFixAfterFight = true;
            transform.DOMove(fixedPosition, 0.1f).OnComplete(() =>
            {
                isInPosFixAfterFight = false;
                escapeFromFight = false;
                isInEscaping = false;
            });
        }
        
        /// <summary>
        /// 绑定敌人
        /// </summary>
        /// <param name="bmCtr">Bm ctr.</param>
		public void SetEnemy(BattleMonsterController bmCtr){
			this.enemy = bmCtr;
		}


		/// <summary>
		/// 开始进入战斗的逻辑
		/// </summary>
		/// <param name="bmCtr">怪物控制器</param>
		public void StartFight(BattleMonsterController bmCtr){
            // 如果已经在战斗中，直接返回，防止重复开始战斗
			if(isInFight){
				return;
			}

            // 一些战斗初始化设置
			isInFight = true;

			boxCollider.enabled = false;

			ClearAllSkillCallBacks ();

			InitTriggeredPassiveSkillCallBacks (this,bmCtr);

			// 默认玩家在战斗中将先出招，首次攻击不用等待
			currentUsingActiveSkill = normalAttack;
			UseSkill (currentUsingActiveSkill);


		}

      
		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="skill">Skill.</param>
		public override void UseSkill (ActiveSkill skill)
		{
			if(isDead){
				return;
			}

			Player.mainPlayer.mana -= skill.manaConsume;

			if (attackCoroutine != null) {
				StopCoroutine (attackCoroutine);
			}

			currentUsingActiveSkill = skill;

			if (currentUsingActiveSkill.skillId != 0)
            {
				isInSkillAttackAnimBeforeHit = true;
            }

			// 播放技能对应的角色动画，角色动画结束后播放攻击间隔动画
			this.PlayRoleAnim (skill.selfRoleAnimName, 1, () => {

				if(!isInEscaping){
					exploreManager.expUICtr.bpUICtr.UpdateActiveSkillButtonsWhenAttackAnimFinish();
				}

				// 播放等待动画
				if (armatureCom.animation.lastAnimationName != CommonData.roleAttackIntervalAnimName)
				{
					// 检测到死亡时停止战斗中的行为逻辑
					if(isDead){
						return;
					}
					this.PlayRoleAnim(CommonData.roleAttackIntervalAnimName, 0, null);
				}
               
			});

		}
			


        /// <summary>
        /// 执行技能逻辑
        /// </summary>
		protected override void AgentExcuteHitEffect ()
		{

            if (isDead)
            {
                return;
            }

			if (!isInFight) {
				return;
			}

			if (enemy == null) {
				return;
			}
                     
			if(currentUsingActiveSkill.sfxName != string.Empty){
				// 播放技能对应的音效
                GameManager.Instance.soundManager.PlayAudioClip(currentUsingActiveSkill.sfxName);
			}


			MapMonster mm = enemy.GetComponent<MapMonster> ();

			if (mm != null) {
				mm.isReadyToFight = true;
			}

            if (currentUsingActiveSkill.skillId != 0)
            {
                isInSkillAttackAnimBeforeHit = false;
            }

			// 技能效果影响玩家和怪物
			currentUsingActiveSkill.AffectAgents(this,enemy);
		
			UpdateStatusPlane ();

			if (enemy == null) {
				return;
			}
				
			enemy.UpdateStatusPlane ();

			bool fightEnd = CheckFightEnd();

			// 如果战斗没有结束，则默认在攻击间隔时间之后按照默认攻击方式进行攻击
			if (isInFight && !isDead && !fightEnd) {
				currentUsingActiveSkill = normalAttack;
				attackCoroutine = InvokeAttack (currentUsingActiveSkill);
				StartCoroutine (attackCoroutine);
			} 
		}


		/// <summary>
		/// 判断本次战斗是否结束,如果怪物死亡则执行怪物死亡对应的方法
		/// </summary>
		/// <returns><c>true</c>, if end was fought, <c>false</c> otherwise.</returns>
		public override bool CheckFightEnd(){

			if (agent.health <= 0)
            {
                AgentDie();
                isInFight = false;
                return true;
            }

			if(enemy == null){
				return true;
			}

			if (enemy.agent.health <= 0) {
				enemy.AgentDie ();
				isInFight = false;
				return true;
			} 
				
			return false;         
		}
			
        /// <summary>
        /// 等待攻击间隔后重新开始攻击【战斗中切换武器时专用】
        /// </summary>
        /// <param name="animInfo">Animation info.</param>
        /// <param name="interval">Interval.</param>
		public void ResetAttackAfterInterval(HLHRoleAnimInfo animInfo,float interval){

            // 切换武器时首先播放空手攻击间隔动画
            // 首先适配出动画名称
			string intervalAnimName = RoleAnimNameAdapt(CommonData.playerIntervalBareHandName);

			PlayRoleAnim(intervalAnimName, 0, null);

			if(attackDelayWhenChangeWeapon != null){
				StopCoroutine(attackDelayWhenChangeWeapon);
			}

			attackDelayWhenChangeWeapon = MyResetAttackAfterInterval(animInfo, interval);

			StartCoroutine(attackDelayWhenChangeWeapon);

		}

        /// <summary>
        /// 等待攻击间隔的协程
        /// </summary>
        /// <returns>The reset attack after interval.</returns>
        /// <param name="animInfo">Animation info.</param>
        /// <param name="interval">Interval.</param>
		private IEnumerator MyResetAttackAfterInterval(HLHRoleAnimInfo animInfo,float interval){

			yield return new WaitForSeconds(interval);

			ResetAttack(animInfo);
            
		}

        /// <summary>
        /// 重新开始攻击动作
        /// </summary>
        /// <param name="animInfo">Animation info.</param>
		public void ResetAttack(HLHRoleAnimInfo animInfo){
			StopWaitRoleAnimEndCoroutine();
			if(attackDelayWhenChangeWeapon != null){
				StopCoroutine(attackDelayWhenChangeWeapon);
			}
			string animName = string.Empty;
			if(IsInAttackAnim(animInfo.roleAnimName)){
				animName = animInfo.roleAnimName;
			}else{
				animName = RoleAnimNameAdapt(CommonData.playerAttackBareHandName);
			}
			PlayRoleAnim(animName, 1, delegate {
			
				if (!isInEscaping)
                {
                    exploreManager.expUICtr.bpUICtr.UpdateActiveSkillButtonsWhenAttackAnimFinish();
                }

                // 播放等待动画
                if (armatureCom.animation.lastAnimationName != CommonData.roleAttackIntervalAnimName)
                {
                    if (isDead)
                    {
                        return;
                    }
                    this.PlayRoleAnim(CommonData.roleAttackIntervalAnimName, 0, null);
                }
			});
		}


        /// <summary>
        /// 战斗结束后停止所有协程
        /// </summary>
		protected override void StopCoroutinesWhenFightEnd ()
		{
			base.StopCoroutinesWhenFightEnd ();
			playerSide.transform.localPosition = Vector3.zero;
			playerForward.transform.localPosition = Vector3.zero;
			playerBackWard.transform.localPosition = Vector3.zero;
			IEnumerator resetPlayerPosCoroutine = ResetPlayerPos();
			StartCoroutine(resetPlayerPosCoroutine);
		}

        /// <summary>
        /// 保险起见，重设玩家位置用，防止位置出现偏移
        /// </summary>
        /// <returns>The player position.</returns>
		private IEnumerator ResetPlayerPos(){
			yield return new WaitForSeconds(0.05f);
			playerSide.transform.localPosition = Vector3.zero;
			playerForward.transform.localPosition = Vector3.zero;
			playerBackWard.transform.localPosition = Vector3.zero;
		}


		/// <summary>
		/// 玩家开始隐身
		/// </summary>
		public void PlayerFade(){

			fadeStepsLeft = 20;

			boxCollider.enabled = false;

		}
        

        /// <summary>
        /// 获取当前角色动画信息
        /// </summary>
        /// <returns>The current role animation info.</returns>
		public HLHRoleAnimInfo GetCurrentRoleAnimInfo(){
			string currentAnimName = armatureCom.animation.lastAnimationName;
			AnimationState state = armatureCom.animation.GetState (currentAnimName);
			float animTime = state.currentTime;
			int playTimes = state.playTimes;
			return new HLHRoleAnimInfo (currentAnimName, playTimes, animTime, animEndCallBack);
		}

        /// <summary>
        /// 停止等待角色动画结束的协程
        /// </summary>
		public void StopWaitRoleAnimEndCoroutine(){
			// 如果还有等待上个角色动作结束的协程存在，则结束该协程
            if (waitRoleAnimEndCoroutine != null)
            {
                StopCoroutine(waitRoleAnimEndCoroutine);
            }

			string currentAnimName = armatureCom.animation.lastAnimationName;

			if(attackCoroutine != null){
				StopCoroutine(attackCoroutine);
			}

			if(!IsInIntervalAnim(currentAnimName)){
				PlayRoleAnim(CommonData.roleAttackIntervalAnimName, 0, null);
			}

		}

        /// <summary>
        /// 在战斗中是否需要重置攻击动作[正面和背面进入战斗时需要在转身时重播,侧面不需要]
        /// </summary>
        /// <returns><c>true</c>, if reset attack was needed, <c>false</c> otherwise.</returns>
		public bool NeedResetAttack(){
			return modelActive != playerSide && isInFight;
		}

        /// <summary>
        /// 播放角色动画
        /// </summary>
        /// <param name="animName">Animation name.</param>
        /// <param name="playTimes">Play times.</param>
        /// <param name="cb">Cb.</param>
		public override void PlayRoleAnim (string animName, int playTimes, CallBack cb)
		{
			animName = RoleAnimNameAdapt (animName);
			base.PlayRoleAnim (animName, playTimes, cb);
			//Debug.Log(animName);
		}


        /// <summary>
        /// 从指定时间开始播放角色动画
        /// </summary>
        /// <param name="animName">Animation name.</param>
        /// <param name="animBeginTime">Animation begin time.</param>
        /// <param name="playTimes">Play times.</param>
        /// <param name="cb">Cb.</param>
		public void PlayRoleAnimByTime(string animName,float animBeginTime,int playTimes,CallBack cb){

			isIdle = animName == CommonData.roleIdleAnimName;
         
			// 如果还有等待上个角色动作结束的协程存在，则结束该协程
			if (waitRoleAnimEndCoroutine != null) {
				StopCoroutine (waitRoleAnimEndCoroutine);
			}
				
			animName = RoleAnimNameAdapt (animName);

			// 播放新的角色动画
			armatureCom.animation.GotoAndPlayByTime(animName,animBeginTime,playTimes);

			// 如果有角色动画结束后要执行的回调，则开启一个新的等待角色动画结束的协程，等待角色动画结束后执行回调
			if (cb != null) {
				waitRoleAnimEndCoroutine = ExcuteCallBackAtEndOfRoleAnim (cb);
				StartCoroutine(waitRoleAnimEndCoroutine);
			}
		}

        /// <summary>
        /// 是否在攻击动作的动画中
        /// </summary>
        /// <returns><c>true</c>, if in attack animation was ised, <c>false</c> otherwise.</returns>
        /// <param name="animName">Animation name.</param>
		private bool IsInAttackAnim(string animName){
			return animName == CommonData.roleAttackAnimName
			|| animName == CommonData.playerAttackBareHandName
			|| animName == CommonData.playerAttackWithSwordName
			|| animName == CommonData.playerAttackWithStaffName
			|| animName == CommonData.playerAttackWithAxeName
			|| animName == CommonData.playerAttackWithDraggerName;

		}

        /// <summary>
        /// 是否在攻击间隔等待的动画中
        /// </summary>
        /// <returns><c>true</c>, if in interval animation was ised, <c>false</c> otherwise.</returns>
        /// <param name="animName">Animation name.</param>
		private bool IsInIntervalAnim(string animName){
			return animName == CommonData.roleAttackIntervalAnimName
			|| animName == CommonData.playerIntervalBareHandName
			|| animName == CommonData.playerIntervalWithSwordName
			|| animName == CommonData.playerIntervalWithStaffName
			|| animName == CommonData.playerIntervalWithAxeName
			|| animName == CommonData.playerIntervalWithDraggerName;
		}

        /// <summary>
        /// 是否在物理攻击动作的动画中
        /// </summary>
        /// <returns><c>true</c>, if in physical skill animation was ised, <c>false</c> otherwise.</returns>
        /// <param name="animName">Animation name.</param>
		private bool IsInPhysicalSkillAnim(string animName){
			return animName == CommonData.rolePhysicalSkillAnimName
			|| animName == CommonData.playerPhysicalSkillBareHandName
			|| animName == CommonData.playerPhysicalSkillWithSwordName
			|| animName == CommonData.playerPhysicalSkillWithStaffName
			|| animName == CommonData.playerPhysicalSkillWithAxeName
			|| animName == CommonData.playerPhysicalSkillWithDraggerName;
		}

        /// <summary>
        /// 是否在魔法攻击动作的动画中
        /// </summary>
        /// <returns><c>true</c>, if in magicl skill animation was ised, <c>false</c> otherwise.</returns>
        /// <param name="animName">Animation name.</param>
		private bool IsInMagiclSkillAnim(string animName){
			return animName == CommonData.roleMagicalSkillAnimName
			|| animName == CommonData.playerMagicalSkillBareHandName
			|| animName == CommonData.playerMagicalSkillWithSwordName
			|| animName == CommonData.playerMagicalSkillWithStaffName
			|| animName == CommonData.playerMagicalSkillWithAxeName
			|| animName == CommonData.playerMagicalSkillWithDraggerName;

		}

        /// <summary>
        /// 角色动画名称适配
        /// </summary>
        /// <returns>The animation name adapt.</returns>
        /// <param name="animName">Animation name.</param>
		private string RoleAnimNameAdapt(string animName){

			string adaptName = animName;

            // 行走动画，ilde动画，死亡动画不适配，直接返回原动画名称
			bool adaptException = animName ==CommonData.roleWalkAnimName
			                 || animName == CommonData.roleIdleAnimName
			                 || animName == CommonData.roleDieAnimName;

            // 其余动画根据武器不同适配不同的名称
			if (!adaptException) {
				
				Equipment playerWeapon = Player.mainPlayer.allEquipedEquipments [0];

				switch (playerWeapon.weaponType) {
					case WeaponType.None:
    					if (IsInAttackAnim(animName)) {
    						adaptName = CommonData.playerAttackBareHandName;
                        } else if (IsInPhysicalSkillAnim(animName) && (towards == MyTowards.Left || towards == MyTowards.Right)) {
    						adaptName = CommonData.playerPhysicalSkillBareHandName;
                        } else if (IsInMagiclSkillAnim(animName) && (towards == MyTowards.Left || towards == MyTowards.Right)) {
    						adaptName = CommonData.playerMagicalSkillBareHandName;
    					} else if (IsInIntervalAnim(animName)) {
    						adaptName = CommonData.playerIntervalBareHandName;
    					}
    					break;
    				case WeaponType.Sword:
    					if (IsInAttackAnim(animName)) {
    						adaptName = CommonData.playerAttackWithSwordName;
                        } else if (IsInPhysicalSkillAnim(animName) && (towards == MyTowards.Left || towards == MyTowards.Right)) {
    						adaptName = CommonData.playerPhysicalSkillWithSwordName;
                        } else if (IsInMagiclSkillAnim(animName) && (towards == MyTowards.Left || towards == MyTowards.Right)) {
    						adaptName = CommonData.playerMagicalSkillWithSwordName;
    					} else if (IsInIntervalAnim(animName)) {
    						adaptName = CommonData.playerIntervalWithSwordName;
    					}
    					break;
    				case WeaponType.Staff:
    					if (IsInAttackAnim(animName)) {
    						adaptName = CommonData.playerAttackWithStaffName;
                        } else if (IsInPhysicalSkillAnim(animName) && (towards == MyTowards.Left || towards == MyTowards.Right)) {
    						adaptName = CommonData.playerPhysicalSkillWithStaffName;
                        } else if (IsInMagiclSkillAnim(animName) && (towards == MyTowards.Left || towards == MyTowards.Right)) {
    						adaptName = CommonData.playerMagicalSkillWithStaffName;
    					} else if (IsInIntervalAnim(animName)) {
    						adaptName = CommonData.playerIntervalWithStaffName;
    					}
    					break;
    				case WeaponType.Axe:
    					if (IsInAttackAnim(animName)) {
    						adaptName = CommonData.playerAttackWithAxeName;
                        } else if (IsInPhysicalSkillAnim(animName) && (towards == MyTowards.Left || towards == MyTowards.Right)) {
    						adaptName = CommonData.playerPhysicalSkillWithAxeName;
                        } else if (IsInMagiclSkillAnim(animName) && (towards == MyTowards.Left || towards == MyTowards.Right)) {
    						adaptName = CommonData.playerMagicalSkillWithAxeName;
    					} else if (IsInIntervalAnim(animName)) {
    						adaptName = CommonData.playerIntervalWithAxeName;
    					}
    					break;
    				case WeaponType.Dragger:
    					if (IsInAttackAnim(animName)) {
    						adaptName = CommonData.playerAttackWithDraggerName;
                        } else if (IsInPhysicalSkillAnim(animName) && (towards == MyTowards.Left || towards == MyTowards.Right)) {
    						adaptName = CommonData.playerPhysicalSkillWithDraggerName;
                        } else if (IsInMagiclSkillAnim(animName) && (towards == MyTowards.Left || towards == MyTowards.Right)) {
    						adaptName = CommonData.playerMagicalSkillWithDraggerName;
    					} else if (IsInIntervalAnim(animName)) {
    						adaptName = CommonData.playerIntervalWithDraggerName;
    					}
    					break;
				}

			}
            

			return adaptName;

		}


		/// <summary>
		/// 更新玩家状态栏
		/// </summary>
		public override void UpdateStatusPlane(){
			if (bpUICtr != null) {
				bpUICtr.UpdateAgentStatusPlane ();
			}
		}



		/// <summary>
		/// 清理引用
		/// </summary>
		public void ClearReference(){

			ClearAllEffectStatesAndSkillCallBacks ();

			enemy = null;

			bpUICtr = null;

		}

        /// <summary>
        /// 退出探索
        /// </summary>
		public void QuitExplore(){

			AllEffectAnimsIntoPool();

			ClearReference ();

			gameObject.SetActive (false);

		}
			
        /// <summary>
        /// 退出战斗，重置状态
        /// </summary>
		public override void QuitFight(){

			for (int i = 0; i < Player.mainPlayer.allLearnedSkills.Count; i++)
            {
                Player.mainPlayer.allLearnedSkills[i].hasTriggered = false;
            }

			StopCoroutinesWhenFightEnd ();
            
			isInFight = false;
			isInEvent = false;
			escapeFromFight = false;
			isInEscaping = false;
			currentUsingActiveSkill = null;

			boxCollider.enabled = true;

			SetRoleAnimTimeScale (1.0f);

			agent.ResetBattleAgentProperties (false);         

		}

		/// <summary>
		/// 玩家死亡
		/// </summary>
		override public void AgentDie(){

			if (isDead) {
				return;
			}

			isDead = true;

            // 如果逃脱战斗的过程中，则停止逃脱
			if(isInEscaping){
				isInEscaping = false;
				bpUICtr.StopEscapeDisplay();
			}
                     
			ExploreManager.Instance.DisableAllInteractivity ();

			exploreManager.expUICtr.ShowFullMask();
         
			if(enemy != null){
				enemy.boxCollider.enabled = true;
			}
         
			bool fromFight = isInFight;

			if (enemy != null)
			{
				enemy.QuitFight();
			}

            QuitFight();

			exploreManager.expUICtr.QuitFight();

			isInEvent = true;

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.playerDieAudioName);

			if (enemy != null)
			{
				enemy.AllEffectAnimsIntoPool();
			}

			StopMoveAtEndOfCurrentStep();

            // 播放死亡动画，回收全部特效，询问是否购买复活
			PlayRoleAnim (CommonData.roleDieAnimName, 1, ()=>{
				IEnumerator queryBuyLifeCoroutine = QueryBuyLife(fromFight);
				StartCoroutine(queryBuyLifeCoroutine);
				AllEffectAnimsIntoPool();
				if(enemy != null){
					enemy.AllEffectAnimsIntoPool();
                    enemy = null;
				}            
			});

		}
			
		private IEnumerator QueryBuyLife(bool fromFight){

			yield return new WaitForSeconds (0.5f);

            // 如果是从战斗中死亡，先走战斗结束的逻辑【战斗结束的逻辑中最后有询问是否买活的逻辑】
			if(fromFight){
				exploreManager.BattlePlayerLose ();
			}else{
				exploreManager.expUICtr.ShowBuyLifeQueryHUD();
			}
		}

      
        /// <summary>
        /// 复活并回复100%的生命和魔法
        /// </summary>
		public void RecomeToLife(){
			FixPositionToStandard ();
			agent.ResetBattleAgentProperties (true);         
			isInFight = false;
			inSingleMoving = false;
			isInEvent = false;
			needPosFix = false;
            isDead = false;
			PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
			moveDestination = transform.position;
			singleMoveEndPos = transform.position;
			pathPosList.Clear ();
			boxCollider.enabled = false;
			if(fadeStepsLeft == 0){
				SetEffectAnim(CommonData.yinShenEffectName, null, 0, 0);
			}
			fadeStepsLeft = Mathf.Max(fadeStepsLeft, 5);
			GameManager.Instance.persistDataManager.SaveCompletePlayerData();
		}

        /// <summary>
        /// 复活并回复30%的生命和魔法
        /// </summary>
		public void PartlyRecomeToLife(){
			FixPositionToStandard();
			agent.ResetBattleAgentProperties(false);
			agent.health += (int)(0.3f * agent.maxHealth);
			agent.mana += (int)(0.3f * agent.maxMana);
            isInFight = false;
            inSingleMoving = false;
            isInEvent = false;
            needPosFix = false;
            isDead = false;
            PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
            moveDestination = transform.position;
            singleMoveEndPos = transform.position;
            pathPosList.Clear();
            boxCollider.enabled = false;
            if (fadeStepsLeft == 0)
            {
                SetEffectAnim(CommonData.yinShenEffectName, null, 0, 0);
            }
            fadeStepsLeft = Mathf.Max(fadeStepsLeft, 5);
            GameManager.Instance.persistDataManager.SaveCompletePlayerData();
		}



	}
}
