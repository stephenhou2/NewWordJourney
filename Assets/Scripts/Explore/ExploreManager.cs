using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;



namespace WordJourney
{
	using DragonBones;
	using Transform = UnityEngine.Transform;

	public class ExploreManager : MonoBehaviour{

        // 探索控制器单例
		private static ExploreManager mExploreManager;
		public static ExploreManager Instance{
			get{
				if (mExploreManager == null) {
					Transform exploreManagerTrans = TransformManager.FindTransform ("ExploreManager");
					if (exploreManagerTrans == null) {
						return null;
					}
					mExploreManager = exploreManagerTrans.GetComponent<ExploreManager> ();
				}
				return mExploreManager;
			}
		}
        	
        // 地图生成器
		public NewMapGenerator newMapGenerator;	

		// 当前碰到的怪物控制器
		private BattleMonsterController battleMonsterCtr;

		// 玩家控制器
		public BattlePlayerController battlePlayerCtr;

        // 探索UI控制器
		[HideInInspector]public ExploreUICotroller expUICtr;

        // 当前碰到的地图事件
		public MapEvent currentEnteredMapEvent;

        // 标记探索界面是否已经准备完成
		public bool exploreSceneReady;

        // 探索中的单词记录
		private List<HLHWord> correctWordList = new List<HLHWord>();//正确单词列表
		private List<HLHWord> wrongWordList = new List<HLHWord>();//错误单词列表

		void Awake()
		{

            // 绑定组件
			Transform battlePlayer = Player.mainPlayer.transform.Find ("BattlePlayer");

			battlePlayer.gameObject.SetActive (true);

			battlePlayerCtr = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ();

			battlePlayerCtr.ActiveBattlePlayer (false, false, false);  

			Transform exploreCanvas = TransformManager.FindTransform("ExploreCanvas");

			expUICtr = exploreCanvas.GetComponent<ExploreUICotroller>();
		}

		void Start(){
			// 编辑器模式下需要手动把shader绑定一次，否则地图事件上的单词无法正常显示（里面使用的textmeshpro有自定义的shader，根据平台打包之后无法正常在编辑器下使用）
			#if UNITY_EDITOR
			Renderer[] renderers = GetComponentsInChildren<Renderer> ();
			for (int i = 0; i < renderers.Length; i++) {
				if(renderers[i].gameObject.name != "Word"){
					continue;
				}
				renderers [i].material.shader = Shader.Find (renderers [i].sharedMaterial.shader.name);
			}
			#endif

			//#if UNITY_EDITOR || UNITY_IOS 
			//Material m = newMapGenerator.fogOfWarPlane.GetComponent<Renderer>().material;
			//m.shader = Resources.Load("FOWShader") as Shader;
			//#endif

		}
			
        /// <summary>
        /// 初始化探索场景
        /// </summary>
        /// <param name="from">From.</param>
		public void SetUpExploreView(MapSetUpFrom from)
		{
			// 标记探索场景 not ready
			exploreSceneReady = false;
         
            // 加载探索场景所需的游戏资源
			GameManager.Instance.gameDataCenter.InitExplorePrepareGameData();

            // 是否是最后一关
			bool isFinalChapter = Player.mainPlayer.currentLevelIndex == CommonData.maxLevelIndex;

            // 垃圾回收
			System.GC.Collect();

            DisableExploreInteractivity();

            // 生成地图
			newMapGenerator.SetUpMap(from);
         
            // 清除人物身上的字母碎片
            Player.mainPlayer.ClearCollectedCharacters();

            // 记录人物的存档位置和存档朝向
			//Player.mainPlayer.savePosition = battlePlayerCtr.transform.position;
            //Player.mainPlayer.saveTowards = battlePlayerCtr.towards;

            // 加载玩家数据
			//PlayerData playerData = GameManager.Instance.persistDataManager.LoadPlayerData();
            //// 初始化人物数据
            //Player.mainPlayer.SetUpPlayerWithPlayerData(playerData);

            // 存档
			SaveDataInExplore(null,false);

            // 初始化探索UI界面
			expUICtr.SetUpExploreCanvas();

            // 初始化玩家角色
            battlePlayerCtr.InitBattlePlayer();

            // 如果是终章
			if(isFinalChapter){
				// 隐藏底部bar
				expUICtr.HideUpAndBottomUIs();
                // 加载终章画布
				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.finalChapterCanvasBundleName, "FinalChapterCanvas", delegate
				{
					TransformManager.FindTransform("FinalChapterCanvas").GetComponent<FinalChapterViewControlller>().SetUpFinalChapterView();
				});
			}

			EnableExploreInteractivity();

            // 探索场景ready
			exploreSceneReady = true;
         
		}
      
        /// <summary>
        /// 检测玩家点击事件
        /// </summary>
		private void Update(){
			
#if UNITY_STANDALONE || UNITY_EDITOR

			if(!Input.GetMouseButtonDown(0)){
				return;
			}

			Vector3 clickPos = Vector3.zero;

	
            // 如果玩家点击在
			if(EventSystem.current.IsPointerOverGameObject() || !GameManager.Instance.gameDataCenter.gameSettings.newPlayerGuideFinished){
				//Debug.LogFormat("点击在UI上{0},guide finished:{1}",EventSystem.current.currentSelectedGameObject,GameManager.Instance.gameDataCenter.gameSettings.newPlayerGuideFinished);
				return;
			}

			clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			UserClickAt(clickPos);

#elif UNITY_ANDROID || UNITY_IOS

            // 如果没有检测到点击事件，则直接返回
			if (Input.touchCount == 0) {
				return;
			}

			Vector3 clickPos = Vector3.zero;

            // 如果检测到点击事件，且状态为开始点击
			if (Input.GetTouch(0).phase == TouchPhase.Began){

				if(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)){
					return;
				}
                // 屏幕位置转化到世界坐标位置
				clickPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                // 用户点击
				UserClickAt(clickPos);
			}
#endif
		}

        /// <summary>
        /// 用户点击
        /// </summary>
        /// <param name="clickPos">Click position.</param>
		private void UserClickAt(Vector3 clickPos){

			int targetX = 0;
			int targetY = 0;
			Vector3 targetPos = Vector3.zero;

			// 由于地图贴图 tile时是以中心点为参考，宽高为1，所以如果以实际拼出的地图左下角为坐标原点，则点击位置需要进行如下坐标转换
			targetX = (int)(clickPos.x + 0.5f);

			targetY = (int)(clickPos.y + 0.5f);

			if (targetX >= newMapGenerator.columns || targetY >= newMapGenerator.rows
				|| targetX < 0 || targetY < 0) {
				return;
			}


			// 以地图左下角为坐标原点时的点击位置
			targetPos = new Vector3(targetX, targetY, 0);


			// 如果点在镂空区域，则直接返回
			if (newMapGenerator.mapWalkableInfoArray [(int)targetPos.x, (int)targetPos.y] == -1) {
				newMapGenerator.PlayDestinationAnim (targetPos, false);
				return;
			}

			// 游戏角色按照自动寻路路径移动到点击位置
			bool arrivable = battlePlayerCtr.MoveToPosition (targetPos,newMapGenerator.mapWalkableInfoArray);

			// 地图上点击位置生成提示动画
//			mapGenerator.PlayDestinationAnim(targetPos,arrivable);
			newMapGenerator.PlayDestinationAnim (targetPos, arrivable);

		}

        /// <summary>
        /// 检查单词是否在正确单词列表中
        /// </summary>
        /// <returns><c>true</c>, if word exist in correct record list was checked, <c>false</c> otherwise.</returns>
        /// <param name="word">Word.</param>
		public bool CheckWordExistInCorrectRecordList(HLHWord word){
			bool exist = false;
			for (int i = 0; i < correctWordList.Count; i++)
            {
				HLHWord wordRecord = correctWordList[i];
				if (word.wordId == wordRecord.wordId)
                {
					exist = true;
                    break;
                }
            }
            return exist;
		}

        /// <summary>
        /// 检查单词是否在错误单词列表中
        /// </summary>
        /// <returns><c>true</c>, if word exist in wrong record list was checked, <c>false</c> otherwise.</returns>
        /// <param name="word">Word.</param>
		public bool CheckWordExistInWrongRecordList(HLHWord word){
			bool exist = false;
            for (int i = 0; i < wrongWordList.Count; i++)
            {
                HLHWord wordRecord = wrongWordList[i];
				if (word.wordId == wordRecord.wordId)
                {
					exist = true;
                    break;
                }
            }
			return exist;
		}
        
		/// <summary>
		/// 记录选择过的单词,这里的记录将用于更新数据库
		/// </summary>
		/// <param name="word">Word.</param>
		/// <param name="isChooseRight">If set to <c>true</c> is choose right.</param>
		public void RecordWord(HLHWord word,bool isChooseRight){

            // 选择正确
			if (isChooseRight) {
				
				bool update = !CheckWordExistInCorrectRecordList(word);

				if (update)
                {
                    correctWordList.Add(word);
                }
                // 如果错误单词列表中也有这个单词的话，将该单词从错误单词列表中移除
				for (int i = 0; i < wrongWordList.Count;i++){
					HLHWord wordRecord = wrongWordList[i];
					if(wordRecord.wordId == word.wordId){
						wrongWordList.RemoveAt(i);
					}
				}

			} else {
				bool update = !CheckWordExistInWrongRecordList(word);

                if (update)
                {
					wrongWordList.Add(word);
				}     

				for (int i = 0; i < correctWordList.Count; i++)
                {
					HLHWord wordRecord = correctWordList[i];
                    if (wordRecord.wordId == word.wordId)
                    {
						correctWordList.RemoveAt(i);
                    }
                }

			}

		}

        /// <summary>
        /// 更新单词数据库
        /// </summary>
		public void UpdateWordDataBase(){
         
            // 连接到数据库
			MySQLiteHelper sql = MySQLiteHelper.Instance;

            sql.GetConnectionWith(CommonData.dataBaseName);

            // 开启事务
			sql.BeginTransaction();

            // 获取当前单词的表单
            string currentWordsTableName = LearningInfo.Instance.GetCurrentLearningWordsTabelName();

            // 更新项
			string[] colFields = { "learnedTimes", "ungraspTimes","isFamiliar" };

			HLHWord word = null;
            
            // 正确单词列表中所有单词数据更新进数据库
			for (int i = 0; i < correctWordList.Count;i++){
				
				word = correctWordList[i];

				string familiarStr = word.isFamiliar ? "1" : "0";
               
				string[] conditions = { "wordId=" + word.wordId };
				string[] values = { word.learnedTimes.ToString(), word.ungraspTimes.ToString(),familiarStr};
				sql.UpdateValues(currentWordsTableName, colFields, values, conditions, true);
			}

			// 错误单词列表中所有单词数据更新进数据库
			for (int i = 0; i < wrongWordList.Count;i++){
				word = wrongWordList[i];
				   
				string familiarStr = word.isFamiliar ? "1" : "0";
				string[] conditions = { "wordId=" + word.wordId };
				string[] values = { word.learnedTimes.ToString(), word.ungraspTimes.ToString(),familiarStr };
                sql.UpdateValues(currentWordsTableName, colFields, values, conditions, true);
			}

            // 清理工作
			correctWordList.Clear();
			wrongWordList.Clear();

			sql.EndTransaction();

			sql.CloseConnection(CommonData.dataBaseName);    

		}

        /// <summary>
        /// 显示单词选择面板
        /// </summary>
        /// <param name="wordsArray">Words array.</param>
        /// <param name="extraInfo">Extra info.</param>
		public void ShowWordsChoosePlane(HLHWord[] wordsArray,string extraInfo = null){
			MapWalkableEventsStopAction ();
			expUICtr.SetUpWordHUD (wordsArray,extraInfo);
		}

        /// <summary>
        /// 显示字母填充面板
        /// </summary>
        /// <param name="word">Word.</param>
		public void ShowCharacterFillPlane(HLHWord word){
			
			expUICtr.SetUpWordHUD (word);
		}

        /// <summary>
        /// 显示谜语面板
        /// </summary>
        /// <param name="answerRightCallBack">Answer right call back.</param>
        /// <param name="answerWrongCallBack">Answer wrong call back.</param>
		public void ShowPuzzleView(CallBack answerRightCallBack,CallBack answerWrongCallBack)
        {
            MapWalkableEventsStopAction();
			expUICtr.SetUpPuzzleView(answerRightCallBack,answerWrongCallBack);
        }

        /// <summary>
        /// 判断选择错误时是否需要展示单词详细信息页面
        /// </summary>
        /// <returns><c>true</c>, if show full word detail when choose wrong was needed, <c>false</c> otherwise.</returns>
		public bool NeedShowFullWordDetailWhenChooseWrong(){

			MapEvent me = currentEnteredMapEvent.GetComponent<MapEvent>();
            
			return me.IsFullWordNeedToShowWhenChooseWrong();

		}

        /// 在单词选择面板中选择了某个答案
		public void ChooseAnswerInWordHUD(bool isChooseCorrect){
			MapEvent me = currentEnteredMapEvent;
			if(me == null){
				battlePlayerCtr.isInEvent = false;
				return;
			}
            // 根据选择正确与否，触发地图事件
			me.MapEventTriggered(isChooseCorrect, battlePlayerCtr);
		}
        
        /// <summary>
        /// 在字母填充面板里确认填充
        /// </summary>
        /// <param name="isFillCorrect">If set to <c>true</c> is fill correct.</param>
		public void ConfirmFillCharactersInWordHUD(bool isFillCorrect){

			MapEvent me = currentEnteredMapEvent.GetComponent<MapEvent> ();

			me.MapEventTriggered (true, battlePlayerCtr);

			MapWalkableEventsStartAction ();

		}
              
        /// <summary>
        /// 禁止探索场景中的行走点击【激活底部遮罩】
        /// </summary>
		public void DisableExploreInteractivity(){
			expUICtr.ShowExploreMask ();
			expUICtr.HideFullMask ();
		}

        /// <summary>
        /// 开启探索场景中的行走点击
        /// </summary>
		public void EnableExploreInteractivity(){
			expUICtr.HideExploreMask ();
			expUICtr.HideFullMask ();
		}
        
        /// <summary>
        /// 禁止探索界面交互【部分禁止，具体禁止哪些参考场景中的fullmask的层级】
        /// </summary>
		public void DisableAllInteractivity(){
			expUICtr.HideExploreMask ();
			expUICtr.ShowFullMask ();
		}
        

        /// <summary>
        /// 获取奖励
        /// </summary>
        /// <param name="reward">Reward.</param>
		public void ObtainReward(Item reward){

            // 添加物品
			Player.mainPlayer.AddItem (reward);

            // 显示物品简单信息面板
			expUICtr.SetUpSimpleItemDetail (reward);

            // 更新底部bar
			expUICtr.UpdateBottomBar();

		}

        /// <summary>
        /// 所有地图上的可运动生命体【怪物，boss，npc】立刻停止运动
        /// </summary>
		public void MapWalkableEventsStopActionImmidiately()
        {
            for (int i = 0; i < newMapGenerator.allMonstersInMap.Count; i++)
            {
                MapMonster mapMonster = newMapGenerator.allMonstersInMap[i];
				mapMonster.StopMoveImmidiately();
            }

            for (int i = 0; i < newMapGenerator.allNPCsInMap.Count; i++)
            {
				newMapGenerator.allNPCsInMap[i].StopMoveImmidiately();
            }
        }


        /// <summary>
		/// 所有地图上的可运动生命体【怪物，boss，npc】在本步行动结束后停止运动
        /// </summary>
		public void MapWalkableEventsStopAction(){
			for (int i = 0; i < newMapGenerator.allMonstersInMap.Count; i++) {
				MapMonster mapMonster = newMapGenerator.allMonstersInMap[i];
				mapMonster.StopMoveAtEndOfCurrentMove ();
			}

			for (int i = 0; i < newMapGenerator.allNPCsInMap.Count; i++)
            {
				newMapGenerator.allNPCsInMap[i].StopMoveAtEndOfCurrentMove();
            }
		}

        /// <summary>
		/// 所有地图上的可运动生命体【怪物，boss，npc】开始运动
        /// </summary>
		public void MapWalkableEventsStartAction(){
         
			if (battlePlayerCtr.isInEvent) {
				return;
			}

			for (int i = 0; i < newMapGenerator.allMonstersInMap.Count; i++) {
				newMapGenerator.allMonstersInMap [i].StartMove ();
			}

			for (int i = 0; i < newMapGenerator.allNPCsInMap.Count;i++){
				newMapGenerator.allNPCsInMap [i].StartMove();
			}
		}


		/// <summary>
		/// 遭遇怪物时的响应方法
		/// </summary>
		/// <param name="monsterTrans">Monster trans.</param>
		public void EnterFight(Transform monsterTrans){

            // 禁止寻路点击
			DisableAllInteractivity();

            
			battleMonsterCtr = monsterTrans.GetComponent<BattleMonsterController> ();
            // 初始化怪物
			battleMonsterCtr.InitMonster (monsterTrans);

            // 设置敌人
			battlePlayerCtr.SetEnemy (battleMonsterCtr);
			battleMonsterCtr.SetEnemy (battlePlayerCtr);


		}

	
        // 更新玩家状态面板
		public void UpdatePlayerStatusPlane(){
			expUICtr.UpdatePlayerStatusBar ();
		}


        /// <summary>
        /// 玩家和怪物开始战斗
        /// </summary>
		public void PlayerAndMonsterStartFight(){

			expUICtr.ShowFightPlane();

			// 执行玩家角色战斗前技能回调
			battleMonsterCtr.StartFight (battlePlayerCtr);
			battlePlayerCtr.StartFight (battleMonsterCtr);

			battlePlayerCtr.ExcuteBeforeFightSkillCallBacks(battleMonsterCtr);
			battleMonsterCtr.ExcuteBeforeFightSkillCallBacks(battlePlayerCtr);

		}

        /// <summary>
        /// 玩家开始战斗
        /// </summary>
		public void PlayerStartFight(){

			expUICtr.ShowFightPlane();

			battlePlayerCtr.StartFight (battleMonsterCtr);
			// 执行玩家角色战斗前技能回调
			battlePlayerCtr.ExcuteBeforeFightSkillCallBacks(battleMonsterCtr);

		}

        /// <summary>
        /// 怪物开始战斗
        /// </summary>
		public void MonsterStartFight(){

			expUICtr.ShowFightPlane();

			battleMonsterCtr.StartFight (battlePlayerCtr);
			// 执行怪物角色战斗前技能回调
			battleMonsterCtr.ExcuteBeforeFightSkillCallBacks(battlePlayerCtr);

		}


        /// <summary>
        /// 设置地图目标位置的可行走信息
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="walkaleInfo">Walkale info.</param>
		public void ResetMapWalkableInfo(Vector3 position,int walkaleInfo){
			newMapGenerator.mapWalkableInfoArray [(int)position.x, (int)position.y] = walkaleInfo;
		}
			
        /// <summary>
        /// 显示npc交互界面
        /// </summary>
        /// <param name="mapNPC">Map npc.</param>
		public void ShowNPCPlane(MapNPC mapNPC){
			MapWalkableEventsStopAction ();
			expUICtr.EnterNPC (mapNPC.npc);
		}
			
        /// <summary>
		/// 显示billboard
        /// </summary>
        /// <param name="bb">Bb.</param>
		public void ShowBillboard(Billboard bb){
			MapWalkableEventsStopAction ();
			expUICtr.SetUpBillboard (bb);
		}


		public void PlayerFade(){
			battlePlayerCtr.PlayerFade ();
		}
        
        /// <summary>
        /// 玩家战斗胜利逻辑
        /// </summary>
        /// <param name="monsterTransArray">Monster trans array.</param>
		public void BattlePlayerWin(Transform[] monsterTransArray){

            // 如果没有传入敌人，直接返回
			if (monsterTransArray.Length <= 0) {
				return;
			}

            // 记录击败的怪物数
			Player.mainPlayer.totaldefeatMonsterCount++;
         
            // 重置角色骨骼动画速率
			battlePlayerCtr.SetRoleAnimTimeScale (1.0f);
			battleMonsterCtr.SetRoleAnimTimeScale (1.0f);

			Transform trans = monsterTransArray [0];

			BattleMonsterController bmCtr = trans.GetComponent<BattleMonsterController> ();

			Monster monster = trans.GetComponent<Monster> ();

			MapWalkableEvent walkableEvent = trans.GetComponent<MapWalkableEvent> ();

			Player player = Player.mainPlayer;
            
            // 执行战斗结束的回调
			FightEndCallBacks ();

            // 重置所有技能产生的属性变化
            battlePlayerCtr.agent.ClearPropertyChangesFromSkill();

            // 重置角色属性【按照脱离战斗时持有装备的正常情况重新计算一遍人物属性】
			battlePlayerCtr.agent.ResetBattleAgentProperties (false);

            // 怪物清空所有技能产生的属性变化
            battleMonsterCtr.agent.ClearPropertyChangesFromSkill();

            // 怪物重置属性
            battleMonsterCtr.agent.ResetBattleAgentProperties(false);
         
            // 玩家位置修正到标准位置【整数点位置】
            battlePlayerCtr.FixPositionToStandard ();

            // 玩家等待当前动作结束
			battlePlayerCtr.ResetToWaitAfterCurrentRoleAnimEnd ();

			Vector3 monsterPos = trans.position;

			walkableEvent.RefreshWalkableInfoWhenQuit (true);
                     
			MapMonster mm = bmCtr.GetComponent<MapMonster> ();
            
            if (mm != null) {

                // 生成奖励物品
                Item rewardItem = mm.GenerateRewardItem();

				if (rewardItem != null) {
					newMapGenerator.SetUpRewardInMap (rewardItem, trans.position);
				}

				if (mm.pairEventPos != -Vector3.one) {
					newMapGenerator.ChangeMapEventStatusAtPosition (mm.pairEventPos);
				}

				if(monster.isBoss){
					MapEventsRecord.AddEventTriggeredRecord(mm.mapIndex, mm.oriPos);
				}


			}


            // 目前npc不参与战斗，下面的代码后续扩展可用
			MapNPC mn = bmCtr.GetComponent<MapNPC> ();

			if (mn != null) {
				
				Item rewardItem = Item.NewItemWith (mn.fightReward.rewardValue,1);

				if (rewardItem != null) {

					if (rewardItem.itemType == ItemType.Equipment) {
						Equipment eqp = rewardItem as Equipment;
						EquipmentQuality quality = (EquipmentQuality)mn.fightReward.attachValue;
						eqp.ResetPropertiesByQuality (quality);
					}

					newMapGenerator.SetUpRewardInMap (rewardItem, trans.position);
				}

			}


            // 开启屏幕行走点击
			EnableExploreInteractivity ();
            // 标记不在战斗
			battlePlayerCtr.isInFight = false;
            // 标记不再地图事件中
			battlePlayerCtr.isInEvent = false;
            // 角色包围盒开启
			battlePlayerCtr.boxCollider.enabled = true;
            // 清除敌方信息
			battlePlayerCtr.enemy = null;

            //更新玩家金钱
            int goldGain = monster.rewardGold + player.extraGold;
            player.totalGold += goldGain;

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.goldAudioName);

            expUICtr.SetUpGoldGainTintHUD(goldGain);

            player.experience += monster.rewardExperience + player.extraExperience;//更新玩家经验值

            bool isLevelUp = player.LevelUpIfExperienceEnough();//判断是否升级

			newMapGenerator.allMonstersInMap.Remove(mm);

            if (isLevelUp)
			{

                PlayLevelUpAnim();// 升级时播放升级动画

                DisableExploreInteractivity();

                expUICtr.ShowLevelUpPlane();
            }
            else
            {
                MapWalkableEventsStartAction();
            }

            // 更新角色状态栏
			battlePlayerCtr.UpdateStatusPlane();
         
		}


        /// <summary>
        /// 播放升级动画
        /// </summary>
		private void PlayLevelUpAnim(){
			battlePlayerCtr.SetEffectAnim (CommonData.levelUpEffectName);
            GameManager.Instance.soundManager.PlayAudioClip (CommonData.levelUpAudioName);
		}

        /// <summary>
        /// 玩家在战斗中失败
        /// </summary>
		public void BattlePlayerLose(){
         
            // 清除战斗中技能带来的属性变化
            battlePlayerCtr.agent.ClearPropertyChangesFromSkill();
            battleMonsterCtr.agent.ClearPropertyChangesFromSkill();

            battleMonsterCtr.agent.ResetBattleAgentProperties(false);

			battlePlayerCtr.SetRoleAnimTimeScale (1.0f);
			battleMonsterCtr.SetRoleAnimTimeScale (1.0f);
               
			FightEndCallBacks ();

			battleMonsterCtr.ResetToWaitAfterCurrentRoleAnimEnd ();

			expUICtr.QuitFight ();

			expUICtr.ShowBuyLifeQueryHUD();
         
		}



		private void FightEndCallBacks(){

			// 执行玩家角色战斗结束技能回调
			battlePlayerCtr.ExcuteFightEndCallBacks(battleMonsterCtr);

			// 执行怪物角色战斗结束技能回调
			battleMonsterCtr.ExcuteFightEndCallBacks(battlePlayerCtr);

			// 清理所有状态和技能回调
			battlePlayerCtr.ClearAllEffectStatesAndSkillCallBacks ();
			battleMonsterCtr.ClearAllEffectStatesAndSkillCallBacks ();

		}
        
        /// <summary>
        /// 进入关卡
        /// </summary>
        /// <param name="level">关卡序号.</param>
        /// <param name="exitType">出口类型.</param>
		public void EnterLevel(int level,ExitType exitType){

			exploreSceneReady = false;

			MapWalkableEventsStopAction();

			correctWordList.Clear();
			wrongWordList.Clear();

			IEnumerator enterLevelCoroutine = LatelyEnterLevel(level, exitType);

			StartCoroutine(enterLevelCoroutine);         

		}


        /// <summary>
        /// 等待0.1s进入关卡【在出口这个地方场景会逐渐变暗，给0.1s等待时间使过渡更加平滑】
        /// </summary>
        /// <returns>The enter level.</returns>
        /// <param name="level">Level.</param>
        /// <param name="exitType">Exit type.</param>
		private IEnumerator LatelyEnterLevel(int level, ExitType exitType){

			yield return new WaitForSeconds(0.1f);

			MapWalkableEventsStopAction();

            // 清除发音缓存
            GameManager.Instance.pronounceManager.ClearPronunciationCache();

            Time.timeScale = 1;

            DisableExploreInteractivity();

            Player player = Player.mainPlayer;

            player.currentLevelIndex = level;

            if (player.currentLevelIndex > player.maxUnlockLevelIndex)
            {
                player.maxUnlockLevelIndex = player.currentLevelIndex;
            }         

			if (player.currentLevelIndex <= CommonData.maxLevelIndex)
			{            
            
				switch (exitType)
				{
					case ExitType.ToLastLevel:
						SetUpExploreView(MapSetUpFrom.NextLevel);
						break;
					case ExitType.ToNextLevel:
						SetUpExploreView(MapSetUpFrom.LastLevel);
						break;
				}


			}
		}

        /// <summary>
        /// 进入下一关
        /// </summary>
		public void EnterNextLevel(){

            // 更新单词数据库
			UpdateWordDataBase();
              
            // 重置探索数据
			ResetExploreData();

			int level = Player.mainPlayer.currentLevelIndex + 1;
            Player.mainPlayer.currentLevelIndex = level;

            if (Player.mainPlayer.maxUnlockLevelIndex < level)
            {
                Player.mainPlayer.maxUnlockLevelIndex = level;
            }

            EnterLevel(level, ExitType.ToNextLevel);
            
		}


		private void ResetExploreData(){
			GameManager.Instance.gameDataCenter.currentMapEventsRecord.Reset();
			GameManager.Instance.persistDataManager.ClearCurrentMapWordsRecordAndSave();
			GameManager.Instance.persistDataManager.ResetCurrentMapMiniMapRecordAndSave();
		}

        /// <summary>
		/// 设定不能直接返回上一关
		/// 直接提示门已封印
        /// </summary>
		public void EnterLastLevel(){

			expUICtr.SetUpSingleTextTintHUD("这里好像已经被封印了...");

			//UpdateWordDataBase();

			//int level = Player.mainPlayer.currentLevelIndex - 1;

			//GameManager.Instance.persistDataManager.SaveMapEventsRecord();

			//EnterLevel(level,ExitType.LastLevel);
         
			//Debug.LogFormat("finish loading time:{0}", Time.time);
		}
        

        /// <summary>
        /// 探索场景中保存数据
        /// </summary>
        /// <param name="saveFinishCallBack">Save finish call back.</param>
        /// <param name="updateDB">If set to <c>true</c> update db.</param>
		public void SaveDataInExplore(CallBack saveFinishCallBack,bool updateDB = true){

			//DisableAllInteractivity();

			//Time.timeScale = 0f;

			//         // 保存数据UI显示
			//expUICtr.SetUpSaveDataHintViewAndSave(delegate
			//{
			//	// 实际的保存操作
			//	if (updateDB)
			//             {
			//                 UpdateWordDataBase();
			//             }

			//             GameManager.Instance.persistDataManager.SaveGameSettings();
			//             GameManager.Instance.persistDataManager.SaveMapEventsRecord();
			//             GameManager.Instance.persistDataManager.SaveCompletePlayerData();
			//             GameManager.Instance.persistDataManager.SaveCurrentMapMiniMapRecord();
			//             GameManager.Instance.persistDataManager.SaveCurrentMapEventsRecords();
			//             GameManager.Instance.persistDataManager.SaveChatRecords();
			//             GameManager.Instance.persistDataManager.SaveCurrentMapWordsRecords();

			//}, delegate
			//{
			//	// 保存完成后的回调
			//	Time.timeScale = 1f;

			//	EnableExploreInteractivity();

			//	if(battlePlayerCtr.enemy != null){
			//		battlePlayerCtr.isInEvent = true;
			//	}else{
			//		battlePlayerCtr.isInEvent = false;
			//	}

			//	MapWalkableEventsStartAction();

			//	if(saveFinishCallBack != null){
			//		saveFinishCallBack();
			//	}

			//});

			Player.mainPlayer.savePosition = battlePlayerCtr.transform.position;
			Player.mainPlayer.saveTowards = battlePlayerCtr.towards;


			// 实际的保存操作
            if (updateDB)
            {
                UpdateWordDataBase();
            }

			GameManager.Instance.persistDataManager.SaveGameSettings();
            GameManager.Instance.persistDataManager.SaveMapEventsRecord();
            GameManager.Instance.persistDataManager.SaveCompletePlayerData();
            GameManager.Instance.persistDataManager.SaveCurrentMapMiniMapRecord();
            GameManager.Instance.persistDataManager.SaveCurrentMapEventsRecords();
            GameManager.Instance.persistDataManager.SaveChatRecords();
            GameManager.Instance.persistDataManager.SaveCurrentMapWordsRecords();

            if (battlePlayerCtr.enemy != null)
            {
                battlePlayerCtr.isInEvent = true;
            }
            else
            {
                battlePlayerCtr.isInEvent = false;
            }

            if (saveFinishCallBack != null)
            {
                saveFinishCallBack();
            }

		}

        /// <summary>
        /// 退出探索场景
        /// </summary>
		public void QuitExploreScene(){

			Time.timeScale = 1;

			this.gameObject.SetActive(false);

			GameManager.Instance.persistDataManager.SaveDataInExplore(null, true);

            // 停止播放探索背景音乐
			GameManager.Instance.soundManager.StopBgm ();

			Camera.main.transform.SetParent (null);

			Transform exploreMask = Camera.main.transform.Find ("ExploreMask");
			exploreMask.GetComponent<UnityArmatureComponent> ().animation.Stop ();
			exploreMask.gameObject.SetActive (false);

            // 玩家退出探索场景
			battlePlayerCtr.QuitExplore ();
            // 退出探索UI界面
			expUICtr.QuitExplore();
         
            // 释放无用资源
			GameManager.Instance.gameDataCenter.ReleaseDataWithDataTypes(new GameDataCenter.GameDataType[] {
				GameDataCenter.GameDataType.GameLevelDatas,
				GameDataCenter.GameDataType.EquipmentModels,
				GameDataCenter.GameDataType.EquipmentSprites,
				GameDataCenter.GameDataType.ConsumablesModels,
				GameDataCenter.GameDataType.ConsumablesSprites,
				GameDataCenter.GameDataType.SpecialItemModels,
				GameDataCenter.GameDataType.SpecialItemSprites,
				GameDataCenter.GameDataType.SkillGemstoneModels,
				GameDataCenter.GameDataType.SkillGemstoneSprites,
				GameDataCenter.GameDataType.SpellItemModels,
				GameDataCenter.GameDataType.SkillScrollModels,
				GameDataCenter.GameDataType.SkillScrollSprites,
				GameDataCenter.GameDataType.CharacterSprites,
				GameDataCenter.GameDataType.ChatRecord,
				GameDataCenter.GameDataType.Diary,
				GameDataCenter.GameDataType.Monsters,
				GameDataCenter.GameDataType.MonstersUI,
				GameDataCenter.GameDataType.MonstersData,
				GameDataCenter.GameDataType.NPCs,
				GameDataCenter.GameDataType.Skills,
				GameDataCenter.GameDataType.SkillSprites,
				GameDataCenter.GameDataType.Effects,
				GameDataCenter.GameDataType.MapSprites,
				GameDataCenter.GameDataType.MapTileAtlas,
				GameDataCenter.GameDataType.MiniMapSprites,
				GameDataCenter.GameDataType.CurrentMapMiniMapRecord,
				GameDataCenter.GameDataType.MapEventsRecords,
				GameDataCenter.GameDataType.CurrentMapEventsRecord,
				GameDataCenter.GameDataType.CurrentMapWordsRecord,            
				GameDataCenter.GameDataType.Proverbs,
				GameDataCenter.GameDataType.Puzzle,
				GameDataCenter.GameDataType.BagCanvas,
				GameDataCenter.GameDataType.ShareCanvas,
				GameDataCenter.GameDataType.NPCCanvas,
				GameDataCenter.GameDataType.ExploreScene,
				GameDataCenter.GameDataType.SettingCanvas,
				GameDataCenter.GameDataType.ShareCanvas,
				GameDataCenter.GameDataType.GuideCanvas
			});

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {
				TransformManager.FindTransform ("HomeCanvas").GetComponent<HomeViewController> ().SetUpHomeView ();
			}, true, false);

			Destroy (this.gameObject,0.3f);
		}

	}
}

