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

		// 地图生成器
		private MapGenerator mapGenerator;	

		public NewMapGenerator newMapGenerator;	
	
		// 当前关卡所有怪物
//		private List<BattleMonsterController> battleMonsters = new List<BattleMonsterController>();	

		// 当前碰到的怪物控制器
		private BattleMonsterController battleMonsterCtr;

		// 玩家控制器
		public BattlePlayerController battlePlayerCtr;

		[HideInInspector]public ExploreUICotroller expUICtr;

		//private Transform crystalEntered;

//		private Transform monsterEntered;

		public MapEvent currentEnteredMapEvent;

//		public bool isExploreClickValid;

//		private bool detectFight;


//		[HideInInspector]public bool clickForConsumablesPos;

//		public float cameraRollSpeed = 6f;//镜头拉到传送阵的速度
//		public float cameraStayDuration = 0.5f;//镜头停留时长

		private List<HLHWord> correctWordList = new List<HLHWord>();
		private List<HLHWord> wrongWordList = new List<HLHWord>();


		void Awake()
		{

			mapGenerator = GetComponent<MapGenerator>();

			Transform battlePlayer = Player.mainPlayer.transform.Find ("BattlePlayer");

			battlePlayer.gameObject.SetActive (true);

			battlePlayerCtr = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ();

//			battlePlayerCtr.isInExplore = true;
			battlePlayerCtr.ActiveBattlePlayer (false, false, false);

			Transform exploreCanvas = TransformManager.FindTransform ("ExploreCanvas");

			expUICtr = exploreCanvas.GetComponent<ExploreUICotroller> ();

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

			#if UNITY_EDITOR || UNITY_IOS 
			Material m = newMapGenerator.fogOfWarPlane.GetComponent<Renderer>().material;
			m.shader = Resources.Load("FOWShader") as Shader;
			#endif

		}
			
		//Initializes the game for each level.
		public void SetUpExploreView()
		{

			DisableExploreInteractivity ();

			if (!Player.mainPlayer.isNewPlayer && (!GameManager.Instance.soundManager.bgmAS.isPlaying 
			                                       || GameManager.Instance.soundManager.bgmAS.clip.name != CommonData.exploreBgmName)) {
				GameManager.Instance.soundManager.PlayBgmAudioClip (CommonData.exploreBgmName);
			}

			newMapGenerator.SetUpMap();

			Player.mainPlayer.ClearCollectedCharacters();
         
			expUICtr.SetUpExploreCanvas ();

			battlePlayerCtr.InitBattlePlayer ();

			EnableExploreInteractivity ();

		}

//		private IEnumerator ShowTransportPosAndRollBack(){
//
//			yield return new WaitForSeconds (cameraStayDuration);
//
//			Vector3 transportPosInMap = mapGenerator.GetTransportPosInMap ();
//
//			Vector3 cameraPos = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,0);
//
//			Vector3 offsetVector = transportPosInMap - cameraPos;
//
//			float distance = offsetVector.magnitude;
//
////			Debug.Log (distance);
//
//			float cameraRollDuration = distance / cameraRollSpeed;
//
////			Debug.Log (cameraRollDuration);
//
//			float moveSpeedX = offsetVector.x / cameraRollDuration;
//			float moveSpeedY = offsetVector.y / cameraRollDuration;
//
//			float timer = 0;
//
//			while (timer < cameraRollDuration) {
//
//				Vector3 moveVector = new Vector3 (moveSpeedX * Time.deltaTime, moveSpeedY * Time.deltaTime, 0);
//
//				Camera.main.transform.position += moveVector; 
//
//				timer += Time.deltaTime;
//
//				yield return null;
//
//			}
//
//			yield return new WaitForSeconds (cameraStayDuration);
//
//			timer = 0;
//
//			while (timer < cameraRollDuration) {
//
//				Vector3 moveVector = new Vector3 (moveSpeedX * Time.deltaTime, moveSpeedY * Time.deltaTime, 0);
//
//				Camera.main.transform.position -= moveVector; 
//
//				timer += Time.deltaTime;
//
//				yield return null;
//
//			}
//
//			Camera.main.transform.localPosition = new Vector3 (0, 0, -5);;
//
//			EnableExploreInteractivity ();
//		}

		public void ItemsAroundAutoIntoLifeWithBasePoint(Vector3 basePostion,CallBack cb = null){

			mapGenerator.ItemsAroundAutoIntoLifeWithBasePoint (basePostion,cb);

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
				correctWordList.Add (word);
			} else {
				wrongWordList.Add (word);
			}

		}


		public void ShowWordsChoosePlane(HLHWord[] wordsArray,string extraInfo = null){
			AllWalkableEventsStopMove ();
			expUICtr.SetUpWordHUD (wordsArray,extraInfo);
		}

		public void ShowCharacterFillPlane(HLHWord word){
			AllWalkableEventsStopMove ();
			expUICtr.SetUpWordHUD (word);
		}

		public void ChooseAnswerInWordHUD(bool isChooseCorret){

			MapEvent me = currentEnteredMapEvent.GetComponent<MapEvent> ();

			if (me is MapNPC) {
				MapNPC mn = me as MapNPC;
				//if (isChooseCorret) {
				//	expUICtr.SetUpNPCWhenWordChooseRight (mn.npc);
				//} else {
				//	expUICtr.SetUpNPCWhenWordChooseWrong (mn.npc);
				//}
			} else {
				me.MapEventTriggered (isChooseCorret, battlePlayerCtr);
			}

			AllWalkableEventsStartMove ();

		}


		public void ConfirmFillCharactersInWordHUD(bool isFillCorrect){

			MapEvent me = currentEnteredMapEvent.GetComponent<MapEvent> ();

			me.MapEventTriggered (true, battlePlayerCtr);

			AllWalkableEventsStartMove ();

		}

//		public void ShowTint(string tintText){
//			expUICtr.SetUpSingleTextTintHUD (tintText);
//		}


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

		}

		public void AllWalkableEventsStopMove(){
			for (int i = 0; i < newMapGenerator.allMonstersInMap.Count; i++) {
				newMapGenerator.allMonstersInMap [i].StopMoveAtEndOfCurrentMove ();
			}
		}

		public void AllWalkableEventsStartMove(){
			if (battlePlayerCtr.isInEvent) {
				return;
			}
			for (int i = 0; i < newMapGenerator.allMonstersInMap.Count; i++) {
				newMapGenerator.allMonstersInMap [i].StartMove ();
			}
		}


		/// <summary>
		/// 遭遇怪物时的响应方法
		/// </summary>
		/// <param name="monsterTrans">Monster trans.</param>
		public void EnterFight(Transform monsterTrans){

//			detectFight = true;

			DisableExploreInteractivity ();

			battleMonsterCtr = monsterTrans.GetComponent<BattleMonsterController> ();

			battleMonsterCtr.InitMonster (monsterTrans);

			battlePlayerCtr.SetEnemy (battleMonsterCtr);
			battleMonsterCtr.SetEnemy (battlePlayerCtr);

//			battlePlayerCtr.isInFight = true;
		}

	

		public void UpdatePlayerStatusPlane(){
			expUICtr.UpdatePlayerStatusBar ();
		}



		public void PlayerAndMonsterStartFight(){

			// 执行玩家角色战斗前技能回调
			battleMonsterCtr.StartFight (battlePlayerCtr);
			battlePlayerCtr.StartFight (battleMonsterCtr);

			battlePlayerCtr.ExcuteBeforeFightSkillCallBacks(battleMonsterCtr);
			battleMonsterCtr.ExcuteBeforeFightSkillCallBacks(battlePlayerCtr);

			expUICtr.ShowFightPlane ();

		}

		public void PlayerStartFight(){
			
			battlePlayerCtr.StartFight (battleMonsterCtr);
			// 执行玩家角色战斗前技能回调
			battlePlayerCtr.ExcuteBeforeFightSkillCallBacks(battleMonsterCtr);

			expUICtr.ShowFightPlane ();

		}

		public void MonsterStartFight(){
			
			battleMonsterCtr.StartFight (battlePlayerCtr);
			// 执行怪物角色战斗前技能回调
			battleMonsterCtr.ExcuteBeforeFightSkillCallBacks(battlePlayerCtr);
			expUICtr.ShowFightPlane ();
		}

		public void ResetMapWalkableInfo(Vector3 position,int walkaleInfo){
			newMapGenerator.mapWalkableInfoArray [(int)position.x, (int)position.y] = walkaleInfo;
		}
			

		public void ShowNPCPlane(MapNPC mapNPC){
			AllWalkableEventsStopMove ();
			expUICtr.EnterNPC (mapNPC.npc);
		}
			

		public void ShowBillboard(Billboard bb){
			AllWalkableEventsStopMove ();
			expUICtr.SetUpBillboard (bb);
		}




//		public void ShowConsumablesValidPointTintAround(Consumables consumables){
//
////			Debug.Log ("显示消耗品使用范围提示");
//
//			if (battlePlayerCtr.pathPosList.Count > 0) {
//				battlePlayerCtr.StopMoveAtEndOfCurrentStep ();
//			}
//
//			StartCoroutine ("LatelyShowConsumablesValidTints",consumables);
//
//		}

//		private IEnumerator LatelyShowConsumablesValidTints(Consumables consumables){
//
//			yield return new WaitUntil (() => battlePlayerCtr.isIdle);
//
//			mapGenerator.ShowConsumablesValidPointsTint (consumables);
//
//		}


		//public void ChangeCrystalStatus(){
		//	crystalEntered.GetComponent<Crystal> ().CrystalExausted ();
		//	mapGenerator.mapWalkableInfoArray [(int)crystalEntered.position.x, (int)crystalEntered.position.y] = 1;
		//	expUICtr.GetComponent<BattlePlayerUIController> ().UpdateAgentStatusPlane ();
		//}

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

			//player.DestroyEquipmentInBagAttachedSkills ();

			battlePlayerCtr.enemy = null;

			bmCtr.enemy = null;

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

            if (isLevelUp)
			{

                PlayLevelUpAnim();

                DisableExploreInteractivity();

                expUICtr.ShowLevelUpPlane();
            }
            else
            {
                AllWalkableEventsStartMove();
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

			//(battlePlayerCtr.agent as Player).DestroyEquipmentInBagAttachedSkills ();

			battlePlayerCtr.enemy = null;

			battleMonsterCtr.enemy = null;

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


//		private void ResetCamareAndContinueMove(Vector3 oriMonsterPos){
//
////			StartCoroutine ("ResetCamera");
//
//			EnableExploreInteractivity ();
//
//			battlePlayerCtr.PlayerMoveToEnemyPosAfterFight (oriMonsterPos);
//		}
			

//		public void RefrestCurrentLevel(){
//
//			Time.timeScale = 1;
//
////			StopCoroutine ("AdjustAgentsAndCameraAndStartFight");
//
//			mapGenerator.PrepareToResetMap ();
//
//			expUICtr.PrepareForRefreshment ();
//
//			PlayerData playerData = GameManager.Instance.persistDataManager.LoadPlayerData ();
//
//			Player.mainPlayer.SetUpPlayerWithPlayerData (playerData);
//
//			int gameLevel = Player.mainPlayer.currentLevelIndex;
//
//			// 游戏中统一使用关卡信息的拷贝，这样不会影响到原始数据
//			GameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas [gameLevel].Copy();
//
//			battlePlayerCtr.ResetAgent ();
//
//			if (monsterEntered != null) {
//				monsterEntered.GetComponent<BattleMonsterController> ().ResetAgent ();
//				monsterEntered = null;
//			}
//
//			if (monsterEntered != null) {
//				monsterEntered.GetComponent<Agent> ().ResetBattleAgentProperties (true);
//			}
//
//
//			SetUpExploreView (levelData);
//
//		}

        

		public void EnterLevel(int level){

			AllWalkableEventsStopMove();

            GameManager.Instance.pronounceManager.ClearPronunciationCache();

            Time.timeScale = 1;

            DisableExploreInteractivity();

            Player player = Player.mainPlayer;

			player.currentLevelIndex = level;

            if (player.currentLevelIndex > player.maxUnlockLevelIndex)
            {
                player.maxUnlockLevelIndex = player.currentLevelIndex;
            }

            if (player.currentLevelIndex >= 50)
            {
                Debug.Log("通关");
                return;
            }

            GameManager.Instance.persistDataManager.SaveCompletePlayerData();

            SetUpExploreView();

		}

		public void EnterNextLevel(){

			int level = Player.mainPlayer.currentLevelIndex + 1;

			EnterLevel(level);

		}


		public void QuitExploreScene(bool saveData){

			Time.timeScale = 1;

			this.gameObject.SetActive(false);

			GameManager.Instance.soundManager.StopBgm ();

			if (saveData) {
				GameManager.Instance.persistDataManager.SaveCompletePlayerData ();
			}

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
			}, false, false);

			Destroy (this.gameObject,0.3f);
		}

	}
}

