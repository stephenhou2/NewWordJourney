using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{

    /// <summary>
    /// 地图数据模型
    /// </summary>
	[System.Serializable]
	public class HLHMapData
	{
		// 地图行数【地图按照沙盘创建，地图行数即地图高度】
		public int rowCount;
		// 地图列数【地图按照沙盘创建，地图行数即地图宽度】
		public int columnCount;
        // 地图创建所使用的图集名称
		public string mapTilesImageName;
        // 地图层信息列表【用于绘制地图，包括地板层，墙体层，装饰层】
		public List<MapLayer> mapLayers;
        // 附加信息层列表【用于构建地图事件】
		public List<MapAttachedInfoLayer> attachedInfoLayers;

        /// <summary>
        /// 地图数据构造函数
        /// </summary>
        /// <param name="rowCount">Row count.</param>
        /// <param name="columnCount">Column count.</param>
        /// <param name="mapTilesImageName">Map tiles image name.</param>
        /// <param name="mapLayers">Map layers.</param>
        /// <param name="attachedInfoLayers">Attached info layers.</param>
		public HLHMapData(int rowCount,int columnCount,string mapTilesImageName, List<MapLayer> mapLayers,List<MapAttachedInfoLayer> attachedInfoLayers){
			this.rowCount = rowCount;
			this.columnCount = columnCount;
			this.mapTilesImageName = mapTilesImageName;
			this.mapLayers = mapLayers;
			this.attachedInfoLayers = attachedInfoLayers;
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
			return mapEventCount;
		}

        /// <summary>
        /// 加载指定关卡序号的地图数据
        /// </summary>
        /// <returns>The map data of level.</returns>
        /// <param name="level">Level.</param>
		public static HLHMapData LoadMapDataOfLevel(int level)
        {

            string mapDataFilePath = string.Format("{0}/MapData/Level_{1}.json", CommonData.persistDataPath, level);

            HLHMapData mapData = DataHandler.LoadDataToSingleModelWithPath<HLHMapData>(mapDataFilePath);

            return mapData;

        }
	}


    /// <summary>
    /// 地图层模型
    /// </summary>
	[System.Serializable]
	public class MapLayer
	{
		// 层名称
		public string layerName;
        // 地图块数据列表
		public List<MapTile> tileDatas;

		public MapLayer(string layerName, List<MapTile> tileDatas){
			this.layerName = layerName;
			this.tileDatas = tileDatas;
		}

	}
    
    /// <summary>
    /// 附加信息层模型
    /// </summary>
	[System.Serializable]
	public class MapAttachedInfoLayer
	{
		// 层名称
		public string layerName;
        // 附加信息层图块数据
		public List<MapAttachedInfoTile> tileDatas;

		public MapAttachedInfoLayer(string layerName,List<MapAttachedInfoTile> tileDatas){
			this.layerName = layerName;
			this.tileDatas = tileDatas;
		}
	}
    
    /// <summary>
    /// 附加信息层图块数据模型
    /// </summary>
	[System.Serializable]
	public class MapAttachedInfoTile{

        // 图块类型【根据图块类型区分地图事件，如：门的type是door】
		public string type;
        // 图块在地图上的位置
		public Vector2 position;
        // 附加属性键值对
		public List<KVPair> properties;

		public MapAttachedInfoTile(string type,Vector2 position,List<KVPair> properties){
			this.type = type;
			this.position = position;
			this.properties = properties;
		}
              
	}   

	// 附加信息层对应的附加信息
	public enum MiniMapDisplayType
	{
		Wall,// 房间的墙和房间内装饰用的墙
		Door,// 不需要使用钥匙打开的门【暂不显示】
		KeyDoor,// 需要使用钥匙打开的门【暂不显示】
		Exit,// 进入下一关的出口
		ReturnExit,// 返回上一关的出口
		Pot,// 瓦罐【暂不显示】
		Bucket,// 木桶 【暂不显示】
		TreasureBox,// 宝箱【暂不显示】
		Crystal,// 水晶 【暂不显示】
		NPC,// npc
		Goldpack,// 钱袋 【暂不显示】
		Item// 可拾取物品【暂不显示】
	}



	[System.Serializable]
	public class MapTile{

        // 图块位置
		public Vector2 position;
		// 图块在图集中的序号
		public int tileIndex;
        // 是否可行走
		public bool canWalk;
		// 图块在小地图上的显示信息【使用时转化为MiniMapDisplayType】
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
