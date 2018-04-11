using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic; 		//Allows us to use Lists.


namespace WordJourney	
{
	using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.
//	using Transform = UnityEngine.Transform;
//	using DragonBones;

	public class MapGenerator:MonoBehaviour
	{
		// 地图的行数和列数
		[HideInInspector]public int columns; 										
		[HideInInspector]public int rows;								


		// 地图信息（用于绘制地图）
		private MapData mapInfo;
//		private TileInfo tileInfo;

		// 地板模型
		public Transform floorModel;
		// 地图上npc模型
		public Transform mapNpcModel;

		// 地图上掉落的物品模型
		public Transform rewardItemModel;

		public Transform crystalModel;

		public Transform consumablesValidPosTintModel;


		// 所有地图元素在场景中的父容器
		public Transform floorsContainer;
		public Transform mapItemsContainer;
		public Transform npcsContainer;
		public Transform monstersContainer;
		public Transform effectAnimContainer;
		public Transform rewardsContainer;
		public Transform consumablesValidPosTintContainer;

		// 所有的缓存池
		public InstancePool floorPool;
		public InstancePool mapItemPool;
		public InstancePool monsterPool;
		public InstancePool effectAnimPool;
		public InstancePool rewardItemPool;
		public InstancePool consumablesValidPosTintPool;

		public Transform destinationAnimation;


		// 关卡数据
		private GameLevelData levelData;

//		public UnityEngine.Material spriteMaterial; 

		public int[,] mapWalkableInfoArray;

		/// <summary>
		/// 地图原始可行走数据
		/// 地图如果不是一次显示完成时慎用
		/// </summary>
		public int[,] originalMapWalkableInfoArray;

		public float rewardFlyDuration;

		private BattlePlayerController bpCtr;

		// 获得地板层和附加信息层的数据
		private MapLayer floorLayer = null;
		private MapLayer attachedInfoLayer = null;
		private MapLayer attachedItemInfoLayer = null;


		// 障碍物模型数组
		public Obstacle treeModel;
		public Obstacle stoneModel;


		// 陷阱模型
		public ThornTrap trapModel;

		// 开关模型
		public ThornTrapSwitch trapSwitchModel;

		// 宝箱模型
		public TreasureBox lockedTreasureBoxModel;

		// 木桶模型
		public TreasureBox buckModel;

		// 瓦罐模型
		public TreasureBox potModel;

		// 传送阵模型
		public Exit transportModel;

		// 可移动地板模型
		public MovableFloor movableFloorModel;

		// 门模型
		public Door doorModel;

		//告示牌模型
		public Billboard billboardModel;

		//火焰陷阱模型
		public FireTrap fireTrapModel;

		// 坑洞模型
		public SecretStairs holeModel;

		// 可移动箱子模型
		public MovableBox movableBoxModel;


		// 植物模型
		public Plant plantModel;

		// 压力开关模型
		public PressSwitch pressSwitchModel;

		// 装饰用障碍物模型
		public Block blockModel;


		private Vector3 transportPositionInMap;


		private List<Trap> allTrapsInMap = new List<Trap> ();
		private List<SecretStairs> allHolesInMap = new List<SecretStairs> ();
//		private List<Vector3> allMovableFloorsPositions = new List<Vector3> (); 

		private List<Vector3> totalValidPosGridList = new List<Vector3> ();
		private List<Vector3> playerOriginalPosList = new List<Vector3> ();

		private List<Transform> allSleepingMonsters = new List<Transform> ();
		private List<Transform> allSleepingOtherItems = new List<Transform> ();
		private List<Transform> allSleepingFloors = new List<Transform> ();

		private List<Transform> allAliveOtherItems = new List<Transform> ();
		private List<Transform> allAliveMonsters = new List<Transform> ();

		private Consumables currentUsingConsumables;



		void Awake(){

//			floorPool = InstancePool.GetOrCreateInstancePool ("FloorPool",CommonData.exploreScenePoolContainerName);
//			mapItemPool = InstancePool.GetOrCreateInstancePool ("MapItemPool",CommonData.exploreScenePoolContainerName);
//			monsterPool = InstancePool.GetOrCreateInstancePool ("MonsterPool",CommonData.exploreScenePoolContainerName);
//			effectAnimPool = InstancePool.GetOrCreateInstancePool ("EffectAnimPool",CommonData.exploreScenePoolContainerName);
//			rewardItemPool = InstancePool.GetOrCreateInstancePool ("RewardItemPool", CommonData.exploreScenePoolContainerName);
//			consumablesValidPosTintPool = InstancePool.GetOrCreateInstancePool ("ConsumablesValidPosTintPool", CommonData.exploreScenePoolContainerName);

		}

		//SetupScene initializes our level and calls the previous functions to lay out the game board
		public void SetUpMap (GameLevelData levelData)
		{

			AllMapInstancesToPool ();

			ResetDestinationAnim ();

			this.levelData = levelData;

			mapInfo = MapData.GetMapDataOfLevel(levelData.gameLevelIndex);

			// 获取地图建模的行数和列数
			rows = mapInfo.rowCount;
			columns = mapInfo.columnCount;

			// 初始化地图原始数据
			InitMapDatas();

			ResetMap ();

//			SetupAllAnimatorControllers ();

			// 初始化地面和背景
			SetUpFloorAndBackground();

//			SetUpMapWithAttachedInfo ();

			SetUpPlayer ();

			ShowTransport ();

			ClearPools ();

			destinationAnimation.gameObject.SetActive (true);
		}


		private void ShowTransport(){

			DirectlyShowSleepingTilesAtPosition (transportPositionInMap);

		}


		private void ResetMap(){
			
			// 地图上的原始(地图完全解锁时的)可行走信息数组
			originalMapWalkableInfoArray = new int[columns, rows];
			// 地图当前的可行走信息数组
			mapWalkableInfoArray = new int[columns,rows];

			ResetMapWalkableInfoArray ();

			allTrapsInMap.Clear ();

			totalValidPosGridList.Clear ();

			playerOriginalPosList.Clear ();

			allSleepingFloors.Clear ();
			allSleepingMonsters.Clear ();
			allSleepingOtherItems.Clear ();
			allAliveOtherItems.Clear ();
			allAliveMonsters.Clear ();
		}

		/// <summary>
		/// 将地图范围内的所有点都设置为不可行走点
		/// </summary>
		private void ResetMapWalkableInfoArray (){
			for (int i = 0; i < columns; i++) {
				for (int j = 0; j < rows ; j++) {
					mapWalkableInfoArray [i, j] = -1;
					originalMapWalkableInfoArray [i, j] = -1;
				}
			}
		}

		public Vector3 GetTransportPosInMap(){
			return transportPositionInMap;
		}

		/// <summary>
		/// 地图数据分离出地板层数据和附加信息层数据
		/// 根据地板层数据初始化基础的地图可行走信息
		/// </summary>
		private void InitMapDatas(){

			for (int i = 0; i < mapInfo.mapLayers.Count; i++) {

				switch (mapInfo.mapLayers [i].layerName) {
				case "FloorLayer":
					floorLayer = mapInfo.mapLayers [i];
					break;
				case "AttachedInfoLayer":
					attachedInfoLayer = mapInfo.mapLayers [i];
					break;
				case "AttachedItemLayer":
					attachedItemInfoLayer = mapInfo.mapLayers [i];
					break;
				}
			
			}

//			for (int j = 0; j < floorLayer.tileDatas.Count; j++) {
//
//				MapTile floorTile = floorLayer.tileDatas [j];

//				if (floorTile.walkable) {
//					totalValidPosGridList.Add (floorTile.position);
//				}

//			}


			if (floorLayer == null || attachedInfoLayer == null || attachedItemInfoLayer == null) {
				Debug.LogError ("地图数据不完整");
			}

		}



		private Item GetAttachedItem(Vector2 position,AttachedInfoType attachedInfoType){

			Item attachedItem = null;

			for (int i = 0; i < attachedItemInfoLayer.tileDatas.Count; i++) {

				MapTile attachedItemTile = attachedItemInfoLayer.tileDatas [i];

				AttachedItemType type = AttachedItemType.Floor;

				if (attachedItemTile.position == position) {
					type = (AttachedItemType)(attachedItemTile.tileIndex);
			
					switch (type) {
					case AttachedItemType.Medicine:
						attachedItem = Item.NewItemWith (100, 1);
						break;
					case AttachedItemType.PickAxe:
						attachedItem = Item.NewItemWith (101, 1);
						break;
					case AttachedItemType.Saw:
						attachedItem = Item.NewItemWith (102, 1);
						break;
					case AttachedItemType.Sickle:
						attachedItem = Item.NewItemWith (103, 1);
						break;
					case AttachedItemType.Torch:
						attachedItem = Item.NewItemWith (104, 1);
						break;
					case AttachedItemType.Soil:
						attachedItem = Item.NewItemWith (105, 1);
						break;
					case AttachedItemType.Water:
						attachedItem = Item.NewItemWith (106, 1);
						break;
					case AttachedItemType.Floor:
						attachedItem = Item.NewItemWith (107, 1);
						break;
					case AttachedItemType.Key:
						attachedItem = Item.NewItemWith (108, 1);
						break;
					case AttachedItemType.Tree:
						int randomPlantId = Random.Range (109, 120);
						attachedItem = Item.NewItemWith (randomPlantId, 1);
						break;
					case AttachedItemType.Switch:
						attachedItem = Item.NewItemWith (110, 1);
						break;
					case AttachedItemType.Scroll:
						attachedItem = Item.NewItemWith (111, 1);
						break;
					case AttachedItemType.Random:
						attachedItem = GetRandomItemFromType (attachedInfoType);
						break;
					}

					return attachedItem;
				}
			}
			return attachedItem;
		}

		private Item GetRandomItemFromType(AttachedInfoType attachedInfoType){

			Item randomItem = null;

			int randomIndex = 0;

			switch (attachedInfoType) {
			case AttachedInfoType.Pot:
			case AttachedInfoType.Buck:
				if (levelData.mustAppearItemsInUnlockedBox.Count > 0) {
					randomIndex = Random.Range(0,levelData.mustAppearItemsInUnlockedBox.Count);
					randomItem = levelData.mustAppearItemsInUnlockedBox[randomIndex];
					levelData.mustAppearItemsInUnlockedBox.RemoveAt (0);
				} else {
					randomIndex = Random.Range (0, levelData.possiblyAppearItemsInUnlockedBox.Count);
					randomItem = levelData.possiblyAppearItemsInUnlockedBox [randomIndex];
				}
				break;
			case AttachedInfoType.TreasureBox:
				randomIndex = Random.Range (0, levelData.possiblyAppearItemsInLockedBox.Count);
				randomItem = levelData.possiblyAppearItemsInLockedBox [randomIndex];
				break;

			}

			return randomItem;

		}

		/// <summary>
		/// 根据附加信息层数据初始化关卡的其他信息
		/// </summary>
//		private void SetUpMapWithAttachedInfo(){
//
////			List<Vector3> movableFloorOriList = new List<Vector3> ();
//			List<Vector3> allMovableFloorEndPos = new List<Vector3> ();
//			List<MovableFloor> allMovableFloorsInMap = new List<MovableFloor> ();
//			List<PressSwitch> allPressSwitchesInMap = new List<PressSwitch> ();
//			List<Door> allDoorsImMap = new List<Door> ();
//
//			for (int i = 0; i < attachedInfoLayer.tileDatas.Count; i++) {
//				MapTile attachedInfoTile = attachedInfoLayer.tileDatas [i];
//				Vector2 pos = attachedInfoTile.position;
//				AttachedInfoType attachedInfoType = (AttachedInfoType)(attachedInfoTile.tileIndex);
//				switch (attachedInfoType) {
//				 
//				// 人物初始点 水晶 商人 npc 传送阵 门 木桶 瓦罐 宝箱 石头 树木 陷阱开关 陷阱关 陷阱开 可移动石板 boss 怪物
//				case AttachedInfoType.PlayerOriginPosition:
//					playerOriginalPosList.Add (pos);
//					break;
//				case AttachedInfoType.Crystal:
//					GenerateMapItem (MapItemType.Crystal, pos, null);
//					break;
////				case AttachedInfoType.Trader:
////					NPC trader = GameManager.Instance.gameDataCenter.allNpcs.Find (delegate(NPC obj) {
////						return obj.npcId == 0;
////					});
////					(trader as Trader).InitGoodsGroupOfLevel (levelData.gameLevelIndex);
////					MapNPC mapTrader = GenerateMapItem (MapItemType.MapNPC, pos, null) as MapNPC;
////					mapTrader.npc = trader;
////					break;
////				case AttachedInfoType.NPC:
////					#warning 这里没有其他npc，暂时都初始化为商人
////					NPC npc = GameManager.Instance.gameDataCenter.allNpcs.Find (delegate(NPC obj) {
////						return obj.npcId == 0;
////					});
////					MapNPC mapNpc = GenerateMapItem (MapItemType.MapNPC, pos, null) as MapNPC;
////					mapNpc.npc = npc;
////					break;
//				case AttachedInfoType.Transport:
//					GenerateMapItem (MapItemType.Transport, pos, null);
//					break;
//				case AttachedInfoType.Door:
//					Door door = GenerateMapItem (MapItemType.Door, pos, null) as Door;
//					allDoorsImMap.Add (door);
//					break;
//				case AttachedInfoType.Buck:
//					Item attachedItem = GetAttachedItem (pos,attachedInfoType);
//					GenerateMapItem (MapItemType.Buck, pos, attachedItem);
//					break;
//				case AttachedInfoType.Pot:
//					attachedItem = GetAttachedItem (pos,attachedInfoType);
//					GenerateMapItem (MapItemType.Pot, pos, attachedItem);
//					break;
//				case AttachedInfoType.TreasureBox:
//					attachedItem = GetAttachedItem (pos,attachedInfoType);
//					GenerateMapItem (MapItemType.TreasureBox, pos, attachedItem);
//					break;
//				case AttachedInfoType.Stone:
//					GenerateMapItem (MapItemType.Stone, pos, null);
//					break;
//				case AttachedInfoType.Tree:
//					GenerateMapItem (MapItemType.Tree, pos, null);
//					break;
//				case AttachedInfoType.Switch:
//					GenerateMapItem (MapItemType.Switch, pos, null);
//					break;
//				case AttachedInfoType.TrapOn:
//					Trap trapOn = GenerateMapItem (MapItemType.NormalTrapOn, pos, null) as Trap;
//					allTrapsInMap.Add (trapOn);
//					break;
//				case AttachedInfoType.TrapOff:
//					Trap trapOff = GenerateMapItem (MapItemType.NormalTrapOff, pos, null) as Trap;
//					allTrapsInMap.Add (trapOff);
//					break;
//				case AttachedInfoType.MovableFloorStart:
//					MapEvent movableFloor = GenerateMapItem (MapItemType.MovableFloor, pos, null);
//					allMovableFloorsInMap.Add (movableFloor as MovableFloor);
//					break;
//				case AttachedInfoType.MovableFloorEnd:
//					allMovableFloorEndPos.Add (pos);
//					break;
//				case AttachedInfoType.Monster:
//					SetUpMonster (pos);
//					break;
//				case AttachedInfoType.Boss://boss点暂时先做地图物品测试点
//					SetUpBoss (pos);
//					break;
//				case AttachedInfoType.FireTrap:
//					GenerateMapItem (MapItemType.FireTrap, pos, null);
//					break;
//				case AttachedInfoType.Hole:
//					SecretStairs hole = GenerateMapItem (MapItemType.Hole, pos, null) as SecretStairs;
//					allHolesInMap.Add (hole); 
//					break;
//				case AttachedInfoType.MovableBox:
//					GenerateMapItem (MapItemType.MovableBox, pos, null);
//					break;
//				case AttachedInfoType.LauncherUp:
//					GenerateMapItem (MapItemType.LauncherTowardsUp, pos, null);
//					break;
//				case AttachedInfoType.LauncherDown:
//					GenerateMapItem (MapItemType.LauncherTowardsDown, pos, null);
//					break;
//				case AttachedInfoType.LauncherLeft:
//					GenerateMapItem (MapItemType.LauncherTowardsLeft, pos, null);
//					break;
//				case AttachedInfoType.LauncherRight:
//					GenerateMapItem (MapItemType.LauncherTowardsRight, pos, null);
//					break;
//				case AttachedInfoType.Plant:
//					Item attachedPlant = Plant.GenerateRandomReward ();
//					GenerateMapItem (MapItemType.Plant, pos, attachedPlant);
//					break;
//				case AttachedInfoType.PressSwitch:
//					PressSwitch ps = GenerateMapItem (MapItemType.PressSwitch, pos, null) as PressSwitch;
//					allPressSwitchesInMap.Add (ps);
//					break;
//				case AttachedInfoType.Docoration:
//					GenerateMapItem (MapItemType.Docoration, pos, null);
//					break;
//				}
//
//			}
//
////			PairMovableFloorPositions (allMovableFloorsInMap, allMovableFloorEndPos);
////			PairAllPressSwitchAndDoorInMap (allPressSwitchesInMap, allDoorsImMap);
//		}

//		private void PairMovableFloorPositions(List<MovableFloor> movableFloors,List<Vector3> endPosList){
//
//			for (int i = 0; i < movableFloors.Count; i++) {
//
//				MovableFloor mf = movableFloors [i];
//
//				mf.movePosPair [0] = mf.transform.position;
//
//				mf.movePosPair [1] = MyTool.FindNearestPos (mf.transform.position,endPosList);
//
//				endPosList.Remove (mf.movePosPair [1]);
//
//			}
//
//		}

//		private void PairAllPressSwitchAndDoorInMap(List<PressSwitch> pressSwitchList,List<Door> doorList){
//
//			List<Vector3> doorPosList = new List<Vector3> ();
//			for (int i = 0; i < doorList.Count; i++) {
//				doorPosList.Add(doorList[i].transform.position);
//			}
//
//			for (int i = 0; i < pressSwitchList.Count; i++) {
//
//				int nearestDoorIndex = MyTool.FindNearestPosIndex (pressSwitchList [i].transform.position, doorPosList);
//
//				pressSwitchList [i].controlledDoor = doorList [nearestDoorIndex];
//
//			}
//
//		}


//		public MapEvent GenerateMapItem(MapItemType mapItemType, Vector2 position, Item attachedItem = null){
//
//			MapEvent mapEvent = null;
//
//			switch (mapItemType) {
//			case MapItemType.Door:
//				mapEvent = mapItemPool.GetInstanceWithName<Door> (doorModel.name,doorModel.gameObject, mapItemsContainer);
//				originalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
//				break;
//			case MapItemType.TreasureBox:
//				mapEvent = mapItemPool.GetInstanceWithName<TreasureBox> (lockedTreasureBoxModel.name, lockedTreasureBoxModel.gameObject, mapItemsContainer);
//				(mapEvent as TreasureBox).rewardItem = attachedItem;
//				originalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
//				break;
//			case MapItemType.Buck:
//				mapEvent = mapItemPool.GetInstanceWithName<TreasureBox> (buckModel.name, buckModel.gameObject, mapItemsContainer);
//				(mapEvent as TreasureBox).rewardItem = attachedItem;
//				originalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
//				break;
//			case MapItemType.Pot:
//				mapEvent = mapItemPool.GetInstanceWithName<TreasureBox> (potModel.name, potModel.gameObject, mapItemsContainer);
//				(mapEvent as TreasureBox).rewardItem = attachedItem;
//				originalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
//				break;
//			case MapItemType.MovableFloor:
//				mapEvent = mapItemPool.GetInstanceWithName<MovableFloor> (movableFloorModel.name, movableFloorModel.gameObject, mapItemsContainer);
//				originalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 10;
//				break;
//			case MapItemType.Stone:
//				mapEvent = mapItemPool.GetInstanceWithName<Obstacle> (stoneModel.name, stoneModel.gameObject, mapItemsContainer);
//				originalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
//				break;
//			case MapItemType.Switch:
//				mapEvent = mapItemPool.GetInstanceWithName<ThornTrapSwitch> (trapSwitchModel.name, trapSwitchModel.gameObject, mapItemsContainer);
//				originalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
//				break;
//			case MapItemType.Transport:
//				mapEvent = mapItemPool.GetInstanceWithName<Exit> (transportModel.name, transportModel.gameObject, mapItemsContainer);
//				originalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
//				transportPositionInMap = position;
//				break;
//			case MapItemType.NormalTrapOff:
//				mapEvent = mapItemPool.GetInstanceWithName<ThornTrap> (trapModel.name, trapModel.gameObject, mapItemsContainer);
//				(mapEvent as ThornTrap).SetTrapOff ();
//				(mapEvent as ThornTrap).mapItemType = MapItemType.NormalTrapOff;
//				originalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 10;
//				break;
//			case MapItemType.NormalTrapOn:
//				mapEvent = mapItemPool.GetInstanceWithName<ThornTrap> (trapModel.name, trapModel.gameObject, mapItemsContainer);
//				(mapEvent as ThornTrap).SetTrapOn ();
//				(mapEvent as ThornTrap).mapItemType = MapItemType.NormalTrapOn;
//				originalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 10;
//				break;
//			case MapItemType.Tree:
//				mapEvent = mapItemPool.GetInstanceWithName<Obstacle> (treeModel.name, treeModel.gameObject, mapItemsContainer);
//				originalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
//				break;
//			case MapItemType.Billboard:
//				mapEvent = mapItemPool.GetInstanceWithName<Billboard> (billboardModel.name, billboardModel.gameObject, mapItemsContainer);
//				originalMapWalkableInfoArray [(int)position.x, (int)position.y] = 0;
//				break;
//			case MapItemType.FireTrap:
//				mapEvent = mapItemPool.GetInstanceWithName<FireTrap> (fireTrapModel.name, fireTrapModel.gameObject, mapItemsContainer);
//				originalMapWalkableInfoArray [(int)position.x, (int)position.y] = 10;
//				break;
//			case MapItemType.Hole:
//				mapEvent = mapItemPool.GetInstanceWithName<SecretStairs> (holeModel.name, holeModel.gameObject, mapItemsContainer);
//				originalMapWalkableInfoArray [(int)position.x, (int)position.y] = 10;
//				break;
//			case MapItemType.MovableBox:
//				mapEvent = mapItemPool.GetInstanceWithName<MovableBox> (movableBoxModel.name, movableBoxModel.gameObject, mapItemsContainer);
//				originalMapWalkableInfoArray [(int)position.x, (int)position.y] = 0;
//				break;
////			case MapItemType.LauncherTowardsUp:
////				mapEvent = mapItemPool.GetInstanceWithName<Launcher> (launcherModel.name, launcherModel.gameObject, mapItemsContainer);
////				originalMapWalkableInfoArray [(int)position.x, (int)position.y] = 0;
////				Launcher launcher = mapEvent as Launcher;
////				launcher.SetTowards (MyTowards.Up);
////				launcher.SetRange (columns, rows);
////				break;
////			case MapItemType.LauncherTowardsDown:
////				mapEvent = mapItemPool.GetInstanceWithName<Launcher> (launcherModel.name, launcherModel.gameObject, mapItemsContainer);
////				originalMapWalkableInfoArray [(int)position.x, (int)position.y] = 0;
////				launcher = mapEvent as Launcher;
////				launcher.SetTowards (MyTowards.Down);
////				launcher.SetRange (columns, rows);
////				break;
////			case MapItemType.LauncherTowardsLeft:
////				mapEvent = mapItemPool.GetInstanceWithName<Launcher> (launcherModel.name, launcherModel.gameObject, mapItemsContainer);
////				originalMapWalkableInfoArray [(int)position.x, (int)position.y] = 0;
////				launcher = mapEvent as Launcher;
////				launcher.SetTowards (MyTowards.Left);
////				launcher.SetRange (columns, rows);
////				break;
////			case MapItemType.LauncherTowardsRight:
////				mapEvent = mapItemPool.GetInstanceWithName<Launcher> (launcherModel.name, launcherModel.gameObject, mapItemsContainer);
////				originalMapWalkableInfoArray [(int)position.x, (int)position.y] = 0;
////				launcher = mapEvent as Launcher;
////				launcher.SetTowards (MyTowards.Right);
////				launcher.SetRange (columns, rows);
////				break;
//			case MapItemType.Plant:
//				mapEvent = mapItemPool.GetInstanceWithName<Plant> (plantModel.name, plantModel.gameObject, mapItemsContainer);
//				(mapEvent as Plant).attachedItem = attachedItem;
//				originalMapWalkableInfoArray [(int)position.x, (int)position.y] = 0;
//				break;
//			case MapItemType.PressSwitch:
//				mapEvent = mapItemPool.GetInstanceWithName<PressSwitch> (pressSwitchModel.name, pressSwitchModel.gameObject, mapItemsContainer);
//				originalMapWalkableInfoArray [(int)position.x, (int)position.y] = 10;
//				break;
//			case MapItemType.Crystal:
//				mapEvent = mapItemPool.GetInstanceWithName<Crystal> (crystalModel.name, crystalModel.gameObject, mapItemsContainer);
//				originalMapWalkableInfoArray [(int)position.x, (int)position.y] = 0;
//				break;
//			case MapItemType.MapNPC:
//				mapEvent = mapItemPool.GetInstanceWithName<MapNPC> (mapNpcModel.name, mapNpcModel.gameObject, mapItemsContainer);
//				originalMapWalkableInfoArray [(int)position.x, (int)position.y] = 0;
//				break;
//			case MapItemType.Docoration:
//				mapEvent = mapItemPool.GetInstanceWithName<Block> (blockModel.name, blockModel.gameObject, mapItemsContainer);
//				originalMapWalkableInfoArray [(int)position.x, (int)position.y] = 0;
//				break;
//			}
//
//			mapEvent.mapItemType = mapItemType;
//
//			mapEvent.transform.position = new Vector3 (position.x, position.y, 0);
//
//			mapEvent.gameObject.SetActive (false);
//
//			allSleepingOtherItems.Add (mapEvent.transform);
//
//			return mapEvent;
//		}


		public void ChangeAllTrapStatusInMap(){
			for (int i = 0; i < allTrapsInMap.Count; i++) {
				allTrapsInMap [i].ChangeTrapStatus ();
			}
		}

		public List<SecretStairs> GetAllHolesInMap(){
			return allHolesInMap;
		}




		private MapTile GetTileAtPosition(MapLayer layer,Vector3 position){
			MapTile tile = null;
			for (int i = 0; i < layer.tileDatas.Count; i++) {
				MapTile t = layer.tileDatas [i];
				if ((int)(t.position.x) == (int)(position.x) && 
					(int)(t.position.y) == (int)(position.y)) {
					tile = t;
				}
			}
			return tile;
		}

		/// <summary>
		/// 初始化地面和背景图，初始化地图上的基础可行走信息
		/// </summary>
		private void SetUpFloorAndBackground ()
		{

			// 获得地图图集
			string floorImageName = mapInfo.mapTilesImageName;

			// 获得背景图片
//			string backgroundImageName = mapInfo.backgroundImageName;

//			Sprite backgroundSprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite obj) {
//				return obj.name == backgroundImageName;
//			});

//			Transform background = Camera.main.transform.Find ("Background");
//			background.GetComponent<SpriteRenderer> ().sprite = backgroundSprite;
//			background.gameObject.SetActive (true);

			// 创建地板
			for (int i = 0; i < floorLayer.tileDatas.Count; i++) {
				MapTile tile = floorLayer.tileDatas [i];

				Transform floor = floorPool.GetInstance<Transform> (floorModel.gameObject, floorsContainer);
//				floor.position = new Vector3 (tile.position.x, tile.position.y, -100f);
				floor.position = new Vector3 (tile.position.x, tile.position.y, 0);
				floor.gameObject.SetActive (false);
//				if (tile.walkable) {
//					originalMapWalkableInfoArray [(int)tile.position.x, (int)tile.position.y] = 1;
//				}

				string tileSpriteName = string.Format ("{0}_{1}", floorImageName, tile.tileIndex);
				Sprite tileSprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite obj) {
					return obj.name == tileSpriteName;
				});

				SpriteRenderer floorTileRenderer = floor.GetComponent<SpriteRenderer> ();
				floorTileRenderer.sprite = tileSprite;
				floorTileRenderer.sortingOrder = -(int)tile.position.y;
				allSleepingFloors.Add(floor);
//				Debug.Log (floor.position);
			}
		}

		private bool PositionSame(Vector3 position1,Vector3 position2){

			if (position1.x > position2.x - 0.2f &&
			    position1.x < position2.x + 0.2f &&
			    position1.y > position2.y - 0.2f &&
			    position1.y < position2.y + 0.2f) {
				return true;
			}

			return false;

		}

		/// <summary>
		/// 初始化玩家
		/// </summary>
		/// <param name="position">Position.</param>
		private void SetUpPlayer(){

			int randomIndex = Random.Range (0, playerOriginalPosList.Count);
//
			Vector3 position = playerOriginalPosList [randomIndex];

			Transform player = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ().transform;

			player.position = position;

			bpCtr = player.GetComponent<BattlePlayerController> ();


			bpCtr.ActiveBattlePlayer (true, true, true);

			bpCtr.SetSortingOrder (-(int)position.y);

			DirectlyShowSleepingTilesAtPosition (position);

			ItemsAroundAutoIntoLifeWithBasePoint (position);

			bpCtr.StopMoveAndWait ();
			bpCtr.singleMoveEndPos = position;
			bpCtr.pathPosList.Clear ();

			player.rotation = Quaternion.identity;

			// 视角聚焦到玩家身上
			Camera.main.transform.SetParent (player, false);

			Camera.main.transform.rotation = Quaternion.identity;

			Camera.main.transform.localPosition = new Vector3 (0, 0, -10);

			Camera.main.orthographicSize = 6.0f;

			Transform background = Camera.main.transform.Find ("Background");
			background.transform.localPosition = new Vector3 (0.2f*(columns/2-position.x), 0.2f*(rows/2-position.y), 5);

			bpCtr.TowardsDown ();
			// 默认进入关卡后播放的角色动画
			bpCtr.PlayRoleAnim ("wait", 0, null);

			bpCtr.isInFight = false;

			bpCtr.SetRoleAnimTimeScale (1.0f);

//			StartCoroutine ("PlayerPlayWaitAnim");
		}

		private IEnumerator PlayerPlayWaitAnim(){
			yield return null;
			bpCtr.ActiveBattlePlayer (false, false, true);
			// 默认进入关卡后播放的角色动画
			bpCtr.PlayRoleAnim ("wait", 0, null);
		}

		/// <summary>
		/// 获取场景中的玩家人物模型
		/// </summary>
		/// <returns>The battle player.</returns>
		private BattlePlayerController GetBattlePlayer(){

			if (bpCtr == null) {
				Transform player = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ().transform;
				bpCtr = player.GetComponent<BattlePlayerController> ();
			}
				
			return bpCtr;

		}



		/// <summary>
		/// 初始化地图上的怪物
		/// </summary>
		/// <param name="position">Position.</param>
		private void SetUpMonster(Vector2 position){

			// 随机拿到一个本关中的怪物
			int monsterIndexInData = Random.Range (0, levelData.monsterInfos.Length);

			// 拿到怪物模型
			MonsterInfo monsterInfo = levelData.monsterInfos [monsterIndexInData];

			if (monsterInfo.monsterCount == 0) {
				SetUpMonster(position);
				return;
			}

			Transform monster = null;
			string monsterName = MyTool.GetMonsterName(monsterInfo.monsterId);
			for (int i = 0; i < monsterPool.transform.childCount; i++) {
				Transform monsterInPool = monsterPool.transform.GetChild (i);
				if(monsterInPool.name == monsterName){
					monster = monsterInPool;
					break;
				}
			}

			if (monster == null) {
				monster = levelData.LoadMonster (monsterInfo.monsterId);
			}

			monster.SetParent (monstersContainer);
			monster.localScale = Vector3.one;
			monster.localRotation = Quaternion.identity;

			levelData.monsterInfos [monsterIndexInData].monsterCount--;

//			Debug.LogFormat ("加载怪物{0}", monsterName);
//			levelData.monsters.RemoveAt (monsterIndexInData);

			originalMapWalkableInfoArray [(int)position.x, (int)position.y] = 0;

			monster.position = new Vector3 (position.x, position.y, 0);

//			monster.GetComponent<BattleAgentController> ().agent.ResetBattleAgentProperties (true);

			allSleepingMonsters.Add (monster);

		}

		private void SetUpBoss(Vector2 position){

			Transform boss = null;
			string bossName = MyTool.GetMonsterName(levelData.bossId);
			for (int i = 0; i < monsterPool.transform.childCount; i++) {
				Transform monsterInPool = monsterPool.transform.GetChild (i);
				if(monsterInPool.name == bossName){
					boss = monsterInPool;
				}
			}

			if (boss == null) {
				boss = levelData.LoadMonster (levelData.bossId);
			}

			originalMapWalkableInfoArray [(int)position.x, (int)position.y] = 0;

			boss.position = new Vector3 (position.x, position.y, 0);
			boss.SetParent (monstersContainer);
			boss.localScale = Vector3.one;
			boss.localRotation = Quaternion.identity;


//			boss.GetComponent<BattleMonsterController> ().agent.ResetBattleAgentProperties (true);

			allSleepingMonsters.Add (boss);

		}



		public Vector3 GetARandomWalkablePositionAround(Vector3 oriPosition){

			List<Vector3> walkablePositionsAround = new List<Vector3> ();

			int oriPosX = Mathf.RoundToInt (oriPosition.x);
			int oriPosY = Mathf.RoundToInt (oriPosition.y);

			if (mapWalkableInfoArray[oriPosX,oriPosY + 1] == -1 && 
				originalMapWalkableInfoArray[oriPosX, oriPosY + 1] == 1) {
				Vector3 position = new Vector3 (oriPosition.x, oriPosition.y + 1, oriPosition.z);
				walkablePositionsAround.Add (position);
			} 
			if ((mapWalkableInfoArray[oriPosX-1,oriPosY] == -1 && originalMapWalkableInfoArray [oriPosX - 1, oriPosY] == 1)
				|| mapWalkableInfoArray[oriPosX-1,oriPosY] == 1) {
				Vector3 position = new Vector3 (oriPosition.x - 1, oriPosition.y, oriPosition.z);
				walkablePositionsAround.Add (position);
			} 
			if ((mapWalkableInfoArray[oriPosX,oriPosY-1] == -1 && originalMapWalkableInfoArray [oriPosX, oriPosY - 1] == 1)
				|| mapWalkableInfoArray[oriPosX,oriPosY-1] == 1){
				Vector3 position = new Vector3 (oriPosition.x, oriPosition.y - 1, oriPosition.z);
				walkablePositionsAround.Add (position);
			}
			if (mapWalkableInfoArray[oriPosX+1,oriPosY] == -1 && originalMapWalkableInfoArray [oriPosX + 1, oriPosY] == 1
				|| mapWalkableInfoArray[oriPosX+1,oriPosY] == 1) {
				Vector3 position = new Vector3 (oriPosition.x + 1, oriPosition.y, oriPosition.z);
				walkablePositionsAround.Add (position);
			}


			if (walkablePositionsAround.Count > 0) {
				int randomWalkablePositionIndex = Random.Range (0, walkablePositionsAround.Count);
				return walkablePositionsAround [randomWalkablePositionIndex];
			} else {
				return oriPosition;
			}

		}

		public void DirectlyShowSleepingTilesAtPosition(Vector3 position){

			for (int i = 0; i < allSleepingFloors.Count; i++) {
				Transform sleepingFloor = allSleepingFloors [i];
				if(PositionSame(sleepingFloor.position,position)){
//					sleepingFloor.position = new Vector3(position.x,position.y,0);
					sleepingFloor.gameObject.SetActive (true);
					allSleepingFloors.RemoveAt (i);
					break;
				}
			}

			for (int i = 0; i < allSleepingMonsters.Count; i++) {
				Transform sleepingMonster = allSleepingMonsters [i];
				if(PositionSame(sleepingMonster.position,position)){
//					sleepingMonster.position = new Vector3(position.x,position.y,0);
					allSleepingMonsters.RemoveAt (i);
					allAliveMonsters.Add (sleepingMonster);
					sleepingMonster.gameObject.SetActive (true);
					sleepingMonster.GetComponent<BattleMonsterController> ().SetAlive ();
					break;
				}
			}

			for (int i = 0; i < allSleepingOtherItems.Count; i++) {
				Transform sleepingOther = allSleepingOtherItems [i];
				if(PositionSame(sleepingOther.position,position)){
//					sleepingOther.position = new Vector3(position.x,position.y,0);
					allSleepingOtherItems.RemoveAt (i);
					allAliveOtherItems.Add (sleepingOther);
					sleepingOther.gameObject.SetActive (true);
//					sleepingOther.GetComponent<MapEvent> ().InitMapItem ();
					break;
				}
			}

			int posX = (int)position.x;
			int posY = (int)position.y;

			mapWalkableInfoArray [posX, posY] = originalMapWalkableInfoArray [posX, posY];

		}



		public void ItemsAroundAutoIntoLifeWithBasePoint(Vector3 basePosition,CallBack cb = null){

			Vector3 upPoint = new Vector3 (basePosition.x, basePosition.y + 1);
			Vector3 downPoint = new Vector3 (basePosition.x, basePosition.y - 1);
			Vector3 leftPoint = new Vector3 (basePosition.x - 1, basePosition.y);
			Vector3 rightPoint = new Vector3 (basePosition.x + 1, basePosition.y);

			float delay = 0;

			delay = ValidTilesAutoIntoLifeAtPoint (upPoint,delay);
			delay = ValidTilesAutoIntoLifeAtPoint (downPoint,delay);
			delay = ValidTilesAutoIntoLifeAtPoint (leftPoint,delay);
			ValidTilesAutoIntoLifeAtPoint (rightPoint,delay);
		}


		private float ValidTilesAutoIntoLifeAtPoint(Vector3 position,float delay){

			InitValidTiles (position);

			if (validSleepingTiles.Count == 0) {
				return delay;
			}

			bool continueInitAroundItems = validSleepingTiles.Count == 1 && validSleepingTiles [0].type == SleepingTileType.Floor;

			Transform floor = null;
			Transform other = null;

			for (int i = 0; i < validSleepingTiles.Count; i++) {
				SleepingTile tile = validSleepingTiles [i];
				if (tile.type == SleepingTileType.Floor) {
					floor = tile.tileTransform;
				} else if (tile.type == SleepingTileType.Other) {
					other = tile.tileTransform;
				}
			}

			if (floor != null) {
				
				IEnumerator otherComeIntoLife = other != null ? OtherComeIntoLife (other) : null;

				IEnumerator floorComeIntoLife = FloorComeIntoLife (floor, otherComeIntoLife, continueInitAroundItems, delay);

				StartCoroutine (floorComeIntoLife);

				float delayBase = 0.3f;

				delay += delayBase;

				return delay;

			} else {
				IEnumerator otherComeIntoLife = other != null ? OtherComeIntoLife (other) : null;

				StartCoroutine (otherComeIntoLife);

				return delay;
			}
				
		}

		public float floorComeIntoLifeInterval;

		public float floorOriginalScale;
		public float floorOriginalPositionOffsetX;
		public float floorOriginalPositionOffsetY;

		private float scalerIncreaseBase{
			get{ return (1 - floorOriginalScale) / floorComeIntoLifeInterval; }
		}

		private float positionYFixBase{
			get{ return -floorOriginalPositionOffsetY / floorComeIntoLifeInterval; }
		}
		private float positionXFixBase{
			get{ return -floorOriginalPositionOffsetX / floorComeIntoLifeInterval; }
		}

		private IEnumerator FloorComeIntoLife(Transform floor,IEnumerator otherComeIntoLife,bool continueInitAroundItems,float delay){

			yield return new WaitForSeconds (delay);

			floor.gameObject.SetActive (true);

			float timer = 0;

			Vector3 floorTargetPosition = new Vector3 (floor.position.x, floor.position.y, 0);

			float floorScale = floorOriginalScale;

			floor.localScale = new Vector3 (floorOriginalScale, floorOriginalScale, 1f);

			floor.position = new Vector3 (floorTargetPosition.x + floorOriginalPositionOffsetX, floorTargetPosition.y + floorOriginalPositionOffsetY, 0);

//			floor.GetComponent<SpriteRenderer>().sortingLayerName = "Ground";


			while (timer < floorComeIntoLifeInterval) {

				Vector3 positionFix = new Vector3 (positionXFixBase * Time.deltaTime, positionYFixBase * Time.deltaTime, 0);

				floorScale += scalerIncreaseBase * Time.deltaTime;

				floor.localScale = new Vector3 (floorScale, floorScale, 1);;

				floor.position = floor.position + positionFix;

				timer += Time.deltaTime;

				yield return null;
			}

			floor.localScale = Vector3.one;
			floor.position = floorTargetPosition;

			if (otherComeIntoLife != null) {
				StartCoroutine (otherComeIntoLife);
			} else {
				mapWalkableInfoArray [(int)(floor.position.x), (int)(floor.position.y)] = originalMapWalkableInfoArray [(int)(floor.position.x), (int)(floor.position.y)];
			}

			if (continueInitAroundItems) {
				ItemsAroundAutoIntoLifeWithBasePoint (floorTargetPosition);
			}
		}

		public float otherComeIntoLifeInterval;

		public float otherOriginalPositionOffsetY;

		public float otherPositonYFixBase{
			get{return -otherOriginalPositionOffsetY / otherComeIntoLifeInterval;}
		}

		private IEnumerator OtherComeIntoLife(Transform other){
			
			other.gameObject.SetActive (true);

			if (other.GetComponent<MapEvent> () != null) {
//				other.GetComponent<MapEvent> ().InitMapItem ();
			}

			if (other.GetComponent<BattleMonsterController> () != null) {
				other.GetComponent<BattleMonsterController> ().SetAlive ();
			}

			Vector3 otherTargetPosition = new Vector3 (other.position.x, other.position.y, 0);

			other.position = new Vector3 (otherTargetPosition.x, otherTargetPosition.y + otherOriginalPositionOffsetY, 0);

			float timer = 0;

			while (timer < otherComeIntoLifeInterval) {

				Vector3 positionFix = new Vector3 (0, otherPositonYFixBase * Time.deltaTime, 0);

				timer += Time.deltaTime;

				other.position = other.position + positionFix;

				yield return null;
			}

			other.position = otherTargetPosition;

			int posX = (int)other.position.x;
			int posY = (int)other.position.y;


			mapWalkableInfoArray [posX,posY] = originalMapWalkableInfoArray[posX,posY];
	
		}


		private enum SleepingTileType{
			Floor,
			Other
		}

		private class SleepingTile
		{
			public Transform tileTransform;
			public SleepingTileType type;

			public SleepingTile(Transform tileTrans,SleepingTileType type){
				this.tileTransform = tileTrans;
				this.type = type;
			}
		}

		private List<SleepingTile> validSleepingTiles = new List<SleepingTile>();

		private void InitValidTiles(Vector3 position){

			validSleepingTiles.Clear ();

			for (int i = 0; i < allSleepingMonsters.Count; i++) {
				Transform monster = allSleepingMonsters [i];
				if(PositionSame(position,monster.position)){
					validSleepingTiles.Add(new SleepingTile(monster,SleepingTileType.Other));
					allSleepingMonsters.Remove (monster);
					allAliveMonsters.Add (monster);
					break;
				}
			}

			for (int i = 0; i < allSleepingOtherItems.Count; i++) {
				Transform mapItem = allSleepingOtherItems [i];
				if (PositionSame(position,mapItem.position)) {
					validSleepingTiles.Add(new SleepingTile(mapItem,SleepingTileType.Other));
					allSleepingOtherItems.Remove (mapItem);
					allAliveOtherItems.Add (mapItem);
					break;
				}
			}

			for (int i = 0; i < allSleepingFloors.Count; i++) {
				Transform floor = allSleepingFloors [i];
				if (PositionSame(position,floor.position)) {
					validSleepingTiles.Add(new SleepingTile(floor,SleepingTileType.Floor));
					allSleepingFloors.Remove (floor);
					break;
				}
			}
		}

		public EffectAnim GetEffectAnim(string effectName,Transform effectContainer){

			EffectAnim effectAnim = effectAnimPool.GetInstanceWithName<EffectAnim> (effectName);

			if (effectAnim == null) {

				EffectAnim effectModel = GameManager.Instance.gameDataCenter.allEffects.Find (delegate(EffectAnim obj) {
					return obj.effectName == effectName;
				});

				effectAnim = Instantiate (effectModel.gameObject,effectContainer).GetComponent<EffectAnim>();

			}
			effectAnim.transform.localPosition = new Vector3(effectAnim.localPos.x,effectAnim.localPos.y,0);
			effectAnim.transform.localScale = Vector3.one;
			effectAnim.transform.localRotation = Quaternion.identity;
			effectAnim.gameObject.SetActive (true);

			return effectAnim;
		}

		public void AddEffectAnimToPool(Transform effectAnim){
			effectAnim.gameObject.SetActive (false);
			effectAnimPool.AddInstanceToPool (effectAnim.gameObject);
		}
			

		public void PlayDestinationAnim(Vector3 targetPos,bool arrivable){

//			Animator destinationAnimator = destinationAnimation.GetComponent<Animator> ();

			destinationAnimation.position = targetPos;

			StartCoroutine ("LatelyPlayDestinationTintAnim", arrivable);
		}

		private IEnumerator LatelyPlayDestinationTintAnim(bool arrivable){

			yield return new WaitUntil (() => Time.timeScale == 1);

			yield return null;

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
			

//		public void ShowConsumablesValidPointsTint(Consumables consumables){
//
//			Vector3 basePosition = GetBattlePlayer ().transform.position;
//
//			Vector3[] aroundPositions = GetPositionsAround (basePosition);
//
//			for (int i = 0; i < aroundPositions.Length; i++) {
//				GenerateConsumablesPosTintAt (aroundPositions[i],consumables);
//			}
//
//		}


//		private void GenerateConsumablesPosTintAt(Vector3 pos,Consumables consumables){
//
//			currentUsingConsumables = consumables;
//
//			bool targetMatch = false;
//
//			switch (consumables.itemName) { 
//			case "地板":
//				targetMatch = CheckTargetMatchFloor (pos);
//				break;
//			case "锄头":
//				targetMatch = CheckTargetMatchPickaxe (pos);
//				break;
//			case "锯子":
//				targetMatch = CheckTargetMatchSaw (pos);
//				break;
//			case "镰刀":
//				targetMatch = CheckTargetMatchSickle (pos);
//				break;
//			case "钥匙":
//				targetMatch = CheckTargetMatchKey (pos);
//				break;
//			case "火把":
//				targetMatch = CheckTargetMatchTorch (pos);
//				break;
//			case "水":
//				targetMatch = CheckTargetMatchWater (pos);
//				break;
//			case "树苗":
//				targetMatch = CheckTargetMatchPlant (pos);
//				break;
//			case "土块":
//				targetMatch = CheckTargetMatchClod (pos);
//				break;
//			case "开关":
//				targetMatch = CheckTargetMatchSwitch (pos);
//				break;
//			}
//
//			Transform consumablesValidPosTint = consumablesValidPosTintPool.GetInstance<Transform> (consumablesValidPosTintModel.gameObject, consumablesValidPosTintContainer);
//
//			consumablesValidPosTint.position = pos;
//
//			SpriteRenderer sr = consumablesValidPosTint.GetComponent<SpriteRenderer> ();
//
//			if (!targetMatch) {
//				sr.color = new Color (1, 0, 0, 0.5f);
//			} else {
//				sr.color = new Color (0, 1, 0, 0.5f);
//			}
//			consumablesValidPosTint.gameObject.SetActive (true);
//		}

		private bool CheckTargetMatchFloor(Vector3 targetPos){
			if(targetPos.x < 0 || targetPos.x >= columns || targetPos.y < 0 || targetPos.y >= rows){
				return false;
			}
			return mapWalkableInfoArray [(int)targetPos.x, (int)targetPos.y] == -1;
		}

//		private bool CheckTargetMatchPickaxe(Vector3 targetPos){
//			Transform mapItemTrans = GetAliveOtherItemAt (targetPos);
//			if (mapItemTrans == null) {
//				return false;
//			}
//			MapEvent mapItem = mapItemTrans.GetComponent<MapEvent>();
//			return mapItem != null && mapItem.mapItemType == MapItemType.Stone;
//		}
//
//		private bool CheckTargetMatchSaw (Vector3 targetPos){
//			Transform mapItemTrans = GetAliveOtherItemAt (targetPos);
//			if (mapItemTrans == null) {
//				return false;
//			}
//			MapEvent mapItem = mapItemTrans.GetComponent<MapEvent>();
//			return mapItem != null && mapItem.mapItemType == MapItemType.Tree;
//		}
//
//		private bool CheckTargetMatchSickle(Vector3 targetPos){
//			Transform mapItemTrans = GetAliveOtherItemAt (targetPos);
//			if (mapItemTrans == null) {
//				return false;
//			}
//			MapEvent mapItem = mapItemTrans.GetComponent<MapEvent>();
//			return mapItem != null && mapItem.mapItemType == MapItemType.Plant;
//		}

//		private bool CheckTargetMatchKey(Vector3 targetPos){
//			Transform mapItemTrans = GetAliveOtherItemAt (targetPos);
//			if (mapItemTrans == null) {
//				return false;
//			}
//			MapEvent mapItem = mapItemTrans.GetComponent<MapEvent>();
//
//			if (!(mapItem is TreasureBox)) {
//				return false;
//			}
//
//			return (mapItem as TreasureBox).locked;
//		}

//		private bool CheckTargetMatchTorch(Vector3 targetPos){
//
//			if (mapWalkableInfoArray [(int)targetPos.x, (int)targetPos.y] != 1) {
//				return false;
//			} 
//
//			Transform mapItemTrans = GetAliveOtherItemAt (targetPos);
//			if (mapItemTrans != null) {
//				if (mapItemTrans.GetComponent<MapEvent> ().isDroppable) {
//					return true;
//				}
//				return false;
//			}
//				
//			return true;
//		}

//		private bool CheckTargetMatchWater(Vector3 targetPos){
//			Transform mapItemTrans = GetAliveOtherItemAt (targetPos);
//			if (mapItemTrans == null) {
//				return false;
//			}
//			MapEvent mapItem = mapItemTrans.GetComponent<MapEvent>();
//			return mapItem != null && mapItem.mapItemType == MapItemType.FireTrap;
//		}

//		private bool CheckTargetMatchPlant(Vector3 targetPos){
//			
//			if (mapWalkableInfoArray [(int)targetPos.x, (int)targetPos.y] != 1) {
//				return false;
//			}
//
//			Transform mapItemTrans = GetAliveOtherItemAt (targetPos);
//			if (mapItemTrans != null) {
//				if (mapItemTrans.GetComponent<MapEvent> ().isDroppable) {
//					return true;
//				}
//				return false;
//			}
//
//			return true;
//		}
//
//		private bool CheckTargetMatchSwitch(Vector3 targetPos){
//
//			if (mapWalkableInfoArray [(int)targetPos.x, (int)targetPos.y] != 1) {
//				return false;
//			}
//
//			Transform mapItemTrans = GetAliveOtherItemAt (targetPos);
//			if (mapItemTrans != null) {
//				if (mapItemTrans.GetComponent<MapEvent> ().isDroppable) {
//					return true;
//				}
//				return false;
//			}
//
//			return true;
//
//		}
//
//		private bool CheckTargetMatchClod(Vector3 targetPos){
//
//			Transform mapItemTrans = GetAliveOtherItemAt (targetPos);
//			if (mapItemTrans == null) {
//				return false;
//			}
//			MapEvent mapItem = mapItemTrans.GetComponent<MapEvent>();
//			return mapItem != null && mapItem.mapItemType == MapItemType.Hole;
//
//		}


		public Transform GetAliveMonsterAt(Vector3 pos){

			Transform aliveMonster = null;

			for (int i = 0; i < allAliveMonsters.Count; i++) {
				if(PositionSame(allAliveMonsters [i].position,pos) && allAliveMonsters [i].gameObject.activeInHierarchy){
					aliveMonster = allAliveMonsters [i];
					break;
				}
			}

			return aliveMonster;
		}

		public Transform GetAliveOtherItemAt(Vector3 pos){

			Transform aliveOtherItem = null;

			for (int i = 0; i < allAliveOtherItems.Count; i++) {
				if(PositionSame(allAliveOtherItems [i].position,pos) && allAliveOtherItems [i].gameObject.activeInHierarchy){
					aliveOtherItem = allAliveOtherItems [i];
					break;
				}
			}

			return aliveOtherItem;

		}

		public void RemoveConsumablesTints(){
			AddConsumablesValidPosTintsToPool ();
		}

//		public void ClickConsumablesPosAt(Vector3 pos){
//
//			RemoveConsumablesTints ();
//
//			if (!IsClickPosValid (pos)) {
//				return;
//			}
//
//			int posX = (int)pos.x;
//			int posY = (int)pos.y;
//
//			bool removeConsumablesFromBag = false;
//
//			switch (currentUsingConsumables.itemName) { 
//			case "地板":
//				if (CheckTargetMatchFloor(pos)) {
//					Transform floor = floorPool.GetInstance<Transform> (floorModel.gameObject, floorsContainer);
//					floor.position = new Vector3 (posX, posY, 0);
//					floor.GetComponent<SpriteRenderer> ().sortingOrder = -posY;
//					originalMapWalkableInfoArray [posX, posY] = 1;
//					mapWalkableInfoArray [posX, posY] = 1;
//					ItemsAroundAutoIntoLifeWithBasePoint (pos, null);
//					removeConsumablesFromBag = true;
//				}
//				break;
//			case "锄头":
//				if (CheckTargetMatchPickaxe(pos)) {
//					Obstacle stone = GetAliveOtherItemAt (pos).GetComponent<Obstacle> ();
//					stone.DestroyObstacle (null);
//					mapWalkableInfoArray[posX,posY] = 1;
//					removeConsumablesFromBag = true;
//				}
//				break;
//			case "锯子":
//				if (CheckTargetMatchSaw(pos)) {
//					Obstacle tree = GetAliveOtherItemAt (pos).GetComponent<Obstacle> ();
//					tree.DestroyObstacle (null);
//					mapWalkableInfoArray[posX,posY] = 1;
//					removeConsumablesFromBag = true;
//				}
//				break;
//			case "镰刀":
//				if (CheckTargetMatchSickle(pos)) {
//					Plant plant = GetAliveOtherItemAt (pos).GetComponent<Plant> ();
//					SetUpRewardInMap (plant.attachedItem, pos);
//					plant.AddToPool (mapItemPool);
//					mapWalkableInfoArray [posX, posY] = 1;
//					removeConsumablesFromBag = true;
//				}
//				break;
//			case "钥匙":
//				if (CheckTargetMatchKey(pos)) {
//					TreasureBox tb = GetAliveOtherItemAt (pos).GetComponent<TreasureBox> ();
//					if (tb.locked) {
//						tb.PlayTreasureBoxAnimAndAudio (delegate{
//							SetUpRewardInMap(tb.rewardItem,pos);
//						});
//
//						removeConsumablesFromBag = true;
//					}
//				}
//				break;
//			case "火把":
//				if (CheckTargetMatchTorch(pos)) {
//					Transform fireTrap = mapItemPool.GetInstanceWithName<Transform> (fireTrapModel.name, fireTrapModel.gameObject, mapItemsContainer);
//					fireTrap.position = new Vector3 (posX, posY, 0);
//					mapWalkableInfoArray [posX, posY] = 10;
//					fireTrap.GetComponent<FireTrap> ().SetTrapOn ();
//					allAliveOtherItems.Add (fireTrap);
//					removeConsumablesFromBag = true;
//					Transform aliveMonster = GetAliveMonsterAt (pos);
//					if (aliveMonster != null) {
//						BattleMonsterController monster = aliveMonster.GetComponent<BattleMonsterController> ();
//						monster.boxCollider.enabled = false;
////						allAliveMonsters.Remove (monster.transform);
////						string tintText = monster.agent.maxHealth.ToString ();
////						monster.AddTintTextToQueue (tintText, SpecialAttackResult.None);
//						monster.PlayRoleAnim ("die", 1, delegate {
//							monster.GetComponent<MapMonster>().AddToPool(monsterPool);
//						});
//					}
//				}
//				break;
//			case "水":
//				if (CheckTargetMatchWater(pos)) {
//					FireTrap fireTrap = GetAliveOtherItemAt (pos).GetComponent<FireTrap> ();
//					fireTrap.SetTrapOff ();
////					allAliveOtherItems.Remove (fireTrap.transform);
//					mapWalkableInfoArray [posX, posY] = 1;
//					removeConsumablesFromBag = true;
//				}
//				break;
//			case "树苗":
//				if (CheckTargetMatchPlant (pos)) {
//					Transform tree = mapItemPool.GetInstanceWithName<Transform> (treeModel.name, treeModel.gameObject, mapItemsContainer);
//					tree.position = new Vector3 (posX, posY, 0);
//					mapWalkableInfoArray [posX, posY] = 0;
//					tree.GetComponent<MapEvent> ().InitMapItem ();
//					allAliveOtherItems.Add (tree);
//					removeConsumablesFromBag = true;
//				}
//				break;
//			case "开关":
//				if (CheckTargetMatchSwitch (pos)) {
//					Transform normalTrapSwitch = mapItemPool.GetInstanceWithName<Transform> (trapSwitchModel.name, trapSwitchModel.gameObject, mapItemsContainer);
//					normalTrapSwitch.position = new Vector3 (posX, posY, 0);
//					mapWalkableInfoArray [posX, posY] = 0;
//					normalTrapSwitch.GetComponent<MapEvent> ().InitMapItem ();
//					allAliveOtherItems.Add (normalTrapSwitch);
//					removeConsumablesFromBag = true;
//				}
//				break;
//			case "土块":
//				if (CheckTargetMatchClod (pos)) {
//					SecretStairs hole = mapItemPool.GetInstanceWithName<SecretStairs> (trapSwitchModel.name, trapSwitchModel.gameObject, mapItemsContainer);
//					hole.AddToPool (mapItemPool);
//					mapWalkableInfoArray [posX, posY] = 1;
////					allAliveOtherItems.Remove (hole.transform);
//					removeConsumablesFromBag = true;
//				}
//				break;
//
//			}
//
//			if (removeConsumablesFromBag) {
//				Player.mainPlayer.RemoveItem (currentUsingConsumables,1);
//				GetComponent<ExploreManager> ().expUICtr.UpdateBottomBar ();
//			}
//
//		}

		private bool IsClickPosValid(Vector3 clickPos){
			bool clickAtValidPosition = false;
			Vector3 playerPos = GetBattlePlayer ().transform.position;
			Vector3[] aroundPositions = GetPositionsAround (playerPos);
			for (int i = 0; i < aroundPositions.Length; i++) {
				if(PositionSame(clickPos,aroundPositions[i])){
					clickAtValidPosition = true;
					break;
				}
			}

			return clickAtValidPosition;
		}

		private Vector3[] GetPositionsAround(Vector3 basePosition){

			Vector3 upPoint = new Vector3 (basePosition.x, basePosition.y + 1);
			Vector3 downPoint = new Vector3 (basePosition.x, basePosition.y - 1);
			Vector3 leftPoint = new Vector3 (basePosition.x - 1, basePosition.y);
			Vector3 rightPoint = new Vector3 (basePosition.x + 1, basePosition.y); 

			return new Vector3[]{ upPoint, downPoint, leftPoint, rightPoint };

		}

		private void AddConsumablesValidPosTintsToPool(){

			for (int i = 0; i < consumablesValidPosTintContainer.childCount; i++) {
				Transform consumablesValidPosTint = consumablesValidPosTintContainer.GetChild (i);
				consumablesValidPosTint.gameObject.SetActive (false);
				consumablesValidPosTintPool.AddInstanceToPool (consumablesValidPosTint.gameObject);
				i--;
			}

		}



			

		private class RewardInMap
		{
			public Transform rewardTrans;
			public Item reward;

			public RewardInMap(Transform rewardTrans,Item reward){
				this.rewardTrans = rewardTrans;
				this.reward = reward;
			}


		}

		public void SetUpRewardInMap(Item reward, Vector3 rewardPosition){

			Transform rewardTrans = rewardItemPool.GetInstance<Transform> (rewardItemModel.gameObject, rewardsContainer);

			SpriteRenderer sr = rewardTrans.GetComponent<SpriteRenderer> ();

			sr.sprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (reward);
				
			rewardTrans.position = new Vector3 (rewardPosition.x, rewardPosition.y + 1f, rewardPosition.z);

			sr.sortingOrder = -(int)rewardPosition.y;

			rewardTrans.gameObject.SetActive (true);

			RewardInMap rewardInMap = new RewardInMap (rewardTrans, reward);

			StartCoroutine ("RewardFlyToPlayer", rewardInMap);

		}

		private IEnumerator RewardFlyToPlayer(RewardInMap rewardInMap){

			yield return new WaitUntil (()=>Time.timeScale == 1);

//			Item reward = rewardInMap.reward;
//
//			Player.mainPlayer.AddItem (reward);

			Transform rewardTrans = rewardInMap.rewardTrans;

			float rewardUpAndDownSpeed = 0.5f;

			float timer = 0;
			while (timer < 0.5f) {
				Vector3 moveVector = new Vector3 (0, rewardUpAndDownSpeed * Time.deltaTime, 0);
				rewardTrans.position += moveVector;
				timer += Time.deltaTime;
				yield return null;
			}
			while (timer < 1f) {
				Vector3 moveVector = new Vector3 (0, -rewardUpAndDownSpeed * Time.deltaTime, 0);
				rewardTrans.position += moveVector;
				timer += Time.deltaTime;
				yield return null;
			}

			float passedTime = 0;

			float leftTime = rewardFlyDuration - passedTime;

			BattlePlayerController bpCtr = GetBattlePlayer ();

			float distance = Mathf.Sqrt (Mathf.Pow ((bpCtr.transform.position.x - rewardTrans.position.x), 2.0f) 
				+ Mathf.Pow ((bpCtr.transform.position.y - rewardTrans.position.y), 2.0f));

			while (distance > 0.5f) {

				if (leftTime <= 0) {
					Debug.LogError ("0 can not be devided");
				}

				Vector3 rewardVelocity = new Vector3 ((bpCtr.transform.position.x - rewardTrans.position.x) / leftTime, 
					(bpCtr.transform.position.y - rewardTrans.position.y) / leftTime, 0);

				Vector3 newRewardPos = new Vector3 (rewardTrans.position.x + rewardVelocity.x * Time.deltaTime, 
					rewardTrans.position.y + rewardVelocity.y * Time.deltaTime);

				rewardTrans.position = newRewardPos;

//				rewardInMap.position = Vector3.MoveTowards(rewardInMap.position,bpCtr.transform.position,

				passedTime += Time.deltaTime;

				leftTime = rewardFlyDuration - passedTime;

				distance = Mathf.Sqrt (Mathf.Pow ((bpCtr.transform.position.x - rewardTrans.position.x), 2.0f) 
					+ Mathf.Pow ((bpCtr.transform.position.y - rewardTrans.position.y), 2.0f));

				yield return null;

			}


			Item reward = rewardInMap.reward;


			if (reward.itemType != ItemType.CharacterFragment && Player.mainPlayer.CheckBagFull (reward)) {
				GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
					TransformManager.FindTransform ("BagCanvas").GetComponent<BagViewController> ().AddBagItemWhenBagFull (reward);
				}, false, true);
			} else {

				GetComponent<ExploreManager> ().ObtainReward (reward);
			}


			AddRewardItemToPool (rewardTrans);

		}

		private void AddRewardItemToPool(Transform rewardItem){
			rewardItemPool.AddInstanceToPool (rewardItem.gameObject);
			rewardItem.gameObject.SetActive (false);
		}

		public void AddMapItemInPool(Transform mapItem){
			mapItem.GetComponent<MapEvent> ().AddToPool (mapItemPool);
		}


		public void DestroyInstancePools(){

			Destroy (floorPool.gameObject);

			Destroy (mapItemPool.gameObject);

			Destroy (monsterPool.gameObject);

			Destroy (effectAnimPool.gameObject);

			Destroy (rewardItemPool.gameObject);

//			Destroy (otherAnimPool.gameObject);

			Destroy (consumablesValidPosTintPool.gameObject);
		}


		/// <summary>
		/// 将场景中的地板，npc，地图物品，怪物加入缓存池中
		/// </summary>
		private void AllMapInstancesToPool(){
			
			AddAllFloorsToPool ();

			AddAllMapItemsToPool();

			AddAllMonstersToPool();

			AddAllEffectAnimToPool ();

			AddAllRewardItemToPool ();

			consumablesValidPosTintPool.AddChildInstancesToPool (consumablesValidPosTintContainer);
		}

		public void AddAllEffectAnimToPool(){
			while (effectAnimContainer.childCount > 0) {
				AddEffectAnimToPool (effectAnimContainer.GetChild (0));
			}
		}

		public void AddAllRewardItemToPool(){
			while (rewardsContainer.childCount > 0) {
				AddRewardItemToPool (rewardsContainer.GetChild (0));
			}
		}
			

		public void AddAllMapItemsToPool(){
			while(mapItemsContainer.childCount > 0){
				mapItemsContainer.GetChild (0).GetComponent<MapEvent> ().AddToPool (mapItemPool);
			}
		}

		public void AddAllMonstersToPool(){
			while(monstersContainer.childCount > 0){
				monstersContainer.GetChild (0).GetComponent<MapMonster> ().AddToPool (monsterPool);
			}
		}

		public void AddAllFloorsToPool(){

			while(floorsContainer.childCount > 0){
				Transform floor = floorsContainer.GetChild (0);
				floor.gameObject.SetActive (false);
				floorPool.AddInstanceToPool(floor.gameObject);
			}
		}

		public void AddMonsterToPool(BattleMonsterController monster){
			monster.boxCollider.enabled = false;
			monster.gameObject.SetActive (false);
			monster.GetComponent<MapMonster>().AddToPool (monsterPool);
		}

		/// <summary>
		/// 每关初始化完毕后清除缓存池中没有复用到的游戏体
		/// </summary>
		private void ClearPools(){
			floorPool.ClearInstancePool ();
			mapItemPool.ClearInstancePool ();
			monsterPool.ClearInstancePool ();
		}


		public void PrepareToResetMap(){

			StopAllCoroutines ();

			AllMapInstancesToPool ();

			ResetDestinationAnim();

//			destinationAnimation.gameObject.SetActive (false);

		}

		void OnDestroy(){
//			mapInfo = null;
//			floorPool = null;
//			mapItemPool = null;
//			monsterPool = null;
//			effectAnimPool = null;
//			rewardItemPool = null;
//			consumablesValidPosTintPool = null;
//			levelData = null;
//			mapWalkableInfoArray = null;
//			originalMapWalkableInfoArray = null;
//			bpCtr = null;
//			allTrapsInMap = null;
//			allHolesInMap = null;
//
//			totalValidPosGridList = null;
//			playerOriginalPosList = null;
//
//			allSleepingMonsters = null;
//			allSleepingOtherItems = null;
//			allSleepingFloors = null;
//
//			allAliveOtherItems = null;
//			allAliveMonsters = null;
//
//			currentUsingConsumables = null;
		}

	}
		
}
