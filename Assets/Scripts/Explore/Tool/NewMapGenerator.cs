using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	using System.Data;
	using DragonBones;
	using Transform = UnityEngine.Transform;

	public class NewMapGenerator : MonoBehaviour {

		// 地图信息（用于绘制地图）
		private HLHMapData mapData;

		[HideInInspector]public int rows;
		[HideInInspector]public int columns;

		// 地图可行走信息数组
		public int[,] mapWalkableInfoArray;

		// 地图上怪物和npc位置信息数组(有怪物的位置或者行走中的怪物的行走终点为1，没有怪物的位置或者行走中的怪物的行走原点为0)
		public int[,] mapWalkableEventInfoArray;

		//private HLHGameLevelData levelData;


		//************* 模型 *************//
		public Transform floorModel;//地板模型

		public Transform wallModel;// 墙体模型

		public Transform decorationModel;// 装饰模型

		public PickableItem pickableItemModel;//可拾取物品模型

		public Trap trapModel;// 陷阱模型

		public TreasureBox treasureBoxModel;// 宝箱模型

		public Treasure bucketModel;// 木桶模型

		public Treasure potModel;// 瓦罐模型

		public Exit exitModel;// 出口模型

		public Door doorModel;// 门模型

		public KeyDoor keyDoorModel;// 需要用钥匙开启的门模型

		public Billboard billboardModel;//告示牌模型

      
		public PressSwitch pressSwitchModel;// 压力开关模型

		//public Block blockModel;// 装饰用障碍物模型

		public GoldPack goldPackModel;//钱袋模型

		public Crystal crystalModel;//水晶模型

		public Transform rewardModel;//奖励物品模型

		public Transform characterFragmentModel;//字母碎片模型

		public DiaryPaper diaryPaperModel;//日记模型


		//************ 容器 **************//
		public Transform floorsContainer;//地板容器

		public Transform wallsContainer;//墙体容器

		public Transform decorationsContainer;//装饰容器

		public Transform mapEventsContainer;//事件容器

		public Transform monstersContainer;//怪物容器

		public Transform npcsContainer;//NPC容器

		public Transform rewardContainer;//奖励物品容器

		public Transform characterFragmentContainer;//字母碎片容器


		//************ 缓存池 ************//
		public InstancePool floorsPool;//地板缓存池

		public InstancePool wallsPool;//墙体缓存池

		public InstancePool decorationsPool;//装饰缓存池

		public InstancePool mapEventsPool;//地图事件缓存池

		public InstancePool monstersPool;//怪物缓存池

		public InstancePool npcsPool;//npc缓存池

		public InstancePool rewardPool;//奖励物品缓存池

		public InstancePool effectAnimPool;//技能效果缓存池

		public InstancePool characterFragmentPool;//字母碎片缓存池
        

		// 行走提示动画
		public UnityArmatureComponent walkTint;

		// 数据库处理助手
		private MySQLiteHelper mySql;

		// 记录玩家初始位置
		private Vector2 playerStartPos;
        
        // 记录玩家返回该层的位置
		private Vector2 playerReturnPos;

		private List<TriggeredGear> allTriggeredMapEvents = new List<TriggeredGear> ();
		private List<Trap> allTrapsInMap = new List<Trap>();
		public List<MapMonster> allMonstersInMap = new List<MapMonster> ();
		public List<MapNPC> allNPCsInMap = new List<MapNPC>();

		private List<int> valableNpcIds = new List<int> { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };


		//public Transform fogOfWarPlane;

		// 进入下一层的出口位置
		private Vector2 exitPos;

        // 进入上一层的出口位置
		private Vector2 returnExitPos;

		private List<Vector3> allValidCharacterFragmentPositions = new List<Vector3>();

		public SpellItem spellItemOfCurrentLevel;


        // ********* 小地图元素模型 ******** //
		public Transform miniMapWallModel;

		//public Transform miniMapDoorModel;

		//public Transform miniMapKeyDoorModel;

		//public Transform miniMapLadderModel;

		//public Transform miniMapHoleModel;

		public Transform miniMapExitModel;

		public Transform miniMapReturnExitModel;

		//public Transform miniMapPotModel;

		//public Transform miniMapBucketModel;

		//public Transform miniMapTreasureBoxModel;

		//public Transform miniMapCrystalModel;

		//public Transform miniMapNpcModel;

		//public Transform miniMapGoldPackModel;

		//public Transform miniMapItemModel;
      
		public Transform miniMapMaskModel;

		public Transform miniMapPlayerModel;

		// ********* 小地图元素缓存池 ******** //
		public InstancePool miniMapInstancePool;

		public InstancePool miniMapMaskPool;


		// ********* 小地图元素容器 ******** //
		//public Transform miniMapWallContainer;

		//public Transform miniMapDoorContainer;

		//public Transform miniMapKeyDoorContainer;

		//public Transform miniMapLadderContainer;

		//public Transform miniMapHoleContainer;

		//public Transform miniMapExitContainer;

		//public Transform miniMapReturnExitContainer;

		//public Transform miniMapPotContainer;

		//public Transform miniMapBucketContainer;

		//public Transform miniMapTreasureBoxContainer;

		//public Transform miniMapCrystalContainer;

		//public Transform miniMapNpcContainer;

		//public Transform miniMapGoldPackContainer;

		//public Transform miniMapItemContainer;

		public Transform miniMapInstanceContainer;

		public Transform miniMapMaskContainer;



		[HideInInspector]public Transform miniMapPlayer;

		private List<Transform> allMiniMapMasks = new List<Transform>();

		private Camera miniMapCamera;
              

        /// <summary>
        /// 初始化地图
        /// </summary>
        /// 从上一关进入时人物出现在初始位置，从下一关回来时，出现在返回位置
		public void SetUpMap(bool fromLastLevel)
		{

			if(miniMapCamera == null){
				miniMapCamera = TransformManager.FindTransform("MiniMapCamera").GetComponent<Camera>();
			}

			GameManager.Instance.gameDataCenter.LoadGameLevelDatas();

			Reset ();

			// 绘制实际地图和小地图
			DrawMap ();

			mySql = MySQLiteHelper.Instance;
			mySql.GetConnectionWith (CommonData.dataBaseName);

			// 初始化地图事件
			InitializeMapEvents ();

			InitializePlayerAndSetCamera(fromLastLevel);       

			//DestroyUnusedMonsterAndNpc ();

			int mapIndex = Player.mainPlayer.GetMapIndex();

			if(!MapEventsRecord.IsSpellFinish(mapIndex) && Player.mainPlayer.currentLevelIndex < CommonData.maxLevel){
				GenerateCharacterFragments();
			}

			if((Player.mainPlayer.currentLevelIndex+1) % 5 == 0 && Player.mainPlayer.currentLevelIndex < 45 && !MapEventsRecord.IsDiaryFinish(mapIndex)){
				GenerateDiaryPaper();
			}
                     
			mySql.CloseConnection(CommonData.dataBaseName);

			DestroyUnusedMapInstances();
		}
        
		private void GenerateDiaryPaper(){

			int randomSeed = Random.Range(0,allValidCharacterFragmentPositions.Count);

			Vector3 validPos = allValidCharacterFragmentPositions[randomSeed];

			DiaryPaper diaryPaper = Instantiate(diaryPaperModel.gameObject,mapEventsContainer).GetComponent<DiaryPaper>();
         
			int currentMapIndex = Player.mainPlayer.GetMapIndex();

			MapAttachedInfoTile paperInfo = new MapAttachedInfoTile("diaryPaper", validPos, null);

			diaryPaper.InitializeWithAttachedInfo(currentMapIndex, paperInfo);

			int paperPosX = Mathf.RoundToInt(validPos.x);
			int paperPosY = Mathf.RoundToInt(validPos.y);

            // 日记的可行走信息改为2，保证怪物不会走到单词碎片点上
			mapWalkableInfoArray[paperPosX, paperPosY] = 2;

		}

		private void GenerateCharacterFragments(){

			List<SpellItemModel> allUnusedSpellItemModels = new List<SpellItemModel>();

			List<SpellItemModel> allSpellItemModels = GameManager.Instance.gameDataCenter.allSpellItemModels;

			for (int i = 0; i < allSpellItemModels.Count;i++){

				if(!allSpellItemModels[i].hasUsed){
					allUnusedSpellItemModels.Add(allSpellItemModels[i]);
				}            
			}


			int randomSeed = Random.Range(0, allUnusedSpellItemModels.Count);

			SpellItemModel spellItemModel = allUnusedSpellItemModels[randomSeed];

			spellItemOfCurrentLevel = new SpellItem(spellItemModel,1);

			string spell = spellItemModel.spell;

			char[] characters = spell.ToCharArray();

			for (int i = 0; i < characters.Length;i++){

				char character = characters[i];

				CharacterFragment characterFragment = characterFragmentPool.GetInstance<CharacterFragment>(characterFragmentModel.gameObject, characterFragmentContainer);

				characterFragment.SetPool(characterFragmentPool);

				randomSeed = Random.Range(0, allValidCharacterFragmentPositions.Count);

				Vector3 characterFragmentPosition = allValidCharacterFragmentPositions[randomSeed];

				characterFragment.GenerateCharacterFragment(character, characterFragmentPosition, PlayerObtainCharacterFragment);

				int characterFragmentPosX = Mathf.RoundToInt(characterFragmentPosition.x);
				int characterFragmentPosY = Mathf.RoundToInt(characterFragmentPosition.y);

                // 碎片位置的可行走信息改为2，保证怪物不会走到单词碎片点上
				mapWalkableInfoArray[characterFragmentPosX, characterFragmentPosY] = 2;

                // 保证碎片不重合
				allValidCharacterFragmentPositions.Remove(characterFragmentPosition);

			}
            
		}

		private void PlayerObtainCharacterFragment(char character){
			Player.mainPlayer.allCollectedCharacters.Add(character);
			ExploreManager.Instance.expUICtr.UpdateCharacterFragmentsHUD();         
		}


      
		/// <summary>
		/// 重置地图初始化工具
		/// </summary>
		private void Reset(){

			ClearMapInfos();

			allMiniMapMasks.Clear();

            int randomMapIndex = Player.mainPlayer.GetMapIndex();

#warning 暂时关卡不随机，后面去掉
			//randomMapIndex = Player.mainPlayer.currentLevelIndex;
            
			//randomMapIndex = 39;

			//Player.mainPlayer.currentLevelIndex = 39;

			//Player.mainPlayer.maxUnlockLevelIndex = 39;
         
			mapData = GameManager.Instance.gameDataCenter.LoadMapDataOfLevel (randomMapIndex);

			AllMapInstancesToPool();

			AllMiniMapInstancesToPool();
                     
			// 获取地图建模的行数和列数
			rows = mapData.rowCount;
			columns = mapData.columnCount;

            // 绘制小地图上每个地图块上的遮罩
			DrawMiniMapMasks();

			mapWalkableInfoArray = new int[columns, rows];
			mapWalkableEventInfoArray = new int[columns, rows];

			valableNpcIds = new List<int> { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

			InitMapWalkableInfoAndMonsterPosInfo ();
         
		}

        /// <summary>
		/// 绘制小地图上每个地图块上的遮罩
        /// </summary>
		private void DrawMiniMapMasks(){

			for (int i = 0; i < rows;i++){
				for (int j = 0; j < columns;j++){

					Vector3 maskLocalPos = new Vector3(j, i, 0);

					Transform miniMapMask = miniMapMaskPool.GetInstance<Transform>(miniMapMaskModel.gameObject, miniMapMaskContainer);

					miniMapMask.localPosition = maskLocalPos;

					miniMapMask.localRotation = Quaternion.identity;

					miniMapMask.localScale = Vector3.one;

					allMiniMapMasks.Add(miniMapMask);

				}
			}

		}

        /// <summary>
        /// 清除小地图上一个点附近的阴影遮罩
        /// </summary>
        /// <param name="position">Position.</param>
		public void ClearMiniMapMaskAround(Vector3 position){

			int basePosX = Mathf.RoundToInt(position.x);
			int basePosY = Mathf.RoundToInt(position.y);

			for (int i = basePosX - 3; i < basePosX + 4; i++)
			{
				for (int j = basePosY - 3; j < basePosY + 4; j++)
				{
					if (i < 0 || i >= columns || j < 0 || j >= rows){
						continue;
					}

					int index = columns * j + i;

					allMiniMapMasks[index].gameObject.SetActive(false);

				}            
			}         
		}

		public void MiniMapCameraLatelySleep(){
			miniMapCamera.enabled = true;
			StopCoroutine("MiniMapCameraSleepInOneFrame");
			StartCoroutine("MiniMapCameraSleepInOneFrame");
		}

		private IEnumerator MiniMapCameraSleepInOneFrame(){
			yield return null;
			miniMapCamera.enabled = false;
		}



		/// <summary>
		/// 生成一个空白的可行走信息数组（所有位置初始为透明区域）
		/// </summary>
		private void InitMapWalkableInfoAndMonsterPosInfo(){
			for (int i = 0; i < columns; i++) {
				for (int j = 0; j < rows ; j++) {
					mapWalkableInfoArray [i, j] = -2;
					mapWalkableEventInfoArray [i, j] = 0;
				}
			}
		}


		private void ClearMapInfos(){         
			allTriggeredMapEvents.Clear ();
			allMonstersInMap.Clear();
			allNPCsInMap.Clear();
			allTrapsInMap.Clear();
			allValidCharacterFragmentPositions.Clear();
		}

		/// <summary>
		/// 绘制所有地图元素
		/// 绘制小地图元素
		/// </summary>
		private void DrawMap(){

			for (int i = 0; i < mapData.mapLayers.Count; i++) {

				MapLayer layer = mapData.mapLayers [i];

				DrawMapLayer (layer);

			}

		}
        
      

		/// <summary>
		/// 初始化所有地图事件
		/// </summary>
		private void InitializeMapEvents(){

			int mapEventCount = mapData.GetMapEventCount ();

			int currentMapIndex = Player.mainPlayer.GetMapIndex();

			// 获取当前地图中要使用的单词
			HLHWord[] words = InitLearnWordsInMap (mapEventCount);

			// 生成一个记录可使用单词序号的列表
			List<int> unusedWordIndexRecordList = new List<int> ();

			// 初始化可使用单词列表
			for (int i = 0; i < mapEventCount; i++) {
				unusedWordIndexRecordList.Add (i);
			}

			for (int i = 0; i < mapData.attachedInfoLayers.Count; i++) {
                
				MapAttachedInfoLayer layer = mapData.attachedInfoLayers [i];

				InitializeMapEventsOfLayer (currentMapIndex, layer,words,unusedWordIndexRecordList);

			}

		}

		/// <summary>
		/// 初始化地图上要用到的单词
		/// </summary>
		/// <returns>The learn words in map.</returns>
		/// <param name="mapEventCount">Map event count.</param>
		private HLHWord[] InitLearnWordsInMap(int mapEventCount)
		{

			HLHWord[] words = new HLHWord[mapEventCount];

			string currentWordsTableName = LearningInfo.Instance.GetCurrentLearningWordsTabelName();

			//Debug.Log("tableName" + currentWordsTableName);

			string query = string.Format("SELECT learnedTimes FROM {0} ORDER BY learnedTimes ASC", currentWordsTableName);

			IDataReader reader = mySql.ExecuteQuery(query);

			reader.Read();

			int wholeLearnTime = reader.GetInt16(0);

			query = string.Format("SELECT COUNT(*) FROM {0} WHERE learnedTimes=ungraspTimes AND learnedTimes>0", currentWordsTableName);

			reader = mySql.ExecuteQuery(query);

			reader.Read();

			int wrongWordCount = reader.GetInt32(0);

			if (wrongWordCount >= mapEventCount)
			{
				query = string.Format("SELECT * FROM {0} WHERE learnedTimes=ungraspTimes AND learnedTimes>0 LIMIT {1}", currentWordsTableName, mapEventCount);

				int index = 0;

				reader = mySql.ExecuteQuery(query);

				for (int i = 0; i < mapEventCount; i++)
				{
					reader.Read();

					int wordId = reader.GetInt32(0);

					string spell = reader.GetString(1);

					string phoneticSymble = reader.GetString(2);

					string explaination = reader.GetString(3);

					string sentenceEN = reader.GetString(4);

					string sentenceCH = reader.GetString(5);

					string pronounciationURL = reader.GetString(6);

					int wordLength = reader.GetInt16(7);

					int learnedTimes = reader.GetInt16(8);

					int ungraspTimes = reader.GetInt16(9);

					bool isFamiliar = reader.GetInt16(10) == 1;

					HLHWord word = new HLHWord(wordId, spell, phoneticSymble, explaination, sentenceEN, sentenceCH, pronounciationURL, wordLength, learnedTimes, ungraspTimes,isFamiliar);

					words[index] = word;

					index++;

				}
			}
			else
			{

				query = string.Format("SELECT COUNT(*) FROM {0} WHERE learnedTimes={1}", currentWordsTableName, wholeLearnTime);

				reader = mySql.ExecuteQuery(query);

				reader.Read();

				int validWordCount = reader.GetInt32(0);

				if (validWordCount < mapEventCount - wrongWordCount)
				{

					string[] colFields = { "learnedTimes" };
					string[] values = { (wholeLearnTime + 1).ToString() };
					string[] conditions = { "learnedTimes=" + wholeLearnTime.ToString() };

					mySql.UpdateValues(currentWordsTableName, colFields, values, conditions, true);

					wholeLearnTime++;

				}

				int index = 0;

				//Debug.Log(wrongWordCount);

				query = string.Format("SELECT * FROM {0} WHERE learnedTimes=ungraspTimes AND learnedTimes>0 LIMIT {1}", currentWordsTableName, wrongWordCount);

				reader = mySql.ExecuteQuery(query);

				for (int i = 0; i < wrongWordCount; i++)
				{
					reader.Read();

					int wordId = reader.GetInt32(0);

					string spell = reader.GetString(1);

					string phoneticSymble = reader.GetString(2);

					string explaination = reader.GetString(3);

					string sentenceEN = reader.GetString(4);

					string sentenceCH = reader.GetString(5);

					string pronounciationURL = reader.GetString(6);

					int wordLength = reader.GetInt16(7);

					int learnedTimes = reader.GetInt16(8);

					int ungraspTimes = reader.GetInt16(9);

					bool isFamiliar = reader.GetInt16(10) == 1;

					HLHWord word = new HLHWord(wordId, spell, phoneticSymble, explaination, sentenceEN, sentenceCH, pronounciationURL, wordLength, learnedTimes, ungraspTimes,isFamiliar);

					words[index] = word;

					index++;

					//Debug.LogFormat("{0}/{1}/{2}",word.spell,word.learnedTimes,word.ungraspTimes);

				}

				// 边界条件
				string[] condition = { string.Format("learnedTimes={0} ORDER BY RANDOM() LIMIT {1}", wholeLearnTime, mapEventCount - wrongWordCount) };

				reader = mySql.ReadSpecificRowsOfTable(currentWordsTableName, null, condition, true);

				while (reader.Read())
				{

					int wordId = reader.GetInt32(0);

					string spell = reader.GetString(1);

					string phoneticSymble = reader.GetString(2);

					string explaination = reader.GetString(3);

					string sentenceEN = reader.GetString(4);

					string sentenceCH = reader.GetString(5);

					string pronounciationURL = reader.GetString(6);

					int wordLength = reader.GetInt16(7);

					int learnedTimes = reader.GetInt16(8);

					int ungraspTimes = reader.GetInt16(9);

					bool isFamiliar = reader.GetInt16(10) == 1;

                    HLHWord word = new HLHWord(wordId, spell, phoneticSymble, explaination, sentenceEN, sentenceCH, pronounciationURL, wordLength, learnedTimes, ungraspTimes, isFamiliar);

					words[index] = word;

					index++;

				}

			}


    //			for (int i = 0; i < words.Length; i++)
    //			{
				//Debug.LogFormat("spell:{0},learnedTimes:{1},ungraspTimes:{2}",words[i].spell,words[i].learnedTimes,words[i].ungraspTimes);
    			//}

    			return words;
    		}



			private void InitializeMapEventsOfLayer(int mapIndex, MapAttachedInfoLayer layer, HLHWord[] words, List<int> unusedWordIndexList)
			{

				bool exitOpen = true;
				Exit exit = null;


				for (int i = 0; i < layer.tileDatas.Count; i++)
				{

					MapAttachedInfoTile eventTile = layer.tileDatas[i];

					int posX = Mathf.RoundToInt(eventTile.position.x);
					int posY = Mathf.RoundToInt(eventTile.position.y);

					MapEvent mapEvent = null;

					switch (eventTile.type)
					{
						case "playerStart":
							playerStartPos = new Vector2(posX, posY);
							break;
						case "playerReturn":
							playerReturnPos = new Vector2(posX, posY);
							break;
						case "exit":
							mapEvent = mapEventsPool.GetInstanceWithName<Exit>(exitModel.name, exitModel.gameObject, mapEventsContainer);
							mapWalkableInfoArray[posX, posY] = 5;
							exitPos = new Vector2(posX, posY);
							allTriggeredMapEvents.Add(mapEvent as TriggeredGear);
							exit = mapEvent as Exit;
							exit.SetUpExitType(ExitType.NextLevel);
    						int towardsInt = int.Parse(KVPair.GetPropertyStringWithKey("direction", eventTile.properties));
    						MyTowards towards = (MyTowards)towardsInt;                  
    						Transform miniMapInstance = DrawMiniMapInstance(miniMapExitModel, eventTile.position, miniMapInstanceContainer);
    						mapEvent.miniMapInstance = miniMapInstance;

						switch(towards){
							case MyTowards.Up:
								miniMapInstance.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
								//miniMapInstance.localPosition = eventTile.position + new Vector2(0, 0.3f);
								break;
							case MyTowards.Down:
								miniMapInstance.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
								//miniMapInstance.localPosition = eventTile.position + new Vector2(0, -0.3f);
								break;
							case MyTowards.Left:
								miniMapInstance.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
								//miniMapInstance.localPosition = eventTile.position + new Vector2(-0.3f, 0);
								break;
							case MyTowards.Right:
								miniMapInstance.localRotation = Quaternion.Euler(new Vector3(0, 0, 270));
								//miniMapInstance.localPosition = eventTile.position + new Vector2(0.3f, 0);
								break;
						}
                                          
							break;
						case "return":
							mapEvent = mapEventsPool.GetInstanceWithName<Exit>(exitModel.name, exitModel.gameObject, mapEventsContainer);
							mapWalkableInfoArray[posX, posY] = 5;
							returnExitPos = new Vector2(posX, posY);
							allTriggeredMapEvents.Add(mapEvent as TriggeredGear);
							Exit returnExit = mapEvent as Exit;
							returnExit.SetUpExitType(ExitType.LastLevel);
    						miniMapInstance = DrawMiniMapInstance(miniMapReturnExitModel, eventTile.position, miniMapInstanceContainer);
    						mapEvent.miniMapInstance = miniMapInstance;
                            //miniMapInstance.localPosition = eventTile.position + new Vector2(0, -0.3f);
							break;
						case "billboard":
							mapEvent = mapEventsPool.GetInstanceWithName<Billboard>(billboardModel.name, billboardModel.gameObject, mapEventsContainer);
							mapWalkableInfoArray[posX, posY] = 0;
							break;
						case "item":
							if (MapEventsRecord.IsMapEventTriggered(mapIndex, eventTile.position))
							{
								continue;
							}
							mapEvent = mapEventsPool.GetInstanceWithName<PickableItem>(pickableItemModel.name, pickableItemModel.gameObject, mapEventsContainer);
							mapWalkableInfoArray[posX, posY] = 5;
    						// 取3个不同的单词（首项做为目标单词，其余两项做为混淆单词）
                            HLHWord[] wordsArray = GetDifferentWords(words, unusedWordIndexList, 3);                  
                            mapEvent.wordsArray = wordsArray;
							break;
						case "monster":
							mapEvent = GetMonster(eventTile);
							if (mapEvent != null)
							{
								(mapEvent as MapMonster).SetPosTransferSeed(rows);
								mapWalkableInfoArray[posX, posY] = 5;
								mapWalkableEventInfoArray[posX, posY] = 1;
								allMonstersInMap.Add(mapEvent as MapMonster);
							}
							break;
						case "boss":
							if (MapEventsRecord.IsMapEventTriggered(mapIndex, eventTile.position))
							{
								continue;
							}
							mapEvent = GetBoss(eventTile);
							if (mapEvent != null)
							{
								(mapEvent as MapMonster).SetPosTransferSeed(rows);
								mapWalkableInfoArray[posX, posY] = 5;
								mapWalkableEventInfoArray[posX, posY] = 1;
								allMonstersInMap.Add(mapEvent as MapMonster);
								exitOpen = false;
							}
							break;
						case "goldPack":
							mapEvent = mapEventsPool.GetInstanceWithName<GoldPack>(goldPackModel.name, goldPackModel.gameObject, mapEventsContainer);
							mapWalkableInfoArray[posX, posY] = 0;
    						// 取3个不同的单词（首项做为目标单词，其余两项做为混淆单词）
                            wordsArray = GetDifferentWords(words, unusedWordIndexList, 3);                  
                            mapEvent.wordsArray = wordsArray;
							break;
						case "bucket":
							mapEvent = mapEventsPool.GetInstanceWithName<Treasure>(bucketModel.name, bucketModel.gameObject, mapEventsContainer);
							mapWalkableInfoArray[posX, posY] = 0;
							(mapEvent as Treasure).treasureType = TreasureType.Bucket;
    						// 取3个不同的单词（首项做为目标单词，其余两项做为混淆单词）
                            wordsArray = GetDifferentWords(words, unusedWordIndexList, 3);                  
                            mapEvent.wordsArray = wordsArray;
							break;
						case "pot":
							mapEvent = mapEventsPool.GetInstanceWithName<Treasure>(potModel.name, potModel.gameObject, mapEventsContainer);
							mapWalkableInfoArray[posX, posY] = 0;
							(mapEvent as Treasure).treasureType = TreasureType.Pot;
    						// 取3个不同的单词（首项做为目标单词，其余两项做为混淆单词）
                            wordsArray = GetDifferentWords(words, unusedWordIndexList, 3);                  
                            mapEvent.wordsArray = wordsArray;
							break;
						case "treasure":
							mapEvent = mapEventsPool.GetInstanceWithName<TreasureBox>(treasureBoxModel.name, treasureBoxModel.gameObject, mapEventsContainer);
							mapWalkableInfoArray[posX, posY] = 0;
    						// 取3个不同的单词（首项做为目标单词，其余两项做为混淆单词）
                            wordsArray = GetDifferentWords(words, unusedWordIndexList, 3);                  
                            mapEvent.wordsArray = wordsArray;
							break;
						case "crystal":
							mapEvent = mapEventsPool.GetInstanceWithName<Crystal>(crystalModel.name, crystalModel.gameObject, mapEventsContainer);
							mapWalkableInfoArray[posX, posY] = 0;
    						// 取3个不同的单词（首项做为目标单词，其余两项做为混淆单词）
                            wordsArray = GetDifferentWords(words, unusedWordIndexList, 3);                  
                            mapEvent.wordsArray = wordsArray;
							break;
						case "doorGear":
							mapEvent = mapEventsPool.GetInstanceWithName<Door>(doorModel.name, doorModel.gameObject, mapEventsContainer);
							(mapEvent as Door).SetPosTransferSeed(rows);
							mapWalkableInfoArray[posX, posY] = 5;
							allTriggeredMapEvents.Add(mapEvent as TriggeredGear);
						    DrawExtraWallsAroundDoor(eventTile);
    						// 取3个不同的单词（首项做为目标单词，其余两项做为混淆单词）
                            wordsArray = GetDifferentWords(words, unusedWordIndexList, 3);                  
                            mapEvent.wordsArray = wordsArray;
							break;
						case "keyDoorGear":
							mapEvent = mapEventsPool.GetInstanceWithName<KeyDoor>(keyDoorModel.name, keyDoorModel.gameObject, mapEventsContainer);
							(mapEvent as KeyDoor).SetPosTransferSeed(rows);
							mapWalkableInfoArray[posX, posY] = 5;
							allTriggeredMapEvents.Add(mapEvent as TriggeredGear);
						    DrawExtraWallsAroundDoor(eventTile);
    						// 取3个不同的单词（首项做为目标单词，其余两项做为混淆单词）
                            wordsArray = GetDifferentWords(words, unusedWordIndexList, 3);                  
                            mapEvent.wordsArray = wordsArray;
							break;                  
						case "pressSwitch":
							mapEvent = mapEventsPool.GetInstanceWithName<PressSwitch>(pressSwitchModel.name, pressSwitchModel.gameObject, mapEventsContainer);
							(mapEvent as PressSwitch).SetPosTransferSeed(rows);
							mapWalkableInfoArray[posX, posY] = 0;
							allTriggeredMapEvents.Add(mapEvent as TriggeredGear);
    						// 取3个不同的单词（首项做为目标单词，其余两项做为混淆单词）
                            wordsArray = GetDifferentWords(words, unusedWordIndexList, 3);                  
                            mapEvent.wordsArray = wordsArray;
							break;
    					case "thornTrap":
						case "fireTrap":
					    case "poisonTrap":
    						mapEvent = mapEventsPool.GetInstanceWithName<Trap>(trapModel.name, trapModel.gameObject, mapEventsContainer);
                            mapWalkableInfoArray[posX, posY] = 1;
                            allTriggeredMapEvents.Add(mapEvent as TriggeredGear);
                            allTrapsInMap.Add(mapEvent as Trap);
						    break;
						case "npc":
							mapEvent = GetMapNPC(eventTile);
							if (mapEvent != null)
							{
								mapWalkableInfoArray[posX, posY] = 0;
								mapWalkableEventInfoArray[posX, posY] = 1;
								allNPCsInMap.Add(mapEvent as MapNPC);
							}
							break;

					}

					if (mapEvent != null)
					{               
						mapEvent.InitializeWithAttachedInfo(mapIndex, eventTile);

						if (allValidCharacterFragmentPositions.Contains(eventTile.position))
						{
							allValidCharacterFragmentPositions.Remove(eventTile.position);
						}

					}

				}

				if (exit != null && !exitOpen)
				{
					exit.isOpen = false;
				}

			}

		private void DrawExtraWallsAroundDoor(MapAttachedInfoTile eventTile)
		{
			MyTowards towards = (MyTowards)(int.Parse(KVPair.GetPropertyStringWithKey("direction", eventTile.properties)));

            // 只有朝上和朝左的门才在小地图上绘制额外的墙，这样就不会重复绘制
			switch(towards){
				case MyTowards.Up:
					for (int i = 0; i < 3;i++){
						Vector3 extraWallPos = new Vector3(eventTile.position.x - 1, eventTile.position.y + i + 1, 0);
						DrawMiniMapInstance(miniMapWallModel, extraWallPos, miniMapInstanceContainer);
					}
					for (int i = 0; i < 3; i++)
                    {
                        Vector3 extraWallPos = new Vector3(eventTile.position.x + 1, eventTile.position.y + i + 1, 0);
                        DrawMiniMapInstance(miniMapWallModel, extraWallPos, miniMapInstanceContainer);
                    }
					break;
				case MyTowards.Left:
					for (int i = 0; i < 3; i++)
                    {
						Vector3 extraWallPos = new Vector3(eventTile.position.x - i - 1, eventTile.position.y + 1, 0);
                        DrawMiniMapInstance(miniMapWallModel, extraWallPos, miniMapInstanceContainer);
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Vector3 extraWallPos = new Vector3(eventTile.position.x - i - 1, eventTile.position.y -1 , 0);
                        DrawMiniMapInstance(miniMapWallModel, extraWallPos, miniMapInstanceContainer);
                    }
					break;
				default:
					break;
			}
		}

		public void ChangeMapEventStatusAtPosition(Vector3 position)
			{

				for (int i = 0; i < allTriggeredMapEvents.Count; i++)
				{

					TriggeredGear tg = allTriggeredMapEvents[i];

					if (!MyTool.ApproximatelySamePosition2D(tg.transform.position, position))
					{
						continue;
					}

					tg.ChangeStatus();

				}
			}

			public void ChangeAllTrapsStatusInMap()
			{

				for (int i = 0; i < allTriggeredMapEvents.Count; i++)
				{

					TriggeredGear tg = allTriggeredMapEvents[i];

					tg.ChangeStatus();

				}
			}

			/// <summary>
			/// 选择指定数量的不重复的随机单词
			/// </summary>
			/// <returns>The different words.</returns>
			/// <param name="words">Words.</param>
			/// <param name="unusedWordRecordList">Unused word record list.</param>
			/// <param name="count">Count.</param>
			private HLHWord[] GetDifferentWords(HLHWord[] words, List<int> unusedWordRecordList, int count)
			{

				if (words.Length < count)
				{
					return null;
				}

				HLHWord[] wordsArray = new HLHWord[count];

				// 记录已选过的单词在words数组中的位置信息
				List<int> tempList = new List<int>();

				// 从未使用过的单词列表中随机1个单词做为目标单词
				int targetWordIndexInUnusedWordList = Random.Range(0, unusedWordRecordList.Count);

				int targetWordIndexInWords = unusedWordRecordList[targetWordIndexInUnusedWordList];

				unusedWordRecordList.RemoveAt(targetWordIndexInUnusedWordList);

				wordsArray[0] = words[targetWordIndexInWords];

				tempList.Add(targetWordIndexInWords);

				// 已选出的不相同的随机混淆单词的数量
				int tempCounter = 1;

				// 从整个单词列表中随机两个不同的混淆单词（最终三个单词互不相同）
				while (tempCounter < count)
				{

					int index = Random.Range(0, words.Length);

					if (!tempList.Contains(index))
					{

						tempList.Add(index);

						wordsArray[tempCounter] = words[index];

						tempCounter++;
					}

				}

				return wordsArray;
			}


			/// <summary>
			/// 按照id获取怪物
			/// </summary>
			/// <returns>The monster.</returns>
			/// <param name="info">Info.</param>
			private MapEvent GetMonster(MapAttachedInfoTile info)
			{

				HLHGameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas[Player.mainPlayer.currentLevelIndex];

				int randomSeed = Random.Range(0, levelData.monsterIds.Count);

				int monsterId = levelData.monsterIds[randomSeed];

				levelData.monsterIds.RemoveAt(randomSeed);

				Transform monster = null;

				string monsterName = MyTool.GetMonsterName(monsterId);

				if (monsterName.Equals(string.Empty))
				{
					return null;
				}

				for (int i = 0; i < monstersPool.transform.childCount; i++)
				{
					Transform monsterInPool = monstersPool.transform.GetChild(i);
					if (monsterInPool.name == monsterName)
					{
						monster = monsterInPool;
						break;
					}
				}

				if (monster == null)
				{
					monster = GameManager.Instance.gameDataCenter.LoadMonster(monsterName).transform;
				}

				monster.SetParent(monstersContainer);
				monster.localScale = Vector3.one;
				monster.localRotation = Quaternion.identity;

				return monster.GetComponent<MapMonster>();
			}

			/// <summary>
			/// 按照id获取怪物
			/// </summary>
			/// <returns>The monster.</returns>
			/// <param name="info">Info.</param>
			private MapEvent GetBoss(MapAttachedInfoTile info)
			{

				HLHGameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas[Player.mainPlayer.currentLevelIndex];

				if (!HLHGameLevelData.IsBossLevel())
				{
					return null;
				}

				int bossId = levelData.bossId;

				Transform boss = null;

				string bossName = MyTool.GetMonsterName(bossId);

				if (bossName.Equals(string.Empty))
				{
					return null;
				}

				boss = GameManager.Instance.gameDataCenter.LoadMonster(bossName).transform;

				boss.SetParent(monstersContainer);
				boss.localScale = Vector3.one;
				boss.localRotation = Quaternion.identity;

				return boss.GetComponent<MapMonster>();
			}


			private MapEvent GetMapNPC(MapAttachedInfoTile info)
			{

				int npcId = int.Parse(KVPair.GetPropertyStringWithKey("npcID", info.properties));

				if (npcId == 0 && !HLHGameLevelData.HasWiseMan())
				{
					return null;
				}

				if (npcId == -1)
				{
					int randomSeed = Random.Range(0, valableNpcIds.Count);
					npcId = valableNpcIds[randomSeed];
					valableNpcIds.Remove(npcId);
				}

				string npcName = MyTool.GetNpcName(npcId);

				Transform mapNpcTrans = null;

				for (int i = 0; i < npcsPool.transform.childCount; i++)
				{
					Transform npcInPool = npcsPool.transform.GetChild(i);
					if (npcInPool.name == npcName)
					{
						mapNpcTrans = npcInPool;
						break;
					}
				}

				if (mapNpcTrans == null)
				{
					mapNpcTrans = GameManager.Instance.gameDataCenter.LoadMapNpc(npcName).transform;
				}

				mapNpcTrans.SetParent(npcsContainer);
				mapNpcTrans.localScale = Vector3.one;
				mapNpcTrans.localRotation = Quaternion.identity;

				MapNPC mapNpc = mapNpcTrans.GetComponent<MapNPC>();

				mapNpc.SetNpcId(npcId);

				return mapNpc;
			}

      
			//public void AddNewPickableItem(Vector3 position, Item attachedItem)
			//{

			//	Transform pickableItem = Instantiate(pickableItemModel.gameObject, mapEventsContainer).transform;

			//	pickableItem.GetComponent<PickableItem>().InitializeWithItemAndPosition(position, attachedItem);

			//}



			/// <summary>
			/// 将人物放置在人物初始点
			/// </summary>
			private void InitializePlayerAndSetCamera(bool fromLastLevel)
			{


				MyTowards towards = MyTowards.Down;
				Vector3 position = Vector3.zero;

				if (fromLastLevel)
				{
					if (Mathf.RoundToInt(playerStartPos.x) == Mathf.RoundToInt(returnExitPos.x))
					{
						towards = playerStartPos.y >= returnExitPos.y ? MyTowards.Up : MyTowards.Down;
					}
					else
					{
						towards = playerStartPos.x >= returnExitPos.x ? MyTowards.Right : MyTowards.Left;
					}
					position = playerStartPos;
				}
				else
				{
					if (Mathf.RoundToInt(playerReturnPos.x) == Mathf.RoundToInt(exitPos.x))
					{
						towards = playerReturnPos.y >= exitPos.y ? MyTowards.Up : MyTowards.Down;
					}
					else
					{
						towards = playerReturnPos.x >= exitPos.x ? MyTowards.Right : MyTowards.Left;
					}
					position = playerReturnPos;
				}


				Transform player = Player.mainPlayer.transform.Find("BattlePlayer");
				player.position = position;

				BattlePlayerController bp = player.GetComponent<BattlePlayerController>();
				bp.singleMoveEndPos = position;

				bp.pathPosList.Clear();
				bp.ActiveBattlePlayer(true, true, true);
				bp.SetSortingOrder(-Mathf.RoundToInt(position.y));

				bp.isIdle = false;

				switch (towards)
				{
					case MyTowards.Up:
						bp.TowardsUp();
						break;
					case MyTowards.Down:
						bp.TowardsDown();
						break;
					case MyTowards.Right:
						bp.TowardsRight();
						break;
					case MyTowards.Left:
						bp.TowardsLeft();
						break;

				}

				bp.StopMoveAndWait();

				bp.isInFight = false;
				bp.isInEvent = false;
				bp.isInEscaping = false;
				bp.isDead = false;
				bp.isInPosFixAfterFight = false;

				Camera mainCamera = Camera.main;
				mainCamera.transform.SetParent(player, false);
				mainCamera.transform.localPosition = new Vector3(0, 0, -10);
				mainCamera.transform.localScale = Vector3.one;
				mainCamera.transform.localRotation = Quaternion.identity;
            
			    miniMapPlayer = DrawMiniMapInstance(miniMapPlayerModel, position, miniMapInstanceContainer);

				SetUpExploreMask(0);

			    ClearMiniMapMaskAround(position);

		    	MiniMapCameraLatelySleep();

			}

        /// <summary>
        /// Sets up explore mask.
        /// </summary>
		/// <param name="maskStatus">【0:较暗 1:较亮】</param>
			public void SetUpExploreMask(int maskStatus)
			{

				Transform exploreMask = Camera.main.transform.Find("ExploreMask");
				exploreMask.gameObject.SetActive(true);
				UnityArmatureComponent maskArmCom = exploreMask.GetComponent<UnityArmatureComponent>();
			    switch (maskStatus)
				{
					case 0:
						maskArmCom.animation.Play(CommonData.exploreDarktMaskAnimName, 0);
						break;
					case 1:
						maskArmCom.animation.Play(CommonData.exploreLightMaskAnimName, 0);
						break;
				}

			}




			/// <summary>
			/// 绘制单个地图层
			/// </summary>
			/// <param name="layer">Layer.</param>
			private void DrawMapLayer(MapLayer layer)
			{

				string tileSetsImageName = mapData.mapTilesImageName;

				List<Sprite> allMapSprites = GameManager.Instance.gameDataCenter.GetMapTileSpritesFrom(tileSetsImageName);


				Transform mapTileModel;
				Transform mapTilesContainer;
				InstancePool mapTilesPool;

				PrepareForMapLayer(layer.layerName, out mapTileModel, out mapTilesContainer, out mapTilesPool);

				for (int i = 0; i < layer.tileDatas.Count; i++)
				{

					MapTile tile = layer.tileDatas[i];

					string tileImageName = string.Format("{0}_{1}", tileSetsImageName, tile.tileIndex);

					Sprite tileSprite = allMapSprites.Find(delegate (Sprite obj)
					{
						return obj.name == tileImageName;

					});

					Transform mapTile = mapTilesPool.GetInstance<Transform>(mapTileModel.gameObject, mapTilesContainer);

					SpriteRenderer sr = mapTile.GetComponent<SpriteRenderer>();

					sr.sprite = tileSprite;
            
					mapTile.position = tile.position;

					int posX = Mathf.RoundToInt(tile.position.x);
					int posY = Mathf.RoundToInt(tile.position.y);

					if (layer.layerName.Equals("FloorLayer") && tile.canWalk)
					{
						allValidCharacterFragmentPositions.Add(tile.position);
					}

					if (layer.layerName.Equals("WallLayer"))
					{
						if (allValidCharacterFragmentPositions.Contains(tile.position))
						{
							allValidCharacterFragmentPositions.Remove(tile.position);
						}
					}

					if (layer.layerName.Equals("DecorationLayer"))
					{
						sr.sortingOrder = -posY;
						if (allValidCharacterFragmentPositions.Contains(tile.position))
						{
							allValidCharacterFragmentPositions.Remove(tile.position);
						}
						               
                        
					}


					if (mapWalkableInfoArray[posX, posY] == -2
						|| mapWalkableInfoArray[posX, posY] == 1)
					{
						mapWalkableInfoArray[posX, posY] = tile.canWalk ? 1 : -1;
					}

                    // 绘制小地图上的部分元素
    				if(tile.miniMapInfo >=0){
    					
    					MiniMapDisplayType miniMapDisplayType = (MiniMapDisplayType)tile.miniMapInfo;

    					switch(miniMapDisplayType){
    						case MiniMapDisplayType.Wall:
							DrawMiniMapInstance(miniMapWallModel, tile.position, miniMapInstanceContainer);
    							break;
    						default:
    							break;
    					}
    				}

				}

			}
        
        /// <summary>
        /// 绘制小地图上的元素
        /// </summary>
        /// <param name="instanceModel">小地图上的元素模型</param>
        /// <param name="localPosition">小地图上的元素位置</param>
        /// <param name="container">Container.</param>
		/// <return> 小地图上的元素 </return>
		private Transform DrawMiniMapInstance(Transform instanceModel,Vector3 localPosition,Transform container){
			
			Transform miniMapInstance = miniMapInstancePool.GetInstanceWithName<Transform>(instanceModel.name, instanceModel.gameObject, container);

			miniMapInstance.localPosition = localPosition;

			miniMapInstance.localRotation = Quaternion.identity;

			miniMapInstance.localScale = Vector3.one;

			return miniMapInstance;

		}
              

		/// <summary>
		/// 根据图层名获取用于绘制地图的图块模型
		/// </summary>
		/// <returns>The map tile model for layer.</returns>
		/// <param name="layerName">Layer name.</param>
		private void PrepareForMapLayer(string layerName, out Transform mapTileModel, out Transform mapTileContainer, out InstancePool mapTilePool)
			{

				mapTileModel = null;
				mapTileContainer = null;
				mapTilePool = null;

				switch (layerName)
				{
					case "FloorLayer":
						mapTileModel = floorModel;
						mapTileContainer = floorsContainer;
						mapTilePool = floorsPool;
						break;
					case "WallLayer":
						mapTileModel = wallModel;
						mapTileContainer = wallsContainer;
						mapTilePool = wallsPool;
						break;
					case "DecorationLayer":
						mapTileModel = decorationModel;
						mapTileContainer = decorationsContainer;
						mapTilePool = decorationsPool;
						break;
				}
			}


			public void PlayDestinationAnim(Vector3 targetPos, bool arrivable)
			{

		    	walkTint.transform.position = targetPos;
         
    			if(arrivable){
    				walkTint.animation.Play("move_able", 1);
    			}else{
    				walkTint.animation.Play("move_disable", 1);
    			}

    			
			} 

    		private IEnumerator WaitWalkTintEndAndReset(){

    			yield return new WaitUntil(()=>walkTint.animation.isCompleted);

    			walkTint.animation.Stop();
                walkTint.transform.position = -Vector3.one;

    		}

			public void SetUpRewardInMap(Item reward, Vector3 rewardPosition)
			{

				RewardInMap rewardInMap = rewardPool.GetInstance<RewardInMap>(rewardModel.gameObject, rewardContainer);

			    rewardInMap.SetUpRewardInMap(reward, rewardPosition, rewardPool);

			}

			/// <summary>
			/// 获取指向下层入口的方向向量
			/// </summary>
			/// <returns>The direction vector towards next level exit.</returns>
			public Vector3 GetDirectionVectorTowardsNextLevelExit()
			{

				Transform playerTrans = ExploreManager.Instance.battlePlayerCtr.transform;

				return new Vector3(exitPos.x - playerTrans.position.x, exitPos.y - playerTrans.position.y, 0);

			}

			/// <summary>
			/// 获取指向上层入口的方向向量
			/// </summary>
			/// <returns>The direction vector towards last level exit.</returns>
			public Vector3 GetDirectionVectorTowardsLastLevelExit()
			{

				Transform playerTrans = ExploreManager.Instance.battlePlayerCtr.transform;

				return new Vector3(returnExitPos.x - playerTrans.position.x, returnExitPos.y - playerTrans.position.y, 0);
			}



			public EffectAnim GetEffectAnim(string effectName, Transform effectContainer)
			{


    			EffectAnim effectAnim = null;

    			for (int i = 0; i < effectContainer.childCount;i++){

    				EffectAnim tempEa = effectContainer.GetChild(i).GetComponent<EffectAnim>();

    				if(tempEa.effectName == effectName){
    					effectAnim = tempEa;
					    break;
    				}

    			}

			    if(effectAnim != null && effectAnim.playTime == 0){
    				return effectAnim;
    			}

				effectAnim = effectAnimPool.GetInstanceWithName<EffectAnim>(effectName);

				if (effectAnim == null)
				{

					EffectAnim effectModel = GameManager.Instance.gameDataCenter.allEffects.Find(delegate (EffectAnim obj)
					{
						return obj.effectName == effectName;
					});

					if (effectModel == null)
					{
						return null;
					}

					effectAnim = Instantiate(effectModel.gameObject).GetComponent<EffectAnim>();
					effectAnim.name = effectModel.effectName;
				}

				effectAnim.transform.SetParent(effectContainer, false);

				//Vector3 newPos = effectContainer.position + effectAnim.localPos;

				//Debug.LogFormat("effectAnim pos:{0},container Pos:{1}", effectAnim.localPos, effectContainer.position);

				effectAnim.transform.localPosition = new Vector3(effectAnim.localPos.x, effectAnim.localPos.y, 0);
				effectAnim.transform.localRotation = Quaternion.identity;
				//effectAnim.transform.localScale = Vector3.one;
				effectAnim.gameObject.SetActive(true);

				return effectAnim;
			}

			public void AddEffectAnimToPool(EffectAnim ea)
			{
				ea.gameObject.SetActive(false);
				effectAnimPool.AddInstanceToPool(ea.gameObject);
			}



			/// <summary>
			/// 驱散部分怪物（怪物放入缓存池中）
			/// </summary>
			/// <param name="percentage">Percentage.</param>
			public void SomeMonstersToPool(float percentage)
			{

				int monsterToPoolCount = (int)(allMonstersInMap.Count * percentage);

				for (int i = 0; i < monsterToPoolCount; i++)
				{

					int randomSeed = Random.Range(0, allMonstersInMap.Count);

				    MapMonster mapMonster = allMonstersInMap[randomSeed];

					mapMonster.AddToPool(monstersPool);

				    allMonstersInMap.RemoveAt(randomSeed);

				}

			}

			/// <summary>
			/// 使地图上所有陷阱失效
			/// </summary>
			public void AllTrapsOff()
			{

				for (int i = 0; i < allTrapsInMap.Count; i++)
				{

					Trap trap = allTrapsInMap[i];

					trap.SetTrapOff();

				}

			}

			/// <summary>
			/// 将场景中的地板，npc，地图物品，怪物加入缓存池中
			/// </summary>
			private void AllMapInstancesToPool()
			{

				floorsPool.AddChildInstancesToPool(floorsContainer);

				wallsPool.AddChildInstancesToPool(wallsContainer);

				decorationsPool.AddChildInstancesToPool(decorationsContainer);

				AllMapEventsToPool();

				rewardPool.AddChildInstancesToPool(rewardContainer);

				while (characterFragmentContainer.childCount > 0)
				{
					characterFragmentContainer.GetChild(0).GetComponent<CharacterFragment>().AddToPool(characterFragmentPool);
				}
				//characterFragmentPool.AddChildInstancesToPool(characterFragmentContainer);

			}
        
        

		private void AllMiniMapInstancesToPool(){

			//miniMapInstancePool.AddChildInstancesToPool(miniMapWallContainer);         
			//miniMapInstancePool.AddChildInstancesToPool(miniMapDoorContainer);         
			//miniMapInstancePool.AddChildInstancesToPool(miniMapKeyDoorContainer);         
			//miniMapInstancePool.AddChildInstancesToPool(miniMapLadderContainer);         
			//miniMapInstancePool.AddChildInstancesToPool(miniMapHoleContainer);         
			//miniMapInstancePool.AddChildInstancesToPool(miniMapExitContainer);         
			//miniMapInstancePool.AddChildInstancesToPool(miniMapReturnExitModel);
			//miniMapInstancePool.AddChildInstancesToPool(miniMapPotContainer);
			//miniMapInstancePool.AddChildInstancesToPool(miniMapBucketContainer);
			//miniMapInstancePool.AddChildInstancesToPool(miniMapTreasureBoxContainer);
			//miniMapInstancePool.AddChildInstancesToPool(miniMapCrystalContainer);
			//miniMapInstancePool.AddChildInstancesToPool(miniMapNpcContainer);
			//miniMapInstancePool.AddChildInstancesToPool(miniMapGoldPackContainer);
			//miniMapInstancePool.AddChildInstancesToPool(miniMapItemContainer);

			miniMapInstancePool.AddChildInstancesToPool(miniMapInstanceContainer);

			miniMapMaskPool.AddChildInstancesToPool(miniMapMaskContainer);

		}

		private void DestroyUnusedMapInstances(){
			floorsPool.ClearInstancePool();
			wallsPool.ClearInstancePool();
			decorationsPool.ClearInstancePool();
			miniMapInstancePool.ClearInstancePool();
			miniMapMaskPool.ClearInstancePool();
			rewardPool.ClearInstancePool();         
			effectAnimPool.ClearInstancePool();         
			characterFragmentPool.ClearInstancePool();
			monstersPool.ClearInstancePool();
            npcsPool.ClearInstancePool();
            mapEventsPool.ClearInstancePool();
		}

			private void AllMapEventsToPool()
			{
				while (mapEventsContainer.childCount > 0)
				{
					MapEvent me = mapEventsContainer.GetChild(0).GetComponent<MapEvent>();
					me.AddToPool(mapEventsPool);
				}
				AllMonstersToPool();
				AllNpcsToPool();

			}

			private void AllMonstersToPool()
			{
				while (monstersContainer.childCount > 0)
				{
					MapMonster mm = monstersContainer.GetChild(0).GetComponent<MapMonster>();
					mm.AddToPool(monstersPool);
				}
			}

			private void AllNpcsToPool()
			{
				while (npcsContainer.childCount > 0)
				{
					MapNPC mn = npcsContainer.GetChild(0).GetComponent<MapNPC>();
					mn.AddToPool(npcsPool);
				}
			}

	}
}
