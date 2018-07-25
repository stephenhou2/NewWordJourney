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
		public void SetUpExploreView(bool fromLastLevel)
		{
			exploreSceneReady = false;

			bool isFinalChapter = Player.mainPlayer.currentLevelIndex == CommonData.maxLevel;

			System.GC.Collect();

            DisableExploreInteractivity();

            newMapGenerator.SetUpMap(fromLastLevel);

            Player.mainPlayer.ClearCollectedCharacters();

            expUICtr.SetUpExploreCanvas();

            battlePlayerCtr.InitBattlePlayer();

			if(isFinalChapter){
				expUICtr.transitionMask.color = new Color(0, 0, 0, 0.75f);
				expUICtr.transitionMask.gameObject.SetActive(true);
				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.finalChapterCanvasBundleName, "FinalChapterCanvas", delegate
				{
					TransformManager.FindTransform("FinalChapterCanvas").GetComponent<FinalChapterViewControlller>().SetUpFinalChapterView();
				});
			}else{
				EnableExploreInteractivity();   
			}
            
			exploreSceneReady = true;

			//GameManager.Instance.UIManager.RemoveCanvasCache("HomeCanvas");
			//GameManager.Instance.UIManager.RemoveCanvasCache("ShareCanvas");
			//GameManager.Instance.UIManager.RemoveCanvasCache("SettingCanvas");
			//GameManager.Instance.UIManager.RemoveCanvasCache("LoadingCanvas");
			//GameManager.Instance.UIManager.RemoveCanvasCache("GuideCanvas");

         
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

	
			if(EventSystem.current.IsPointerOverGameObject()){
//				Debug.LogFormat("点击在UI上{0}",EventSystem.current.currentSelectedGameObject);
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
			

		/// <summary>
		/// 记录选择过的单词
		/// </summary>
		/// <param name="word">Word.</param>
		/// <param name="isChooseRight">If set to <c>true</c> is choose right.</param>
		public void RecordWord(HLHWord word,bool isChooseRight){

			if (isChooseRight) {
				if(!correctWordList.Contains(word)){
					correctWordList.Add (word);
                }
			} else {
				if(!wrongWordList.Contains(word)){
					wrongWordList.Add(word);
				}            
			}

		}

		public void UpdateWordDataBase(){
         
			MySQLiteHelper sql = MySQLiteHelper.Instance;

            sql.GetConnectionWith(CommonData.dataBaseName);

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
            
			sql.CloseConnection(CommonData.dataBaseName);    

		}


		public void ShowWordsChoosePlane(HLHWord[] wordsArray,string extraInfo = null){
			MapWalkableEventsStopAction ();
			expUICtr.SetUpWordHUD (wordsArray,extraInfo);
		}

		public void ShowCharacterFillPlane(HLHWord word){
			MapWalkableEventsStopAction ();
			expUICtr.SetUpWordHUD (word);
		}

		public void ChooseAnswerInWordHUD(bool isChooseCorret){

			MapEvent me = currentEnteredMapEvent.GetComponent<MapEvent> ();

			//if (me is MapNPC) {
			//	MapNPC mn = me as MapNPC;
			//	//if (isChooseCorret) {
			//	//	expUICtr.SetUpNPCWhenWordChooseRight (mn.npc);
			//	//} else {
			//	//	expUICtr.SetUpNPCWhenWordChooseWrong (mn.npc);
			//	//}
			//} else {
				me.MapEventTriggered (isChooseCorret, battlePlayerCtr);
			//}

			MapWalkableEventsStartAction ();

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

			DisableExploreInteractivity ();

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

				//int posX = Mathf.RoundToInt(this.transform.position.x);
                //int posY = Mathf.RoundToInt(this.transform.position.y);

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

		public void BattlePlayerLose(){
         
            battlePlayerCtr.agent.ClearPropertyChangesFromSkill();
            battleMonsterCtr.agent.ClearPropertyChangesFromSkill();

            battleMonsterCtr.agent.ResetBattleAgentProperties(false);

			battlePlayerCtr.SetRoleAnimTimeScale (1.0f);
			battleMonsterCtr.SetRoleAnimTimeScale (1.0f);
            

			FightEndCallBacks ();

			battleMonsterCtr.ResetToWaitAfterCurrentRoleAnimEnd ();

			expUICtr.QuitFight ();

			expUICtr.ShowBuyLifeQueryHUD ();

			battlePlayerCtr.isInEvent = false;

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

			if (player.currentLevelIndex <= CommonData.maxLevel)
			{

				GameManager.Instance.persistDataManager.SaveCompletePlayerData();

				bool fromLastLevel = true;

				switch (exitType)
				{
					case ExitType.LastLevel:
						fromLastLevel = false;
						break;
					case ExitType.NextLevel:
						fromLastLevel = true;
						break;
				}

				SetUpExploreView(fromLastLevel);
			}
		}

		public void EnterNextLevel(){

			UpdateWordDataBase();

			int level = Player.mainPlayer.currentLevelIndex + 1;
         

			if(Player.mainPlayer.maxUnlockLevelIndex < level){
				Player.mainPlayer.maxUnlockLevelIndex = level;
			}

			GameManager.Instance.persistDataManager.SaveMapEventsRecord();

			EnterLevel(level, ExitType.NextLevel);



		}

		public void EnterLastLevel(){
         
			UpdateWordDataBase();

			int level = Player.mainPlayer.currentLevelIndex - 1;

			GameManager.Instance.persistDataManager.SaveMapEventsRecord();

			EnterLevel(level,ExitType.LastLevel);


			//Debug.LogFormat("finish loading time:{0}", Time.time);
		}


		public void QuitExploreScene(){

			Time.timeScale = 1;

			this.gameObject.SetActive(false);

			GameManager.Instance.soundManager.StopBgm ();

			GameManager.Instance.persistDataManager.SaveCompletePlayerData ();
			GameManager.Instance.persistDataManager.SaveMapEventsRecord();
			UpdateWordDataBase();

			Camera.main.transform.SetParent (null);

			Transform exploreMask = Camera.main.transform.Find ("ExploreMask");
			exploreMask.GetComponent<UnityArmatureComponent> ().animation.Stop ();
			exploreMask.gameObject.SetActive (false);

			battlePlayerCtr.QuitExplore ();

			TransformManager.DestroyTransfromWithName (CommonData.exploreScenePoolContainerName);

			GameManager.Instance.gameDataCenter.ReleaseDataWithDataTypes (new GameDataCenter.GameDataType[] {
				GameDataCenter.GameDataType.MapSprites,
				GameDataCenter.GameDataType.SkillSprites,
				GameDataCenter.GameDataType.NPCs,
				GameDataCenter.GameDataType.GameLevelDatas

			});

			MyResourceManager.Instance.UnloadAssetBundle (CommonData.exploreSceneBundleName, true);


			TransformManager.FindTransform ("ExploreCanvas").GetComponent<ExploreUICotroller> ().QuitExplore ();

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {
				TransformManager.FindTransform ("HomeCanvas").GetComponent<HomeViewController> ().SetUpHomeView ();
			}, true, false);

			Destroy (this.gameObject,0.3f);
		}

	}
}

