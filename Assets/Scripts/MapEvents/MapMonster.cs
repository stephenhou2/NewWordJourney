using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using System.Data;
	using DragonBones;
	using Transform = UnityEngine.Transform;

    /// <summary>
    /// 地图怪物控制器
    /// </summary>
	public class MapMonster : MapWalkableEvent {

        // 怪物探测区域
		public MonsterAlertArea[] alertAreas;

        // 警示【发现人物角色时头上的感叹号闪烁】
		public UnityArmatureComponent alertTint;

        // 碰撞层
		public LayerMask collisionLayer;
        
        // 警示图标的偏移量
		private float alertIconOffsetX;
		private float alertIconOffsetY;

        // 怪物说的话的偏移量
		private float monsterSayContainerOffsetX;
		private float monsterSayContainerOffsetY;
      
        // 怪物说的话容器
		public SpriteRenderer monsterSayContainer;

        // 标示是否已做好战斗准备
		public bool isReadyToFight;

		// 触发机关的位置【目前只有boss有这个功能，如果没有触发的机关，则设置为（-1，-1，-1）】
		public Vector3 pairEventPos;

        // 在地图上的原始位置
		public Vector2 oriPos;

        // 地图高度【用于进行坐标转换，例如击败怪物控制地图上一个事件的状态发生变化，由于地图数据中传入的位置是以左上角为原点，所以要进行一次y轴上的坐标转换】
		private int mapHeight;

        // 地图序号【用于地图事件记录，目前只有boss被击败后在对应地图序号的地图事件上会记录boss的原始位置，下次初始化地图的时候boss就不会再出来了】
		public int mapIndex;

        // 标示是否有奖励
		public bool hasReward = true;

        // 怪物骨骼朝向【怪物骨骼朝向只有左右朝向，但是根据行走方向，还定义了朝上和朝下】
		private MyTowards boneTowards;


        // 控制怪物说话的显示和消失的协程
		private IEnumerator monsterSayCoroutine;

        // 传入地图高度
		public void SetPosTransferSeed(int mapHeight){
			this.mapHeight = mapHeight;
		}


		protected override void Awake ()
		{

			base.Awake();

			alertIconOffsetX = alertTint.transform.localPosition.x;
			alertIconOffsetY = alertTint.transform.localPosition.y;

			if(monsterSayContainer != null){
				monsterSayContainerOffsetX = 1000 + monsterSayContainer.transform.localPosition.x;
                monsterSayContainerOffsetY = monsterSayContainer.transform.localPosition.y;
			}

		}

        /// <summary>
        /// 加入缓存池
        /// </summary>
        /// <param name="pool">Pool.</param>
		public override void AddToPool (InstancePool pool)
		{
			StopMoveImmidiately ();
			if(delayMoveCoroutine != null){
				StopCoroutine(delayMoveCoroutine);
			}
			DisableAllDetect ();
			isReadyToFight = false;
			HideAllAlertAreas ();
			bc2d.enabled = false;
			gameObject.SetActive (false);
			pool.AddInstanceToPool (this.gameObject);

		}

        /// <summary>
        /// 添加到当前地图的地图事件记录中
        /// </summary>
		public void AddToCurrentMapEventRecord(){
			GameManager.Instance.gameDataCenter.currentMapEventsRecord.AddEventTriggeredRecord(mapIndex, oriPos);
		}

        /// <summary>
        /// 当怪物死亡时重置
        /// </summary>
		public override void ResetWhenDie(){

			StopAllCoroutines ();
			HideAllAlertAreas ();
			alertTint.gameObject.SetActive(false);

		}

        /// <summary>
        /// 退出战斗，等待一段时间后重新开始行走
        /// </summary>
        /// <param name="delay">Delay.</param>
		public override void QuitFightAndDelayMove(int delay){

            // 首先立刻停止运动
			StopMoveImmidiately ();

            // 重置一些属性
			isInAutoWalk = false;
			isTriggered = false;
			isReadyToFight = false;

            // 隐藏探测区域
			HideAllAlertAreas ();

			bc2d.enabled = true;

            
			if (delayMoveCoroutine != null)
            {
                StopCoroutine(delayMoveCoroutine);
            }

			delayMoveCoroutine = DelayedMovement(delay);

			StartCoroutine (delayMoveCoroutine);

		}


		private IEnumerator DelayedMovement(int delay){

			yield return new WaitForSeconds (delay);

			StartMove ();

		}


        /// <summary>
        /// 显示警示区域
        /// </summary>
		public void ShowAlertAreaTint(){
			MyTowards towards = baCtr.towards;
			for (int i = 0; i < alertAreas.Length; i++) {
				if (i == (int)towards) {
					alertAreas [i].ShowAlerAreaTint ();
				} else {
					alertAreas [i].HideAlertAreaTint ();
				}
			}
		}

        /// <summary>
        /// disable所有探测区域
        /// </summary>
		public void DisableAllDetect(){

			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].DisableAlertDetect ();
			}

			bc2d.enabled = false;

		}

		/// <summary>
		/// 隐藏所有的警示区域
		/// </summary>
		public void HideAllAlertAreas(){
			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].HideAlertAreaTint ();
			}
		}


        
        /// <summary>
        /// 当trigger进入到怪物的探测包围盒中时
        /// </summary>
        /// <param name="col">Col.</param>
		public void OnTriggerEnter2D (Collider2D col){

			BattlePlayerController bp = col.GetComponent<BattlePlayerController> ();

            // 如果进入包围盒的不是玩家角色，直接返回
			if (bp == null) {
				return;
			}

            // 如果玩家处于隐身状态，直接返回
			if (bp.fadeStepsLeft > 0)
            {
                return;
            }

            // 如果玩家在地图事件中，直接返回
			if (bp.isInEvent) {
				return;
			}

            // 如果玩家在战斗中，直接返回
			if (bp.isInFight) {
				return;
			}

            // 如果怪物已经死亡，直接返回【例如被一刀打死了】
			if (baCtr.isDead) {
				return;
			}

            // 如果玩家在位置修正中，直接返回
			if (bp.isInPosFixAfterFight) {
				return;
			}

			isReadyToFight = true;

            // 探测到玩家
			DetectPlayer(bp);
		}

        /// <summary>
        /// 进入地图事件
        /// </summary>
        /// <param name="bp">Bp.</param>
		public override void EnterMapEvent (BattlePlayerController bp)
		{         
			if (isInMoving) {
				RefreshWalkableInfoWhenTriggeredInMoving ();
			}

			bp.isInEvent = true;

            // 所有运动中的怪物/npc停止运动【本步结束以后】
			ExploreManager.Instance.MapWalkableEventsStopAction ();
                     
            // 自己这只怪物立刻停止运动
			StopMoveImmidiately ();

            // 人物停止运动并且原地等待
			bp.StopMoveAndWait ();

            // 进入战斗
			ExploreManager.Instance.EnterFight (this.transform);

			MapEventTriggered (false, bp);
		}

        /// <summary>
        /// 探测到用户
        /// </summary>
        /// <param name="bp">Bp.</param>
		public void DetectPlayer(BattlePlayerController bp){

			if(bp.fadeStepsLeft > 0){
				return;
			}

			if (bp.escapeFromFight) {
				return;
			}

			bp.isInEvent = true;

			if (isInMoving) {
				RefreshWalkableInfoWhenTriggeredInMoving ();
			}

			ExploreManager.Instance.MapWalkableEventsStopAction ();

			StopMoveImmidiately ();

			bp.StopMoveAtEndOfCurrentStep ();

			ExploreManager.Instance.EnterFight (this.transform);

			MapEventTriggered (false, bp);
            
		}

        /// <summary>
        /// 使用地图附加信息初始化怪物
        /// </summary>
        /// <param name="mapIndex">Map index.</param>
        /// <param name="attachedInfo">Attached info.</param>
		public override void InitializeWithAttachedInfo(int mapIndex,MapAttachedInfoTile attachedInfo)
		{
			// 地图序号
			this.mapIndex = mapIndex;

			this.oriPos = attachedInfo.position;

			this.moveOrigin = attachedInfo.position;

			this.moveDestination = attachedInfo.position;

			if(monsterSayContainer != null){
				monsterSayContainer.enabled = false;
			}

			if(tmPro != null){
				tmPro.text = string.Empty;
			}
                     

            // 标记怪物是否可以移动
			canMove = bool.Parse(KVPair.GetPropertyStringWithKey("canMove",attachedInfo.properties));
            // 击败怪物对应控制的地图事件的位置信息
			string pairEventPosString = KVPair.GetPropertyStringWithKey ("pairEventPos", attachedInfo.properties);

            // 击败怪物后控制地图事件的位置信息
			if (pairEventPosString != string.Empty && pairEventPosString != "-1") {

				string[] posXY = pairEventPosString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);

				int posX = int.Parse (posXY [0]);
				int posY = mapHeight - int.Parse (posXY [1]) - 1;

				pairEventPos = new Vector3 (posX, posY, transform.position.z);
			} else {
				pairEventPos = -Vector3.one;
			}

			this.gameObject.SetActive(true);

			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].InitializeAlertArea ();
			}

            // 隐藏所有的探测区域
			HideAllAlertAreas ();

            // 随机朝向
            RandomTowards();

            // 如果可以运动，则显示探测区域
			if (canMove) {
				ShowAlertAreaTint ();
			}

			transform.position = attachedInfo.position;

			gameObject.SetActive (true);

			GetComponent<Monster> ().ResetBattleAgentProperties (true);

            // 激活怪物
			baCtr.SetAlive();

            // 重置属性
			bc2d.enabled = true;
			isReadyToFight = false;

			isTriggered = false;

			baCtr.isIdle = false;
			isInMoving = false;
			isInAutoWalk = false;
         
			alertTint.gameObject.SetActive(false);
         
		}

        /// <summary>
        /// 随机朝向
        /// </summary>
		private void RandomTowards(){

			int towardsIndex = Random.Range (0, 4);

			boneTowards = MyTowards.Right;

			switch (towardsIndex) {
			case 0:
				baCtr.TowardsRight ();
				boneTowards = MyTowards.Right;

				break;
			case 1:
				baCtr.TowardsLeft ();
				boneTowards = MyTowards.Left;
				break;
			case 2:
				baCtr.TowardsUp ();
                // 朝上时要查一下骨骼实际的朝向
				boneTowards = (baCtr as BattleMonsterController).GetMonsterBoneTowards();               
				break;
			case 3:
				baCtr.TowardsDown ();
				// 朝下时要查一下骨骼实际的朝向
				boneTowards = (baCtr as BattleMonsterController).GetMonsterBoneTowards();         
				break;
			}




		}


		/// <summary>
		/// 怪物头上红色感叹号闪烁动画
		/// </summary>
		/// <returns>The to fight icon shining.</returns>
		private void AlertTintSpark(){

            // 根据骨骼动画的朝向，决定红色❗️的位置
			switch (boneTowards)
            {
                case MyTowards.Right:
                    alertTint.transform.localPosition = new Vector3(alertIconOffsetX, alertIconOffsetY, 0);
                    break;
                case MyTowards.Left:
                    alertTint.transform.localPosition = new Vector3(-alertIconOffsetX, alertIconOffsetY, 0);
                    break;
            }
         
			alertTint.gameObject.SetActive(true);

			IEnumerator alertCoroutine = AlertTintLatelyHide();
			StartCoroutine(alertCoroutine);
		}


        /// <summary>
        /// 显示怪物说的话
        /// </summary>
        /// <param name="say">Say.</param>
		private void ShowMonsterSay(string say){
			switch (boneTowards)
            {
                case MyTowards.Right:
					monsterSayContainer.transform.localPosition = new Vector3(monsterSayContainerOffsetX - 1000, monsterSayContainerOffsetY, 0);
                    break;
                case MyTowards.Left:
					monsterSayContainer.transform.localPosition = new Vector3(-monsterSayContainerOffsetX - 1000, monsterSayContainerOffsetY, 0);
                    break;
            }

			monsterSayContainer.enabled = true;

			tmPro.text = say;

			if(monsterSayCoroutine != null){
				StopCoroutine(monsterSayCoroutine);
			}

            // 控制怪物说的话消失的协程
			monsterSayCoroutine = MonsterSayDelayDisappear();

			StartCoroutine(monsterSayCoroutine);
		}


		private IEnumerator MonsterSayDelayDisappear(){
			yield return new WaitForSeconds(2.0f);
			monsterSayContainer.enabled = false;
			tmPro.text = string.Empty;
		}

        /// <summary>
        /// 获取怪物说的话
        /// </summary>
        /// <returns>The monster say.</returns>
		private string GetMonsterSay(){
			string say = string.Empty;
			if (wordsArray != null && wordsArray.Length > 0)
			{


				int randomSeed = Random.Range(0, 2);
				if (randomSeed == 0)
				{
					randomSeed = Random.Range(0, 3);
					say = (baCtr.agent as Monster).monsterSays[randomSeed];
				}
				else
				{
					randomSeed = Random.Range(0, 3);
					HLHWord word = wordsArray[randomSeed];
					say = word.spell;
				}
			}

			return say;
		}

        /// <summary>
        /// 警示区域延迟消失
        /// </summary>
        /// <returns>The tint lately hide.</returns>
		private IEnumerator AlertTintLatelyHide(){

			yield return new WaitForSeconds(0.1f);

			alertTint.animation.Play("default", 1);

			yield return new WaitUntil(() => alertTint.animation.isCompleted);

			alertTint.gameObject.SetActive(false);

		}
			
        /// <summary>
        /// 地图事件触发后的逻辑
        /// </summary>
        /// <param name="isSuccess">If set to <c>true</c> is success.</param>
        /// <param name="bp">Bp.</param>
		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			if (isTriggered) {
				return;
			}
            

			bp.escapeFromFight = false;
			bp.isInEscaping = false;

			if (!isReadyToFight) {            
				ExploreManager.Instance.PlayerStartFight ();
			}

			IEnumerator resetPositionAndFightCoroutine = ResetPositionAndStartFight(bp);
			StartCoroutine (resetPositionAndFightCoroutine);

			isTriggered = true;
                     
		}

        /// <summary>
        /// 调整位置后开始战斗
        /// </summary>
        /// <returns>The position and start fight.</returns>
        /// <param name="battlePlayerCtr">Battle player ctr.</param>
		private IEnumerator ResetPositionAndStartFight(BattlePlayerController battlePlayerCtr){

            // 等待准备好
			yield return new WaitUntil (() => isReadyToFight);
            // 头山闪红色叹号
			AlertTintSpark();
            // 播放idle动画
			baCtr.PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);
            // 判断玩家角色是否需要重新开始攻击动作
			bool playerNeedResetAttack = battlePlayerCtr.NeedResetAttack();
            
			yield return new WaitForSeconds (0.4f);

			HideAllAlertAreas ();
			DisableAllDetect ();

            Vector3 playerOriPos = battlePlayerCtr.transform.position;
            Vector3 monsterOriPos = transform.position;

            int playerPosX = Mathf.RoundToInt(playerOriPos.x);
            int playerPosY = Mathf.RoundToInt(playerOriPos.y);
            int monsterPosX = Mathf.RoundToInt(monsterOriPos.x);
            int monsterPosY = Mathf.RoundToInt(monsterOriPos.y);

            int monsterLayerOrder = -monsterPosY;

			int posOffsetX = playerPosX - monsterPosX; 
            int posOffsetY = playerPosY - monsterPosY;

			Vector3 monsterRunPos = Vector3.zero;
			Vector3 monsterFightPos = Vector3.zero;
            Vector3 playerFightPos = new Vector3(playerPosX, playerPosY, 0);

            int minX = 0;
            int maxX = ExploreManager.Instance.newMapGenerator.columns - 1;

			HLHRoleAnimInfo playerCurrentAnimInfo = battlePlayerCtr.GetCurrentRoleAnimInfo ();
         


            // 根据玩家角色和怪物的位置，战斗点附近的可行走情况决定人物和怪物的战斗位置
            //人物在怪物右边
			if (posOffsetX > 0) {
				// 人物角色先朝左
				battlePlayerCtr.TowardsLeft(!battlePlayerCtr.isInFight);
                // 如果怪物在人物水平向左一格的附近位置，并且该位置没有超出地图左侧界限
				if (playerPosX - 1 >= minX && playerPosX - 1 == monsterPosX && playerPosY == monsterPosY) {
					// 怪物跑向的位置【人物左边一格】
					monsterRunPos = new Vector3(playerPosX - 1f, playerPosY, 0);
                    // 怪物战斗位置【人物左边一格】
					monsterFightPos = new Vector3 (playerPosX - 1, playerPosY, 0);
                    // 怪物层级调整到人物层级
                    monsterLayerOrder = -playerPosY;
				} 
                // 人物左侧没有超过地图左侧界限，且地图上人物左侧位置上没有东西【可行走信息为1】
				else if (playerPosX - 1 >= minX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[playerPosX - 1, playerPosY] == 1) {
					monsterRunPos = new Vector3(playerPosX - 0.5f, playerPosY, 0);
					monsterFightPos = new Vector3 (playerPosX - 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
                } 
                // 人物左侧没有超过地图左侧界限，地图上人物左侧位置没有东西，但是可行走信息不为1，但是是当前怪物的单步行走目标位置【每个怪物在行走之前都会把目标位置可行走信息设为5，防止其他怪物也走上去】
				else if (playerPosX - 1 >= minX && playerPosX - 1 == Mathf.RoundToInt (moveDestination.x) && playerPosY == Mathf.RoundToInt (moveDestination.y)) {
					monsterRunPos = new Vector3(playerPosX - 1f, playerPosY, 0);
					monsterFightPos = new Vector3(playerPosX - 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
				} 
                // 其他情况都要让怪物和人物的战斗位置尽量放进同一个格子里了
				else {
					// 如果人物靠上
                    if (posOffsetY > 0)
                    {     
						//怪物跑到的位置
						monsterRunPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);
                        // 怪物战斗位置
                        monsterFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);
                        // 怪物的层级比人物高1级
                        monsterLayerOrder = -playerPosY + 1;
                        // 人物的战斗位置
                        playerFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);

						baCtr.SetSortingOrder(monsterLayerOrder);

                    }
					else// 如果人物靠下
                    {             
						monsterRunPos = new Vector3(playerPosX - 0.25f, playerPosY + 0.15f, 0);
                        monsterFightPos = new Vector3(playerPosX - 0.25f, playerPosY + 0.15f, 0);
                        monsterLayerOrder = -playerPosY - 1;

                        playerFightPos = new Vector3(playerPosX + 0.25f, playerPosY - 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY - 1);
                    }
				}

			} else if (posOffsetX == 0) {

                if (playerPosX + 1 <= maxX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {

					battlePlayerCtr.TowardsRight (!battlePlayerCtr.isInFight);

					monsterRunPos = new Vector3(playerPosX + 0.5f, playerPosY, 0);

					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);

                    monsterLayerOrder = -playerPosY;

                } else if (playerPosX + 1 <= maxX && playerPosX + 1 == Mathf.RoundToInt (moveDestination.x) && playerPosY == Mathf.RoundToInt (moveDestination.y)) {
                    
					battlePlayerCtr.TowardsRight(!battlePlayerCtr.isInFight);

					monsterRunPos = new Vector3(playerPosX + 0.5f, playerPosY, 0);

					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);

                    monsterLayerOrder = -playerPosY;

                } else if (playerPosX - 1 >= minX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX - 1, playerPosY] == 1) {

					battlePlayerCtr.TowardsLeft (!battlePlayerCtr.isInFight);

					monsterRunPos = new Vector3(playerPosX - 0.5f, playerPosY, 0);

					monsterFightPos = new Vector3 (playerPosX - 1, playerPosY, 0);

                    monsterLayerOrder = -playerPosY;

                } else if (playerPosX - 1 >= minX && playerPosX - 1 == Mathf.RoundToInt (moveDestination.x) && playerPosY == Mathf.RoundToInt (moveDestination.y)) {
                    
					battlePlayerCtr.TowardsLeft (!battlePlayerCtr.isInFight);

					monsterRunPos = new Vector3(playerPosX - 0.5f, playerPosY, 0);

					monsterFightPos = new Vector3 (playerPosX - 1, playerPosY, 0);

                    monsterLayerOrder = -playerPosY;

				} else {

                    if(posOffsetY > 0){

						monsterRunPos = new Vector3(playerPosX + 0.25f, playerPosY - 0.15f, 0);

                        monsterFightPos = new Vector3(playerPosX + 0.25f, playerPosY - 0.15f, 0);

                        monsterLayerOrder = -playerPosY + 1;

                        playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY + 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY + 1);

						battlePlayerCtr.TowardsRight(!battlePlayerCtr.isInFight);

                    }else{

						monsterRunPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);

                        monsterFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);

                        monsterLayerOrder = -playerPosY - 1;

                        playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY - 1);

						battlePlayerCtr.TowardsRight(!battlePlayerCtr.isInFight);
                  
                    }
					
				}

			} else if (posOffsetX < 0) {

				battlePlayerCtr.TowardsRight (!battlePlayerCtr.isInFight);
            
				if (playerPosX + 1 <= maxX && playerPosX + 1 == monsterPosX && playerPosY == monsterPosY) {
					monsterRunPos = new Vector3(playerPosX + 1f, playerPosY, 0);
					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
                } else if (playerPosX + 1 <= maxX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[playerPosX + 1, playerPosY] == 1) {
					monsterRunPos = new Vector3(playerPosX + 0.5f, playerPosY, 0);
					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
                } else if (playerPosX + 1 <= maxX && playerPosX + 1 == Mathf.RoundToInt (moveDestination.x) && playerPosY == Mathf.RoundToInt (moveDestination.y)) {
					monsterRunPos = new Vector3(playerPosX + 1f, playerPosY, 0);
					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
				} else {
                    if (posOffsetY > 0)
                    {

						monsterRunPos = new Vector3(playerPosX + 0.25f, playerPosY -0.15f, 0);
                        monsterFightPos = new Vector3(playerPosX + 0.25f, playerPosY - 0.15f, 0);

                        monsterLayerOrder = -playerPosY + 1;

                        playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY + 0.15f, 0);
                  
                        baCtr.SetSortingOrder(-playerPosY + 1);

                    }
                    else
                    {
						monsterRunPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);
                        monsterFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);

                        monsterLayerOrder = -playerPosY - 1;

                        playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);
                  
                        baCtr.SetSortingOrder(-playerPosY - 1);
                    }
				}
			}
            
            // 如果玩家需要重置战斗动作
			if(playerNeedResetAttack){
				battlePlayerCtr.ResetAttack(playerCurrentAnimInfo);
				ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons();
			}
         
            // 玩家位置修正
            battlePlayerCtr.FixPosTo(playerFightPos, null);
				
            // 怪物跑到位置
			RunToPosition (monsterRunPos, delegate {
            
                // 如果跑到的时候已经死了，直接返回
				if(baCtr.isDead){
					return;
				}

                // 调整怪物位置到战斗位置
				baCtr.FixPosTo(monsterFightPos, null);

                // 如果玩家没有逃离战斗
				if(!battlePlayerCtr.escapeFromFight){

                    // 根据双方位置关系重新调整朝向
					if(transform.position.x <= ExploreManager.Instance.battlePlayerCtr.transform.position.x){
						baCtr.TowardsRight();
						boneTowards = MyTowards.Right;
					}else{
						baCtr.TowardsLeft();
						boneTowards = MyTowards.Left;
					}

                    // 如果玩家没有逃离战斗，并且玩家还没有进入战斗，则玩家和怪物都进入战斗
					if (!battlePlayerCtr.isInEscaping && !battlePlayerCtr.isInFight) {
						
						ExploreManager.Instance.PlayerAndMonsterStartFight();
						               
					} else {
						// 其他情况下只有怪物进入战斗
						ExploreManager.Instance.MonsterStartFight ();

					}
				}
                // 玩家在怪物跑的过程中脱离了战斗【这个现在从动画播放时长和跑动的时长上看是不会实现的，但是逻辑先做着】
				else{
					bool monsterDie = baCtr.agent.health <= 0;
					RefreshWalkableInfoWhenQuit(monsterDie);
					QuitFightAndDelayMove(5);
					battlePlayerCtr.escapeFromFight = false;
				}
            },monsterLayerOrder);

		}

        /// <summary>
        /// 走到指定位置上
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="cb">Cb.</param>
        /// <param name="showAlertArea">If set to <c>true</c> show alert area.</param>
		public override void WalkToPosition(Vector3 position,CallBack cb,bool showAlertArea = true){

			baCtr.PlayRoleAnim (CommonData.roleWalkAnimName, 0, null);

			int oriPosX = Mathf.RoundToInt (transform.position.x);
			int oriPosY = Mathf.RoundToInt (transform.position.y);

			int targetPosX = Mathf.RoundToInt (position.x);
			int targetPosY = Mathf.RoundToInt (position.y);


			moveOrigin = new Vector3 (oriPosX, oriPosY, 0);
            moveDestination = new Vector3(targetPosX, targetPosY, 0);

			RefreshWalkableInfoWhenStartMove ();

			if (targetPosY == oriPosY) {
				if (targetPosX >= oriPosX)
				{
					baCtr.TowardsRight();
					boneTowards = MyTowards.Right;
					alertTint.transform.localPosition = new Vector3(alertIconOffsetX, alertIconOffsetY, 0);
					if (monsterSayContainer != null)
					{
						monsterSayContainer.transform.localPosition = new Vector3(monsterSayContainerOffsetX - 1000, monsterSayContainerOffsetY, 0);
					}
				}
				else
				{
					baCtr.TowardsLeft();
					boneTowards = MyTowards.Left;
					alertTint.transform.localPosition = new Vector3(-alertIconOffsetX, alertIconOffsetY, 0);
					if (monsterSayContainer != null)
					{
						monsterSayContainer.transform.localPosition = new Vector3(-monsterSayContainerOffsetX - 1000, monsterSayContainerOffsetY, 0);                  
					}
				}
			} 
			else if(targetPosX == oriPosX){

				if (targetPosY >= oriPosY) {
					baCtr.TowardsUp ();
				} else {
					baCtr.TowardsDown ();
				}
			}

			if (moveCoroutine != null) {
				StopCoroutine (moveCoroutine);
			}

			float timeScale = 3f;

			if (showAlertArea) {
				ShowAlertAreaTint ();
			}

			moveCoroutine = MoveTo (position,timeScale,delegate{
				
				baCtr.PlayRoleAnim(CommonData.roleIdleAnimName,0,null);

				bool saySomething = Random.Range(0,10) <= 1 && !(baCtr.agent as Monster).isBoss;

				if(saySomething){
					string say = GetMonsterSay();
					tmPro.alignment = TMPro.TextAlignmentOptions.Center;
					ShowMonsterSay(say);
				}

				if(cb != null){
					cb();
				}

				SetSortingOrder (-Mathf.RoundToInt (position.y));
			});

			StartCoroutine (moveCoroutine);

		}


        /// <summary>
        /// 跑到指定位置上
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="cb">Cb.</param>
        /// <param name="layerOrder">Layer order.</param>
		protected override void RunToPosition(Vector3 position,CallBack cb,int layerOrder){

			baCtr.PlayRoleAnim (CommonData.roleRunAnimName, 0, null);

			int oriPosX = Mathf.RoundToInt (transform.position.x);
			int oriPosY = Mathf.RoundToInt (transform.position.y);

			// 怪物不是直接跑到战斗位置，而是先跑小角度，然后再做位置调整，所以这里给0.1的位置修正（[向右跑给+0.1，向左跑给-0.1]），保证下面近似出的targetPosX是正确的目标位置         
			float targetPosFix = position.x > oriPosX ? 0.1f : -0.1f;


			int targetPosX = Mathf.RoundToInt (position.x + targetPosFix);
			int targetPosY = Mathf.RoundToInt (position.y);


			moveOrigin = new Vector3 (oriPosX, oriPosY, 0);
			moveDestination = new Vector3 (targetPosX, targetPosY, 0);

//			Debug.LogFormat ("MOVE ORIGIN:{0}++++++MOVE DESTINATION:{1}", moveOrigin, moveDestination);

			RefreshWalkableInfoWhenStartMove ();

			float timeScale = 0.8f;

			if (position.x >= transform.position.x + 0.2f) {
				baCtr.TowardsRight ();
			} else if(position.x <= transform.position.x - 0.2f){
				baCtr.TowardsLeft ();
			}


			moveCoroutine = MoveTo (position,timeScale,delegate{

				this.transform.position = position;

				baCtr.PlayRoleAnim(CommonData.roleIdleAnimName,0,null);

				if(cb != null){
					cb();
				}

                SetSortingOrder (layerOrder);
			});

			StartCoroutine (moveCoroutine);

		}
        
        /// <summary>
        /// 设置骨骼动画层级
        /// </summary>
        /// <param name="order">Order.</param>
		public override void SetSortingOrder (int order)
		{
			baCtr.SetSortingOrder (order);
		}
			


        /// <summary>
        /// 获取一个可用的单词
        /// </summary>
        /// <returns>The AV alid word.</returns>
		private HLHWord GetAValidWord()
        {

            MySQLiteHelper mySql = MySQLiteHelper.Instance;

            mySql.GetConnectionWith(CommonData.dataBaseName);

            string currentWordsTableName = LearningInfo.Instance.GetCurrentLearningWordsTabelName();

			string[] condition = { "wordLength <= 12 ORDER BY RANDOM() LIMIT 1" };

            IDataReader reader = mySql.ReadSpecificRowsOfTable(currentWordsTableName, null, condition, true);

            reader.Read();

			HLHWord word = HLHWord.GetWordFromReader(reader);

            return word;
        }

        
        /// <summary>
        /// 生成奖励物品
        /// </summary>
        /// <returns>The reward item.</returns>
        public Item GenerateRewardItem()
        {
			Monster monster = GetComponent<Monster>();

            Item rewardItem = null;

			if(monster.isBoss)
            {
				if(!hasReward){
					return null;
				}
				
                int index = 0;
				if(monster.monsterId % 2 == 0){
					
                    index = (Player.mainPlayer.currentLevelIndex / 5 + 1) * 1000;

                    List<EquipmentModel> ems = GameManager.Instance.gameDataCenter.allEquipmentModels.FindAll(delegate (EquipmentModel obj)
                    {
                        return obj.equipmentGrade == index;
                    });

                    int randomSeed = Random.Range(0, ems.Count);

                    rewardItem = new Equipment(ems[randomSeed], 1);
                }else{
					index = (Player.mainPlayer.currentLevelIndex / 5 + 2);
					if(index == 10){
						index = 9;
					}

                    List<EquipmentModel> ems = GameManager.Instance.gameDataCenter.allEquipmentModels.FindAll(delegate (EquipmentModel obj)
                    {
                        return obj.equipmentGrade == index;
                    });

                    int randomSeed = Random.Range(0, ems.Count);
               
                    rewardItem = new Equipment(ems[randomSeed], 1);

					(rewardItem as Equipment).SetToGoldQuality();
                }

            }
            else
            {

                int randomSeed = Random.Range(0, 100);

				int dropItemSeed = 0;

				switch(Player.mainPlayer.luckInMonsterTreasure){
					case 0:
						dropItemSeed = 5 + Player.mainPlayer.extraLuckInMonsterTreasure;
						break;
					case 1:
						dropItemSeed = 10 + Player.mainPlayer.extraLuckInMonsterTreasure;
						break;
				}

				if (randomSeed >= 0 && randomSeed < dropItemSeed)
                {
					randomSeed = Random.Range(0, 10);

                    // 掉落物品是30%的概率掉落装备
                    if (randomSeed <= 2)
                    {

                        int index = Player.mainPlayer.currentLevelIndex / 5 + 1;
                        
                        if (index == 10)
                        {
                            index = 9;
                        }

                        List<EquipmentModel> ems = GameManager.Instance.gameDataCenter.allEquipmentModels.FindAll(delegate (EquipmentModel obj)
                        {
                            return obj.equipmentGrade == index;
                        });
                        randomSeed = Random.Range(0, ems.Count);
                        rewardItem = new Equipment(ems[randomSeed], 1);
                    }
                    else
                    {
						int consumablesGrade = Player.mainPlayer.currentLevelIndex / 10;

						if(consumablesGrade >= 4){
							consumablesGrade = 3;
						}

						List<ConsumablesModel> cms = GameManager.Instance.gameDataCenter.allConsumablesModels.FindAll(delegate (ConsumablesModel obj)
						{
							return obj.consumablesGrade == consumablesGrade;
						});

						randomSeed = Random.Range(0, cms.Count);

						rewardItem = new Consumables(cms[randomSeed], 1);
                    }
                }else{
				    rewardItem = null;
			    }
                   
            }            

            return rewardItem;
        }

       

		
	}
}
