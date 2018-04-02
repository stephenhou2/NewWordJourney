using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEditor;
	using System.IO;
	using MiniJSON;

	public class MapDataHelper {

//		[System.Serializable]
//		public struct OriginalMapData
//		{
//			public OriginalTileSet[] tilesets;
//			public int width;
//			public int height;
////			public int tilewidth;
////			public int tileheight;
//			public OriginalLayer[] layers;
//
//			public int floorTileFirstGid;
//			public int attachedInfoFirstGid;
//			public int attachedItemFirstGid;
//
//		}
//
//		[System.Serializable]
//		public struct OriginalLayer
//		{
//			public int[] data;
//			public string name;
//			public AttachedProperty properties;
//
//		}
//
//		[System.Serializable]
//		public struct AttachedProperty{
////			public string image;
//			public string backgroundImageName;
//			public string floorImageName;
//		}
//
//		[System.Serializable]
//		public struct OriginalTileSet{
//			public int firstgid;
//			public string source;
//		}
//
//		[System.Serializable]
//		public class FloorTilesInfo
//		{
//			public string floorImageName;
//			public int[] walkableInfoArray;
//		}



//		[MenuItem("EditHelper/MapDataHelper")]
//		public static void SetUpMapData(){
//
//			//************ 获取所有的地图原始数据文件信息 *************//
//			string originalMapDataDirectory = "/Users/houlianghong/Desktop/MyGameData/Map/NewMapDatas";
//
//			DirectoryInfo directory = new DirectoryInfo (originalMapDataDirectory);
//
//			FileInfo[] originalMapDatasFiles = directory.GetFiles ();
//
//
//
//			//************ 读取地图块可行走信息数据 ***********//
//			string floorTilesInfoPath = "/Users/houlianghong/Desktop/MyGameData/Map/FloorTilesInfo.json";
//
//			FloorTilesInfo[] floorTilesInfoArray = DataHandler.LoadDataToModelsWithPath<FloorTilesInfo> (floorTilesInfoPath);
//
//
//
//			FloorTilesInfo tilesInfo = null;
//			Layer attachedInfoLayer = null;
//			Layer attachedItemLayer = null;
//
//			for (int i = 0; i < originalMapDatasFiles.Length; i++) {
//
//				FileInfo fi = originalMapDatasFiles [i];
//
//				if (fi.Extension != ".json") {
//					continue;
//				}
//
//				string oriMapData = DataHandler.LoadDataString (fi.FullName);
//				Dictionary<string,object> oriMapDataDic = Json.Deserialize (oriMapData) as Dictionary<string,object>;
//
//				OriginalMapData oriMap = new OriginalMapData ();
//
//				oriMap.height = int.Parse(oriMapDataDic ["height"].ToString ());
//				oriMap.width = int.Parse (oriMapDataDic ["width"].ToString ());
//
//				List<Dictionary<string,object>> tempData = new List<Dictionary<string, object>> ();
//				tempData = oriMapDataDic ["layers"] as List < Dictionary<string,object>;
//
////				OriginalMapData oriMapData = DataHandler.LoadDataToSingleModelWithPath<OriginalMapData> (fi.FullName);
//
//				for (int j = 0; j < oriMapData.tilesets.Length; j++) {
//
//					OriginalTileSet ts = oriMapData.tilesets [j];
//
//					if (ts.source.Contains ("Floor")) {
//						oriMapData.floorTileFirstGid = ts.firstgid;
//					} else if (ts.source.Contains ("AttachedInfo")) {
//						oriMapData.attachedInfoFirstGid = ts.firstgid;
//					} else if (ts.source.Contains ("AttachedItem")) {
//						oriMapData.attachedItemFirstGid = ts.firstgid;
//					}
//					else {
//						Debug.LogError(string.Format("未查询到地图／附加信息的原始贴图数据"));
//					}
//
//
//				}
//
//
//				Layer[] newLayers = new Layer[oriMapData.layers.Length];
////				string backgroundImageName = null;
//				string floorImageName = null;
//
//
//				for (int j = 0; j < oriMapData.layers.Length; j++) {
//					
//					OriginalLayer layer = oriMapData.layers [j];
//
//					int firstGid = 0;
//
//					int row = 0;
//					int col = 0;
//
//					List<Tile> tileDatas = new List<Tile> ();
//
//					switch (layer.name) {
//					case "FloorLayer":
//						firstGid = oriMapData.floorTileFirstGid;
//						floorImageName = layer.properties.floorImageName;
//
//						for (int k = 0; k < layer.data.Length; k++) {
//							row = oriMapData.height - k / oriMapData.width - 1;
//							col = k % oriMapData.width;
//							int tileIndex = layer.data [k] - firstGid;
//							if (tileIndex >= 0) {
////								bool walkable = tileInfo.walkableInfoArray [tileIndex] == 1;
//								bool walkable = true;
//								Tile tile = new Tile (new Vector2 (col, row), tileIndex ,walkable);
//								tileDatas.Add (tile);
//								Debug.LogFormat ("mapName:{0},layerName:{1},index:{2},tileInfo:{3}",fi.Name,"floor",k,tile);
//							}
//
//						}
//						newLayers[j] = new Layer (layer.name, tileDatas); 
//						break;
//					case "AttachedInfoLayer":
//						firstGid = oriMapData.attachedInfoFirstGid;
////						backgroundImageName = layer.properties.backgroundImageName;
//
//						for (int k = 0; k < layer.data.Length; k++) {
//							row = oriMapData.height - k / oriMapData.width - 1;
//							col = k % oriMapData.width;
//							int tileIndex = layer.data [k] - firstGid;
//							if (tileIndex >= 0) {
//								Tile tile = new Tile (new Vector2 (col, row), tileIndex, false);
//								tileDatas.Add (tile);
////								Debug.LogFormat ("mapName:{0},layerName:{1},index:{2},tileInfo:{3}",fi.Name,"attachedInfo",k,tile);
//							}
//
//						}
//						attachedInfoLayer = new Layer (layer.name, tileDatas); 
//						newLayers [j] = attachedInfoLayer; 
//						break;
//					case "AttachedItemLayer":
//						firstGid = oriMapData.attachedItemFirstGid;
//
//						for (int k = 0; k < layer.data.Length; k++) {
//							row = oriMapData.height - k / oriMapData.width - 1;
//							col = k % oriMapData.width;
//							int tileIndex = layer.data [k] - firstGid;
//							if (tileIndex >= 0) {
//								Tile tile = new Tile (new Vector2 (col, row), tileIndex, false);
//								tileDatas.Add (tile);
//								Debug.LogFormat ("mapName:{0},layerName:{1},index:{2},tileInfo:{3}",fi.Name,"attachedItem",k,tile);
//							}
//
//						}
//						attachedItemLayer = new Layer (layer.name, tileDatas); 
//						newLayers [j] = attachedItemLayer;
//						break;
//					default:
//						Debug.LogError(string.Format("地图{0}层名称设置错误或缺失",fi.Name));
//						break;
//					}
//				}
//
//				for (int k = 0; k < attachedInfoLayer.tileDatas.Count; k++) {
//
//					Tile attachedInfoTile = attachedInfoLayer.tileDatas [k];
//
//					if ((AttachedInfoType)(attachedInfoTile.tileIndex) == AttachedInfoType.Buck ||
//						(AttachedInfoType)(attachedInfoTile.tileIndex) == AttachedInfoType.Pot ||
//						(AttachedInfoType)(attachedInfoTile.tileIndex) == AttachedInfoType.TreasureBox) {
//
//						bool attachedInfoDataExist = false;
//
//						for (int m = 0; m < attachedItemLayer.tileDatas.Count; m++) {
//
//							if (attachedItemLayer.tileDatas [m].position == attachedInfoTile.position) {
//								attachedInfoDataExist = true;
//								break;
//							}
//
//
//
//						}
//
//						if (!attachedInfoDataExist) {
//
//							Tile tile = new Tile (attachedInfoTile.position, (int)AttachedItemType.Random, true);
//
//							attachedItemLayer.tileDatas.Add (tile);
////							Debug.LogError (string.Format ("地图{0}上箱子内部没有对应物品---pos:[{1},{2}]",fi.Name, attachedInfoTile.position.x, attachedInfoTile.position.y));
//						}
//
//					}
//				}
//
//
//				MapData newMapData = new MapData (oriMapData.height, oriMapData.width, floorImageName, newLayers);
//
//				string newMapDataDirectory = CommonData.originDataPath + "/MapData";
//
//				SaveNewMapData (newMapData, newMapDataDirectory, fi.Name);
//			}
//
//		}
			
//		private static void SaveNewMapData(MapData mapData, string directory, string fileName){
//
//			if (!Directory.Exists (directory)) {
//				Directory.CreateDirectory (directory);
//			}
//
//			string path = string.Format ("{0}/{1}", directory, fileName);
//
//			string mapDataJson = JsonUtility.ToJson (mapData);
//
//			File.WriteAllText (path, mapDataJson);
//
//		}
		
	}
}
