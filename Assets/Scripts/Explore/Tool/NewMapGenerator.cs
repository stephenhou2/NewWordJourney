using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	using System.Data;
	using DragonBones;
	using Transform = UnityEngine.Transform;

	public enum MapSetUpFrom{
		LastLevel,
        NextLevel,
        Home
	}

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

		public GoldPack goldPackModel;//钱袋模型

		public Crystal crystalModel;//水晶模型

		public ReviewPool wishPoolModel;//许愿池模型


		public Transform rewardModel;//奖励物品模型

		public Transform characterFragmentModel;//字母碎片模型

		public DiaryPaper diaryPaperModel;//日记模型

		public FinalExit finalExitModel;//最终出口模型

		public RebuildStone rebuildStoneModel;//重塑石柱模型

		public SavePoint savePointModel;//存档点模型


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
      
        // 玩家存档点位置
		//private Vector2 playerSavePos;

        // 存档点是否合法
		//private bool savePosValid;

        // 玩家存档时的朝向
		//private MyTowards playerSaveTowards;

        // 所有已触发的地图事件记录
		private List<TriggeredGear> allTriggeredMapEvents = new List<TriggeredGear> ();
        // 地图上所有陷阱
		private List<Trap> allTrapsInMap = new List<Trap>();
        // 地图上所有怪物
		public List<MapMonster> allMonstersInMap = new List<MapMonster> ();
        // 地图上所有npc
		public List<MapNPC> allNPCsInMap = new List<MapNPC>();

        // 可以使用的npc id 列表
		private List<int> valableNpcIds = new List<int> { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

		// 进入下一层的出口位置
		private Vector2 exitPos;

        // 进入上一层的出口位置
		private Vector2 returnExitPos;

        // 所有可用的单词碎片位置
		private List<Vector3> allValidCharacterFragmentPositions = new List<Vector3>();

		public SpellItem spellItemOfCurrentLevel;


        // ********* 小地图元素模型 ******** //
		public Transform miniMapWallModel;

		public Transform miniMapExitModel;

		public Transform miniMapReturnExitModel;

		public Transform miniMapSavePointModel;

		public Transform miniMapNpcModel;
      
		public Transform miniMapMaskModel;

		public Transform miniMapPlayerModel;

		// ********* 小地图元素缓存池 ******** //
		public InstancePool miniMapInstancePool;// 小地图元素缓存池

		public InstancePool miniMapMaskPool;// 小地图探索迷雾缓存池


		// ********* 小地图元素容器 ******** //      
		public Transform miniMapInstanceContainer;// 小地图元素容器

		public Transform miniMapMaskContainer;// 小地图探索迷雾容器



		[HideInInspector]public Transform miniMapPlayer;// 小地图玩家图标

        // 小地图上所有迷雾列表
		private List<Transform> allMiniMapMasks = new List<Transform>();

        // 小地图观察相机
		private Camera miniMapCamera;

        // 小地图每走一步更新一次，更新
		private IEnumerator minimapSleepCoroutine;
        
        // 记录所有存档点
		//private List<SavePoint> allSavePoints = new List<SavePoint>();

        // 初始存储点
		//private SavePoint startSavePoint;

		/// <summary>
		/// 初始化地图
		/// </summary>
		/// 从上一关进入时人物出现在初始位置，从下一关回来时，出现在返回位置
		public void SetUpMap(MapSetUpFrom from)
		{
            // 绑定小地图相机
			if(miniMapCamera == null){
				miniMapCamera = TransformManager.FindTransform("MiniMapCamera").GetComponent<Camera>();
			}

            // 加载关卡数据
			GameManager.Instance.gameDataCenter.LoadGameLevelDatas();

            // 重置地图
			Reset ();

			// 绘制实际地图和小地图
			DrawMap ();

            // 初始化地图的过程中要加载单词数据，故需连接数据库
			mySql = MySQLiteHelper.Instance;
			mySql.GetConnectionWith (CommonData.dataBaseName);

            // 获取本关关卡id
			int mapIndex = Player.mainPlayer.GetMapIndex();

            // 根据记录清除小地图上已经探索过的地区的迷雾
			ClearMiniMapMasksByRecord(mapIndex);

			// 初始化地图事件
			InitializeMapEvents ();

            // 初始化玩家和主相机
			InitializePlayerAndSetCamera(from);   

            // 如果本层拼写没有完成，则重新生成字母碎片
			if(!MapEventsRecord.IsSpellFinish(mapIndex) && Player.mainPlayer.currentLevelIndex < CommonData.maxLevelIndex){
				GenerateCharacterFragments();
			}

            // 如果本层的日记没有看过，则重新生成日记
			if(HLHGameLevelData.HasDiaryPaper() && !MapEventsRecord.IsDiaryFinish(mapIndex)){
				GenerateDiaryPaper();
			}

            // 关闭数据库
			mySql.CloseConnection(CommonData.dataBaseName);

            // 销毁未使用的游戏体
			DestroyUnusedMapInstances();

		}
        

        /// <summary>
        /// 在地图上生成本关的日记
        /// </summary>
		private void GenerateDiaryPaper(){

			int randomSeed = Random.Range(0,allValidCharacterFragmentPositions.Count);

			Vector3 validPos = allValidCharacterFragmentPositions[randomSeed];

            // 生成日记游戏体
			DiaryPaper diaryPaper = Instantiate(diaryPaperModel.gameObject,mapEventsContainer).GetComponent<DiaryPaper>();
         
			int currentMapIndex = Player.mainPlayer.GetMapIndex();

            // 生成一个日记的附加信息数据
			MapAttachedInfoTile paperInfo = new MapAttachedInfoTile("diaryPaper", validPos, null);

            // 初始化日记
			diaryPaper.InitializeWithAttachedInfo(currentMapIndex, paperInfo);

			int paperPosX = Mathf.RoundToInt(validPos.x);
			int paperPosY = Mathf.RoundToInt(validPos.y);

            // 日记的可行走信息改为2，保证怪物不会走到日记上
			mapWalkableInfoArray[paperPosX, paperPosY] = 2;

			allValidCharacterFragmentPositions.Remove(validPos);

		}

        /// <summary>
        /// 在地图上生成单词碎片
        /// </summary>
		private void GenerateCharacterFragments(){
         
			// 获取一个还未使用过的拼写物品【当还存在未使用过的拼写物品是，已经拼过的物品不再出现】
			SpellItemModel spellItemModel = SpellItemModel.GetAUnusedSpellItemModel();

			spellItemOfCurrentLevel = new SpellItem(spellItemModel,1);

			string spell = spellItemModel.spell;

            // 将拼写物品的拼写拆分为字母数组
			char[] characters = spell.ToCharArray();

			for (int i = 0; i < characters.Length;i++){

				char character = characters[i];

				CharacterFragment characterFragment = characterFragmentPool.GetInstance<CharacterFragment>(characterFragmentModel.gameObject, characterFragmentContainer);

				characterFragment.SetPool(characterFragmentPool);

				int randomSeed = Random.Range(0, allValidCharacterFragmentPositions.Count);

				Vector3 characterFragmentPosition = allValidCharacterFragmentPositions[randomSeed];

				characterFragment.GenerateCharacterFragment(character, characterFragmentPosition, Player.mainPlayer.PlayerObtainCharacterFragment);

				int characterFragmentPosX = Mathf.RoundToInt(characterFragmentPosition.x);
				int characterFragmentPosY = Mathf.RoundToInt(characterFragmentPosition.y);

                // 碎片位置的可行走信息改为2，保证怪物不会走到单词碎片点上
				mapWalkableInfoArray[characterFragmentPosX, characterFragmentPosY] = 2;

                // 保证碎片不重合
				allValidCharacterFragmentPositions.Remove(characterFragmentPosition);

			}
            
		}

       


      
		/// <summary>
		/// 重置地图初始化工具
		/// </summary>
		private void Reset(){

            // 清除前面的地图信息
			ClearMapInfos();

            // 清除所有的小地图迷雾
			allMiniMapMasks.Clear();

            // 随机获得一个地图【地图本身不是随机生成，只是打乱了地图序号】
            int randomMapIndex = Player.mainPlayer.GetMapIndex();
         
            // 加载地图数据
			mapData = GameManager.Instance.gameDataCenter.LoadMapDataOfLevel (randomMapIndex);

            // 所有地图物体先放入缓存池，后面统一处理
			AllMapInstancesToPool();
            // 所有小地图物体放入缓存池，后面统一处理
			AllMiniMapInstancesToPool();
                     
			// 获取地图建模的行数和列数
			rows = mapData.rowCount;
			columns = mapData.columnCount;

			// 绘制小地图上每个地图块上的遮罩
            DrawMiniMapMasks(randomMapIndex);

            // 重置地图可行走信息数组和地图事件信息数组
			mapWalkableInfoArray = new int[columns, rows];
			mapWalkableEventInfoArray = new int[columns, rows];

			InitMapWalkableInfoAndMonsterPosInfo();

			valableNpcIds = new List<int> { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

			//savePosValid = false;
         
		}

        /// <summary>
		/// 绘制小地图上每个地图块上的遮罩
        /// </summary>
		private void DrawMiniMapMasks(int mapIndex){

            // 确保小地图背景在场景中是激活状态，并且材质没有丢失
			Transform minimapBackground = TransformManager.FindTransform("MiniMapCamera/MiniMapBackplane");
         
			if(!minimapBackground.gameObject.activeInHierarchy){
				minimapBackground.gameObject.SetActive(true);
			}else if(minimapBackground.GetComponent<Renderer>().sharedMaterial == null){
				minimapBackground.GetComponent<Renderer>().sharedMaterial = Resources.Load("FloorMaterial") as Material;
			}         
               
            // 小地图上所有位置都先上迷雾【后面会根据探索记录清除部分迷雾】
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
        /// 根据记录清除小地图上已探索过的位置上的迷雾
        /// </summary>
        /// <param name="mapIndex">Map index.</param>
		private void ClearMiniMapMasksByRecord(int mapIndex){
            
			MiniMapRecord miniMapRecord = GameManager.Instance.gameDataCenter.currentMapMiniMapRecord;

            if (miniMapRecord == null)
            {
                miniMapRecord = new MiniMapRecord(mapIndex, columns, rows);            
            }

            if (miniMapRecord.recordArray == null
               || miniMapRecord.mapWidth != columns
               || miniMapRecord.mapHeight != rows)
            {
                miniMapRecord.recordArray = new int[columns * rows];
				miniMapRecord.mapWidth = columns;
				miniMapRecord.mapHeight = rows;
            }

			GameManager.Instance.gameDataCenter.currentMapMiniMapRecord = miniMapRecord;
            

			for (int i = 0; i < rows; i++)
            {

                for (int j = 0; j < columns; j++)
                {
                    Vector3 pos = new Vector3(j, i, 0);
                    int indexInArray = i * columns + j;

                    if (miniMapRecord.recordArray[indexInArray] == 1)
                    {
                        ClearMiniMapMaskAround(pos, false);
                    }
                }
            }
		}
              

        /// <summary>
        /// 清除小地图上一个点附近的阴影遮罩
        /// </summary>
        /// <param name="position">Position.</param>
		public void ClearMiniMapMaskAround(Vector3 position,bool needRecord = true){

			int basePosX = Mathf.RoundToInt(position.x);
            int basePosY = Mathf.RoundToInt(position.y);
                
            // 如果需要记录到小地图记录里
			if(needRecord){
				
				int indexInRecord = basePosY * columns + basePosX;
				
				GameManager.Instance.gameDataCenter.currentMapMiniMapRecord.recordArray[indexInRecord] = 1;
            }

            // 隐藏基点周围3x3范围的迷雾
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

        /// <summary>
        /// 小地图相机激活，一帧之后关闭
        /// </summary>
		public void MiniMapCameraLatelySleep(){
			miniMapCamera.enabled = true;
			if(minimapSleepCoroutine != null){
				StopCoroutine(minimapSleepCoroutine);
			}
			minimapSleepCoroutine = MiniMapCameraSleepInOneFrame();
			StartCoroutine(minimapSleepCoroutine);
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
        
        /// <summary>
        /// 重置地图数据
        /// </summary>
		private void ClearMapInfos(){         
			allTriggeredMapEvents.Clear ();
			allMonstersInMap.Clear();
			allNPCsInMap.Clear();
			allTrapsInMap.Clear();
			allValidCharacterFragmentPositions.Clear();
			//allSavePoints.Clear();
			//startSavePoint = null;
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

			int mapEventCount = mapData.GetMapEventCount () + 3;

			int currentMapIndex = Player.mainPlayer.GetMapIndex();

			// 获取当前地图中要使用的单词
			HLHWord[] words = InitLearnWordsInMap (mapEventCount);

            // 绘制地图是就开始缓存本关地图事件上出现的单词发音数据
			CacheWordsPronounciation(words);

			// 生成一个记录可使用单词序号的列表
			List<int> unusedWordIndexRecordList = new List<int> ();

			// 初始化可使用单词列表
			for (int i = 0; i < mapEventCount; i++) {
				unusedWordIndexRecordList.Add (i);
			}

            //根据附加信息层的数据，初始化地图事件
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
            // 生成一个空数组存放地图事件要使用的所有单词
			HLHWord[] words = new HLHWord[mapEventCount];

			string currentWordsTableName = LearningInfo.Instance.GetCurrentLearningWordsTabelName();
         
            // 升序排列所有单词的学习次数,并获取最小的学习次数
			string query = string.Format("SELECT learnedTimes FROM {0} ORDER BY learnedTimes ASC LIMIT 1", currentWordsTableName);

			IDataReader reader = mySql.ExecuteQuery(query);

			reader.Read();

			int wholeLearnTime = reader.GetInt16(0);
            
            // 获取错误单词【学习次数>0并且学习次数=错误次数】的数量
			query = string.Format("SELECT COUNT(*) FROM {0} WHERE learnedTimes=ungraspTimes AND learnedTimes>0", currentWordsTableName);

			reader = mySql.ExecuteQuery(query);

			reader.Read();

			int wrongWordCount = reader.GetInt32(0);

            // 如果数据库中错误单词的数量可以满足初始化地图事件的单词数量要求
			if (wrongWordCount >= mapEventCount)
			{
				query = string.Format("SELECT * FROM {0} WHERE learnedTimes=ungraspTimes AND learnedTimes>0 LIMIT {1}", currentWordsTableName, mapEventCount);

				int index = 0;

				reader = mySql.ExecuteQuery(query);

                // 填充单词数组
				for (int i = 0; i < mapEventCount; i++)
				{
					reader.Read();

					HLHWord word = HLHWord.GetWordFromReader(reader);

					words[index] = word;

					index++;

				}
			}
			// 如果数据库中错误单词的数量<初始化地图事件的单词数量要求
			else
			{
                // 先获取最小学习次数的单词数量【如20个单词学习过0次，1000个单词学习过1次，则这20个单词需要先加入到数组中进行学习】

                // 获取最小学习次数的单词数量
				query = string.Format("SELECT COUNT(*) FROM {0} WHERE learnedTimes={1}", currentWordsTableName, wholeLearnTime);

				reader = mySql.ExecuteQuery(query);

				reader.Read();

				int validWordCount = reader.GetInt32(0);

                // 这里偷个懒，如果最小学习次数的单词数量+数据库中错误单词数量 < 初始化地图所需的单词数量，则这些未学习过的单词全部更新为已学习过
				if (validWordCount < mapEventCount - wrongWordCount)
				{

					string[] colFields = { "learnedTimes" };
					string[] values = { (wholeLearnTime + 1).ToString() };
					string[] conditions = { "learnedTimes=" + wholeLearnTime.ToString() };

					mySql.UpdateValues(currentWordsTableName, colFields, values, conditions, true);

					wholeLearnTime++;

				}

				int index = 0;

                // 获取数据库中所有背错的单词数据
				query = string.Format("SELECT * FROM {0} WHERE learnedTimes=ungraspTimes AND learnedTimes>0 LIMIT {1}", currentWordsTableName, wrongWordCount);

				reader = mySql.ExecuteQuery(query);

				for (int i = 0; i < wrongWordCount; i++)
				{
					reader.Read();

                    // 先将背错的单词填充到单词数组中
					HLHWord word = HLHWord.GetWordFromReader(reader);

					words[index] = word;

					index++;

				}

                // 剩余所需数量的单词，从数据库中，从最小学习次数的单词中随机读取
				string[] condition = { string.Format("learnedTimes={0} ORDER BY RANDOM() LIMIT {1}", wholeLearnTime, mapEventCount - wrongWordCount) };

				reader = mySql.ReadSpecificRowsOfTable(currentWordsTableName, null, condition, true);

				while (reader.Read())
				{               
					HLHWord word = HLHWord.GetWordFromReader(reader);

					words[index] = word;

					index++;

				}

			}         

    		return words;
    	}

        /// <summary>
        /// 预缓存本关所有单词的发音数据
        /// </summary>
        /// <param name="words">Words.</param>
		private void CacheWordsPronounciation(HLHWord[] words){

			for (int i = 0; i < words.Length;i++){
				
				HLHWord word = words[i];

				GameManager.Instance.pronounceManager.DownloadPronounceCache(word);
			}

		}


        /// <summary>
        /// 初始化指定层上的地图事件
        /// </summary>
        /// <param name="mapIndex">地图序号</param>
        /// <param name="layer">层数据</param>
        /// <param name="words">备用单词数据.</param>
        /// <param name="unusedWordIndexList">未使用过的单词序号列表</param>
		private void InitializeMapEventsOfLayer(int mapIndex, MapAttachedInfoLayer layer, HLHWord[] words, List<int> unusedWordIndexList)
		{

			bool exitOpen = true;
			Exit exit = null;
			Exit returnExit = null;

			List<Door> doors = new List<Door>();

            // 检查本层是否有合法地图事件记录数据，没有地图数据或地图数据不合法时生成新的地图事件记录数据
			if (!CurrentMapEventsRecord.CheckRecordValid(GameManager.Instance.gameDataCenter.currentMapEventsRecord))
			{
				GameManager.Instance.gameDataCenter.currentMapEventsRecord = new CurrentMapEventsRecord(mapIndex, new List<Vector2>());
			}

            // 记录谜语门的位置
			Vector2 puzzleDoorPos_0 = GameManager.Instance.gameDataCenter.currentMapEventsRecord.puzzleDoorPosArray[0];
			Vector2 puzzleDoorPos_1 = GameManager.Instance.gameDataCenter.currentMapEventsRecord.puzzleDoorPosArray[1];

            // 根据涂层中图块数据初始化地图事件
			for (int i = 0; i < layer.tileDatas.Count; i++)
			{
				MapAttachedInfoTile eventTile = layer.tileDatas[i];
				int posX = Mathf.RoundToInt(eventTile.position.x);
				int posY = Mathf.RoundToInt(eventTile.position.y);

				MapEvent mapEvent = null;
				Transform miniMapInstance = null;

				switch (eventTile.type)
				{
					case "playerStart":
						playerStartPos = new Vector2(posX, posY);
						//mapEvent = mapEventsPool.GetInstanceWithName<SavePoint>(savePointModel.name, savePointModel.gameObject, mapEventsContainer);
      //                  mapWalkableInfoArray[posX, posY] = 2;
      //                  if (MyTool.ApproximatelySameIntPosition2D(Player.mainPlayer.savePosition, eventTile.position))
      //                  {
      //                      playerSavePos = eventTile.position;
      //                      playerSaveTowards = Player.mainPlayer.saveTowards;
      //                      savePosValid = true;
						//	startSavePoint = mapEvent as SavePoint;
      //                  }
						//Transform miniMapInstance = DrawMiniMapInstance(miniMapSavePointModel, eventTile.position, miniMapInstanceContainer);
						//mapEvent.miniMapInstance = miniMapInstance;
						//allSavePoints.Add(mapEvent as SavePoint);                  
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
						exit.SetUpExitType(ExitType.ToNextLevel);
						//int towardsInt = int.Parse(KVPair.GetPropertyStringWithKey("direction", eventTile.properties));
						//MyTowards towards = (MyTowards)towardsInt;
						//miniMapInstance = DrawMiniMapInstance(miniMapExitModel, eventTile.position, miniMapInstanceContainer);
						//mapEvent.miniMapInstance = miniMapInstance;

						//switch (towards)
						//{
						//	case MyTowards.Up:
						//		miniMapInstance.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
						//		break;
						//	case MyTowards.Down:
						//		miniMapInstance.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
						//		break;
						//	case MyTowards.Left:
						//		miniMapInstance.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
						//		break;
						//	case MyTowards.Right:
						//		miniMapInstance.localRotation = Quaternion.Euler(new Vector3(0, 0, 270));
						//		break;
						//}
						break;
					case "return":
						mapEvent = mapEventsPool.GetInstanceWithName<Exit>(exitModel.name, exitModel.gameObject, mapEventsContainer);
						mapWalkableInfoArray[posX, posY] = 5;
						returnExitPos = new Vector2(posX, posY);
						allTriggeredMapEvents.Add(mapEvent as TriggeredGear);
						returnExit = mapEvent as Exit;
						returnExit.SetUpExitType(ExitType.ToLastLevel);
						//miniMapInstance = DrawMiniMapInstance(miniMapReturnExitModel, eventTile.position, miniMapInstanceContainer);
						//mapEvent.miniMapInstance = miniMapInstance;
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
						HLHWord[] wordsArray = GetDifferentMonsterWords(words, 3);

						mapEvent.wordsArray = wordsArray;

						break;
					case "monster":
						if (GameManager.Instance.gameDataCenter.currentMapEventsRecord.IsMapEventTriggered(mapIndex, eventTile.position))
                        {
                            continue;
                        }
						mapEvent = RandomGetAMonsterOfCurrentLevel();
						if (mapEvent != null)
						{
							(mapEvent as MapMonster).SetPosTransferSeed(rows);
							mapWalkableInfoArray[posX, posY] = 5;
							mapWalkableEventInfoArray[posX, posY] = 1;
							allMonstersInMap.Add(mapEvent as MapMonster);
							wordsArray = GetDifferentMonsterWords(words, 3);
							mapEvent.wordsArray = wordsArray;
						}
						break;
					case "boss":
						// 如果不是boss关，或者boss已经被打败了，则将出口设置为打开状态
						if (!HLHGameLevelData.IsBossLevel() || MapEventsRecord.IsMapEventTriggered(mapIndex, eventTile.position))
						{
							exitOpen = true;
							continue;
						}

                        // 如果是boss关，并且boss没有打败过
						mapEvent = GetBoss(eventTile);

						if (mapEvent != null)
						{
							(mapEvent as MapMonster).SetPosTransferSeed(rows);
							mapWalkableInfoArray[posX, posY] = 5;
							mapWalkableEventInfoArray[posX, posY] = 1;
							allMonstersInMap.Add(mapEvent as MapMonster);
							exitOpen = false;
							wordsArray = GetDifferentWords(words, unusedWordIndexList, 3);
							mapEvent.wordsArray = wordsArray;
						}
						break;
					case "goldPack":
						if (GameManager.Instance.gameDataCenter.currentMapEventsRecord.IsMapEventTriggered(mapIndex, eventTile.position))
						{
							continue;
						}
						mapEvent = mapEventsPool.GetInstanceWithName<GoldPack>(goldPackModel.name, goldPackModel.gameObject, mapEventsContainer);
						mapWalkableInfoArray[posX, posY] = 0;
						// 取3个不同的单词（首项做为目标单词，其余两项做为混淆单词）
						wordsArray = GetDifferentWords(words, unusedWordIndexList, 3);
						mapEvent.wordsArray = wordsArray;
						break;
					case "bucket":
						if (GameManager.Instance.gameDataCenter.currentMapEventsRecord.IsMapEventTriggered(mapIndex, eventTile.position))
						{
							continue;
						}
						mapEvent = mapEventsPool.GetInstanceWithName<Treasure>(bucketModel.name, bucketModel.gameObject, mapEventsContainer);
						mapWalkableInfoArray[posX, posY] = 0;
						(mapEvent as Treasure).treasureType = TreasureType.Bucket;
						// 取3个不同的单词（首项做为目标单词，其余两项做为混淆单词）
						wordsArray = GetDifferentWords(words, unusedWordIndexList, 3);
						mapEvent.wordsArray = wordsArray;
						break;
					case "pot":
						if (GameManager.Instance.gameDataCenter.currentMapEventsRecord.IsMapEventTriggered(mapIndex, eventTile.position))
						{
							continue;
						}

						mapEvent = mapEventsPool.GetInstanceWithName<Treasure>(potModel.name, potModel.gameObject, mapEventsContainer);
						mapWalkableInfoArray[posX, posY] = 0;
						(mapEvent as Treasure).treasureType = TreasureType.Pot;
						// 取3个不同的单词（首项做为目标单词，其余两项做为混淆单词）
						wordsArray = GetDifferentWords(words, unusedWordIndexList, 3);
						mapEvent.wordsArray = wordsArray;
						break;
					case "treasure":
						if (GameManager.Instance.gameDataCenter.currentMapEventsRecord.IsMapEventTriggered(mapIndex, eventTile.position))
						{
							continue;
						}
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
						bool isWordTrigger = bool.Parse(KVPair.GetPropertyStringWithKey("isWordTrigger", eventTile.properties));
						bool isOpen = bool.Parse(KVPair.GetPropertyStringWithKey("isOpen", eventTile.properties));
						if (!isOpen && isWordTrigger)
						{
							doors.Add(mapEvent as Door);
						}
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
						if (MapEventsRecord.IsMapEventTriggered(mapIndex, eventTile.position))
						{
							continue;
						}
						mapEvent = GetMapNPC(eventTile);
						if (mapEvent != null)
						{
							mapWalkableInfoArray[posX, posY] = 0;
							mapWalkableEventInfoArray[posX, posY] = 1;
							allNPCsInMap.Add(mapEvent as MapNPC);

							miniMapInstance = DrawMiniMapInstance(miniMapNpcModel, eventTile.position, miniMapInstanceContainer);

							mapEvent.miniMapInstance = miniMapInstance;
						}
						break;
					case "wordReview":
						mapEvent = mapEventsPool.GetInstanceWithName<ReviewPool>(wishPoolModel.name, wishPoolModel.gameObject, mapEventsContainer);
						mapWalkableInfoArray[posX, posY] = 0;
						break;
					case "finalExit":
						mapEvent = mapEventsPool.GetInstanceWithName<FinalExit>(finalExitModel.name, finalExitModel.gameObject, mapEventsContainer);
						mapWalkableInfoArray[posX, posY] = 0;
						break;
					case "rebuildStone":
						mapEvent = mapEventsPool.GetInstanceWithName<RebuildStone>(rebuildStoneModel.name, rebuildStoneModel.gameObject, mapEventsContainer);
						mapWalkableInfoArray[posX, posY] = 0;                  
						break;
					case "savePoint":
						//mapEvent = mapEventsPool.GetInstanceWithName<SavePoint>(savePointModel.name, savePointModel.gameObject, mapEventsContainer);
						//mapWalkableInfoArray[posX, posY] = 2;
						//if (MyTool.ApproximatelySameIntPosition2D(Player.mainPlayer.savePosition, eventTile.position))
						//{
						//	playerSavePos = eventTile.position;
						//	playerSaveTowards = Player.mainPlayer.saveTowards;
						//	savePosValid = true;
						//	startSavePoint = mapEvent as SavePoint;
						//}
						//allSavePoints.Add(mapEvent as SavePoint);
						//miniMapInstance = DrawMiniMapInstance(miniMapSavePointModel,eventTile.position, miniMapInstanceContainer);
                        //mapEvent.miniMapInstance = miniMapInstance;
						break;
				}

				if (mapEvent != null)
				{
					// 初始化地图事件
					mapEvent.InitializeWithAttachedInfo(mapIndex, eventTile);

                    // 把地图事件的坐标从可放置单词碎片的坐标列表中移除
					if (allValidCharacterFragmentPositions.Contains(eventTile.position))
					{
						allValidCharacterFragmentPositions.Remove(eventTile.position);
					}

				}

			}

            // 设置谜语门
			if (doors.Count > 0)
			{
                // 如果两个谜语门的位置已经确定【从存档中读取上次进入时谜语门的位置】
				if (puzzleDoorPos_0 != -Vector2.one && puzzleDoorPos_1 != -Vector2.one)
				{

					for (int i = 0; i < doors.Count; i++)
					{
						Door tempDoor = doors[i];
						if (MyTool.ApproximatelySameIntPosition2D(tempDoor.transform.position, puzzleDoorPos_0)
							|| MyTool.ApproximatelySameIntPosition2D(tempDoor.transform.position, puzzleDoorPos_1))
						{
							tempDoor.hasPuzzle = true;
						}
					}

				}
                // 如果两个谜语门的位置不确定，则从地图中所有的门中选择两个配对的门作为谜语门
				else
				{               
					int randomSeed = Random.Range(0, doors.Count);
					Door door = doors[randomSeed];
					door.hasPuzzle = true;

					for (int i = 0; i < doors.Count; i++)
					{
						Door tempDoor = doors[i];
						if (MyTool.ApproximatelySameIntPosition2D(tempDoor.transform.position, door.pairDoorPos))
						{
							tempDoor.hasPuzzle = true;
							break;
						}
					}

					GameManager.Instance.gameDataCenter.currentMapEventsRecord.puzzleDoorPosArray[0] = door.transform.position;
					GameManager.Instance.gameDataCenter.currentMapEventsRecord.puzzleDoorPosArray[1] = door.pairDoorPos;


				}
            
			}

            // 设置出口的状态
            // 如果是boss关，并且出口没有打开
			if (exit != null && HLHGameLevelData.IsBossLevel() && !exitOpen)
            {
                exit.SealExit(SealType.Boss);//将出口封印
			}else if (exit != null){
				exit.OpenSeal();//打开出口的封印
			}

            if (returnExit != null)
            {
                returnExit.SealExit(SealType.ReturnExit);// 封印回上一层的出口
            }
		}
            

        /// <summary>
        /// 在小地图上，绘制配对的两个门之间的墙体【实际地图上没有这些墙】
        /// </summary>
        /// <param name="eventTile">Event tile.</param>
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

        /// <summary>
        /// 转换地图上指定位置的地图事件的状态
        /// </summary>
        /// <param name="position">Position.</param>
		public void ChangeMapEventStatusAtPosition(Vector3 position)
		{

			for (int i = 0; i < allTriggeredMapEvents.Count; i++)
			{

				TriggeredGear tg = allTriggeredMapEvents[i];

                // 寻找指定位置的地图事件
				if (!MyTool.ApproximatelySamePosition2D(tg.transform.position, position))
				{
					continue;
				}

				tg.ChangeStatus();

			}
		}

        /// <summary>
		/// 关闭地图上所有陷阱
        /// </summary>
		public void ChangeAllTrapsStatusInMap()
		{

			for (int i = 0; i < allTriggeredMapEvents.Count; i++)
			{

				TriggeredGear tg = allTriggeredMapEvents[i];

				tg.ChangeStatus();

			}
		}

        /// <summary>
        /// 获取指定数量各不相同的单词，用于怪物说的话【不同怪物之间的单词可以重复】
        /// </summary>
        /// <returns>The different monster words.</returns>
        /// <param name="words">单词数据源.</param>
        /// <param name="count">单词数量.</param>
		private HLHWord[] GetDifferentMonsterWords(HLHWord[] words,int count){
		
            // 如果单词数据源单词数量不足，直接返回null
		    if (words.Length < count || words.Length == 0)
            {
                return null;
            }

            
			HLHWord[] wordsArray = new HLHWord[count];

            // 所有未使用的单词在单词数据源中的序号列表
            List<int> tempList = new List<int>();

             
			for (int i = 0; i < words.Length;i++){
				tempList.Add(i);
			}

			for (int i = 0; i < count;i++){
				int randomSeed = Random.Range(0, tempList.Count);//可用列表内随机一个序号
				int wordIndex = tempList[randomSeed];// 可用列表中，上面随机序号对应的在单词数据源中的实际序号
				tempList.RemoveAt(randomSeed);
				HLHWord word = words[wordIndex];
				wordsArray[i] = word;
			}

            return wordsArray;
            

		}

		/// <summary>
		/// 获取指定数量的不重复的随机单词
		/// </summary>
		/// <returns>The different words.</returns>
		/// <param name="words">Words.</param>
		/// <param name="unusedWordRecordList">Unused word record list.</param>
		/// <param name="count">Count.</param>
		private HLHWord[] GetDifferentWords(HLHWord[] words, List<int> unusedWordRecordList, int count)
		{
            // 如果单词数据源的单词数量不足，直接返回null
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
		/// 随机获取一个本关可使用的怪物
		/// </summary>
		/// <returns>The monster.</returns>
		private MapEvent RandomGetAMonsterOfCurrentLevel()
		{

            // 获取本关的关卡数据
			HLHGameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas[Player.mainPlayer.currentLevelIndex];

            // 随机获取一个可用的怪物id
			int randomSeed = Random.Range(0, levelData.monsterIds.Count);

			int monsterId = levelData.monsterIds[randomSeed];

			levelData.monsterIds.RemoveAt(randomSeed);

			Transform monster = null;

            // 根据怪物id获取怪物资源名称
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

			// 获取本关的关卡数据
			HLHGameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas[Player.mainPlayer.currentLevelIndex];

            // 如果不是boss关，返回null
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

        /// <summary>
        /// 获取地图npc
        /// </summary>
        /// <returns>The map npc.</returns>
        /// <param name="info">Info.</param>
		private MapEvent GetMapNPC(MapAttachedInfoTile info)
		{         
            // 从地图数据中获取npc的id
			int npcId = int.Parse(KVPair.GetPropertyStringWithKey("npcID", info.properties));

            // 获取当前地图事件记录
			CurrentMapEventsRecord currentMapEventsRecord = GameManager.Instance.gameDataCenter.currentMapEventsRecord;

			for (int i = 0; i < currentMapEventsRecord.npcPosArray.Length;i++){				

		        Vector2 pos = currentMapEventsRecord.npcPosArray[i];

				if(MyTool.ApproximatelySameIntPosition2D(pos,info.position)){
					HLHNPC npc = currentMapEventsRecord.npcArray[i];
					npcId = npc.npcId;
              }            

			}
   
            // 如果npc的id=0，本关没有老头子时，返回null
		    if (npcId == 0 && !HLHGameLevelData.HasWiseMan())
			{
				return null;
			}

            // 如果npc的id=-1，则随机一个npc
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

  


		/// <summary>
		/// 初始化人物和主摄像机
		/// </summary>
	    private void InitializePlayerAndSetCamera(MapSetUpFrom from)
		{         

			if(Player.mainPlayer.savePosition.Equals(Vector3.zero)){
				Player.mainPlayer.savePosition = playerStartPos;
			}
			MyTowards towards = Player.mainPlayer.saveTowards;
			Vector3 position = Player.mainPlayer.savePosition;

			switch(from){
				case MapSetUpFrom.LastLevel:
					if (Mathf.RoundToInt(playerStartPos.x) == Mathf.RoundToInt(returnExitPos.x))
                    {
                        towards = playerStartPos.y >= returnExitPos.y ? MyTowards.Up : MyTowards.Down;
                    }
                    else
                    {
                        towards = playerStartPos.x >= returnExitPos.x ? MyTowards.Right : MyTowards.Left;
                    }
                    position = playerStartPos;

					break;
				case MapSetUpFrom.NextLevel:
					if (Mathf.RoundToInt(playerReturnPos.x) == Mathf.RoundToInt(exitPos.x))
                    {
                        towards = playerReturnPos.y >= exitPos.y ? MyTowards.Up : MyTowards.Down;
                    }
                    else
                    {
                        towards = playerReturnPos.x >= exitPos.x ? MyTowards.Right : MyTowards.Left;
                    }
                    position = playerReturnPos;
					break;
				case MapSetUpFrom.Home:  
					break;
			}

              
			Transform player = Player.mainPlayer.transform.Find("BattlePlayer");

			player.position = position;



			//if (startSavePoint == null)
			//{
			//	for (int i = 0; i < allSavePoints.Count; i++)
			//	{
			//		if (MyTool.ApproximatelySameIntPosition2D(allSavePoints[i].transform.position, position))
			//		{
			//			startSavePoint = allSavePoints[i];
			//			break;
			//		}
			//	}
			//}


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

		    bp.isDead = false;
		    bp.PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);

			bp.StopMoveAndWait();

			bp.isInFight = false;
			bp.isInEvent = false;
			bp.isInEscaping = false;
			bp.isDead = false;
			bp.isInPosFixAfterFight = false;
			bp.boxCollider.enabled = false;

			Camera mainCamera = Camera.main;
			mainCamera.transform.SetParent(player, false);
			mainCamera.transform.localPosition = new Vector3(0, 0, -10);
			mainCamera.transform.localScale = Vector3.one;
			mainCamera.transform.localRotation = Quaternion.identity;
        
		    miniMapPlayer = DrawMiniMapInstance(miniMapPlayerModel, position, miniMapInstanceContainer);
         
			if(ExploreManager.Instance.battlePlayerCtr.fadeStepsLeft < 5){
				ExploreManager.Instance.battlePlayerCtr.fadeStepsLeft = 5;
			}

			bp.SetEffectAnim(CommonData.yinShenEffectName,null,0);

			SetUpExploreMask(0);

		    ClearMiniMapMaskAround(position);

	    	MiniMapCameraLatelySleep();

		}

        /// <summary>
        /// 设置探索遮罩
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
		/// 绘制单个地图层【地板层，墙体层，装饰层】
		/// </summary>
		/// <param name="layer">Layer.</param>
		private void DrawMapLayer(MapLayer layer)
		{
            // 获取当前地图所使用地图图集名称
			string tileSetsImageName = mapData.mapTilesImageName;

            // 获取当前地图图集中所有图片
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

                // 可行走信息=-2代表空白区域的点，可行走信息=1代表可行走点，可行走点也可能在其他层的初始化过程中变为不可行走点
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

        /// <summary>
        /// 手动播放存档点动画
        /// </summary>
		//public void ManuallyPlaySavePointAnim(){
		//	startSavePoint.PlayTriggerAnim();
		//}

        /// <summary>
        /// 播放玩家点击点动画
        /// </summary>
        /// <param name="targetPos">动画位置</param>
        /// <param name="arrivable">是否可行走</param>
		public void PlayDestinationAnim(Vector3 targetPos, bool arrivable)
		{

	    	walkTint.transform.position = targetPos;
            
            // 可行走的话播放白色方框动画，不可行走播放红色方框动画
			if(arrivable){
				walkTint.animation.Play("move_able", 1);
			}else{
				walkTint.animation.Play("move_disable", 1);
			}

			
		} 
      
        /// <summary>
        /// 在地图上生成奖励物品
        /// </summary>
        /// <param name="reward">Reward.</param>
        /// <param name="rewardPosition">Reward position.</param>
		public void SetUpRewardInMap(Item reward, Vector3 rewardPosition)
		{

			RewardInMap rewardInMap = rewardPool.GetInstance<RewardInMap>(rewardModel.gameObject, rewardContainer);

		    rewardInMap.SetUpRewardInMap(reward, rewardPosition, rewardPool);

		}
              
        /// <summary>
        /// 获取指定名称的动画特效游戏体
        /// </summary>
        /// <returns>The effect animation.</returns>
        /// <param name="effectName">Effect name.</param>
        /// <param name="effectContainer">传入的指定特效容器.</param>
		public EffectAnim GetEffectAnim(string effectName, Transform effectContainer)
		{         
			EffectAnim effectAnim = null;
            // 遍历特效容器，查询，获得名称一致的特效
			for (int i = 0; i < effectContainer.childCount;i++){

				EffectAnim tempEa = effectContainer.GetChild(i).GetComponent<EffectAnim>();

				if(tempEa.effectName == effectName){
					effectAnim = tempEa;
				    break;
				}

			}
            // 如果指定容器内有该名称的特效，并且该特效为循环播放类型，才返回该特效
            // 指定播放次数的特效在播放完成后会被回收，且部分特效应该是打出一次就播放一次，故指定播放次数的特效不作为可用特效返回
		    if(effectAnim != null && effectAnim.playTime == 0){
				return effectAnim;
			}

            // 如果没有可用特效，则从缓存池中查询获取指定名称的特效
			effectAnim = effectAnimPool.GetInstanceWithName<EffectAnim>(effectName);

            // 如果缓存池中也没有
			if (effectAnim == null)
			{
                // 获取原始特效
				EffectAnim effectModel = GameManager.Instance.gameDataCenter.allEffects.Find(delegate (EffectAnim obj)
				{
					return obj.effectName == effectName;
				});

				if (effectModel == null)
				{
					return null;
				}

                // 通过原始特效复制出新特效以供使用
				effectAnim = Instantiate(effectModel.gameObject).GetComponent<EffectAnim>();
				effectAnim.name = effectModel.effectName;
			}


            // 设置特效位置和层级等信息
			effectAnim.transform.SetParent(effectContainer, false);

			effectAnim.transform.localPosition = new Vector3(effectAnim.localPos.x, effectAnim.localPos.y, 0);
			effectAnim.transform.localRotation = Quaternion.identity;
			effectAnim.gameObject.SetActive(true);

			return effectAnim;
		}


        /// <summary>
        /// 特效动画回收至缓存池
        /// </summary>
        /// <param name="ea">Ea.</param>
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

			if(allMonstersInMap.Count == 0){
				return;
			}

			int monsterToPoolCount = (int)(allMonstersInMap.Count * percentage);

			if(monsterToPoolCount == 0){
				monsterToPoolCount = 1;
			}

			if(monsterToPoolCount > allMonstersInMap.Count){
				return;
			}

			for (int i = 0; i < monsterToPoolCount; i++)
			{

				int randomSeed = Random.Range(0, allMonstersInMap.Count);

			    MapMonster mapMonster = allMonstersInMap[randomSeed];

				Monster monster = mapMonster.GetComponent<Monster>();

				if(monster.isBoss){
					continue;
				}

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

		}
        
        
        /// <summary>
        /// 所有小地图游戏体放入缓存池中
        /// </summary>
		private void AllMiniMapInstancesToPool(){
         
			miniMapInstancePool.AddChildInstancesToPool(miniMapInstanceContainer);

			miniMapMaskPool.AddChildInstancesToPool(miniMapMaskContainer);

		}

        /// <summary>
        /// 销毁没有使用到的游戏体
        /// </summary>
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

        /// <summary>
        /// 所有的地图时间放入缓存池中
        /// </summary>
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

        /// <summary>
        /// 所有的怪物放入缓存池中
        /// </summary>
		private void AllMonstersToPool()
		{
			while (monstersContainer.childCount > 0)
			{
				MapMonster mm = monstersContainer.GetChild(0).GetComponent<MapMonster>();
				mm.AddToPool(monstersPool);
			}
		}

        /// <summary>
        /// 所有的npc放入缓存池中
        /// </summary>
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
