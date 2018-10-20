using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;



namespace WordJourney
{
	
//	using UnityEngine.SceneManagement;
	using DragonBones;
	using Transform = UnityEngine.Transform;

	public class ExploreManager : MonoBehaviour{

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
        	

		public NewMapGenerator newMapGenerator;	
	
		// 当前关卡所有怪物
//		private List<BattleMonsterController> battleMonsters = new List<BattleMonsterController>();	

		// 当前碰到的怪物控制器
		private BattleMonsterController battleMonsterCtr;

		// 玩家控制器
		public BattlePlayerController battlePlayerCtr;

		[HideInInspector]public ExploreUICotroller expUICtr;


		public MapEvent currentEnteredMapEvent;

		public bool exploreSceneReady;

		private List<HLHWord> correctWordList = new List<HLHWord>();
		private List<HLHWord> wrongWordList = new List<HLHWord>();

		void Awake()
		{

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
			
		//Initializes the game for each level.
		public void SetUpExploreView(MapSetUpFrom from)
		{
			exploreSceneReady = false;

			//bool resetGameData = false;         
			//if(Player.mainPlayer.health <= 0){
			//	resetGameData = true;
			//	//GameManager.Instance.persistDataManager.ResetPlayerDataToOriginal();
			//}

			GameManager.Instance.gameDataCenter.InitExplorePrepareGameData();

			bool isFinalChapter = Player.mainPlayer.currentLevelIndex == CommonData.maxLevelIndex;

			System.GC.Collect();

            DisableExploreInteractivity();

			newMapGenerator.SetUpMap(from);
         
            Player.mainPlayer.ClearCollectedCharacters();

			Player.mainPlayer.savePosition = battlePlayerCtr.transform.position;
            Player.mainPlayer.saveTowards = battlePlayerCtr.towards;

			SaveDataInExplore(null,false);

			expUICtr.SetUpExploreCanvas();

            battlePlayerCtr.InitBattlePlayer();

			if(isFinalChapter){
				expUICtr.HideUpAndBottomUIs();
				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.finalChapterCanvasBundleName, "FinalChapterCanvas", delegate
				{
					TransformManager.FindTransform("FinalChapterCanvas").GetComponent<FinalChapterViewControlller>().SetUpFinalChapterView();
				});
			}



			exploreSceneReady = true;
         
			//MapWalkableEventsStartAction();
         
		}
      

		private void Update(){

//			if (!isExploreClickValid) {
//				return;
//			}

#if UNITY_STANDALONE || UNITY_EDITOR

			if(!Input.GetMouseButtonDown(0)){
				return;
			}

			Vector3 clickPos = Vector3.zero;

	
			if(EventSystem.current.IsPointerOverGameObject() || !GameManager.Instance.gameDataCenter.gameSettings.newPlayerGuideFinished){
				//Debug.LogFormat("点击在UI上{0},guide finished:{1}",EventSystem.current.currentSelectedGameObject,GameManager.Instance.gameDataCenter.gameSettings.newPlayerGuideFinished);
				return;
			}

			clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			UserClickAt(clickPos);

#elif UNITY_ANDROID || UNITY_IOS

			if (Input.touchCount == 0) {
				return;
			}

			Vector3 clickPos = Vector3.zero;


			if (Input.GetTouch(0).phase == TouchPhase.Began){

				if(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)){
					return;
				}
				clickPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

				UserClickAt(clickPos);
			}
#endif
		}

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

			if (isChooseRight) {
				
				bool update = !CheckWordExistInCorrectRecordList(word);

				if (update)
                {
                    correctWordList.Add(word);
                }

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

		public void UpdateWordDataBase(){
         
			MySQLiteHelper sql = MySQLiteHelper.Instance;

            sql.GetConnectionWith(CommonData.dataBaseName);

			sql.BeginTransaction();

            string currentWordsTableName = LearningInfo.Instance.GetCurrentLearningWordsTabelName();

			string[] colFields = { "learnedTimes", "ungraspTimes","isFamiliar" };

			HLHWord word = null;
            
			for (int i = 0; i < correctWordList.Count;i++){
				
				word = correctWordList[i];

				string familiarStr = word.isFamiliar ? "1" : "0";
               
				string[] conditions = { "wordId=" + word.wordId };
				string[] values = { word.learnedTimes.ToString(), word.ungraspTimes.ToString(),familiarStr};
				sql.UpdateValues(currentWordsTableName, colFields, values, conditions, true);
			}

			for (int i = 0; i < wrongWordList.Count;i++){
				word = wrongWordList[i];
				   
				string familiarStr = word.isFamiliar ? "1" : "0";
				string[] conditions = { "wordId=" + word.wordId };
				string[] values = { word.learnedTimes.ToString(), word.ungraspTimes.ToString(),familiarStr };
                sql.UpdateValues(currentWordsTableName, colFields, values, conditions, true);
			}

			correctWordList.Clear();
			wrongWordList.Clear();

			sql.EndTransaction();

			sql.CloseConnection(CommonData.dataBaseName);    

		}


		public void ShowWordsChoosePlane(HLHWord[] wordsArray,string extraInfo = null){
			MapWalkableEventsStopAction ();
			expUICtr.SetUpWordHUD (wordsArray,extraInfo);
		}

		public void ShowCharacterFillPlane(HLHWord word){
			
			expUICtr.SetUpWordHUD (word);
		}

		public void ShowPuzzleView(CallBack answerRightCallBack,CallBack answerWrongCallBack)
        {
            MapWalkableEventsStopAction();
			expUICtr.SetUpPuzzleView(answerRightCallBack,answerWrongCallBack);
        }

		public bool NeedShowFullWordDetailWhenChooseWrong(){

			MapEvent me = currentEnteredMapEvent.GetComponent<MapEvent>();

			return me.IsFullWordNeedToShowWhenChooseWrong();

		}

		public void ChooseAnswerInWordHUD(bool isChooseCorrect){
			MapEvent me = currentEnteredMapEvent;
			if(me == null){
				battlePlayerCtr.isInEvent = false;
				return;
			}
			me.MapEventTriggered(isChooseCorrect, battlePlayerCtr);
		}
        

		public void ConfirmFillCharactersInWordHUD(bool isFillCorrect){

			MapEvent me = currentEnteredMapEvent.GetComponent<MapEvent> ();

			me.MapEventTriggered (true, battlePlayerCtr);

			MapWalkableEventsStartAction ();

		}
              

		public void DisableExploreInteractivity(){
			expUICtr.ShowExploreMask ();
			expUICtr.HideFullMask ();
		}

		public void EnableExploreInteractivity(){
			expUICtr.HideExploreMask ();
			expUICtr.HideFullMask ();
		}

		public void DisableAllInteractivity(){
			expUICtr.HideExploreMask ();
			expUICtr.ShowFullMask ();
		}
        


		public void ObtainReward(Item reward){
			
			Player.mainPlayer.AddItem (reward);

			expUICtr.SetUpSimpleItemDetail (reward);

			expUICtr.UpdateBottomBar();

		}

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



		public void MapWalkableEventsStopAction(){
			for (int i = 0; i < newMapGenerator.allMonstersInMap.Count; i++) {
				MapMonster mapMonster = newMapGenerator.allMonstersInMap[i];
				//if(!mapMonster.isInMoving){
				//	continue;
				//}
				mapMonster.StopMoveAtEndOfCurrentMove ();
			}

			for (int i = 0; i < newMapGenerator.allNPCsInMap.Count; i++)
            {
				newMapGenerator.allNPCsInMap[i].StopMoveAtEndOfCurrentMove();
            }
		}

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

			DisableAllInteractivity();

			battleMonsterCtr = monsterTrans.GetComponent<BattleMonsterController> ();

			battleMonsterCtr.InitMonster (monsterTrans);

			battlePlayerCtr.SetEnemy (battleMonsterCtr);
			battleMonsterCtr.SetEnemy (battlePlayerCtr);


		}

	

		public void UpdatePlayerStatusPlane(){
			expUICtr.UpdatePlayerStatusBar ();
		}



		public void PlayerAndMonsterStartFight(){

			expUICtr.ShowFightPlane();

			// 执行玩家角色战斗前技能回调
			battleMonsterCtr.StartFight (battlePlayerCtr);
			battlePlayerCtr.StartFight (battleMonsterCtr);

			battlePlayerCtr.ExcuteBeforeFightSkillCallBacks(battleMonsterCtr);
			battleMonsterCtr.ExcuteBeforeFightSkillCallBacks(battlePlayerCtr);

		}

		public void PlayerStartFight(){

			expUICtr.ShowFightPlane();

			battlePlayerCtr.StartFight (battleMonsterCtr);
			// 执行玩家角色战斗前技能回调
			battlePlayerCtr.ExcuteBeforeFightSkillCallBacks(battleMonsterCtr);

		}

		public void MonsterStartFight(){

			expUICtr.ShowFightPlane();

			battleMonsterCtr.StartFight (battlePlayerCtr);
			// 执行怪物角色战斗前技能回调
			battleMonsterCtr.ExcuteBeforeFightSkillCallBacks(battlePlayerCtr);

		}

		public void ResetMapWalkableInfo(Vector3 position,int walkaleInfo){
			newMapGenerator.mapWalkableInfoArray [(int)position.x, (int)position.y] = walkaleInfo;
		}
			

		public void ShowNPCPlane(MapNPC mapNPC){
			MapWalkableEventsStopAction ();
			expUICtr.EnterNPC (mapNPC.npc);
		}
			

		public void ShowBillboard(Billboard bb){
			MapWalkableEventsStopAction ();
			expUICtr.SetUpBillboard (bb);
		}


		public void PlayerFade(){
			battlePlayerCtr.PlayerFade ();
		}

		public void BattlePlayerWin(Transform[] monsterTransArray){

			if (monsterTransArray.Length <= 0) {
				return;
			}

			Player.mainPlayer.totaldefeatMonsterCount++;
         
			battlePlayerCtr.SetRoleAnimTimeScale (1.0f);
			battleMonsterCtr.SetRoleAnimTimeScale (1.0f);

			Transform trans = monsterTransArray [0];

			BattleMonsterController bmCtr = trans.GetComponent<BattleMonsterController> ();

			Monster monster = trans.GetComponent<Monster> ();

			MapWalkableEvent walkableEvent = trans.GetComponent<MapWalkableEvent> ();

			Player player = Player.mainPlayer;
            
			FightEndCallBacks ();

            battlePlayerCtr.agent.ClearPropertyChangesFromSkill();

			battlePlayerCtr.agent.ResetBattleAgentProperties (false);

            battleMonsterCtr.agent.ClearPropertyChangesFromSkill();

            battleMonsterCtr.agent.ResetBattleAgentProperties(false);
         
            battlePlayerCtr.FixPositionToStandard ();

			battlePlayerCtr.ResetToWaitAfterCurrentRoleAnimEnd ();

			Vector3 monsterPos = trans.position;

			walkableEvent.RefreshWalkableInfoWhenQuit (true);
                     
			MapMonster mm = bmCtr.GetComponent<MapMonster> ();
            
            if (mm != null) {

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

			EnableExploreInteractivity ();

			battlePlayerCtr.isInFight = false;

			battlePlayerCtr.isInEvent = false;

			battlePlayerCtr.boxCollider.enabled = true;

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

                PlayLevelUpAnim();

                DisableExploreInteractivity();

                expUICtr.ShowLevelUpPlane();
            }
            else
            {
                MapWalkableEventsStartAction();
            }

			battlePlayerCtr.UpdateStatusPlane();
         
		}



		private void PlayLevelUpAnim(){
			battlePlayerCtr.SetEffectAnim (CommonData.levelUpEffectName);
            GameManager.Instance.soundManager.PlayAudioClip (CommonData.levelUpAudioName);
		}

        /// <summary>
        /// 战斗中失败
        /// </summary>
		public void BattlePlayerLose(){
         
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
        

		public void EnterLevel(int level,ExitType exitType){

			exploreSceneReady = false;

			MapWalkableEventsStopAction();

			correctWordList.Clear();
			wrongWordList.Clear();
			//GameManager.Instance.gameDataCenter.currentMapWordRecords.Clear();

			IEnumerator enterLevelCoroutine = LatelyEnterLevel(level, exitType);

			StartCoroutine(enterLevelCoroutine);         

		}

		private IEnumerator LatelyEnterLevel(int level, ExitType exitType){

			yield return new WaitForSeconds(0.1f);

			MapWalkableEventsStopAction();

            GameManager.Instance.pronounceManager.ClearPronunciationCache();

			//GameManager.Instance.persistDataManager.SaveMapEventsRecord();

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

		public void EnterNextLevel(){

			UpdateWordDataBase();
                     
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

		public void EnterLastLevel(){

			expUICtr.SetUpSingleTextTintHUD("这里好像已经被封印了...");

			//UpdateWordDataBase();

			//int level = Player.mainPlayer.currentLevelIndex - 1;

			//GameManager.Instance.persistDataManager.SaveMapEventsRecord();

			//EnterLevel(level,ExitType.LastLevel);
         
			//Debug.LogFormat("finish loading time:{0}", Time.time);
		}
        
		public void SaveDataInExplore(CallBack saveFinishCallBack,bool updateDB = true){

			//DisableAllInteractivity();

			Time.timeScale = 0f;

			expUICtr.SetUpSaveDataHintViewAndSave(delegate
			{
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

			}, delegate
			{
				Time.timeScale = 1f;

				EnableExploreInteractivity();

				if(battlePlayerCtr.enemy != null){
					battlePlayerCtr.isInEvent = true;
				}else{
					battlePlayerCtr.isInEvent = false;
				}
            
				MapWalkableEventsStartAction();

				if(saveFinishCallBack != null){
					saveFinishCallBack();
				}

			});
		}


		public void QuitExploreScene(){

			Time.timeScale = 1;

			this.gameObject.SetActive(false);

			GameManager.Instance.soundManager.StopBgm ();

			//GameManager.Instance.persistDataManager.SaveCompletePlayerData ();
			//GameManager.Instance.persistDataManager.SaveMapEventsRecord();
			UpdateWordDataBase();

			Camera.main.transform.SetParent (null);

			Transform exploreMask = Camera.main.transform.Find ("ExploreMask");
			exploreMask.GetComponent<UnityArmatureComponent> ().animation.Stop ();
			exploreMask.gameObject.SetActive (false);

			battlePlayerCtr.QuitExplore ();

			//TransformManager.DestroyTransfromWithName (CommonData.exploreScenePoolContainerName);

			TransformManager.FindTransform("ExploreCanvas").GetComponent<ExploreUICotroller>().QuitExplore();

			PlayerData playerData = GameManager.Instance.persistDataManager.LoadPlayerData();

            Player.mainPlayer.SetUpPlayerWithPlayerData(playerData);

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

