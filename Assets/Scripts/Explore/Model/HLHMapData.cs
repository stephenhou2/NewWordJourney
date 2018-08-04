using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{

	using System;

	[System.Serializable]
	public class HLHMapData
	{
		//public int oriMapIndex;
		public int rowCount;
		public int columnCount;
		public string mapTilesImageName;
		public List<MapLayer> mapLayers;
		public List<MapAttachedInfoLayer> attachedInfoLayers;

//		public Layer floorLayer;
//		public Layer attachedInfoLayer;

		public HLHMapData(int rowCount,int columnCount,string mapTilesImageName, List<MapLayer> mapLayers,List<MapAttachedInfoLayer> attachedInfoLayers){
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

        
		/// <summary>
		/// 获取地图上总的地图事件数量      
		/// </summary>
		/// <returns>The map event count.</returns>
		public int GetMapEventCount(){
			int mapEventCount = 0;
			for (int i = 0; i < attachedInfoLayers.Count; i++) {
				MapAttachedInfoLayer layer = attachedInfoLayers[i];
				for (int j = 0; j < layer.tileDatas.Count;j++){
					MapAttachedInfoTile tileInfo = layer.tileDatas[j];
					if(tileInfo.type == "item" || tileInfo.type == "goldPack" || tileInfo.type == "bucket" 
					   || tileInfo.type == "pot" || tileInfo.type == "treasure" || tileInfo.type == "crystal" 
					   || tileInfo.type == "doorGear" || tileInfo.type == "keyDoorGear"|| tileInfo.type == "pressSwitch")
					mapEventCount++;
				}            
			}

			mapEventCount += 3;

			return mapEventCount;
		}

		public static HLHMapData LoadMapDataOfLevel(int level)
        {

            string mapDataFilePath = string.Format("{0}/MapData/Level_{1}.json", CommonData.persistDataPath, level);

            HLHMapData mapData = DataHandler.LoadDataToSingleModelWithPath<HLHMapData>(mapDataFilePath);

            return mapData;

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
	public enum MiniMapDisplayType
	{
		Wall,// 房间的墙和房间内装饰用的墙
		Door,// 不需要使用钥匙打开的门
		KeyDoor,// 需要使用钥匙打开的门
		Ladder,
		Hole,
		Exit,// 进入下一关的出口
		ReturnExit,// 返回上一关的出口
		Pot,
		Bucket,
		TreasureBox,
		Crystal,
		NPC,
		Goldpack,
		Item
	}



	[System.Serializable]
	public class MapTile{
		
		public Vector2 position;
		// 如果是附加信息层和附加物品层，则该数字代表对应的附加信息,使用（attachedInfoType）进行强制转换
		public int tileIndex;
		public bool canWalk;
        // 图块在小地图上的显示信息
		public int miniMapInfo;

		public MapTile(Vector2 pos,int index,bool canWalk,int miniMapInfo){
			this.position = pos;
			this.tileIndex = index;
			this.canWalk = canWalk;
			this.miniMapInfo = miniMapInfo;
		}


		public override string ToString ()
		{
			return string.Format ("[Tile]\npositon:{0},tileIndex:{1}",position,tileIndex);
		}

	}



}
