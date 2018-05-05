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
		private MapData mapData;

		[HideInInspector]public int rows;
		[HideInInspector]public int columns;

		// 地图可行走信息数组
		public int[,] mapWalkableInfoArray;

		// 地图上怪物和npc位置信息数组(有怪物的位置或者行走中的怪物的行走终点为1，没有怪物的位置或者行走中的怪物的行走原点为0)
		public int[,] mapWalkableEventInfoArray;


		//************* 模型 *************//
		public Transform floorModel;//地板模型

		public Transform wallModel;// 墙体模型

		public Transform decorationModel;// 装饰模型

		public PickableItem pickableItemModel;//可拾取物品模型

		public Obstacle stoneModel;//石头模型

		public ThornTrap thornTrapModel;// 陷阱模型

		public ThornTrapSwitch thornTrapSwitchModel;// 开关模型

		public TreasureBox treasureBoxModel;// 宝箱模型

		public Treasure bucketModel;// 木桶模型

		public Treasure potModel;// 瓦罐模型

		public Exit exitModel;// 出口模型

		public Door doorModel;// 门模型

		public Billboard billboardModel;//告示牌模型

		public FireTrap fireTrapModel;//火焰陷阱模型

		public SecretStairs secrectStairsModel;// 密室楼梯模型

		public MovableBox movableBoxModel;// 可移动箱子模型

		public PressSwitch pressSwitchModel;// 压力开关模型

		public Block blockModel;// 装饰用障碍物模型

		public GoldPack goldPackModel;//钱袋模型

		public Crystal crystalModel;//水晶模型

		public CurePoint curePointModel;//回血点模型

		public Transform rewardModel;//奖励物品模型


		//************ 容器 **************//
		public Transform floorsContainer;//地板容器

		public Transform wallsContainer;//墙体容器

		public Transform decorationsContainer;//装饰容器

		public Transform mapEventsContainer;//事件容器

		public Transform monstersContainer;//怪物容器

		public Transform npcsContainer;//NPC容器

		public Transform rewardContainer;//奖励物品容器

//		public Transform effectAnimContainer;//


		//************ 缓存池 ************//
		public InstancePool floorsPool;//地板缓存池

		public InstancePool wallsPool;//墙体缓存池

		public InstancePool decorationsPool;//装饰缓存池

		public InstancePool mapEventsPool;//地图事件缓存池

		public InstancePool monstersPool;//怪物缓存池

		public InstancePool npcsPool;//npc缓存池

		public InstancePool rewardPool;//奖励物品缓存池

		public InstancePool effectAnimPool;//技能效果缓存池


		// 行走提示动画
		public Transform destinationAnimation;

		// 数据库处理助手
		private MySQLiteHelper mySql;

		// 记录玩家所有可能初始位置的列表
		private List<Vector2> playerStartPosList = new List<Vector2> ();

		private List<TriggeredGear> allTriggeredMapEvents = new List<TriggeredGear> ();
		public List<MapWalkableEvent> allWalkableEventsInMap = new List<MapWalkableEvent> ();


		public Transform fogOfWarPlane;

		// 本层出口位置
		private Vector2 exitPos;


//		void Update(){
//			Material m = fogOfWarPlane.GetComponent<Renderer>().material;
//			Texture t = Resources.Load("FOWTexture") as Texture;
//			m.SetTexture ("_MaskTex", t);
//		}

		public void UpdateFogOfWar(){
			Material m = fogOfWarPlane.GetComponent<Renderer>().material;
			Texture t = Resources.Load("FOWTexture") as Texture;
			m.SetTexture ("_MaskTex", t);
		}


		public void SetUpMap()
		{


			Reset ();

			// 绘制地图
			DrawMap ();

			mySql = MySQLiteHelper.Instance;
			mySql.GetConnectionWith (CommonData.dataBaseName);

			// 初始化地图事件
			InitializeMapEvents ();

			mySql.CloseConnection (CommonData.dataBaseName);

			InitializePlayerAndSetCamera ();

			DestroyUnusedMonsterAndNpc ();

			#if UNITY_EDITOR || UNITY_IOS 
			fogOfWarPlane.gameObject.SetActive(true);
			InitFogOfWarPlane ();
			#elif UNITY_ANDROID
			fogOfWarPlane.gameObject.SetActive(false);
			#endif

		}

		private void DestroyUnusedMonsterAndNpc(){
			monstersPool.ClearInstancePool ();
			npcsPool.ClearInstancePool ();
			mapEventsPool.ClearInstancePool ();
		}

		/// <summary>
		/// 重置地图初始化工具
		/// </summary>
		private void Reset(){
			
			allTriggeredMapEvents.Clear ();

			allWalkableEventsInMap.Clear ();

			playerStartPosList.Clear ();

            int currentLevelGrade = Player.mainPlayer.currentLevelIndex / 10;
            int randomMapIndex = currentLevelGrade * 10 + Random.Range(0, 10);
            mapData = MapData.GetMapDataOfLevel (randomMapIndex);

			// 获取地图建模的行数和列数
			rows = mapData.rowCount;
			columns = mapData.columnCount;

			mapWalkableInfoArray = new int[columns, rows];
			mapWalkableEventInfoArray = new int[columns, rows];
			InitMapWalkableInfoAndMonsterPosInfo ();

			AllMapInstancesToPool ();
		}

		/// <summary>
		/// 初始化地图迷雾
		/// </summary>
		private void InitFogOfWarPlane(){

			float fowPlaneScaler = (float)Mathf.Max (rows, columns) / 10 + 0.1f;

			fogOfWarPlane.position = new Vector3 ((float)columns / 2, (float)rows / 2, -5);
			fogOfWarPlane.localRotation = Quaternion.Euler (new Vector3 (-90, 0, 0));
			fogOfWarPlane.localScale = new Vector3 (fowPlaneScaler, 1, fowPlaneScaler);

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
			playerStartPosList.Clear ();

			allTriggeredMapEvents.Clear ();
		}

		/// <summary>
		/// 绘制所有地图元素
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

			// 获取当前地图中要使用的单词
			HLHWord[] words = InitLearnWordsInMap (mapEventCount);

			// 生成一个记录可使用单词序号的列表
			List<int> unusedWordIndexRecordList = new List<int> ();

			// 初始化可使用单词喜好列表
			for (int i = 0; i < mapEventCount; i++) {
				unusedWordIndexRecordList.Add (i);
			}

			for (int i = 0; i < mapData.attachedInfoLayers.Count; i++) {

				MapAttachedInfoLayer layer = mapData.attachedInfoLayers [i];

				InitializeMapEventsOfLayer (layer,words,unusedWordIndexRecordList);

			}

		}

		private HLHWord[] InitLearnWordsInMap(int mapEventCount){

			HLHWord[] words = new HLHWord[mapEventCount];

			string currentWordsTableName = LearningInfo.Instance.GetCurrentLearningWordsTabelName();

			string query = string.Format ("SELECT learnedTimes FROM {0} ORDER BY learnedTimes ASC", currentWordsTableName);

			IDataReader reader = mySql.ExecuteQuery (query);

			reader.Read ();

			int wholeLearnTime = reader.GetInt16 (0);

			query = string.Format ("SELECT COUNT(*) FROM {0} WHERE learnedTimes={1}", currentWordsTableName, wholeLearnTime);

			reader = mySql.ExecuteQuery (query);

			reader.Read ();

			int validWordCount = reader.GetInt32 (0);

			if (validWordCount < mapEventCount) {

				string[] colFields = new string[]{ "learnedTimes" };
				string[] values = new string[]{ (wholeLearnTime + 1).ToString() };
				string[] conditions = new string[]{"learnedTimes=" + wholeLearnTime.ToString()};

				mySql.UpdateValues (currentWordsTableName, colFields, values, conditions, true);

				wholeLearnTime++;

			}

			// 边界条件
			string[] condition = new string[]{ string.Format("learnedTimes={0} ORDER BY RANDOM() LIMIT {1}",wholeLearnTime,mapEventCount) };

			reader = mySql.ReadSpecificRowsOfTable (currentWordsTableName, null, condition, true);

			int index = 0;

			while(reader.Read()){

				int wordId = reader.GetInt32 (0);

				string spell = reader.GetString (1);

				string phoneticSymble = reader.GetString (2);

				string explaination = reader.GetString (3);

				string sentenceEN = reader.GetString (4);

				string sentenceCH = reader.GetString (5);

				string pronounciationURL = reader.GetString (6);

				int wordLength = reader.GetInt16 (7);

				int learnedTimes = reader.GetInt16 (8);

				int ungraspTimes = reader.GetInt16 (9);

				HLHWord word = new HLHWord (wordId, spell, phoneticSymble, explaination, sentenceEN, sentenceCH, pronounciationURL, wordLength, learnedTimes, ungraspTimes);

				words [index] = word;

				index++;

			}

			return words;
		}
			

		private void InitializeMapEventsOfLayer(MapAttachedInfoLayer layer,HLHWord[] words,List<int> unusedWordIndexList){

            bool exitOpen = true;
            Exit exit = null;

			for (int i = 0; i < layer.tileDatas.Count; i++) {

				MapAttachedInfoTile eventTile = layer.tileDatas [i];

				int posX = Mathf.RoundToInt (eventTile.position.x);
				int posY = Mathf.RoundToInt (eventTile.position.y);

				MapEvent mapEvent = null;

                switch (eventTile.type)
                {
                    case "playerStart":
                        playerStartPosList.Add(new Vector2(posX, posY));
                        break;
                    case "exit":
                        mapEvent = mapEventsPool.GetInstanceWithName<Exit>(exitModel.name, exitModel.gameObject, mapEventsContainer);
                        mapWalkableInfoArray[posX, posY] = 5;
                        exitPos = new Vector2(posX, posY);
                        allTriggeredMapEvents.Add(mapEvent as TriggeredGear);
                        exit = mapEvent as Exit;
                        break;
                    case "billboard":
                        mapEvent = mapEventsPool.GetInstanceWithName<Billboard>(billboardModel.name, billboardModel.gameObject, mapEventsContainer);
                        mapWalkableInfoArray[posX, posY] = 0;
                        break;
                    case "item":
                        mapEvent = mapEventsPool.GetInstanceWithName<PickableItem>(pickableItemModel.name, pickableItemModel.gameObject, mapEventsContainer);
                        mapWalkableInfoArray[posX, posY] = 1;
                        break;
                    case "block":
                        mapEvent = mapEventsPool.GetInstanceWithName<Block>(blockModel.name, blockModel.gameObject, mapEventsContainer);
                        mapWalkableInfoArray[posX, posY] = 0;
                        break;
                    case "box":
                        mapEvent = mapEventsPool.GetInstanceWithName<MovableBox>(movableBoxModel.name, movableBoxModel.gameObject, mapEventsContainer);
                        mapWalkableInfoArray[posX, posY] = 1;
                        break;
                    case "monster":
                        mapEvent = GetMonster(eventTile);
                        if (mapEvent != null)
                        {
                            (mapEvent as MapMonster).SetPosTransferSeed(rows);
                            mapWalkableInfoArray[posX, posY] = 5;
                            mapWalkableEventInfoArray[posX, posY] = 1;
                            allWalkableEventsInMap.Add(mapEvent as MapMonster);
                        }
                        break;
                    case "boss":
                        mapEvent = GetBoss(eventTile);
                        if (mapEvent != null)
                        {
                            (mapEvent as MapMonster).SetPosTransferSeed(rows);
                            mapWalkableInfoArray[posX, posY] = 5;
                            mapWalkableEventInfoArray[posX, posY] = 1;
                            allWalkableEventsInMap.Add(mapEvent as MapMonster);
                            exitOpen = false;
                        }
                        break;
                    case "goldPack":
                        mapEvent = mapEventsPool.GetInstanceWithName<GoldPack>(goldPackModel.name, goldPackModel.gameObject, mapEventsContainer);
                        mapWalkableInfoArray[posX, posY] = 0;
                        break;
                    case "bucket":
                        mapEvent = mapEventsPool.GetInstanceWithName<Treasure>(bucketModel.name, bucketModel.gameObject, mapEventsContainer);
                        mapWalkableInfoArray[posX, posY] = 0;
                        (mapEvent as Treasure).treasureType = TreasureType.Bucket;
                        break;
                    case "pot":
                        mapEvent = mapEventsPool.GetInstanceWithName<Treasure>(potModel.name, potModel.gameObject, mapEventsContainer);
                        mapWalkableInfoArray[posX, posY] = 0;
                        (mapEvent as Treasure).treasureType = TreasureType.Pot;
                        break;
                    case "treasure":
                        mapEvent = mapEventsPool.GetInstanceWithName<TreasureBox>(treasureBoxModel.name, treasureBoxModel.gameObject, mapEventsContainer);
                        mapWalkableInfoArray[posX, posY] = 0;
                        (mapEvent as Treasure).treasureType = TreasureType.TreasuerBox;
                        break;
                    case "stone":
                        mapEvent = mapEventsPool.GetInstanceWithName<Obstacle>(stoneModel.name, stoneModel.gameObject, mapEventsContainer);
                        mapWalkableInfoArray[posX, posY] = 0;
                        break;
                    case "crystal":
                        mapEvent = mapEventsPool.GetInstanceWithName<Crystal>(crystalModel.name, crystalModel.gameObject, mapEventsContainer);
                        mapWalkableInfoArray[posX, posY] = 0;
                        break;
                    case "curePoint":
                        mapEvent = mapEventsPool.GetInstanceWithName<CurePoint>(curePointModel.name, curePointModel.gameObject, mapEventsContainer);
                        mapWalkableInfoArray[posX, posY] = 0;
                        break;
                    case "doorGear":
                        mapEvent = mapEventsPool.GetInstanceWithName<Door>(doorModel.name, doorModel.gameObject, mapEventsContainer);
                        (mapEvent as Door).SetPosTransferSeed(rows);
                        mapWalkableInfoArray[posX, posY] = 5;
                        allTriggeredMapEvents.Add(mapEvent as TriggeredGear);
                        break;
                    case "thornSwitch":
                        mapEvent = mapEventsPool.GetInstanceWithName<ThornTrapSwitch>(thornTrapSwitchModel.name, thornTrapSwitchModel.gameObject, mapEventsContainer);
                        (mapEvent as ThornTrapSwitch).SetPosTransferSeed(rows);
                        mapWalkableInfoArray[posX, posY] = 0;
                        allTriggeredMapEvents.Add(mapEvent as TriggeredGear);
                        break;
                    case "thornTrap":
                        mapEvent = mapEventsPool.GetInstanceWithName<ThornTrap>(thornTrapModel.name, thornTrapModel.gameObject, mapEventsContainer);
                        mapWalkableInfoArray[posX, posY] = 0;
                        allTriggeredMapEvents.Add(mapEvent as TriggeredGear);
                        break;
                    case "pressSwitch":
                        mapEvent = mapEventsPool.GetInstanceWithName<PressSwitch>(pressSwitchModel.name, pressSwitchModel.gameObject, mapEventsContainer);
                        (mapEvent as PressSwitch).SetPosTransferSeed(rows);
                        mapWalkableInfoArray[posX, posY] = 5;
                        allTriggeredMapEvents.Add(mapEvent as TriggeredGear);
                        break;
                    case "downStair":
                        mapEvent = mapEventsPool.GetInstanceWithName<SecretStairs>(secrectStairsModel.name, secrectStairsModel.gameObject, mapEventsContainer);
                        (mapEvent as SecretStairs).SetPosTransferSeed(rows);
                        mapWalkableInfoArray[posX, posY] = 5;
                        break;
                    case "upStair":
                        mapEvent = mapEventsPool.GetInstanceWithName<SecretStairs>(secrectStairsModel.name, secrectStairsModel.gameObject, mapEventsContainer);
                        (mapEvent as SecretStairs).SetPosTransferSeed(rows);
                        mapWalkableInfoArray[posX, posY] = 5;
                        break;
                    case "fireTrap":
                        mapEvent = mapEventsPool.GetInstanceWithName<FireTrap>(fireTrapModel.name, fireTrapModel.gameObject, mapEventsContainer);
                        mapWalkableInfoArray[posX, posY] = 1;
                        break;
                    case "npc":
                        mapEvent = GetMapNPC(eventTile);
                        if (mapEvent != null){
                            mapWalkableInfoArray[posX, posY] = 5;
                            mapWalkableEventInfoArray[posX, posY] = 1;
                            allWalkableEventsInMap.Add(mapEvent as MapNPC);
                        }
					    break;
				    case "tombstone":

					    break;
				    case "statue":

					    break;
				    case "wishpool":

					    break;
                            
				}

				if (mapEvent != null) {

					// 取3个不同的单词（首项做为目标单词，其余两项做为混淆单词）
					HLHWord[] wordsArray = GetDifferentWords (words, unusedWordIndexList, 3);

					mapEvent.wordsArray = wordsArray;
					
					mapEvent.InitializeWithAttachedInfo (eventTile);

				}

			}

            if(!exitOpen){
                exit.isOpen = false;
            }

		}

		public void ChangeMapEventStatusAtPosition(Vector3 position){
			
			for (int i = 0; i < allTriggeredMapEvents.Count; i++) {

				TriggeredGear tg = allTriggeredMapEvents [i];

				if(!MyTool.ApproximatelySamePosition2D(tg.transform.position,position)){
					continue;
				}

				tg.ChangeStatus ();

			}
		}

		public void ChangeAllThornTrapsInMap(){
			
			for (int i = 0; i < allTriggeredMapEvents.Count; i++) {
				
				TriggeredGear tg = allTriggeredMapEvents [i];

				if (tg is ThornTrap) {
					tg.ChangeStatus ();
				}

			}
		}

		/// <summary>
		/// 选择指定数量的不重复的随机单词
		/// </summary>
		/// <returns>The different words.</returns>
		/// <param name="words">Words.</param>
		/// <param name="unusedWordRecordList">Unused word record list.</param>
		/// <param name="count">Count.</param>
		private HLHWord[] GetDifferentWords(HLHWord[] words,List<int> unusedWordRecordList,int count){

			HLHWord[] wordsArray = new HLHWord [count];

			// 记录已选过的单词在words数组中的位置信息
			List<int> tempList = new List<int> ();


			// 从未使用过的单词列表中随机1个单词做为目标单词
			int targetWordIndexInUnusedWordList = Random.Range (0, unusedWordRecordList.Count);

			int targetWordIndexInWords = unusedWordRecordList [targetWordIndexInUnusedWordList];

			unusedWordRecordList.RemoveAt (targetWordIndexInUnusedWordList);

			wordsArray[0] = words [targetWordIndexInWords];

			tempList.Add (targetWordIndexInWords);

			// 已选出的不相同的随机混淆单词的数量
			int tempCounter = 1;

			// 从整个单词列表中随机两个不同的混淆单词（最终三个单词互不相同）
			while (tempCounter < count) {
				
				int index = Random.Range (0, words.Length);

				if (!tempList.Contains (index)) {

					tempList.Add (index);

					wordsArray [tempCounter] = words [index];

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
		private MapEvent GetMonster(MapAttachedInfoTile info){

			HLHGameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas [Player.mainPlayer.currentLevelIndex];

			int randomSeed = Random.Range (0, levelData.monsterIds.Count);

			int monsterId = levelData.monsterIds [randomSeed];

			levelData.monsterIds.RemoveAt (randomSeed);

			Transform monster = null;

			string monsterName = MyTool.GetMonsterName(monsterId);

            if(monsterName.Equals(string.Empty)){
                return null;
            }

			for (int i = 0; i < monstersPool.transform.childCount; i++) {
				Transform monsterInPool = monstersPool.transform.GetChild (i);
				if(monsterInPool.name == monsterName){
					monster = monsterInPool;
					break;
				}
			}

			if (monster == null) {
				monster = GameManager.Instance.gameDataCenter.LoadMonster (monsterName).transform;
			}

			monster.SetParent (monstersContainer);
			monster.localScale = Vector3.one;
			monster.localRotation = Quaternion.identity;

			return monster.GetComponent<MapMonster> ();
		}

		/// <summary>
		/// 按照id获取怪物
		/// </summary>
		/// <returns>The monster.</returns>
		/// <param name="info">Info.</param>
		private MapEvent GetBoss(MapAttachedInfoTile info){

			HLHGameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas [Player.mainPlayer.currentLevelIndex];

            if(!HLHGameLevelData.IsBossLevel()){
                return null;
            }

			int bossId = levelData.bossId;

			Transform boss = null;

			string bossName = MyTool.GetMonsterName(bossId);

            if (bossName.Equals(string.Empty))
            {
                return null;
            }

			boss = GameManager.Instance.gameDataCenter.LoadMonster (bossName).transform;

			boss.SetParent (monstersContainer);
			boss.localScale = Vector3.one;
			boss.localRotation = Quaternion.identity;

			return boss.GetComponent<MapMonster> ();
		}


		private MapEvent GetMapNPC(MapAttachedInfoTile info){
			
			int npcId = int.Parse (KVPair.GetPropertyStringWithKey ("npcID", info.properties));

            if(npcId == 0 && !HLHGameLevelData.HasWiseMan()){
                return null;
            }

			if (npcId == -1) {
				npcId = Random.Range (2,57);
			}

			string npcName = MyTool.GetNpcName (npcId);

			Transform mapNpcTrans = null;

			for (int i = 0; i < npcsPool.transform.childCount; i++) {
				Transform npcInPool = npcsPool.transform.GetChild (i);
				if(npcInPool.name == npcName){
					mapNpcTrans = npcInPool;
					break;
				}
			}

			if (mapNpcTrans == null) {
				mapNpcTrans = GameManager.Instance.gameDataCenter.LoadMapNpc (npcName).transform;
			}

			mapNpcTrans.SetParent (npcsContainer);
			mapNpcTrans.localScale = Vector3.one;
			mapNpcTrans.localRotation = Quaternion.identity;

			MapNPC mapNpc= mapNpcTrans.GetComponent<MapNPC> ();

			mapNpc.SetNpcId(npcId);

			return mapNpc;
		}





		public void AddNewPickableItem(Vector3 position,Item attachedItem){

			Transform pickableItem = Instantiate (pickableItemModel.gameObject, mapEventsContainer).transform;

			pickableItem.GetComponent<PickableItem> ().InitializeWithItemAndPosition (position, attachedItem);

		}



		/// <summary>
		/// 将人物放置在人物初始点
		/// </summary>
		/// <param name="position">Position.</param>
		private void InitializePlayerAndSetCamera(){

			int randomIndex = Random.Range (0, playerStartPosList.Count);

			Vector3 position = playerStartPosList [randomIndex];
			
			Transform player = Player.mainPlayer.transform.Find ("BattlePlayer");
			player.position = position;

			BattlePlayerController bp = player.GetComponent<BattlePlayerController> ();
			bp.singleMoveEndPos = position;

			bp.pathPosList.Clear ();
			bp.ActiveBattlePlayer (true, true, true);
			bp.SetSortingOrder (-Mathf.RoundToInt(position.y));

			bp.TowardsDown ();
			bp.StopMoveAndWait ();
			bp.isInFight = false;

			Camera mainCamera = Camera.main;
			mainCamera.transform.SetParent (player,false);
			mainCamera.transform.localPosition = new Vector3 (0, 0, -10);
			mainCamera.transform.localScale = Vector3.one;
			mainCamera.transform.localRotation = Quaternion.identity;

			#if UNITY_EDITOR || UNITY_IOS
			bp.transform.Find ("FowBrush").gameObject.SetActive (true);
			// 初始化迷雾相机的视窗大小
			Camera fowCamera = TransformManager.FindTransform ("FogOfWarBrushCamera").GetComponent<Camera> ();
			fowCamera.gameObject.SetActive(true);
			fowCamera.orthographicSize = (float)Mathf.Max (rows, columns) / 2;
			fowCamera.transform.position = new Vector3 (columns / 2, rows / 2, -5);
			ExploreManager.Instance.expUICtr.GetComponent<BattlePlayerUIController> ().RefreshMiniMap ();
			UpdateFogOfWar ();
			#elif UNITY_ANDROID 
			bp.transform.Find ("FowBrush").gameObject.SetActive (false);
			Transform fowCamera = TransformManager.FindTransform ("FogOfWarBrushCamera");
			if(fowCamera != null){
				fowCamera.gameObject.SetActive(false);
			}

			#endif

			Transform exploreMask = mainCamera.transform.Find ("ExploreMask");
			exploreMask.gameObject.SetActive (true);
			exploreMask.GetComponent<UnityArmatureComponent> ().animation.Play (CommonData.exploreMaskAnimName, 0);


		}


		/// <summary>
		/// 绘制单个地图层
		/// </summary>
		/// <param name="layer">Layer.</param>
		private void DrawMapLayer(MapLayer layer){

			string tileSetsImageName = mapData.mapTilesImageName;

			List<Sprite> allMapSprites = GameManager.Instance.gameDataCenter.allMapSprites;

			Transform mapTileModel;
			Transform mapTilesContainer;
			InstancePool mapTilesPool;

			PrepareForMapLayer (layer.layerName,out mapTileModel,out mapTilesContainer,out mapTilesPool);

			for (int i = 0; i < layer.tileDatas.Count; i++) {

				MapTile tile = layer.tileDatas [i];

				string tileImageName = string.Format ("{0}_{1}", tileSetsImageName, tile.tileIndex);

				Sprite tileSprite = allMapSprites.Find (delegate(Sprite obj) {
					return obj.name.Equals (tileImageName);
				});

				Transform mapTile = mapTilesPool.GetInstance<Transform> (mapTileModel.gameObject, mapTilesContainer);

				SpriteRenderer sr = mapTile.GetComponent<SpriteRenderer> ();

				sr.sprite = tileSprite;


				mapTile.position = tile.position;

				int posX = Mathf.RoundToInt (tile.position.x);
				int posY = Mathf.RoundToInt (tile.position.y);

				if (layer.layerName.Equals ("DecorationLayer")) {
					sr.sortingOrder = -posY;
				}


				if (mapWalkableInfoArray [posX, posY] == -2
					|| mapWalkableInfoArray [posX, posY] == 1) {
					mapWalkableInfoArray [posX, posY] = tile.canWalk ? 1 : -1;
				} 

			}

		}


		/// <summary>
		/// 根据图层名获取用于绘制地图的图块模型
		/// </summary>
		/// <returns>The map tile model for layer.</returns>
		/// <param name="layerName">Layer name.</param>
		private void PrepareForMapLayer(string layerName,out Transform mapTileModel,out Transform mapTileContainer,out InstancePool mapTilePool){

			mapTileModel = null;
			mapTileContainer = null;
			mapTilePool = null;

			switch (layerName) {
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


		public void PlayDestinationAnim(Vector3 targetPos,bool arrivable){

			destinationAnimation.position = targetPos;

			Animator destinationAnimator = destinationAnimation.GetComponent<Animator> ();

			destinationAnimator.SetTrigger ("Empty");

			if (arrivable) {
				destinationAnimator.SetTrigger ("PlayArrivable");
			} else {
				destinationAnimator.SetTrigger ("PlayUnarrivable");
			}

		}

		private void ResetDestinationAnim(){
			StopCoroutine ("LatelyPlayDestinationTintAnim");
			Animator destinationAnimator = destinationAnimation.GetComponent<Animator> ();
			destinationAnimator.ResetTrigger ("PlayArrivable");
			destinationAnimator.ResetTrigger ("PlayUnarrivable");
			destinationAnimator.SetTrigger ("Empty");
		}


		public void SetUpRewardInMap(Item reward, Vector3 rewardPosition){

			RewardInMap rewardInMap = rewardPool.GetInstance<RewardInMap> (rewardModel.gameObject, rewardContainer);

			rewardInMap.SetUpRewardInMap (reward, rewardPosition);

			rewardInMap.RewardFlyToPlayer (delegate{
				rewardPool.AddInstanceToPool (rewardInMap.gameObject);
				rewardInMap.gameObject.SetActive (false);
				ExploreManager.Instance.expUICtr.UpdateBottomBar();
			});

		}


		public Vector3 GetDirectionVectorTowardsExit(){

			Transform playerTrans = ExploreManager.Instance.battlePlayerCtr.transform;

			return new Vector3 (exitPos.x - playerTrans.position.x, exitPos.y - playerTrans.position.y,0); 

		}



		public EffectAnim GetEffectAnim(string effectName,Transform effectContainer){

			EffectAnim effectAnim = effectAnimPool.GetInstanceWithName<EffectAnim> (effectName);

			if (effectAnim == null) {

				EffectAnim effectModel = GameManager.Instance.gameDataCenter.allEffects.Find (delegate(EffectAnim obj) {
					return obj.effectName == effectName;
				});

				if (effectModel == null) {
					return null;
				}

				effectAnim = Instantiate (effectModel.gameObject).GetComponent<EffectAnim>();
				effectAnim.name = effectModel.effectName;
			}

			effectAnim.transform.SetParent (effectContainer);

//			Vector3 newPos = effectContainer.position + effectAnim.localPos;

			effectAnim.transform.localPosition = new Vector3 (effectAnim.localPos.x, effectAnim.localPos.y, 0);
			effectAnim.transform.localRotation = Quaternion.identity;
			effectAnim.transform.localScale = Vector3.one;
			effectAnim.gameObject.SetActive (true);

			return effectAnim;
		}

		public void AddEffectAnimToPool(EffectAnim ea){
			ea.gameObject.SetActive (false);
			effectAnimPool.AddInstanceToPool (ea.gameObject);
		}






		/// <summary>
		/// 将场景中的地板，npc，地图物品，怪物加入缓存池中
		/// </summary>
		private void AllMapInstancesToPool(){

			floorsPool.AddChildInstancesToPool (floorsContainer);

			wallsPool.AddChildInstancesToPool (wallsContainer);

			decorationsPool.AddChildInstancesToPool (decorationsContainer);

			AllMapEventsToPool ();

			rewardPool.AddChildInstancesToPool (rewardContainer);

		}

		private void AllMapEventsToPool(){
			while (mapEventsContainer.childCount > 0) {
				MapEvent me = mapEventsContainer.GetChild (0).GetComponent<MapEvent>();
				me.AddToPool (mapEventsPool);
			}
			AllMonstersToPool ();
			AllNpcsToPool ();
		}

		private void AllMonstersToPool(){
			while (monstersContainer.childCount > 0) {
				MapMonster mm = monstersContainer.GetChild (0).GetComponent<MapMonster>();
				mm.AddToPool (monstersPool);
			}
		}

		private void AllNpcsToPool(){
			while (npcsContainer.childCount > 0) {
				MapNPC mn = npcsContainer.GetChild (0).GetComponent<MapNPC>();
				mn.AddToPool (npcsPool);
			}
		}

	
	}
}
