using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace WordJourney
{
	using DG.Tweening;
	using DragonBones;
	using Transform = UnityEngine.Transform;
	using TMPro;

	public class BattlePlayerController : BattleAgentController {

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
//		private BattleMonsterController bmCtr;

		private NavigationHelper navHelper;

		public bool isInFight;
		public bool isInEvent; //是否在一个事件触发过程中
		public bool isInExplore;


		public TextMeshPro stepCount;
		private int mFadeStepsLeft;
		private int fadeStepsLeft{get{return mFadeStepsLeft;}
			set{
				mFadeStepsLeft = value;
				if (mFadeStepsLeft == 0) {
					boxCollider.enabled = true;
					stepCount.enabled = false;
				} else {
					stepCount.enabled = true;
					stepCount.text = mFadeStepsLeft.ToString ();
					boxCollider.enabled = false;
					if (mFadeStepsLeft > 5) {
						stepCount.color = Color.green;
					} else {
						stepCount.color = Color.red;
					}
				}
			}
		}

		protected override void Awake(){

			ActiveBattlePlayer (false, false, false);

			agent = GetComponentInParent<Player> ();

			navHelper = GetComponent<NavigationHelper> ();

			isInFight = false;

			isInExplore = false;

//			isAttackActionFinish = true;

			base.Awake ();

		}
			
		public void InitBattlePlayer(){
			
			Transform canvas = TransformManager.FindTransform ("ExploreCanvas");

			agentUICtr = canvas.GetComponent<BattlePlayerUIController> ();

			bpUICtr = agentUICtr as BattlePlayerUIController;

			moveDestination = transform.position;

		}


		public override void SetSortingOrder (int order)
		{
			playerForward.GetComponent<UnityArmatureComponent> ().sortingOrder = order;
			playerBackWard.GetComponent<UnityArmatureComponent> ().sortingOrder = order;
			playerSide.GetComponent<UnityArmatureComponent> ().sortingOrder = order;
		}

		public void SetNavigationOrigin(Vector3 position){
			singleMoveEndPos = position;
			moveDestination = position;
		}

		/// <summary>
		/// 按照指定路径 pathPosList 移动到终点 moveDestination
		/// </summary>
		/// <param name="pathPosList">Path position list.</param>
		/// <param name="moveDestination">End position.</param>
		public bool MoveToPosition(Vector3 moveDestination,int[,] mapWalkableInfoArray){

			if (isInFight) {
				return false;
			}

			// 计算自动寻路路径
			pathPosList = navHelper.FindPath(singleMoveEndPos,moveDestination,mapWalkableInfoArray);

			StopCoroutine ("MoveWithNewPath");

			this.moveDestination = moveDestination;

			if (pathPosList.Count == 0) {

				// 移动路径中没有点时，说明没有有效移动路径，此时终点设置为当前单步移动的终点
				this.moveDestination = singleMoveEndPos;

				moveTweener.OnComplete (() => {
					if(fadeStepsLeft > 0){
						fadeStepsLeft--;
					}
					inSingleMoving = false;
					PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);
				});

				Debug.Log ("无有效路径");

				return false;
			}

			StartCoroutine ("MoveWithNewPath");

			return pathPosList.Count > 0;

		}

		/// <summary>
		/// 按照新路径移动
		/// </summary>
		/// <returns>The with new path.</returns>
		private IEnumerator MoveWithNewPath(){

			// 如果移动动画不为空，则等待当前移动结束
			if (moveTweener != null) {

				// 如果开始新的移动时，原来的移动还没有结束，则将当步的动画结束回调改为只标记已走完，而不删除路径点（因为新的路径就是根据当步的结束点计算的，改点不在新路径内）
				moveTweener.OnComplete (() => {
					if(fadeStepsLeft > 0){
						fadeStepsLeft--;
					}
					inSingleMoving = false;
				});
					
			} 

			yield return new WaitUntil (() => !inSingleMoving);



			// 移动到新路径上的下一个节点
			MoveToNextPosition ();

		}


		/// <summary>
		/// 匀速移动到指定节点位置
		/// </summary>
		/// <param name="targetPos">Target position.</param>
		private void MoveToPosition(Vector3 targetPos){

			#if UNITY_EDITOR || UNITY_IOS
			exploreManager.newMapGenerator.UpdateFogOfWar ();
			#endif

			moveTweener =  transform.DOMove (targetPos, moveDuration).OnComplete (() => {

				bpUICtr.RefreshMiniMap();

				if(fadeStepsLeft > 0){
					fadeStepsLeft--;
				}

				// 动画结束时已经移动到指定节点位置，标记单步行动结束
				inSingleMoving = false;

				SetSortingOrder(-(int)transform.position.y);

				if(pathPosList.Count > 0){

					// 将当前节点从路径点中删除
					pathPosList.RemoveAt(0);

					// 移动到下一个节点位置
					MoveToNextPosition();

				}

			});

			// 设置匀速移动
			moveTweener.SetEase (Ease.Linear);


		}

		/// <summary>
		/// 判断当前是否已经走到了终点位置
		/// </summary>
		/// <returns><c>true</c>, if end point was arrived, <c>false</c> otherwise.</returns>
		private bool ArriveEndPoint(){


			if(MyTool.ApproximatelySamePosition2D(moveDestination,transform.position)){
//				Debug.Log ("到达终点");
				return true;
			}
//			Debug.Log ("继续移动");
			return false;

		}


		public void ActiveBattlePlayer(bool forward,bool backward,bool side){

			playerForward.SetActive (forward);
			playerBackWard.SetActive (backward);
			playerSide.SetActive (side);

			if (forward) {
				modelActive = playerForward;
			} else if (backward) {
				modelActive = playerBackWard;
			} else if (side) {
				modelActive = playerSide;
			}
		}



		/// <summary>
		/// 移动到下一个节点
		/// </summary>
		private void MoveToNextPosition ()
		{
			Vector3 nextPos = Vector3.zero;

			boxCollider.enabled = fadeStepsLeft == 0;

			if (pathPosList.Count > 0) {

				nextPos = pathPosList [0];

				if (ArriveEndPoint() && armatureCom.animation.lastAnimationName != CommonData.roleIdleAnimName) {
					PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);
					return;
				}

				bool resetWalkAnim = false;

				if(MyTool.ApproximatelySamePosition2D(nextPos,transform.position)){
					return;
				}

				if (Mathf.RoundToInt(nextPos.x) == Mathf.RoundToInt(transform.position.x)) {

					if (nextPos.y < transform.position.y) {
						if (modelActive != playerForward) {
							resetWalkAnim = true;
						}
						ActiveBattlePlayer (true, false, false);
						towards = MyTowards.Down;
					} else if (nextPos.y > transform.position.y) {
						if (modelActive != playerBackWard) {
							resetWalkAnim = true;
						}
						ActiveBattlePlayer (false, true, false);
						towards = MyTowards.Up;
					}

				}

				if(Mathf.RoundToInt(nextPos.y) == Mathf.RoundToInt(transform.position.y)){

					if (modelActive != playerSide) {
						resetWalkAnim = true;
					}else if ((nextPos.x > transform.position.x && armatureCom.armature.flipX == true) ||
						(nextPos.x < transform.position.x && armatureCom.armature.flipX == false)){
						resetWalkAnim = true;
					} 

					ActiveBattlePlayer (false, false, true);

					bool nextPosLeft = nextPos.x < transform.position.x;
					armatureCom.armature.flipX = nextPosLeft;
					towards = nextPosLeft ? MyTowards.Left : MyTowards.Right;

				}

				if (isIdle){
					resetWalkAnim = true;
				}

				if (resetWalkAnim) {
					PlayRoleAnim (CommonData.roleWalkAnimName, 0, null);
				} 
			}

			// 到达终点前的单步移动开始前进行碰撞检测
			// 1.如果碰撞体存在，则根据碰撞体类型给exploreManager发送消息执行指定回调
			// 2.如果未检测到碰撞体，则开始本次移动
			if (pathPosList.Count == 1) {

				Vector3 rayStartPos = (transform.position + pathPosList [0]) / 2;

				RaycastHit2D r2d = Physics2D.Linecast (rayStartPos, pathPosList [0], collosionLayer);

				if (r2d.transform != null) {

					MapEvent me = r2d.transform.GetComponent<MapEvent> ();

					bool needToStopMove = true;

					if (me != null) {
						needToStopMove = me.IsPlayerNeedToStopWhenEntered ();
					} else {
						needToStopMove = false;
					}

					if (needToStopMove) {
						StopMoveAndWait ();
					} 
						
					if (me != null) {
						exploreManager.currentEnteredMapEvent = me;

						isInEvent = true;

						me.EnterMapEvent (this);
					}

					if (needToStopMove) {
						return;
					} 

				}
			}
			// 路径中没有节点
			// 按照行动路径已经将所有的节点走完
			if (pathPosList.Count == 0) {

				// 走到了终点
				if (ArriveEndPoint ()) {
					moveTweener.Kill (true);
//					backgroundMoveTweener.Kill (true);
					PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);
//					Debug.Log ("到达终点");
				} else {
					Debug.Log (string.Format("actual pos:{0}/ntarget pos:{1},predicat pos{2}",transform.position,moveDestination,singleMoveEndPos));
					throw new System.Exception ("路径走完但是未到终点");
				}
				return;
			}

			// 如果还没有走到终点
			if (!ArriveEndPoint ()) {

				GameManager.Instance.soundManager.PlayAudioClip ("Other/sfx_Footstep");

				// 记录下一节点位置
				singleMoveEndPos = nextPos;

				// 向下一节点移动
				MoveToPosition (nextPos);

				// 标记单步移动中
				inSingleMoving = true;

			} else {
				moveTweener.Kill (true);
//				backgroundMoveTweener.Kill (true);
				PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);
			}

		}



		public void StopMoveAtEndOfCurrentStep(){

//			Debug.LogFormat ("{0}/{1}", transform.position, singleMoveEndPos);

			StopCoroutine ("MoveWithNewPath");

			this.moveDestination = singleMoveEndPos;

			pathPosList.Clear ();

			pathPosList.Add (singleMoveEndPos);

			if (moveTweener != null) {

				moveTweener.OnComplete (() => {

					if(fadeStepsLeft > 0){
						fadeStepsLeft--;
					}

					// 动画结束时已经移动到指定节点位置，标记单步行动结束
					inSingleMoving = false;

					SetSortingOrder(-(int)transform.position.y);

					if(pathPosList.Count > 0){

						// 将当前节点从路径点中删除
						pathPosList.RemoveAt(0);

						// 移动到下一个节点位置
						MoveToNextPosition();

					}

				});
			}

		}



		public void StopMoveAndWait(){
			StopCoroutine ("MoveWithNewPath");
			moveTweener.Kill (false);
			inSingleMoving = false;
			PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);
			SetSortingOrder (-(int)transform.position.y);
			Vector3 currentPos = new Vector3(Mathf.RoundToInt(transform.position.x),Mathf.RoundToInt(transform.position.y),0);
			moveDestination = currentPos;
			singleMoveEndPos = currentPos;
		}

		public override void TowardsLeft(bool andWait = true){
			ActiveBattlePlayer (false, false, true);
			if (andWait) {
				PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);
			}
			armatureCom.armature.flipX = true;
			towards = MyTowards.Left;
		}

		public override void TowardsRight(bool andWait = true){
			ActiveBattlePlayer (false, false, true);
			if (andWait) {
				PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);
			}
			armatureCom.armature.flipX = false;
			towards = MyTowards.Right;
		}

		public override void TowardsUp(bool andWait = true){
			ActiveBattlePlayer (false, true, false);
			if (andWait) {
				PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);
			}
			towards = MyTowards.Up;
		}

		public override void TowardsDown (bool andWait = true)
		{
			ActiveBattlePlayer (true, false, false);
			if (andWait) {
				PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);
			}
			towards = MyTowards.Down;
			
		}

		public void FixPosition(){
			Vector3 fixedPosition = new Vector3 (Mathf.RoundToInt (transform.position.x), 
				Mathf.RoundToInt (transform.position.y), 
				transform.position.z);
			transform.position = fixedPosition;
			singleMoveEndPos = fixedPosition;
			moveDestination = fixedPosition;
		}

		/// <summary>
		/// 战斗结束之后玩家移动到怪物原来的位置
		/// </summary>
//		public void PlayerMoveToEnemyPosAfterFight(Vector3 oriMonsterPos){
//
//			PlayRoleAnim ("wait", 0, null);
//
//			Vector3 targetPos = oriMonsterPos;
//
//			// 玩家角色位置和原来的怪物位置之间间距大于0.5（玩家是横向进入战斗的），则播放跑的动画到指定位置
//			if (Mathf.Abs (targetPos.x - transform.position.x) > 0.5) {
//				MoveToNextPosition ();
//			} else {// 玩家角色位置和原来的怪物位置之间间距小于0.5（玩家是纵向进入战斗的），则角色直接移动到指定位置，不播动画
//
//				transform.position = targetPos;
//
//				singleMoveEndPos = targetPos;
//
//				pathPosList.Clear ();
//
//				exploreManager.ItemsAroundAutoIntoLifeWithBasePoint (targetPos);
//
//			}
//
//		}



//		public override void InitFightTextDirectionTowards (BattleAgentController enemy)
//		{
//			MyTowards towards = MyTowards.Left;
//			if (transform.position.y == enemy.transform.position.y) {
//				towards = transform.position.x < enemy.transform.position.x ? MyTowards.Left : MyTowards.Right;
//			} else {
//				towards = enemy.towards == MyTowards.Left ? MyTowards.Left : MyTowards.Right;
//			}
//			bpUICtr.fightTextManager.SetUpFightTextManager (transform.position,towards);
//		}
//
//
//
//		public override void InitFightTextDirectionTowards (Vector3 position)
//		{
//			MyTowards fightTextTowards = MyTowards.Left;
//
//			fightTextTowards = position.x < transform.position.x ? MyTowards.Right : MyTowards.Left;
//
//			bpUICtr.fightTextManager.SetUpFightTextManager (transform.position, fightTextTowards);
//
//		}
//
//
//		public override void AddTintTextToQueue (string text, SpecialAttackResult specialAttackType)
//		{
//			if (bpUICtr != null) {
//				ExploreTintText ft = new ExploreTintText (text, specialAttackType);
//				bpUICtr.fightTextManager.AddFightText (ft);
//			}
//		}
//

		public void SetEnemy(BattleMonsterController bmCtr){
			this.enemy = bmCtr;
		}


		/// <summary>
		/// Starts the fight.
		/// </summary>
		/// <param name="bmCtr">怪物控制器</param>
		public void StartFight(BattleMonsterController bmCtr){

			isInFight = true;

			boxCollider.enabled = false;

			ClearAllSkillCallBacks ();

			InitTriggeredPassiveSkillCallBacks (this,bmCtr);

//			this.bmCtr = bmCtr;

			// 初始化玩家战斗UI（技能界面）
//			bpUICtr.SetUpPlayerSkillPlane (agent as Player);

			if (autoFight) {
				// 默认玩家在战斗中将先出招，首次攻击不用等待
				currentUsingActiveSkill = normalAttack;
				UseSkill (currentUsingActiveSkill);
			} else {
				PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);
			}

		}

		public bool autoFight = false;


		/// <summary>
		/// 角色默认战斗逻辑
		/// </summary>
		public override void Fight(){
			if (!autoFight) {
				PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);
			} else {
				currentUsingActiveSkill = normalAttack;
				attackCoroutine = InvokeAttack (currentUsingActiveSkill);
			}
		}



//		public void UseDefaultSkill(){
//			currentSkill = InteligentAttackSkill ();
//			UseSkill (currentSkill);
//		}

		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="skill">Skill.</param>
		public override void UseSkill (ActiveSkill skill)
		{
//			isAttackActionFinish = false;

			if (attackCoroutine != null) {
				StopCoroutine (attackCoroutine);
			}

			currentUsingActiveSkill = skill;

			// 播放技能对应的角色动画，角色动画结束后播放攻击间隔动画
			this.PlayRoleAnim (skill.selfRoleAnimName, 1, () => {
				// 播放等待动画
				this.PlayRoleAnim(CommonData.roleAttackIntervalAnimName,0,null);
			});

		}
			



		protected override void AgentExcuteHitEffect ()
		{
			if (!isInFight) {
				return;
			}

			// 播放技能对应的音效
			GameManager.Instance.soundManager.PlayAudioClip("Skill/" + currentUsingActiveSkill.sfxName);

			MapMonster mm = enemy.GetComponent<MapMonster> ();
			if (mm != null) {
				mm.isReadyToFight = true;
			}

			// 技能效果影响玩家和怪物
			currentUsingActiveSkill.AffectAgents(this,enemy);

			UpdateStatusPlane ();
			enemy.UpdateStatusPlane ();

//			isAttackActionFinish = true;

			if (enemy == null) {
				return;
			}

			// 如果战斗没有结束，则默认在攻击间隔时间之后按照默认攻击方式进行攻击
			if (!CheckFightEnd ()) {
				if (autoFight) {
					currentUsingActiveSkill = normalAttack;
					attackCoroutine = InvokeAttack (currentUsingActiveSkill);
					StartCoroutine (attackCoroutine);
				} else {
//					TransformManager.FindTransform("ExploreCanvas").GetComponent<ExploreUICotroller> ().ResetAttackCheckPosition ();
				}

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
			




		protected override void StopCoroutinesWhenFightEnd ()
		{
			base.StopCoroutinesWhenFightEnd ();
			playerSide.transform.localPosition = Vector3.zero;
			playerForward.transform.localPosition = Vector3.zero;
			playerBackWard.transform.localPosition = Vector3.zero;
//			isAttackActionFinish = true;
			Invoke ("RecheckPlayerPos",0.05f);
		}

		private void RecheckPlayerPos(){
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


		public void PlayRoleAnimByTime(string animName,float animBeginTime,int playTimes,CallBack cb){
			
			isIdle = animName == CommonData.roleIdleAnimName;

			// 如果还有等待上个角色动作结束的协程存在，则结束该协程
			if (waitRoleAnimEndCoroutine != null) {
				StopCoroutine (waitRoleAnimEndCoroutine);
			}


			// 播放新的角色动画
			armatureCom.animation.GotoAndPlayByTime(animName,animBeginTime,playTimes);

			// 如果有角色动画结束后要执行的回调，则开启一个新的等待角色动画结束的协程，等待角色动画结束后执行回调
			if (cb != null) {
				waitRoleAnimEndCoroutine = ExcuteCallBackAtEndOfRoleAnim (cb);
				StartCoroutine(waitRoleAnimEndCoroutine);
			}
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

//			enterMonster = null;
//			enterNpc = null;
//			enterCrystal = null;
//			enterTreasureBox = null;
//			enterObstacle = null;
//			enterTrapSwitch = null;
//			enterDoor = null;
//			enterBillboard = null;
//			enterHole = null;
//			enterMovableBox = null;
//			enterTransport = null;
//			enterPlant = null;

			ClearAllEffectStatesAndSkillCallBacks ();

//			trapTriggered = null;
			enemy = null;

//			playerLoseCallBack = null;

			bpUICtr = null;

		}


		public void QuitExplore(){

//			CollectSkillEffectsToPool ();

			ClearReference ();

			gameObject.SetActive (false);

		}
			

		public override void QuitFight(){

			StopCoroutinesWhenFightEnd ();

			enemy = null;
			isInFight = false;
			isIdle = true;
			currentUsingActiveSkill = null;

			SetRoleAnimTimeScale (1.0f);

			agent.ResetBattleAgentProperties (false);

//			if (!agent.isDead) {
//				PlayRoleAnim ("wait", 0, null);
//			}
		}

		/// <summary>
		/// 玩家死亡
		/// </summary>
		override public void AgentDie(){

			if (agent.isDead) {
				return;
			}

			agent.isDead = true;

			StartCoroutine ("LatelyDie");

		}

		private IEnumerator LatelyDie(){

			yield return new WaitForSeconds (0.5f);

			bool fromFight = isInFight;

			enemy.QuitFight ();

			QuitFight ();

			PlayRoleAnim (CommonData.roleDieAnimName, 1, () => {
				if(fromFight){
					exploreManager.BattlePlayerLose ();
				}else{
					exploreManager.expUICtr.ShowBuyLifeQueryHUD();
				}
			});

			ActiveBattlePlayer (false, false, true);

			ExploreManager.Instance.DisableInteractivity ();
		}


		public void RecomeToLife(){
			agent.ResetBattleAgentProperties (true);
			PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);
			isInFight = false;
			isInExplore = true;
			inSingleMoving = false;
			moveDestination = transform.position;
			singleMoveEndPos = transform.position;
			pathPosList.Clear ();
			fadeStepsLeft = 20;
		}

		void OnDestroy(){
//			StopAllCoroutines ();
//			bpUICtr = null;
//			bmCtr = null;
//			navHelper = null;
//			agent = null;
//			propertyCalculator = null;
//			mExploreManager = null;
//			expUICtr = null;
//			enemy = null;
//			for (int i = 0; i < beforeFightTriggerExcutors.Count; i++) {
//				beforeFightTriggerExcutors [i].OnClear ();
//			}
//			for (int i = 0; i < attackTriggerExcutors.Count; i++) {
//				attackTriggerExcutors [i].OnClear ();
//			}
//			for (int i = 0; i < beAttackedTriggerExcutors.Count; i++) {
//				beAttackedTriggerExcutors [i].OnClear ();
//			}
//			for (int i = 0; i < hitTriggerExcutors.Count; i++) {
//				hitTriggerExcutors [i].OnClear ();
//			}
//			for (int i = 0; i < beHitTriggerExcutors.Count; i++) {
//				beHitTriggerExcutors [i].OnClear ();
//			}
//			for (int i = 0; i < fightEndTriggerExcutors.Count; i++) {
//				fightEndTriggerExcutors [i].OnClear ();
//			}
//			currentSkill = null;
//			for (int i = 0; i < activeSkills.Count; i++) {
//				Destroy (activeSkills [i].gameObject);
//			}
//
//			enterBillboard = null;
//			enterCrystal = null;
//			enterDoor = null;
//			enterHole = null;
//			enterMonster = null;
//			enterMovableBox = null;
//			enterNpc = null;
//			enterObstacle = null;
//			enterPlant = null;
//			enterTransport = null;
//			enterTrapSwitch = null;
//			enterTreasureBox = null;
//
//			moveTweener = null;
//
//			backgroundMoveTweener = null;
//
//			pathPosList = null;

		}


	}
}
