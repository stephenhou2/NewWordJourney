using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEditor;
	using System.IO;
	using MiniJSON;

	public class NewMapDataHelper{

		/// <summary>
		/// 生成地图的地图块的行走数据
		/// </summary>
		[System.Serializable]
		public class MapTilesInfo
		{
			public string mapTileSetsImageName;
			public int[] walkableInfoArray;
		}

		/// <summary>
		/// 根据地图块图片名称获取对应的地图块信息
		/// </summary>
		/// <returns>The map tiles info with name.</returns>
		/// <param name="tileImageName">Tile image name.</param>
		private static MapTilesInfo GetMapTilesInfoWithName(string tileImageName){

			MapTilesInfo tilesInfo = null;

			string mapTilesInfoPath = "/Users/houlianghong/Desktop/MyGameData/Map/MapTilesInfo.json";

			MapTilesInfo[] mapTilesInfoArray = DataHandler.LoadDataToModelsWithPath<MapTilesInfo> (mapTilesInfoPath);

			for (int i = 0; i < mapTilesInfoArray.Length; i++) {

				if (mapTilesInfoArray [i].mapTileSetsImageName.Equals (tileImageName)) {

					tilesInfo = mapTilesInfoArray [i];

					break;

				}

			}

			return tilesInfo;
		}



		/// <summary>
		/// 重写地图数据并保存
		/// </summary>
		[MenuItem("EditHelper/NewMapDataHelper")]
		public static void InitializeNewMapData(){

			//************ 获取所有的地图原始数据文件信息 *************//
			string originalMapDataDirectory = "/Users/houlianghong/Desktop/MyGameData/Map/MapDatas";

			DirectoryInfo directory = new DirectoryInfo (originalMapDataDirectory);

			FileInfo[] originalMapDatasFiles = directory.GetFiles ();


			// 遍历所有的原始地图数据并重新整合后保存
			for (int i = 0; i < originalMapDatasFiles.Length; i++) {

				FileInfo fi = originalMapDatasFiles [i];

				// 判断是否是存储数据用的json文件
				if (fi.Extension != ".json") {
					continue;
				}

                //Debug.LogFormat("地图数据名称" + fi.Name);
				//读取原始地图数据
                MapData mapData = ReadMap (fi.FullName,fi);

				//保存新的地图数据
				SaveNewMapData (mapData, fi.Name);
			}


		}

		/// <summary>
		/// 读取地图数据,并将数据转化为自定义的MapData模型
		/// </summary>
		/// <returns>The map.</returns>
		/// <param name="mapDataPath">Map data path.</param>
        private static MapData ReadMap(string mapDataPath,FileInfo fi){

			// 读取原始json数据
			string oriMapDataString = DataHandler.LoadDataString (mapDataPath);
			// MiniJson解析json数据
			Dictionary<string,object> oriMapDataDic = Json.Deserialize (oriMapDataString) as Dictionary<string,object>;

			// 获取地图宽高
			int mapHeight = int.Parse (oriMapDataDic ["height"].ToString ());
			int mapWidth = int.Parse (oriMapDataDic ["width"].ToString ());

			// 获取标准地图块宽高
			int mapTileHeight = int.Parse (oriMapDataDic ["tileheight"].ToString ());
			int mapTileWidth = int.Parse (oriMapDataDic ["tilewidth"].ToString ());


			// 获取地图上所有图层的数据
			List<object> layersDataArray = (List<object>)oriMapDataDic ["layers"];
			List<object> tileSetsArray = (List<object>)oriMapDataDic ["tilesets"];

			// 所有构建地图使用的图层
			List<MapLayer> mapLayers = new List<MapLayer> ();
			// 所有地图附加信息图层
			List<MapAttachedInfoLayer> attachedInfoLayers = new List<MapAttachedInfoLayer> ();

			// 地图块图集名称
			string tileSetsImageName = string.Empty;
			// 地图块数据（内含walkable info array）
			MapTilesInfo tileInfo = null;

            Dictionary<string, object> tileSetInfo = null;

			for (int j = 0; j < layersDataArray.Count; j++) {

				Dictionary<string,object> layerDataDic = layersDataArray [j] as Dictionary<string,object>;

				List<MapTile> tileDatas = new List<MapTile> ();

				
				//获取当前层的类型名称
				string layerType = layerDataDic ["type"].ToString ();

                // 当前层为地图层
                if (layerType.Equals("tilelayer"))
                {

                    List<object> tileDataArray = (List<object>)layerDataDic["data"];

                    string layerName = layerDataDic["name"].ToString();

                    if (layerName.Equals("FloorLayer"))
                    {
                        Dictionary<string, object> tileSetsNameDic = layerDataDic["properties"] as Dictionary<string, object>;
                        tileSetsImageName = tileSetsNameDic["TilesetImageName"].ToString();
                        tileInfo = GetMapTilesInfoWithName(tileSetsImageName);
                    }



                    if (tileSetInfo == null) { 
                        for (int k = 0; k < tileSetsArray.Count; k++)
                        {
                            Dictionary<string, object> tempTileSetInfo = tileSetsArray[k] as Dictionary<string, object>;
                            string sourceName = tempTileSetInfo["source"] as string;
                            if (sourceName.Contains(tileSetsImageName))
                            {
                                tileSetInfo = tempTileSetInfo;
                                break;
                            }
                        }
                    }


                    //获取当前层使用的地图块的firstGid
                    int firstGid = int.Parse(tileSetInfo["firstgid"].ToString());


					for (int k = 0; k < tileDataArray.Count; k++) {

						int row = mapHeight - k / mapWidth - 1;
						int col = k % mapWidth;

						int tileIndex = int.Parse (tileDataArray [k].ToString ()) - firstGid;
						if (tileIndex >= 0) {
							bool canWalk = false;
                            //Debug.LogFormat("mapName:{0},layerName:{1},tileIndex:{2},firstGid:{3}",fi.Name, layerName, tileIndex,firstGid);
							canWalk = tileInfo.walkableInfoArray [tileIndex] == 1;
							MapTile tile = new MapTile (new Vector2 (col, row), tileIndex,canWalk);
							tileDatas.Add (tile);
						}

					}

					MapLayer layer = new MapLayer (layerName, tileDatas);
					mapLayers.Add (layer);
				}

				// 当前层为事件层
				if (layerType.Equals("objectgroup")) {

					string layerName = layerDataDic ["name"].ToString ();

					List<MapAttachedInfoTile> attachedInfoTiles = new List<MapAttachedInfoTile> ();

					List<object> attachedInfoList = (List<object>)layerDataDic ["objects"];

					//int monsterCount = 0;

					for (int k = 0; k < attachedInfoList.Count; k++) {

						Dictionary<string,object> attachedInfo = attachedInfoList [k] as Dictionary<string,object>;
						string type = attachedInfo ["type"].ToString ();

						//Debug.Log(type);
						//if(type=="monster"){
						//	monsterCount++;
						//}

						float posX = float.Parse(attachedInfo ["x"].ToString());
						float posY = float.Parse (attachedInfo ["y"].ToString ());

						int col = Mathf.RoundToInt (posX / mapTileWidth);
						int row = mapHeight - Mathf.RoundToInt (posY / mapTileHeight);

						Vector2 pos = new Vector2 (col, row);

						Dictionary<string,object> properties = null;
						MapAttachedInfoTile attachedInfoTile = null;
						List<KVPair> kvPairs = new List<KVPair> ();

						if (!attachedInfo.ContainsKey ("properties")) {
							attachedInfoTile = new MapAttachedInfoTile (type, pos, kvPairs);
							attachedInfoTiles.Add (attachedInfoTile);
							continue;
						}

						properties = attachedInfo ["properties"] as Dictionary<string,object>;

						IDictionaryEnumerator dicEnumerator = properties.GetEnumerator ();

						while(dicEnumerator.MoveNext()){

							string key = dicEnumerator.Key.ToString ();
							string value = dicEnumerator.Value.ToString ();
//							Debug.LogFormat ("key:{0},value:{1}",key,value);

							KVPair kv = new KVPair (key,value);

							kvPairs.Add (kv);
						}

						attachedInfoTile = new MapAttachedInfoTile (type, pos, kvPairs);
                        
						attachedInfoTiles.Add (attachedInfoTile);

					}

					//if(monsterCount != 0 && monsterCount != 16){
					//	Debug.LogFormat("monster count:{0},map name:{1}",monsterCount,fi.Name);
					//}

					MapAttachedInfoLayer attachedInfoLayer = new MapAttachedInfoLayer (layerName, attachedInfoTiles);

					attachedInfoLayers.Add (attachedInfoLayer);

				}

			}

			MapData mapData = new MapData (mapHeight, mapWidth, tileSetsImageName, mapLayers,attachedInfoLayers);

			return mapData;
		}

		/// <summary>
		/// 保存自定义格式的地图数据
		/// </summary>
		/// <param name="mapData">Map data.</param>
		/// <param name="fileName">File name.</param>
		private static void SaveNewMapData(MapData mapData, string fileName){

			string directory = CommonData.originDataPath + "/MapData";

			if (!Directory.Exists (directory)) {
				Directory.CreateDirectory (directory);
			}

			string path = string.Format ("{0}/{1}", directory, fileName);

			string mapDataJson = JsonUtility.ToJson (mapData);

			File.WriteAllText (path, mapDataJson);

		}


	}
}
