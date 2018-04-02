using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{

	using System;

	[System.Serializable]
	public class MapData
	{
		public int rowCount;
		public int columnCount;
//		public int tileWidth;
//		public int tileHeight;
		public string mapTilesImageName;
//		public string backgroundImageName;
		public List<MapLayer> mapLayers;
		public List<MapAttachedInfoLayer> attachedInfoLayers;

//		public Layer floorLayer;
//		public Layer attachedInfoLayer;

		public MapData(int rowCount,int columnCount,string mapTilesImageName, List<MapLayer> mapLayers,List<MapAttachedInfoLayer> attachedInfoLayers){
			this.rowCount = rowCount;
			this.columnCount = columnCount;
//			this.tileWidth = tileWidth;
//			this.tileHeight = tileHeight;
			this.mapTilesImageName = mapTilesImageName;
//			this.backgroundImageName = backgroundImageName;
			this.mapLayers = mapLayers;
			this.attachedInfoLayers = attachedInfoLayers;
//			this.floorLayer = floorLayer;
//			this.attachedInfoLayer = attachedInfoLayer;
		}

		public static MapData GetMapDataOfLevel(int level){

			string mapDataFilePath = string.Format ("{0}/MapData/Level_{1}.json", CommonData.persistDataPath, level);

			return DataHandler.LoadDataToSingleModelWithPath<MapData> (mapDataFilePath);

		}

		/// <summary>
		/// 获取地图上总的地图事件数量
		/// </summary>
		/// <returns>The map event count.</returns>
		public int GetMapEventCount(){
			int mapEventCount = 0;
			for (int i = 0; i < attachedInfoLayers.Count; i++) {
				mapEventCount += attachedInfoLayers [i].tileDatas.Count;
			}
			return mapEventCount;
		}


	}



	[System.Serializable]
	public class MapLayer
	{
		public string layerName;
//		public string tileSetsImageName;
		public List<MapTile> tileDatas;

		public MapLayer(string layerName, List<MapTile> tileDatas){
			this.layerName = layerName;
//			this.tileSetsImageName = tileSetsImageName;
			this.tileDatas = tileDatas;
		}

	}

	[System.Serializable]
	public class MapAttachedInfoLayer
	{
		public string layerName;
		public List<MapAttachedInfoTile> tileDatas;

		public MapAttachedInfoLayer(string layerName,List<MapAttachedInfoTile> tileDatas){
			this.layerName = layerName;
			this.tileDatas = tileDatas;
		}
	}

	[System.Serializable]
	public class MapAttachedInfoTile{

		public string type;
		public Vector2 position;
		public List<KVPair> properties;
//		public object properties;

		public MapAttachedInfoTile(string type,Vector2 position,List<KVPair> properties){
			this.type = type;
			this.position = position;
			this.properties = properties;
		}

//		public MapAttachedInfoTile(string type,Vector2 position,object properties){
//			this.type = type;
//			this.position = position;
//			this.properties = properties;
//		}

	}

	// 附加信息层对应的附加信息
	public enum AttachedInfoType{
		PlayerOriginPosition,
		Crystal,
		Trader,
		NPC,
		Transport,
		Door,
		Buck,
		Pot,
		TreasureBox,
		Stone,
		Tree,
		Switch,
		TrapOff,
		TrapOn,
		MovableFloorStart,
		Boss,
		Monster,
		FireTrap,
		Billboard,
		Hole,
		Plant,
		MovableBox,
		LauncherLeft,
		LauncherRight,
		LauncherUp,
		LauncherDown,
		PressSwitch,
		Docoration,
		MovableFloorEnd
	}

	public enum AttachedItemType{
		Key,
		Saw,
		PickAxe,
		Floor,
		Medicine,
		Sickle,
		Water,
		Soil,
		Torch,
		Tree,
		Switch,
		Scroll,
		Random
	}

	[System.Serializable]
	public class MapTile{
		
		public Vector2 position;
		// 如果是附加信息层和附加物品层，则该数字代表对应的附加信息,使用（attachedInfoType）进行强制转换
		public int tileIndex;
		public bool canWalk;

		public MapTile(Vector2 pos,int index,bool canWalk){
			this.position = pos;
			this.tileIndex = index;
			this.canWalk = canWalk;
		}


		public override string ToString ()
		{
			return string.Format ("[Tile]\npositon:{0},tileIndex:{1}",position,tileIndex);
		}

	}



}
