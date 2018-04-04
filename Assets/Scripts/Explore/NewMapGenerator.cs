﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using System.Data;


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

		public Obstacle treeModel;//树模型

		public Obstacle stoneModel;//石头模型

		public ThornTrap thornTrapModel;// 陷阱模型

		public ThornTrapSwitch thornTrapSwitchModel;// 开关模型

		public TreasureBox lockedTreasureBoxModel;// 宝箱模型

		public TreasureBox bucketModel;// 木桶模型

		public TreasureBox potModel;// 瓦罐模型

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

		//************ 缓存池 ************//
		public InstancePool floorsPool;//地板缓存池

		public InstancePool wallsPool;//墙体缓存池

		public InstancePool decorationsPool;//装饰缓存池

		public InstancePool mapEventsPool;//地图事件缓存池

		public InstancePool monstersPool;//怪物缓存池

		public InstancePool npcsPool;//npc缓存池

		public InstancePool rewardPool;//奖励物品缓存池


		private List<Vector2> playerStartPosList = new List<Vector2> ();

		// 行走提示动画
		public Transform destinationAnimation;

		// 数据库处理助手
		private MySQLiteHelper mySql;

//		private HLHGameLevelData currentLevelData;

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

//			currentLevelData = levelData;

			mapData = MapData.GetMapDataOfLevel (Player.mainPlayer.currentLevelIndex);

			// 获取地图建模的行数和列数
			rows = mapData.rowCount;
			columns = mapData.columnCount;

			mapWalkableInfoArray = new int[columns, rows];
			mapWalkableEventInfoArray = new int[columns, rows];
			InitMapWalkableInfoAndMonsterPosInfo ();

			// 绘制地图
			DrawMap ();

			mySql = MySQLiteHelper.Instance;
			mySql.GetConnectionWith (CommonData.dataBaseName);

			// 初始化地图事件
			InitializeMapEvents ();

			InitFogOfWarPlane ();

			mySql.CloseConnection (CommonData.dataBaseName);

			InitializePlayerAndSetCamera ();

		}


		/// <summary>
		/// 初始化地图迷雾
		/// </summary>
		private void InitFogOfWarPlane(){

			float fowPlaneScaler = (float)Mathf.Max (rows, columns) / 10 + 0.1f;

			fogOfWarPlane.position = new Vector3 ((float)columns / 2, (float)rows / 2, -5);
			fogOfWarPlane.localRotation = Quaternion.Euler (new Vector3 (-90, 0, 0));
			fogOfWarPlane.localScale = new Vector3 (fowPlaneScaler, 1, fowPlaneScaler);

			Camera.main.transform.Find ("Mask").gameObject.SetActive (true);

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
			LearnWord[] words = InitLearnWordsInMap (mapEventCount);

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

		private LearnWord[] InitLearnWordsInMap(int mapEventCount){

			LearnWord[] words = new LearnWord[mapEventCount];

			string currentWordsTableName = LearningInfo.Instance.GetCurrentLearningWordsTabelName();

			string query = string.Format ("SELECT learned_times FROM {0} ORDER BY learned_times ASC", currentWordsTableName);

			IDataReader reader = mySql.ExecuteQuery (query);

			reader.Read ();

			int wholeLearnTime = reader.GetInt16 (0);

			query = string.Format ("SELECT COUNT(*) FROM {0} WHERE learned_times={1}", currentWordsTableName, wholeLearnTime);

			reader = mySql.ExecuteQuery (query);

			reader.Read ();

			int validWordCount = reader.GetInt32 (0);

			if (validWordCount < mapEventCount) {

				string[] colFields = new string[]{ "learned_times" };
				string[] values = new string[]{ (wholeLearnTime + 1).ToString() };
				string[] conditions = new string[]{"learned_times=" + wholeLearnTime.ToString()};

				mySql.UpdateValues (currentWordsTableName, colFields, values, conditions, true);

				wholeLearnTime++;

			}

			// 边界条件
			string[] condition = new string[]{ string.Format("learned_times={0} ORDER BY RANDOM() LIMIT {1}",wholeLearnTime,mapEventCount) };

			reader = mySql.ReadSpecificRowsOfTable (currentWordsTableName, null, condition, true);

			int index = 0;

			while(reader.Read()){

				int wordId = reader.GetInt32 (0);

				string spell = reader.GetString (1);

				string phoneticSymble = reader.GetString (2);

				string explaination = reader.GetString (3);

				int learnedTimes = reader.GetInt16 (4);

				int ungraspTimes = reader.GetInt16 (5);

				LearnWord word = new LearnWord (wordId, spell, phoneticSymble, explaination, learnedTimes, ungraspTimes);

				words [index] = word;

				index++;

			}

			return words;
		}
			

		private void InitializeMapEventsOfLayer(MapAttachedInfoLayer layer,LearnWord[] words,List<int> unusedWordIndexList){


			for (int i = 0; i < layer.tileDatas.Count; i++) {

				MapAttachedInfoTile eventTile = layer.tileDatas [i];

				int posX = Mathf.RoundToInt (eventTile.position.x);
				int posY = Mathf.RoundToInt (eventTile.position.y);

				MapEvent mapEvent = null;

				switch (eventTile.type) {
				case "playerStart":
					playerStartPosList.Add (new Vector2 (posX, posY));
					break;
				case "exit":
					mapEvent = mapEventsPool.GetInstanceWithName<Exit> (exitModel.name, exitModel.gameObject, mapEventsContainer);
					mapWalkableInfoArray [posX, posY] = 1;
					exitPos = new Vector2 (posX, posY);
					break;
				case "billboard":
					#warning 这里先用告示牌位置来测试npc
					mapEvent = GetMapNPC (eventTile);
					mapWalkableInfoArray [posX, posY] = 0;
					mapWalkableEventInfoArray [posX, posY] = 1;
					allWalkableEventsInMap.Add (mapEvent as MapNPC);
//					mapEvent = mapEventsPool.GetInstanceWithName<Billboard> (billboardModel.name, billboardModel.gameObject, mapEventsContainer);
//					mapWalkableInfoArray [posX, posY] = 0;
					break;
				case "item":
					mapEvent = mapEventsPool.GetInstanceWithName<PickableItem> (pickableItemModel.name, pickableItemModel.gameObject, mapEventsContainer);
					mapWalkableInfoArray [posX, posY] = 1;
					break;
				case "block":
					mapEvent = mapEventsPool.GetInstanceWithName<Block> (blockModel.name, blockModel.gameObject, mapEventsContainer);
					mapWalkableInfoArray [posX, posY] = 0;
					break;
				case "box":
					mapEvent = mapEventsPool.GetInstanceWithName<MovableBox> (movableBoxModel.name, movableBoxModel.gameObject, mapEventsContainer);
					mapWalkableInfoArray [posX, posY] = 1;
					break;
				case "monster":
//					LearnWord[] wordsArray = GetDifferentWords (words, unusedWordIndexList, 3);
//					InitializeMonster (eventTile,posX,posY,wordsArray);
					mapEvent = GetMonster (eventTile);
					mapWalkableInfoArray [posX, posY] = 2;
					mapWalkableEventInfoArray [posX, posY] = 1;
					allWalkableEventsInMap.Add (mapEvent as MapMonster);
					break;
				case "goldPack":
					mapEvent = mapEventsPool.GetInstanceWithName<GoldPack> (goldPackModel.name, goldPackModel.gameObject, mapEventsContainer);
					mapWalkableInfoArray [posX, posY] = 1;
					break;
				case "bucket":
					mapEvent = mapEventsPool.GetInstanceWithName<TreasureBox> (bucketModel.name, bucketModel.gameObject, mapEventsContainer);
					mapWalkableInfoArray [posX, posY] = 0;
					(mapEvent as TreasureBox).tbType = TreasureBoxType.Bucket;
					break;
				case "pot":
					mapEvent = mapEventsPool.GetInstanceWithName<TreasureBox> (potModel.name, potModel.gameObject, mapEventsContainer);
					mapWalkableInfoArray [posX, posY] = 0;
					(mapEvent as TreasureBox).tbType = TreasureBoxType.Pot;
					break;
				case "treasure":
					mapEvent = mapEventsPool.GetInstanceWithName<TreasureBox> (lockedTreasureBoxModel.name, lockedTreasureBoxModel.gameObject, mapEventsContainer);
					mapWalkableInfoArray [posX, posY] = 0;
					(mapEvent as TreasureBox).tbType = TreasureBoxType.TreasuerBox;
					break;
				case "stone":
					mapEvent = mapEventsPool.GetInstanceWithName<Obstacle> (stoneModel.name, stoneModel.gameObject, mapEventsContainer);
					mapWalkableInfoArray [posX, posY] = 0;
					break;
				case "tree":
					mapEvent = mapEventsPool.GetInstanceWithName<Obstacle> (treeModel.name, treeModel.gameObject, mapEventsContainer);
					mapWalkableInfoArray [posX, posY] = 0;
					break;
				case "crystal":
					mapEvent = mapEventsPool.GetInstanceWithName<Crystal> (crystalModel.name, crystalModel.gameObject, mapEventsContainer);
					mapWalkableInfoArray [posX, posY] = 0;
					break;
				case "curePoint":
					mapEvent = mapEventsPool.GetInstanceWithName<CurePoint> (curePointModel.name, curePointModel.gameObject, mapEventsContainer);
					mapWalkableInfoArray [posX, posY] = 0;
					break;
				case "doorGear":
					mapEvent = mapEventsPool.GetInstanceWithName<Door> (doorModel.name, doorModel.gameObject, mapEventsContainer);
					(mapEvent as Door).SetPosTransferSeed (rows);
					mapWalkableInfoArray [posX, posY] = 0;
					break;
				case "thornSwitch":
					mapEvent = mapEventsPool.GetInstanceWithName<ThornTrapSwitch> (thornTrapSwitchModel.name, thornTrapSwitchModel.gameObject, mapEventsContainer);
					(mapEvent as ThornTrapSwitch).SetPosTransferSeed (rows);
					mapWalkableInfoArray [posX, posY] = 0;
					break;
				case "thornTrap":
					mapEvent = mapEventsPool.GetInstanceWithName<ThornTrap> (thornTrapModel.name, thornTrapModel.gameObject, mapEventsContainer);
//					allThornTrapsInMap.Add (mapEvent as ThornTrap);
					mapWalkableInfoArray [posX, posY] = 0;
					break;
				case "pressSwitch":
					mapEvent = mapEventsPool.GetInstanceWithName<PressSwitch> (pressSwitchModel.name, pressSwitchModel.gameObject, mapEventsContainer);
					(mapEvent as PressSwitch).SetPosTransferSeed (rows);
					mapWalkableInfoArray [posX, posY] = 10;
					break;
				case "downStair":
					mapEvent = mapEventsPool.GetInstanceWithName<SecretStairs> (secrectStairsModel.name, secrectStairsModel.gameObject, mapEventsContainer);
					(mapEvent as SecretStairs).SetPosTransferSeed (rows);
					mapWalkableInfoArray [posX, posY] = 0;
					break;
				case "upStair":
					mapEvent = mapEventsPool.GetInstanceWithName<SecretStairs> (secrectStairsModel.name, secrectStairsModel.gameObject, mapEventsContainer);
					(mapEvent as SecretStairs).SetPosTransferSeed (rows);
					mapWalkableInfoArray [posX, posY] = 0;
					break;
				case "fireTrap":
					mapEvent = mapEventsPool.GetInstanceWithName<FireTrap> (fireTrapModel.name, fireTrapModel.gameObject, mapEventsContainer);
					mapWalkableInfoArray [posX, posY] = 1;
					break;
				case "npc":
					mapEvent = GetMapNPC (eventTile);
					mapWalkableInfoArray [posX, posY] = 0;
					mapWalkableEventInfoArray [posX, posY] = 1;
					allWalkableEventsInMap.Add (mapEvent as MapNPC);
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
					LearnWord[] wordsArray = GetDifferentWords (words, unusedWordIndexList, 3);

					mapEvent.wordsArray = wordsArray;
					
					mapEvent.InitializeWithAttachedInfo (eventTile);

					if (layer.layerName.Equals ("GearEventLayer")) {
						allTriggeredMapEvents.Add (mapEvent as TriggeredGear);
					}
				}

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
		private LearnWord[] GetDifferentWords(LearnWord[] words,List<int> unusedWordRecordList,int count){

			LearnWord[] wordsArray = new LearnWord [count];

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

			int monsterId = int.Parse (KVPair.GetPropertyStringWithKey ("monsterID", info.properties));

			Transform monster = null;
			string monsterName = MyTool.GetMonsterName(monsterId);
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

		private MapEvent GetMapNPC(MapAttachedInfoTile info){

//			int npcId = int.Parse (KVPair.GetPropertyStringWithKey ("npcID", info.properties));

			int npcId = 1;

			Transform mapNpc = null;
			string npcName = string.Format ("NPC_{0}", npcId);

			for (int i = 0; i < npcsPool.transform.childCount; i++) {
				Transform npcInPool = npcsPool.transform.GetChild (i);
				if(npcInPool.name == npcName){
					mapNpc = npcInPool;
					break;
				}
			}

			if (mapNpc == null) {
				mapNpc = GameManager.Instance.gameDataCenter.LoadMapNpc (npcName).transform;
			}

			mapNpc.SetParent (npcsContainer);
			mapNpc.localScale = Vector3.one;
			mapNpc.localRotation = Quaternion.identity;

			return mapNpc.GetComponent<MapNPC> ();
		}



		public void AddNewPickableItem(Vector3 position,Item attachedItem){

			Transform pickableItem = Instantiate (pickableItemModel.gameObject, mapEventsContainer).transform;

			pickableItem.GetComponent<PickableItem> ().InitializeWithItemAndPosition (position, attachedItem);

		}

//		private void InitializeMonster(MapAttachedInfoTile info,int posX,int posY,LearnWord[] wordsArray){
//
//			bool canTalk = bool.Parse(KVPair.GetPropertyStringWithKey ("canTalk", info.properties));
//			int monsterId = int.Parse (KVPair.GetPropertyStringWithKey ("monsterID", info.properties));
//			int dropItemId = int.Parse (KVPair.GetPropertyStringWithKey ("dropItemID", info.properties));
//
//			Transform monster = null;
//			string monsterName = MyTool.GetMonsterName(monsterId);
//			for (int i = 0; i < monstersPool.transform.childCount; i++) {
//				Transform monsterInPool = monstersPool.transform.GetChild (i);
//				if(monsterInPool.name == monsterName){
//					monster = monsterInPool;
//					break;
//				}
//			}
//
//			if (monster == null) {
//				monster = GameManager.Instance.gameDataCenter.LoadMonster (monsterName).transform;
//			}
//
//			monster.SetParent (monstersContainer);
//			monster.localScale = Vector3.one;
//			monster.localRotation = Quaternion.identity;
//
//			mapWalkableInfoArray [posX, posY] = 0;
//
//			monster.position = info.position;
//
//			BattleMonsterController bm = monster.GetComponent<BattleMonsterController> ();
//
//			bm.canTalk = canTalk;
//			bm.dropItemID = dropItemId;
//			monster.gameObject.SetActive (true);
//
//			bm.wordsArray = wordsArray;
//
//			bm.SetAlive();
//
//
//
//		}

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

			bp.transform.Find ("FowBrush").gameObject.SetActive (true);

			Camera mainCamera = Camera.main;
			mainCamera.transform.SetParent (player,false);
			mainCamera.transform.localPosition = new Vector3 (0, 0, -10);
			mainCamera.transform.localScale = Vector3.one;
			mainCamera.transform.localRotation = Quaternion.identity;

			// 初始化迷雾相机的视窗大小
			Camera fowCamera = TransformManager.FindTransform ("FogOfWarBrushCamera").GetComponent<Camera> ();
			fowCamera.orthographicSize = (float)Mathf.Max (rows, columns) / 2;
			fowCamera.transform.position = new Vector3 (columns / 2, rows / 2, -5);

			ExploreManager.Instance.expUICtr.GetComponent<BattlePlayerUIController> ().RefreshMiniMap ();

			mainCamera.transform.Find ("Mask").gameObject.SetActive (true);
			UpdateFogOfWar ();
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
				if (layer.layerName.Equals ("DecorationLayer")) {
					sr.sortingOrder = -Mathf.RoundToInt (tile.position.y);
				}

				mapTile.position = tile.position;

				int posX = Mathf.RoundToInt (tile.position.x);
				int posY = Mathf.RoundToInt (tile.position.y);

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
			});

		}


		public Vector3 GetDirectionVectorTowardsExit(){

			Transform playerTrans = ExploreManager.Instance.battlePlayerCtr.transform;

			return new Vector3 (exitPos.x - playerTrans.position.x, exitPos.y - playerTrans.position.y,0); 

		}

	}
}
