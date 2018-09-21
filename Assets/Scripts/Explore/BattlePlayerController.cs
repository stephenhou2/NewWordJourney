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

        public bool needPosFix;

        public bool escapeFromFight; //是否从战斗中逃脱

        public bool isInEscaping; //是否正在脱离战斗中

        public bool isInPosFixAfterFight;

		public bool isInSkillAttackAnimBeforeHit;// 是否处在 [非普通攻击] 的技能击中前的攻击动作中

		//private bool isInAttackAnimBeforeHit;// 是否处在技能生效前的动作中 [包括普通攻击]
        
		private IEnumerator attackDelayWhenChangeWeapon;// 更换武器时的等待携程

		private IEnumerator newMoveCoroutine;// 新的行走协程

        public TextMeshPro stepCount;
        private int mFadeStepsLeft;
		public int fadeStepsLeft
        {
            get { return mFadeStepsLeft; }
            set
            {
                mFadeStepsLeft = value;
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

        public void InitBattlePlayer()
        {

            Transform canvas = TransformManager.FindTransform("ExploreCanvas");

            agentUICtr = canvas.GetComponent<BattlePlayerUIController>();

            bpUICtr = agentUICtr as BattlePlayerUIController;

            moveDestination = transform.position;

        }


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

            boxCollider.enabled = true;

            if (isInFight)
            {
                return false;
            }

            if (isInEvent)
            {
                return false;
            }

            // 计算自动寻路路径
            pathPosList = navHelper.FindPath(singleMoveEndPos, moveDestination, mapWalkableInfoArray);

			if(newMoveCoroutine != null){
				StopCoroutine(newMoveCoroutine);
			}

            this.moveDestination = moveDestination;

            if (pathPosList.Count == 0)
            {

                // 移动路径中没有点时，说明没有有效移动路径，此时终点设置为当前单步移动的终点
                this.moveDestination = singleMoveEndPos;

                moveTweener.OnComplete(() =>
                {
                    if (fadeStepsLeft > 0)
                    {
                        fadeStepsLeft--;
                    }
                    inSingleMoving = false;
                    PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
					SetSortingOrder(-Mathf.RoundToInt(transform.position.y));
                });

                Debug.Log("无有效路径");

                return false;
            }

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

            // 如果移动动画不为空，则等待当前移动结束
            if (moveTweener != null)
            {
                        
                // 如果开始新的移动时，原来的移动还没有结束，则将当步的动画结束回调改为只标记已走完，而不删除路径点（因为新的路径就是根据当步的结束点计算的，改点不在新路径内）
                moveTweener.OnComplete(() =>
                {
					Vector3 targetPos = transform.position;

					exploreManager.newMapGenerator.miniMapPlayer.localPosition = targetPos;
               
                    exploreManager.newMapGenerator.ClearMiniMapMaskAround(targetPos);

					exploreManager.newMapGenerator.MiniMapCameraLatelySleep();

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


            if (MyTool.ApproximatelySamePosition2D(moveDestination, transform.position))
            {
            //			Debug.Log ("到达终点");
                return true;
            }
            //			Debug.Log ("继续移动");
            return false;

        }


        public void ActiveBattlePlayer(bool forward, bool backward, bool side)
        {


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
        /// 移动到下一个节点
        /// </summary>
        private void MoveToNextPosition()
        {
            Vector3 nextPos = Vector3.zero;

            boxCollider.enabled = fadeStepsLeft == 0;

            bool resetWalkAnim = false;

            if (pathPosList.Count > 0)
            {

                nextPos = pathPosList[0];

                if (ArriveEndPoint() && armatureCom.animation.lastAnimationName != CommonData.roleIdleAnimName)
                {
                    PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
                    return;
                }

               

                if (MyTool.ApproximatelySamePosition2D(nextPos, transform.position))
                {
                    pathPosList.RemoveAt(0);
                    MoveToNextPosition();
                    return;
                }

                if (pathPosList.Count >= 2)
                {

                    Vector3 firstFollowingPos = pathPosList[0];
                    Vector3 secondFollowingPos = pathPosList[1];

                    if ((firstFollowingPos.x - transform.position.x) * (secondFollowingPos.x - transform.position.x) < 0
                        && Mathf.RoundToInt(firstFollowingPos.y) == Mathf.RoundToInt(transform.position.y)
                        && Mathf.RoundToInt(firstFollowingPos.y) == Mathf.RoundToInt(secondFollowingPos.y))
                    {
                        pathPosList.RemoveAt(0);
                        MoveToNextPosition();
                        return;
                    }
                }

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

                Vector3 rayStartPos = (transform.position + pathPosList[0]) / 2;

                RaycastHit2D r2d = Physics2D.Linecast(rayStartPos, pathPosList[0], collosionLayer);

                if (r2d.transform != null)
                {

                    MapEvent me = r2d.transform.GetComponent<MapEvent>();

                    bool needToStopMove = true;

                    if (me != null)
                    {
                        needToStopMove = me.IsPlayerNeedToStopWhenEntered();
                    }
                    else
                    {
                        needToStopMove = false;
                    }

                    if (needToStopMove)
                    {
                        StopMoveAndWait();
                    }

                    if (me != null)
                    {
                        exploreManager.currentEnteredMapEvent = me;

                        isInEvent = true;

                        me.EnterMapEvent(this);
                    }

                    if (needToStopMove)
                    {
                        return;
                    }

                }
            }
            // 路径中没有节点
            // 按照行动路径已经将所有的节点走完
            if (pathPosList.Count == 0)
            {

                // 走到了终点
                if (ArriveEndPoint())
                {
                    moveTweener.Kill(true);

                    if (!isIdle)
                    {
                        PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
                    }
                }
                else
                {
                    Debug.Log(string.Format("actual pos:{0}/ntarget pos:{1},predicat pos{2}", transform.position, moveDestination, singleMoveEndPos));
                    throw new System.Exception("路径走完但是未到终点");
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
               
            }

        }
        
		public void ForceMoveToAndStopWhenEnconterWithMapEvent(Vector3 pos,CallBack callBack){
               
			if (newMoveCoroutine != null)
            {
                StopCoroutine(newMoveCoroutine);
            }

			MoveToPosition(pos);         

            if (moveTweener != null)
            {            
                moveTweener.OnComplete(() =>
                {
                    if (callBack != null)
                    {
                        callBack();
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

                        // 移动到下一个节点位置
                        MoveToNextPosition();

                    }
                });
            }
            else if (!isIdle)
            {
                PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
            }


		}

		public void StopMoveAtEndOfCurrentStep(CallBack callBack = null)
        {
         
			if (newMoveCoroutine != null)
            {
                StopCoroutine(newMoveCoroutine);
            }

            this.moveDestination = singleMoveEndPos;

			MoveToPosition(singleMoveEndPos);

			//MoveToNextPosition();

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

                    if (pathPosList.Count > 0)
                    {

                        // 将当前节点从路径点中删除
                        pathPosList.RemoveAt(0);

                        // 移动到下一个节点位置
                        MoveToNextPosition();

                    }
                });
            }
            else if(!isIdle)
            {
                PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
            }

        }



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

        public override void TowardsUp(bool andWait = true)
        {
            ActiveBattlePlayer(false, true, false);
            if (andWait && !isIdle)
            {
                PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
            }
            towards = MyTowards.Up;
        }

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
              
		public void SetEnemy(BattleMonsterController bmCtr){
			this.enemy = bmCtr;
		}


		/// <summary>
		/// Starts the fight.
		/// </summary>
		/// <param name="bmCtr">怪物控制器</param>
		public void StartFight(BattleMonsterController bmCtr){

			if(isInFight){
				return;
			}

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

			//isInAttackAnimBeforeHit = true;

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
					if(isDead){
						return;
					}
					this.PlayRoleAnim(CommonData.roleAttackIntervalAnimName, 0, null);
				}
               
			});

		}
			



		protected override void AgentExcuteHitEffect ()
		{

			//Debug.Log("hit");

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

			//isInAttackAnimBeforeHit = false;

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

			// 如果战斗没有结束，则默认在攻击间隔时间之后按照默认攻击方式进行攻击
			if (isInFight && !CheckFightEnd ()) {
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

			if (enemy.agent.health <= 0) {
				enemy.AgentDie ();
				isInFight = false;
				return true;
			} else if (agent.health <= 0) {
				AgentDie ();
				isInFight = false;
				return true;
			}else {
				return false;
			}

		}
			
		public void ResetAttackAfterInterval(HLHRoleAnimInfo animInfo,float interval){

			string intervalAnimName = RoleAnimNameAdapt(CommonData.playerIntervalBareHandName);

			PlayRoleAnim(intervalAnimName, 0, null);

			if(attackDelayWhenChangeWeapon != null){
				StopCoroutine(attackDelayWhenChangeWeapon);
			}

			attackDelayWhenChangeWeapon = MyResetAttackAfterInterval(animInfo, interval);

			StartCoroutine(attackDelayWhenChangeWeapon);

		}


		private IEnumerator MyResetAttackAfterInterval(HLHRoleAnimInfo animInfo,float interval){

			yield return new WaitForSeconds(interval);

			ResetAttack(animInfo);
            
		}


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



		protected override void StopCoroutinesWhenFightEnd ()
		{
			base.StopCoroutinesWhenFightEnd ();
			playerSide.transform.localPosition = Vector3.zero;
			playerForward.transform.localPosition = Vector3.zero;
			playerBackWard.transform.localPosition = Vector3.zero;
			IEnumerator resetPlayerPosCoroutine = ResetPlayerPos();
			StartCoroutine(resetPlayerPosCoroutine);
		}

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



		public HLHRoleAnimInfo GetCurrentRoleAnimInfo(){
			string currentAnimName = armatureCom.animation.lastAnimationName;
			AnimationState state = armatureCom.animation.GetState (currentAnimName);
			float animTime = state.currentTime;
			int playTimes = state.playTimes;
			return new HLHRoleAnimInfo (currentAnimName, playTimes, animTime, animEndCallBack);
		}

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

		public override void PlayRoleAnim (string animName, int playTimes, CallBack cb)
		{
			animName = RoleAnimNameAdapt (animName);
			base.PlayRoleAnim (animName, playTimes, cb);
		}

		public void PlayRoleAnimByTime(string animName,float animBeginTime,int playTimes,CallBack cb){
            
			//Debug.Log(animName);

			isIdle = animName == CommonData.roleIdleAnimName;

			//Debug.LogFormat("anima name:{0},  begin time:{1}", animName,animBeginTime);


			//Debug.Break();

			//if(IsInAttackAnim(animName) && isInAttackAnimBeforeHit){
				
			//	StopCoroutine(attackCoroutine);
			//}

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

		private bool IsInAttackAnim(string animName){
			return animName == CommonData.roleAttackAnimName
			|| animName == CommonData.playerAttackBareHandName
			|| animName == CommonData.playerAttackWithSwordName
			|| animName == CommonData.playerAttackWithStaffName
			|| animName == CommonData.playerAttackWithAxeName
			|| animName == CommonData.playerAttackWithDraggerName;

		}

		private bool IsInIntervalAnim(string animName){
			return animName == CommonData.roleAttackIntervalAnimName
			|| animName == CommonData.playerIntervalBareHandName
			|| animName == CommonData.playerIntervalWithSwordName
			|| animName == CommonData.playerIntervalWithStaffName
			|| animName == CommonData.playerIntervalWithAxeName
			|| animName == CommonData.playerIntervalWithDraggerName;
		}

		private bool IsInPhysicalSkillAnim(string animName){
			return animName == CommonData.rolePhysicalSkillAnimName
			|| animName == CommonData.playerPhysicalSkillBareHandName
			|| animName == CommonData.playerPhysicalSkillWithSwordName
			|| animName == CommonData.playerPhysicalSkillWithStaffName
			|| animName == CommonData.playerPhysicalSkillWithAxeName
			|| animName == CommonData.playerPhysicalSkillWithDraggerName;
		}

		private bool IsInMagiclSkillAnim(string animName){
			return animName == CommonData.roleMagicalSkillAnimName
			|| animName == CommonData.playerMagicalSkillBareHandName
			|| animName == CommonData.playerMagicalSkillWithSwordName
			|| animName == CommonData.playerMagicalSkillWithStaffName
			|| animName == CommonData.playerMagicalSkillWithAxeName
			|| animName == CommonData.playerMagicalSkillWithDraggerName;

		}

		private string RoleAnimNameAdapt(string animName){

			string adaptName = animName;

			bool adaptException = animName ==CommonData.roleWalkAnimName
			                 || animName == CommonData.roleIdleAnimName
			                 || animName == CommonData.roleDieAnimName;

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

			//if (towards == MyTowards.Up || towards == MyTowards.Down) {
			//	if (IsInAttackAnim (animName)) {
			//		adaptName = CommonData.roleAttackAnimName;
			//	} else if (IsInIntervalAnim (animName)) {
			//		adaptName = CommonData.roleAttackIntervalAnimName;
			//	} else if (IsInPhysicalSkillAnim (animName)) {
			//		adaptName = CommonData.roleAttackAnimName;
			//	} else if (IsInMagiclSkillAnim (animName)) {
			//		adaptName = CommonData.roleAttackAnimName;
			//	}
			//}

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


		public void QuitExplore(){

			AllEffectAnimsIntoPool();

			ClearReference ();

			gameObject.SetActive (false);

		}
			

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

			BattleAgentController enemyRecord = enemy;
                     
			ExploreManager.Instance.DisableAllInteractivity ();

			exploreManager.expUICtr.ShowFullMask();
         
			enemy.boxCollider.enabled = true;

			bool fromFight = isInFight;

			enemy.QuitFight();
            QuitFight();

			isInEvent = true;

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.playerDieAudioName);

			enemy.AllEffectAnimsIntoPool();

			PlayRoleAnim (CommonData.roleDieAnimName, 1, ()=>{
				IEnumerator queryBuyLifeCoroutine = QueryBuyLife(fromFight, enemyRecord);
				StartCoroutine(queryBuyLifeCoroutine);
				AllEffectAnimsIntoPool();
				enemy.AllEffectAnimsIntoPool();
				enemy = null;
			});

		}
			
		private IEnumerator QueryBuyLife(bool fromFight,BattleAgentController enemyRecord){

			yield return new WaitForSeconds (0.5f);

			if(fromFight){
				exploreManager.BattlePlayerLose (enemyRecord);
			}else{
				exploreManager.expUICtr.ShowBuyLifeQueryHUD();
				PlayRecord playRecord = new PlayRecord(false, "陷阱");
				List<PlayRecord> playRecords = GameManager.Instance.gameDataCenter.allPlayRecords;            
				playRecords.Add(playRecord);
				GameManager.Instance.persistDataManager.SavePlayRecords(playRecords);
			}
		}


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

		void OnDestroy(){

		}


	}
}
