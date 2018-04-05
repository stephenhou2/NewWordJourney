using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;



namespace WordJourney
{
	
//	using UnityEngine.SceneManagement;

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

		private Transform crystalEntered;

//		private Transform monsterEntered;

		public MapEvent currentEnteredMapEvent;

		public bool isExploreClickValid;


//		[HideInInspector]public bool clickForConsumablesPos;

//		public float cameraRollSpeed = 6f;//镜头拉到传送阵的速度
//		public float cameraStayDuration = 0.5f;//镜头停留时长

		private List<LearnWord> correctWordList = new List<LearnWord>();
		private List<LearnWord> wrongWordList = new List<LearnWord>();


		void Awake()
		{

			mapGenerator = GetComponent<MapGenerator>();

			Transform battlePlayer = Player.mainPlayer.transform.Find ("BattlePlayer");

			battlePlayer.gameObject.SetActive (true);

			battlePlayerCtr = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ();

			battlePlayerCtr.isInExplore = true;
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
//			Material m = newMapGenerator.fogOfWarPlane.GetComponent<Renderer>().material;
//			m.shader = Resources.Load("FOWShader") as Shader;
			#endif
			Material m = newMapGenerator.fogOfWarPlane.GetComponent<Renderer>().material;
			m.shader = Resources.Load("FOWShader") as Shader;
			Debug.Log (m.shader.isSupported);
		}
			
		//Initializes the game for each level.
		public void SetUpExploreView()
		{

			PlayerData playerData = GameManager.Instance.persistDataManager.LoadPlayerData ();

			Player.mainPlayer.SetUpPlayerWithPlayerData (playerData);

			DisableInteractivity ();

			if (!SoundManager.Instance.bgmAS.isPlaying 
				|| SoundManager.Instance.bgmAS.clip.name != CommonData.exploreBgmName) {
				SoundManager.Instance.PlayBgmAudioClip (CommonData.exploreBgmName);
			}

			newMapGenerator.SetUpMap();

			ExploreUICotroller expUICtr = TransformManager.FindTransform ("ExploreCanvas").GetComponent <ExploreUICotroller> ();

			expUICtr.SetUpExploreCanvas ();

			battlePlayerCtr.InitBattlePlayer ();

			EnableInteractivity ();


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
//			EnableInteractivity ();
//		}

		public void ItemsAroundAutoIntoLifeWithBasePoint(Vector3 basePostion,CallBack cb = null){

			mapGenerator.ItemsAroundAutoIntoLifeWithBasePoint (basePostion,cb);

		}


		private void Update(){

			if (!isExploreClickValid) {
				return;
			}

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

//			if (clickForConsumablesPos) {
//				targetX = (int)(clickPos.x + 0.5f);
//				targetY = (int)(clickPos.y + 0.5f);
//				// 以地图左下角为坐标原点时的点击位置
//				targetPos = new Vector3(targetX, targetY, 0);
//				mapGenerator.ClickConsumablesPosAt (targetPos);
//				clickForConsumablesPos = false;
//				return;
//			}


//			// 点击位置在地图有效区之外，直接返回
//			if(clickPos.x + 0.5f >= newMapGenerator.columns 
//				|| clickPos.y + 0.5f >= newMapGenerator.rows
//				|| clickPos.x + 0.5f < 0 
//				|| clickPos.y + 0.5f < 0){
//				Debug.Log ("点击在地图有效区外部");
//				return;
//			}


			// 由于地图贴图 tile时是以中心点为参考，宽高为1，所以如果以实际拼出的地图左下角为坐标原点，则点击位置需要进行如下坐标转换
			targetX = (int)(clickPos.x + 0.5f);

			targetY = (int)(clickPos.y + 0.5f);

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
		public void RecordWord(LearnWord word,bool isChooseRight){

			if (isChooseRight) {
				correctWordList.Add (word);
			} else {
				wrongWordList.Add (word);
			}

		}


		public void ShowWordsChoosePlane(LearnWord[] wordsArray){
			AllWalkableEventsStopMove ();
			expUICtr.SetUpWordHUD (wordsArray);
		}

		public void ShowCharacterFillPlane(LearnWord word){
			AllWalkableEventsStopMove ();
			expUICtr.SetUpWordHUD (word);
		}

		public void ChooseAnswerInWordHUD(bool isChooseCorret){

			MapEvent me = currentEnteredMapEvent.GetComponent<MapEvent> ();

			me.MapEventTriggered (isChooseCorret, battlePlayerCtr);
		}


		public void ConfirmFillCharactersInWordHUD(bool isFillCorrect){

			MapEvent me = currentEnteredMapEvent.GetComponent<MapEvent> ();

			me.MapEventTriggered (true, battlePlayerCtr);

		}

		public void ShowTint(string tintText,Sprite tintIcon){
			expUICtr.SetUpTintHUD (tintText, tintIcon);
		}



//		public void ChangeMapEventStatusAtPosition(Vector3 position){
//			newMapGenerator.
//		}

		public void DisableInteractivity(){
			expUICtr.ShowMask ();
			isExploreClickValid = false;
		}

		public void EnableInteractivity(){
			expUICtr.HideMask ();
			isExploreClickValid = true;
		}

		public void ObtainReward(Item reward){
			
			Player.mainPlayer.AddItem (reward);

			expUICtr.SetUpSimpleItemDetail (reward);

		}

		public void AllWalkableEventsStopMove(){
			for (int i = 0; i < newMapGenerator.allWalkableEventsInMap.Count; i++) {
				newMapGenerator.allWalkableEventsInMap [i].StopMoveAtEndOfCurrentMove ();
			}
		}

		public void AllWalkableEventsStartMove(){
			for (int i = 0; i < newMapGenerator.allWalkableEventsInMap.Count; i++) {
				newMapGenerator.allWalkableEventsInMap [i].StartMove ();
			}
		}


		/// <summary>
		/// 遭遇怪物时的响应方法
		/// </summary>
		/// <param name="monsterTrans">Monster trans.</param>
		public void EnterFight(Transform monsterTrans){

			DisableInteractivity ();

			battleMonsterCtr = monsterTrans.GetComponent<BattleMonsterController> ();

			battleMonsterCtr.InitMonster (monsterTrans);

			battlePlayerCtr.SetEnemy (battleMonsterCtr);
			battleMonsterCtr.SetEnemy (battlePlayerCtr);


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
			expUICtr.EnterNPC (mapNPC.npc, Player.mainPlayer.currentLevelIndex);
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


		public void ChangeCrystalStatus(){
			crystalEntered.GetComponent<Crystal> ().CrystalExausted ();
			mapGenerator.mapWalkableInfoArray [(int)crystalEntered.position.x, (int)crystalEntered.position.y] = 1;
			expUICtr.GetComponent<BattlePlayerUIController> ().UpdateAgentStatusPlane ();
		}

		public void PlayerFade(){
			battlePlayerCtr.PlayerFade ();
		}

		public void BattlePlayerWin(Transform[] monsterTransArray){

			battlePlayerCtr.SetRoleAnimTimeScale (1.0f);
			battleMonsterCtr.SetRoleAnimTimeScale (1.0f);

			Transform trans = monsterTransArray [0];

			BattleMonsterController bmCtr = trans.GetComponent<BattleMonsterController> ();

			Monster monster = trans.GetComponent<Monster> ();

			Player player = Player.mainPlayer;

			player.DestroyEquipmentInBagAttachedSkills ();

			battlePlayerCtr.enemy = null;

			bmCtr.enemy = null;

			FightEndCallBacks ();

			battlePlayerCtr.agent.ResetBattleAgentProperties (false);

			battlePlayerCtr.FixPosition ();

			mapGenerator.AddAllEffectAnimToPool ();

			if (monsterTransArray.Length <= 0) {
				return;
			}


			Vector3 monsterPos = bmCtr.originalPos;

			int monsterPosX = Mathf.RoundToInt(monsterPos.x);
			int monsterPosY = Mathf.RoundToInt(monsterPos.y);

			newMapGenerator.mapWalkableInfoArray [monsterPosX, monsterPosY] = 1;

			player.totalGold += monster.rewardGold;//更新玩家金钱

			string tint = string.Format ("+{0}", monster.rewardGold);

			ShowTint (tint, null);

			player.experience += monster.rewardExperience;//更新玩家经验值

			bool isLevelUp = player.LevelUpIfExperienceEnough ();//判断是否升级

			if (isLevelUp) {
				PlayLevelUpAnim ();
				DisableInteractivity ();
				expUICtr.ShowLevelUpPlane ();
			}

			MapMonster mm = bmCtr.GetComponent<MapMonster> ();
			if (mm != null) {
				int itemId = mm.dropItemID;
				float dropProbability = mm.dropItemProbability;
				if (Random.Range (0, 1.0f) <= dropProbability) {
					newMapGenerator.SetUpRewardInMap (Item.NewItemWith (itemId, 1), mm.transform.position);
				}
			}

			EnableInteractivity ();

			battlePlayerCtr.isInFight = false;

			if (bmCtr.GetComponent<MapNPC> () != null) {
				expUICtr.ShowNPCPlane ();
			}

			AllWalkableEventsStartMove ();

		}


		private void PlayLevelUpAnim(){
			battlePlayerCtr.SetEffectAnim ("LevelUp");
			expUICtr.GetComponent<BattlePlayerUIController> ().UpdateAgentStatusPlane ();
			SoundManager.Instance.PlayAudioClip ("Other/sfx_LevelUp");
		}

		public void BattlePlayerLose(){

			battlePlayerCtr.SetRoleAnimTimeScale (1.0f);
			battleMonsterCtr.SetRoleAnimTimeScale (1.0f);

			(battlePlayerCtr.agent as Player).DestroyEquipmentInBagAttachedSkills ();

			battlePlayerCtr.enemy = null;

			battleMonsterCtr.enemy = null;

			FightEndCallBacks ();

			expUICtr.QuitFight ();

			expUICtr.ShowBuyLifeQueryHUD ();
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
//			EnableInteractivity ();
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

		public void EnterNextLevel(){

			AllWalkableEventsStopMove ();

			Time.timeScale = 1;

			DisableInteractivity ();

			Player player = Player.mainPlayer;

			player.RefreshTasksWhenEnterNextLevel ();

			player.currentLevelIndex++;

			if (player.currentLevelIndex >= 50) {
				Debug.Log ("通关");
				return;
			}

			if (player.currentLevelIndex > player.maxUnlockLevelIndex) {
				player.maxUnlockLevelIndex = player.currentLevelIndex;
			}

			GameManager.Instance.persistDataManager.SaveCompletePlayerData ();
				


			SetUpExploreView ();

//			EnableInteractivity ();

		}


		public void QuitExploreScene(bool saveData){

			Time.timeScale = 1;

			this.gameObject.SetActive(false);

			SoundManager.Instance.StopBgm ();

			Camera.main.transform.Find ("Mask").gameObject.SetActive (false);

			if (saveData) {
				GameManager.Instance.persistDataManager.SaveCompletePlayerData ();
			}
				
//			PlayerData playerData = GameManager.Instance.persistDataManager.LoadPlayerData ();
//
//			Player.mainPlayer.SetUpPlayerWithPlayerData (playerData);

//			Camera.main.transform.Find ("Background").gameObject.SetActive (false);
			Camera.main.transform.SetParent (null);
			Camera.main.transform.Find ("Mask").gameObject.SetActive (false);

			battlePlayerCtr.QuitExplore ();
			battlePlayerCtr.isInExplore = false;
		
//			newMapGenerator.DestroyInstancePools ();

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

